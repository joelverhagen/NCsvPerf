using Open.Text.CSV;
using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Open.Text.Csv/
    /// Source: https://github.com/Open-NET-Libraries/Open.Text.CSV/
    /// </summary>
    public class Open_Text_CSV : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();
            using (var reader = new StreamReader(stream))
            {
                var csvReader = new CsvReader(reader);

                IEnumerable<string> fields;
                while ((fields = csvReader.ReadNextRow()) != null)
                {
                    var record = new T();
                    var enu = fields.GetEnumerator();
                    record.Read(i => { enu.MoveNext(); return enu.Current; });
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
