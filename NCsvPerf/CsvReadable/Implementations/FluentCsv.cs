using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/FluentCSV/
    /// Source: https://github.com/aboudoux/FluentCSV/
    /// </summary>
    public class FluentCSV : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var splitter = new global::FluentCsv.CsvParser.Splitters.Rfc4180DataSplitter();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var row = splitter.SplitColumns(line, ",");
                    var record = new T();
                    record.Read(i => row[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
