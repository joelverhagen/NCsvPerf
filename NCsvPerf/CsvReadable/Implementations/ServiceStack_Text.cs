using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/ServiceStack.Text/
    /// Source: https://github.com/ServiceStack/ServiceStack.Text
    /// </summary>
    public class ServiceStack_Text : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public ServiceStack_Text(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var record = activate();
                    var fields = ServiceStack.Text.CsvReader.ParseFields(line);
                    // Empty fields are returned as null by this library. Convert that to empty string to be more
                    // consistent with other libraries.
                    record.Read(i => fields[i] ?? string.Empty);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
