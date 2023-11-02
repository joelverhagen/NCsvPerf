using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/CSVFile/
    /// Source: https://github.com/tspence/csharp-csv-reader
    /// </summary>
    public class CSVFile : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            var config = new global::CSVFile.CSVSettings
            {
                HeaderRowIncluded = false,
            };
            using (var reader = new StreamReader(stream))
            using (var csvReader = new global::CSVFile.CSVReader(reader, config))
            {
                foreach (var row in csvReader)
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
