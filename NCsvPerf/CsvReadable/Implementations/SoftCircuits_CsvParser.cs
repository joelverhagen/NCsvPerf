using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/SoftCircuits.CsvParser/
    /// Source: https://github.com/SoftCircuits/CsvParser
    /// </summary>
    public class SoftCircuits_CsvParser : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new SoftCircuits.CsvParser.CsvReader(stream))
            {
                while (reader.Read())
                {
                    var record = new T();
                    record.Read(i => reader.Columns[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
