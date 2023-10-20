using Ben.Collections.Specialized;
using RecordParser.Extensions.FileReader;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                StringPoolFactory = () => new InternPool().Intern,
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