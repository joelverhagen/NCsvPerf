using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Cursively;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Cursively/
    /// Source: https://github.com/airbreather/Cursively
    /// </summary>
    public sealed class CursivelyCsvReader : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public CursivelyCsvReader(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var vis = new Vis<T>(_activationMethod, 128);
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

        private sealed class Vis<T> : CsvReaderVisitorBase
            where T : ICsvReadable, new()
        {
            private readonly Activate<T> _activate;

            private readonly MyStringPool _stringPool;

            private readonly byte[] _bytes = new byte[1024];

            private readonly List<string> _fields = new List<string>();

            private int _bytesConsumed;

            public Vis(ActivationMethod activationMethod, int sizeLimit)
            {
                _activate = ActivatorFactory.Create<T>(activationMethod);
                _stringPool = new MyStringPool(sizeLimit);
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

                _fields.Add(_stringPool.GetString(chunk));
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
        }

        private sealed class MyStringPool
        {
            private readonly Dictionary<ReadOnlyMemory<byte>, string> _inner = new Dictionary<ReadOnlyMemory<byte>, string>(64, ByteArrayEqualityComparer.Instance);

            private readonly byte[] _checkBuf;

            private byte[] _saveBuf = new byte[81920];

            private int _saveBufUsed;

            public MyStringPool(int sizeLimit)
            {
                _checkBuf = new byte[sizeLimit];
            }

            public string GetString(ReadOnlySpan<byte> bytes)
            {
                if (bytes.Length > _checkBuf.Length)
                {
                    return Encoding.UTF8.GetString(bytes);
                }

                var check = _checkBuf.AsMemory(0, bytes.Length);
                bytes.CopyTo(check.Span);
                if (_inner.TryGetValue(check, out string s))
                {
                    return s;
                }

                if (check.Length > 81920 - _saveBufUsed)
                {
                    _saveBuf = new byte[81920];
                    _saveBufUsed = 0;
                }

                var save = _saveBuf.AsMemory(_saveBufUsed, check.Length);
                _saveBufUsed += check.Length;
                check.CopyTo(save);

                _inner.Add(save, s = Encoding.UTF8.GetString(bytes));
                return s;
            }

            private sealed class ByteArrayEqualityComparer : EqualityComparer<ReadOnlyMemory<byte>>
            {
                public static readonly ByteArrayEqualityComparer Instance = new ByteArrayEqualityComparer();

                private ByteArrayEqualityComparer() { }

                public override bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y)
                {
                    return x.Span.SequenceEqual(y.Span);
                }

                public override int GetHashCode(ReadOnlyMemory<byte> obj)
                {
                    uint h = 17;
                    foreach (byte b in obj.Span)
                    {
                        h = (h * 31) + b;
                    }

                    return (int)h;
                }
            }
        }
    }
}
