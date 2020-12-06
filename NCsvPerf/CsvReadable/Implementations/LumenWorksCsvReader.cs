using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    public class LumenWorksCsvReader : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public LumenWorksCsvReader(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            using (var csvReader = new LumenWorks.Framework.IO.Csv.CsvReader(reader, hasHeaders: false))
            {
                while (csvReader.ReadNextRecord())
                {
                    var record = activate();
                    record.Read(i => csvReader[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
