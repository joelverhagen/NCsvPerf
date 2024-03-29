﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using Knapcode.NCsvPerf.HomeGrown;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: N/A
    /// Source: see CsvUtility.cs in this repository
    /// </summary>
    public class HomeGrown : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();
            var fields = new List<string>();
            var builder = new StringBuilder();

            using (var reader = new StreamReader(stream))
            {
                while (CsvUtility.TryReadLine(reader, fields, builder))
                {
                    var record = new T();
                    record.Read(i => fields[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
