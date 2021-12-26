using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Csv/
    /// Source: https://github.com/stevehansen/csv/
    /// </summary>
    public class Csv : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public Csv(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);

            using (var reader = new StreamReader(stream))
            {
                var options = new global::Csv.CsvOptions
                {
                    HeaderMode = global::Csv.HeaderMode.HeaderAbsent,
                    Separator = ',',
                };

                foreach (var row in global::Csv.CsvReader.Read(reader, options))
                {
                    var record = activate();
                    record.Read(i => row[i]);
                    yield return record;
                }
            }
        }
    }
}
