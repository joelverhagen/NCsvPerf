using System.Collections.Generic;
using System.IO;
using Sylvan;
using Sylvan.Data.Csv;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Sylvan.Data.Csv/
    /// Source: https://github.com/MarkPflug/Sylvan
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
            // 64 should fully cover the values in the dataset.
            var stringPool = new StringPool(64); 

            using (var reader = new StreamReader(stream))
            {
                var opts = new CsvDataReaderOptions() {
                    HasHeaders = false,
                    BufferSize = 0x10000,
                    StringFactory = stringPool.GetString
                };

                var csvReader = CsvDataReader.Create(reader, opts);
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
