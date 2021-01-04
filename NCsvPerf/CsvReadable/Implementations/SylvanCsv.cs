using System.Collections.Generic;
using System.IO;
using Sylvan.Data.Csv;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/NReco.Csv/
    /// Source: https://github.com/nreco/csv
    /// </summary>
    public class SylvanCsv : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public SylvanCsv(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var csvReader = CsvDataReader.Create(reader, new CsvDataReaderOptions() { HasHeaders = false, BufferSize = 0x10000 });
                while (csvReader.Read())
                {
                    var record = activate();
                    record.Read(i => csvReader.GetString(i));
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
