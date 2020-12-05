using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Knapcode.NCsvPerf.CsvReadable
{
    public class FastCsvParserCsvReader : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var parser = new CsvParser.CsvReader(stream, Encoding.UTF8))
            {
                while (parser.MoveNext())
                {
                    var record = new T();
                    record.Read(i => parser.Current[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
