using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/NReco.Csv/
    /// Source: https://github.com/nreco/csv
    /// </summary>
    public class NReco_Csv : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var csvReader = new NReco.Csv.CsvReader(reader);
                while (csvReader.Read())
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
