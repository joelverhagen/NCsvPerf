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
        private readonly ActivationMethod _activationMethod;

        public CsvTextFieldParser(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);

            using (var reader = new StreamReader(stream))
            {
                var parser = new NotVisualBasic.FileIO.CsvTextFieldParser(reader);
                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    var record = activate();
                    record.Read(i => fields[i]);
                    yield return record;
                }
            }
        }
    }
}
