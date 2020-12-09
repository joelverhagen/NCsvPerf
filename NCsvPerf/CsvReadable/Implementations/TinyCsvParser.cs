using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/TinyCsvParser/
    /// Source: https://github.com/bytefish/TinyCsvParser
    /// </summary>
    public class TinyCsvParser : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public TinyCsvParser(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var options = new global::TinyCsvParser.Tokenizer.RFC4180.Options('"', '"', ',');
                var tokenizer = new global::TinyCsvParser.Tokenizer.RFC4180.RFC4180Tokenizer(options);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var record = activate();
                    var fields = tokenizer.Tokenize(line);
                    record.Read(i => fields[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
