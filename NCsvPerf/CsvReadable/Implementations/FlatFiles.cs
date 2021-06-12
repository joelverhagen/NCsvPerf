using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/FlatFiles
    /// Source: https://github.com/jehugaleahsa/FlatFiles
    /// </summary>
    public class FlatFiles : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public FlatFiles(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var csvReader = new global::FlatFiles.SeparatedValueReader(reader);

                while (csvReader.Read())
                {
                    var values = csvReader.GetValues();
                    var record = activate();
                    record.Read(i => values[i]?.ToString() ?? string.Empty);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
