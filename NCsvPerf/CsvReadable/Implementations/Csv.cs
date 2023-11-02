using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Csv/
    /// Source: https://github.com/stevehansen/csv/
    /// </summary>
    public class Csv : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var options = new global::Csv.CsvOptions
                {
                    HeaderMode = global::Csv.HeaderMode.HeaderAbsent,
                    Separator = ',',
                };

                foreach (var row in global::Csv.CsvReader.Read(reader, options))
                {
                    var record = new T();
                    record.Read(i => row[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
