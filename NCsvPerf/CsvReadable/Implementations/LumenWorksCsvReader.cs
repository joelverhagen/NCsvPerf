using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/LumenWorksCsvReader/
    /// Source: https://github.com/phatcher/CsvReader
    /// </summary>
    public class LumenWorksCsvReader : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            using (var csvReader = new LumenWorks.Framework.IO.Csv.CsvReader(reader, hasHeaders: false))
            {
                while (csvReader.ReadNextRecord())
                {
                    var record = new T();
                    record.Read(i => csvReader[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
