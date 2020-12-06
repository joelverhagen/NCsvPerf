using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    public class CsvHelperCsvReader : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public CsvHelperCsvReader(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var csvReader = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
                while (csvReader.Read())
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
