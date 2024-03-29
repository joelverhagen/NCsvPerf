﻿using System.Collections.Generic;
using System.Globalization;
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

            using (var reader = new StreamReader(stream))
            {
                var config = new global::CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
#if ENABLE_STRING_POOLING
                    CacheFields = true,
#endif
                };
                var csvParser = new global::CsvHelper.CsvParser(reader, config);
                while (csvParser.Read())
                {
                    var record = new T();
                    record.Read(i => csvParser[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
