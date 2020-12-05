using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    public class TinyCsvReader : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var options = new TinyCsvParser.Tokenizer.RFC4180.Options('"', '"', ',');
                var tokenizer = new TinyCsvParser.Tokenizer.RFC4180.RFC4180Tokenizer(options);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var record = new T();
                    var fields = tokenizer.Tokenize(line);
                    record.Read(i => fields[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
