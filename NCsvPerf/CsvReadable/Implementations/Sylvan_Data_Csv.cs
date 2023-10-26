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
    public class Sylvan_Data_Csv : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public Sylvan_Data_Csv(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();
#if ENABLE_STRING_POOLING
            var stringPool = new StringPool(128);
#endif

            using (var reader = new StreamReader(stream))
            {
                var options = new CsvDataReaderOptions
                {
                    HasHeaders = false,
                    BufferSize = 0x10000,
#if ENABLE_STRING_POOLING
                    StringFactory = stringPool.GetString,
#endif
                };

                var csvReader = CsvDataReader.Create(reader, options);
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
