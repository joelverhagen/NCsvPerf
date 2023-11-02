using System.Collections.Generic;
using System.IO;
using Sylvan;
using Sylvan.Data.Csv;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Sylvan.Data.Csv/
    /// Source: https://github.com/MarkPflug/Sylvan
    /// </summary>
    public class Sylvan_Data_Csv : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var options = new CsvDataReaderOptions
                {
                    HasHeaders = false,
                    BufferSize = 0x10000,
#if ENABLE_STRING_POOLING
                    StringFactory = new StringPool(128).GetString,
#endif
                };

                var csvReader = CsvDataReader.Create(reader, options);
                while (csvReader.Read())
                {
                    var record = new T();
                    record.Read(i => csvReader.GetString(i));
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
