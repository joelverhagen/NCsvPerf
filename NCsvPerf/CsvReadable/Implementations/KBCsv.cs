using System.Collections.Generic;
using System.IO;
using KBCsv;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/KBCsv/
    /// Source: https://github.com/kentcb/KBCsv
    /// </summary>
    public class KBCsv : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader))
            {
                while (csvReader.HasMoreRecords)
                {
                    var row = csvReader.ReadDataRecord();

                    var record = new T();
                    record.Read(i => row[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
