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
        private readonly ActivationMethod _activationMethod;

        public RecordParser(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            using var streamReader = new StreamReader(stream);

            var options = new VariableLengthReaderRawOptions
            {
                HasHeader = false,
                ContainsQuotedFields = false,
                Trim = false,

                ColumnCount = activate().GetColumnCount(),
                Separator = ",",
#if ENABLE_STRING_POOLING
                StringPoolFactory = () => new InternPool().Intern,
#endif
                ParallelismOptions = new() 
                { 
                    Enabled = true, 
                    EnsureOriginalOrdering = false 
                },
            };

            var items = streamReader.ReadRecordsRaw(options, getField =>
            {
                var record = activate();
                record.Read(getField);
                return record;
            });

            return items.ToList();
        }
    }
}