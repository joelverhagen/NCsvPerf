using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ben.Collections.Specialized;
using RecordParser.Extensions;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/recordparser
    /// Source: https://github.com/leandromoh/RecordParser
    /// </summary>
    public class RecordParser : ICsvReader
    {
        private readonly bool _parallel;
        private readonly bool _ensureOriginalOrdering;

        public RecordParser() : this(parallel: false, ensureOriginalOrdering: true)
        {
        }

        public RecordParser(bool parallel, bool ensureOriginalOrdering)
        {
            _parallel = parallel;
            _ensureOriginalOrdering = ensureOriginalOrdering;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            using var streamReader = new StreamReader(stream);

            var options = new VariableLengthReaderRawOptions
            {
                HasHeader = false,
                ContainsQuotedFields = false,
                Trim = false,

                ColumnCount = new T().GetColumnCount(),
                Separator = ",",
#if ENABLE_STRING_POOLING
                StringPoolFactory = () => new InternPool().Intern,
#endif
                ParallelismOptions = new()
                {
                    Enabled = _parallel,
                    EnsureOriginalOrdering = _ensureOriginalOrdering,
                },
            };

            var items = streamReader.ReadRecordsRaw(options, getField =>
            {
                var record = new T();
                record.Read(getField);
                return record;
            });

            return items.ToList();
        }
    }
}