using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: N/A
    /// Source: here :)
    /// </summary>
    public class string_Split : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var pieces = line.Split(',');
                    var record = new T();
                    record.Read(i => pieces[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
