using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/SoftCircuits.CsvParser/
    /// Source: https://github.com/SoftCircuits/CsvParser
    /// </summary>
    public class SoftCircuits_CsvParser : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public SoftCircuits_CsvParser(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new SoftCircuits.CsvParser.CsvReader(stream))
            {
                string[] columns = null;
                while (reader.ReadRow(ref columns))
                {
                    var record = activate();
                    record.Read(i => columns[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
