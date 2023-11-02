using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Sky.Data.Csv/
    /// Source: https://github.com/fengzhenqiong/Sky.Data.Csv
    /// </summary>
    public class Sky_Data_Csv : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            var csvReader = Sky.Data.Csv.CsvReader.Create(stream);
            foreach (var row in csvReader)
            {
                var record = new T();
                record.Read(i => row[i]);
                allRecords.Add(record);
            }

            return allRecords;
        }
    }
}
