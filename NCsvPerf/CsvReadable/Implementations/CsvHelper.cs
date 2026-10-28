using System;
using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/CsvHelper/
    /// Source: https://github.com/JoshClose/CsvHelper
    /// </summary>
    public class CsvHelper : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            var config = new global::CsvHelper.CsvReaderOptions
            {
                HasHeader = false,
#if ENABLE_STRING_POOLING
                PoolStrings = true,
#endif
            };
            using var reader = global::CsvHelper.CsvReader.Create(stream, config);
            while (reader.Read())
            {
                var record = new T();
                record.Read(i => reader.GetString(i));
                allRecords.Add(record);
            }

            return allRecords;
        }
    }
}
