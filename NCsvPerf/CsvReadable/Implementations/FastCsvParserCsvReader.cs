using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/FastCsvParser/
    /// Source: https://github.com/bopohaa/CsvParser
    /// </summary>
    public class FastCsvParserCsvReader : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public FastCsvParserCsvReader(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var parser = new CsvParser.CsvReader(stream, Encoding.UTF8))
            {
                while (parser.MoveNext())
                {
                    var record = activate();
                    record.Read(i => parser.Current[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
