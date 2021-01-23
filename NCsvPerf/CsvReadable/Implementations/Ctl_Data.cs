using System.Collections.Generic;
using System.IO;
using Ctl.Data;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Ctl.Data/
    /// Source: https://github.com/ctl-global/ctl-data/
    /// </summary>
    public class Ctl_Data : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public Ctl_Data(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var streamReader = new StreamReader(stream))
            {
                var options = new CsvObjectOptions();
                var csvReader = new CsvReader(streamReader, options);
                while (csvReader.Read())
                {
                    var record = activate();
                    // Empty fields are returned as null by this library. Convert that to empty string to be more
                    // consistent with other libraries.
                    record.Read(i => csvReader.CurrentRow[i].Value ?? string.Empty);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
