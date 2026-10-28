using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/CsvHelper/
    /// Source: https://github.com/JoshClose/CsvHelper
    /// </summary>
    public class CsvHelper_MT : ICsvReader
    {
        // Each worker thread gets its own reader, reused for every record it processes. Cache a delegate bound to it
        // so the selector doesn't allocate a closure and delegate per record.
        [ThreadStatic] private static global::CsvHelper.CsvReader t_row;
        [ThreadStatic] private static Func<int, string> t_getField;

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var config = new global::CsvHelper.CsvReaderOptions
            {
                HasHeader = false,
#if ENABLE_STRING_POOLING
                PoolStrings = true,
#endif
            };
            using var reader = global::CsvHelper.CsvReader.Create(stream, config);
            return reader.ParallelEnumerate(row =>
            {
                if (!ReferenceEquals(t_row, row))
                {
                    t_row = row;
                    t_getField = row.GetString;
                }

                var record = new T();
                record.Read(t_getField);
                return record;
            }).ToList();
        }
    }
}
