using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Csv/
    /// Source: https://github.com/stevehansen/csv/
    /// </summary>
    public class ReallySimpleCsvReader : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public ReallySimpleCsvReader(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var options = new Csv.CsvOptions
                {
                    HeaderMode = Csv.HeaderMode.HeaderAbsent,
                    Separator = ',',
                };

                foreach (var row in Csv.CsvReader.Read(reader, options))
                {
                    var record = activate();
                    record.Read(i => row[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
