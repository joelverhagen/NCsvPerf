using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/FastCsvParser/
    /// Source: https://github.com/bopohaa/CsvParser
    /// 
    /// I had to reference my own fork of this project because "CsvParser.dll" collided with SoftCircuits.CsvParser.
    /// The fork is here: https://www.nuget.org/packages/Knapcode.FastCsvParser/
    /// I notified the owner here: https://github.com/bopohaa/CsvParser/issues/3
    /// </summary>
    public class FastCsvParser : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public FastCsvParser(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var parser = new global::FastCsvParser.CsvReader(stream, Encoding.UTF8))
            {
                while (parser.MoveNext())
                {
                    var record = activate();
                    record.Read(i => parser.Current[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
