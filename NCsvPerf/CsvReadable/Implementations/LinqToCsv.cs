using System.Collections.Generic;
using System.IO;
using LINQtoCSV;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/LinqToCsv
    /// Source: https://www.codeproject.com/Articles/25133/LINQ-to-CSV-library
    /// </summary>
    public class LinqToCsv : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var desc = new CsvFileDescription
                {
                    FirstLineHasColumnNames = false,
                };
                var cc = new CsvContext();
                foreach (var row in cc.Read<DataRow>(reader, desc))
                {
                    var record = new T();
                    record.Read(i => row[i].Value ?? string.Empty);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }

        private class DataRow : List<DataRowItem>, IDataRow
        {
        }
    }
}
