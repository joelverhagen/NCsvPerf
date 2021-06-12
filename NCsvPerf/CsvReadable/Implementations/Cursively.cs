#nullable enable
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using Cursively;

using static System.Numerics.BitOperations;
using static System.Runtime.CompilerServices.Unsafe;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Cursively/
    /// Source: https://github.com/airbreather/Cursively
    /// </summary>
    public sealed class Cursively : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public Cursively(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            using var vis = new Vis<T>(_activationMethod);
            if (stream.TryGetBuffer(out var buf))
            {
                CsvSyncInput.ForMemory(buf).Process(vis);
            }
            else
            {
                CsvSyncInput.ForStream(stream).Process(vis);
            }

            return vis.Records;
        }

        private sealed class Vis<T> : CsvReaderVisitorBase, IDisposable
            where T : ICsvReadable, new()
        {
            private readonly Activate<T> _activate;

            private readonly MyStringPool _stringPool;

            private readonly byte[] _bytes = new byte[1024];

            private readonly List<string> _fields = new List<string>();

            private int _bytesConsumed;

            public Vis(ActivationMethod activationMethod)
            {
                _activate = ActivatorFactory.Create<T>(activationMethod);
                _stringPool = new MyStringPool();
            }

            public List<T> Records { get; } = new List<T>();

            public override void VisitEndOfField(ReadOnlySpan<byte> chunk)
            {
                if (_bytesConsumed != 0)
                {
                    chunk.CopyTo(_bytes.AsSpan(_bytesConsumed, chunk.Length));
                    chunk = new ReadOnlySpan<byte>(_bytes, 0, _bytesConsumed + chunk.Length);
                    _bytesConsumed = 0;
                }

                _fields.Add(chunk.IsEmpty ? string.Empty : _stringPool.GetString(chunk));
            }

            public override void VisitEndOfRecord()
            {
                var record = _activate();
                var fields = _fields;
                record.Read(i => fields[i]);
                fields.Clear();
                Records.Add(record);
            }

            public override void VisitPartialFieldContents(ReadOnlySpan<byte> chunk)
            {
                chunk.CopyTo(_bytes.AsSpan(_bytesConsumed, chunk.Length));
                _bytesConsumed += chunk.Length;
            }

            public void Dispose()
            {
                _stringPool.Dispose();
            }
        }

        private sealed class MyStringPool : IDisposable
        {
            private readonly List<byte[]> _rentedBufs = new List<byte[]>();

            private byte[] _saveBuf = Array.Empty<byte>();

            private int _saveBufUsed;

            private int[] _buckets = new int[32];

            private Entry[] _entries = new Entry[32];

            private int _count;

            private bool _disposed;

            ~MyStringPool()
            {
                DisposeCore();
            }

            public string GetString(ReadOnlySpan<byte> bytes)
            {
                if (bytes.Length > byte.MaxValue)
                {
                    // we store length in a byte
                    return Encoding.UTF8.GetString(bytes);
                }

                var entries = _entries;
                var hashCode = GetHashCode(bytes);
                ref var bucket = ref GetBucket(hashCode);
                var i = bucket - 1;
                while ((uint)i < (uint)entries.Length)
                {
                    ref var e = ref entries[i];
                    if (e.hashCode == hashCode && bytes.Length == e.length && bytes.SequenceEqual(e.buf.AsSpan(e.index, e.length)))
                    {
                        return e.s;
                    }

                    i = e.next;
                }

                if (_count == entries.Length)
                {
                    entries = Resize();
                    bucket = ref GetBucket(hashCode);
                }

                if (_saveBufUsed + bytes.Length > _saveBuf.Length)
                {
                    _rentedBufs.Add(_saveBuf = ArrayPool<byte>.Shared.Rent(1 << 20));
                    _saveBufUsed = 0;
                }

                var bufStart = _saveBufUsed;
                bytes.CopyTo(_saveBuf.AsSpan(bufStart, bytes.Length));
                _saveBufUsed += bytes.Length;

                var s = Encoding.UTF8.GetString(bytes);

                var index = _count++;

                ref var entry = ref entries[index];
                entry.buf = _saveBuf;
                entry.index = bufStart;
                entry.length = (byte)bytes.Length;
                entry.hashCode = hashCode;
                entry.next = bucket - 1;
                entry.s = s;
                bucket = index + 1;

                return s;
            }

            public void Dispose()
            {
                DisposeCore();
                GC.SuppressFinalize(this);
            }

            private void DisposeCore()
            {
                if (_disposed)
                {
                    return;
                }

                foreach (var buf in _rentedBufs)
                {
                    ArrayPool<byte>.Shared.Return(buf);
                }

                _disposed = true;
            }

            private static uint GetHashCode(ReadOnlySpan<byte> input)
            {
                var hc64 = XXH64(input);
                return (uint)hc64 ^ (uint)(hc64 >> 32);
            }

            private static ulong XXH64(ReadOnlySpan<byte> input)
            {
                const ulong Prime1 = 11400714785074694791;
                const ulong Prime2 = 14029467366897019727;
                const ulong Prime3 = 1609587929392839161;
                const ulong Prime4 = 9650029242287828579;
                const ulong Prime5 = 2870177450012600261;

                Span<ulong> hashState = stackalloc ulong[]
                {
                    unchecked(Prime1 + Prime2),
                    Prime2,
                    0,
                    unchecked(0UL - Prime1),
                };

                ulong h = 0;
                ref var inputStart = ref AsRef(in input.GetPinnableReference());
                var originalLength = (uint)input.Length;
                var remainingLength = originalLength;
                if (remainingLength >= 32)
                {
                    // feels like SIMD could help this loop, since each iteration only looks at the
                    // next 32 bytes in independent 8-byte chunks.
                    while (remainingLength >= 32)
                    {
                        Process(ref inputStart, hashState);
                        inputStart = ref AddByteOffset(ref inputStart, new IntPtr(32));
                        remainingLength -= 32;
                    }

                    h = RotateLeft(hashState[0], 1) +
                        RotateLeft(hashState[1], 7) +
                        RotateLeft(hashState[2], 12) +
                        RotateLeft(hashState[3], 18);

                    h = (h ^ ProcessSingle(0, hashState[0])) * Prime1 + Prime4;
                    h = (h ^ ProcessSingle(0, hashState[1])) * Prime1 + Prime4;
                    h = (h ^ ProcessSingle(0, hashState[2])) * Prime1 + Prime4;
                    h = (h ^ ProcessSingle(0, hashState[3])) * Prime1 + Prime4;
                }

                h += originalLength;
                while (remainingLength >= 8)
                {
                    h = RotateLeft(h ^ ProcessSingle(0, ReadUnaligned<ulong>(ref inputStart)), 27) * Prime1 + Prime4;
                    inputStart = ref AddByteOffset(ref inputStart, new IntPtr(8));
                    remainingLength -= 8;
                }

                if (remainingLength >= 4)
                {
                    h = RotateLeft(h ^ ReadUnaligned<uint>(ref inputStart), 23) * Prime2 + Prime3;
                    inputStart = ref AddByteOffset(ref inputStart, new IntPtr(4));
                    remainingLength -= 4;
                }

                while (remainingLength != 0)
                {
                    h = RotateLeft(h ^ inputStart * Prime5, 11) * Prime1;
                    inputStart = ref AddByteOffset(ref inputStart, new IntPtr(1));
                    --remainingLength;
                }

                h ^= h >> 33;
                h *= Prime2;
                h ^= h >> 29;
                h *= Prime3;
                h ^= h >> 32;

                return h;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                static void Process(ref byte data, Span<ulong> hashState)
                {
                    hashState[0] = ProcessSingle(hashState[0], ReadUnaligned<ulong>(ref AddByteOffset(ref data, new IntPtr(sizeof(ulong) * 0))));
                    hashState[1] = ProcessSingle(hashState[1], ReadUnaligned<ulong>(ref AddByteOffset(ref data, new IntPtr(sizeof(ulong) * 1))));
                    hashState[2] = ProcessSingle(hashState[2], ReadUnaligned<ulong>(ref AddByteOffset(ref data, new IntPtr(sizeof(ulong) * 2))));
                    hashState[3] = ProcessSingle(hashState[3], ReadUnaligned<ulong>(ref AddByteOffset(ref data, new IntPtr(sizeof(ulong) * 3))));
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                static ulong ProcessSingle(ulong val, ulong input) => unchecked(RotateLeft(val + input * Prime2, 31) * Prime1);
            }

            private Entry[] Resize()
            {
                var count = _count;
                Array.Resize(ref _entries, count * 2);

                var entries = _entries;
                _buckets = new int[entries.Length * 2];
                for (var i = 0; i < count; i++)
                {
                    ref var e = ref entries[i];
                    if (e.next >= -1)
                    {
                        ref var bucket = ref GetBucket(e.hashCode);
                        e.next = bucket - 1;
                        bucket = i + 1;
                    }
                }

                return entries;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private ref int GetBucket(ulong hashCode)
            {
                var buckets = _buckets;
                return ref buckets[hashCode & ((uint)buckets.Length - 1)];
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            private struct Entry
            {
                public uint hashCode;

                public byte[] buf;

                public int index;

                public byte length;

                public int next;

                public string s;
            }
        }
    }
}
