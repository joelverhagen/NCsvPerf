using System.Collections.Generic;
using System.IO;
using Sylvan;
using KBCsv;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/KBCsv/
    /// Source: https://github.com/kentcb/KBCsv
    /// </summary>
    public class KB_Csv : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public KB_Csv(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();
            var stringPool = new StringPool(128);

            using (var reader = new StreamReader(stream))
            using (var csvReader = new CsvReader(reader))
            {
                while (csvReader.HasMoreRecords)
                {
                    var row = csvReader.ReadDataRecord();

                    var record = activate();
                    record.Read(i => row[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
