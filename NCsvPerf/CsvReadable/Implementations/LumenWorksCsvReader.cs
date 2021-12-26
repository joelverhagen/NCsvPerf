using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/LumenWorksCsvReader/
    /// Source: https://github.com/phatcher/CsvReader
    /// </summary>
    public class LumenWorksCsvReader : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public LumenWorksCsvReader(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);

            using (var reader = new StreamReader(stream))
            using (var csvReader = new LumenWorks.Framework.IO.Csv.CsvReader(reader, hasHeaders: false))
            {
                while (csvReader.ReadNextRecord())
                {
                    var record = activate();
                    record.Read(i => csvReader[i]);
                    yield return record;
                }
            }
        }
    }
}
