using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Sky.Data.Csv/
    /// Source: https://github.com/fengzhenqiong/Sky.Data.Csv
    /// </summary>
    public class Sky_Data_Csv : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public Sky_Data_Csv(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);

            var csvReader = Sky.Data.Csv.CsvReader.Create(stream);
            foreach (var row in csvReader)
            {
                var record = activate();
                record.Read(i => row[i]);
                yield return record;
            }
        }
    }
}
