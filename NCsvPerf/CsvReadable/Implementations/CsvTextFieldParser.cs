using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/CsvTextFieldParser/
    /// Source: https://github.com/22222/CsvTextFieldParser
    /// </summary>
    public class CsvTextFieldParser : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var parser = new NotVisualBasic.FileIO.CsvTextFieldParser(reader);
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    var record = new T();
                    record.Read(i => fields[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
