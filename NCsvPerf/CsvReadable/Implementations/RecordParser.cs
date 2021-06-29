using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/recordparser
    /// Source: https://github.com/leandromoh/RecordParser
    /// </summary>
    public class RecordParser : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public RecordParser(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var reader = BuildReader(activate);
            var result = ProcessStream<T>(stream, spanLine =>
            {
                var fields = reader.Parse(spanLine);
                var record = activate();

                record.Read(i => fields[i]);

                return record;
            });

            return result;
        }

        private static global::RecordParser.Parsers.IVariableLengthReader<string[]> BuildReader<T>(Activate<T> activate) where T : ICsvReadable
        {
            var columnCount = activate().GetColumnCount();
            var buffer = new string[columnCount];
            var builder = new global::RecordParser.Builders.Reader.VariableLengthReaderBuilder<string[]>();

            for (var i = 0; i < buffer.Length; i++)
                builder.Map(buildExpression(i), i);

            var cache = new Dictionary<int, string>()
            {
                [string.GetHashCode(Span<char>.Empty)] = string.Empty
            };

            builder.DefaultTypeConvert(GetOrAddString);

            var reader = builder.Build(",", factory: () => buffer);

            return reader;

            string GetOrAddString(ReadOnlySpan<char> span)
            {
                var key = string.GetHashCode(span);

                return cache.TryGetValue(key, out var text)
                ? text
                : cache[key] = new string(span);
            }

            static Expression<Func<string[], string>> buildExpression(int i)
            {
                var arrayExpr = Expression.Parameter(typeof(string[]));
                var indexExpr = Expression.Constant(i);
                var arrayAccessExpr = Expression.ArrayAccess(arrayExpr, indexExpr);

                return Expression.Lambda<Func<string[], string>>(arrayAccessExpr, arrayExpr);
            }
        }

        private static List<T> ProcessStream<T>(MemoryStream stream, FuncSpanT<T> parser)
        {
            var reader = PipeReader.Create(stream);
            var records = new List<T>();

            while (true)
            {
                ReadResult read = reader.ReadAsync().Result;
                ReadOnlySequence<byte> buffer = read.Buffer;
                while (TryReadLine(ref buffer, out ReadOnlySequence<byte> sequence))
                {
                    var item = ProcessSequence(sequence, parser);

                    records.Add(item);
                }

                reader.AdvanceTo(buffer.Start, buffer.End);
                if (read.IsCompleted)
                {
                    break;
                }
            }

            return records;
        }

        private static bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            var position = buffer.PositionOf((byte)'\n');
            if (position == null)
            {
                line = default;
                return false;
            }

            line = buffer.Slice(0, position.Value);
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));

            return true;
        }

        private static T ProcessSequence<T>(ReadOnlySequence<byte> sequence, FuncSpanT<T> parser)
        {
            if (sequence.IsSingleSegment)
            {
                return Parse(sequence.FirstSpan, parser);
            }

            var length = (int)sequence.Length;

            Span<byte> span = stackalloc byte[length];

            sequence.CopyTo(span);

            return Parse(span, parser);
        }

        private static T Parse<T>(ReadOnlySpan<byte> bytes, FuncSpanT<T> parser)
        {
            Span<char> chars = stackalloc char[bytes.Length];
            Encoding.UTF8.GetChars(bytes, chars);

            return parser(chars);
        }
    }
}
