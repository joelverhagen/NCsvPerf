using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/CsvHelper/
    /// Source: https://github.com/JoshClose/CsvHelper
    /// </summary>
    public class CsvHelper : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public CsvHelper(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var csvReader = new global::CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
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
