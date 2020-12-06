using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: N/A
    /// Source: here :)
    /// </summary>
    public class StringSplitCsvReader : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public StringSplitCsvReader(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var pieces = line.Split(',');
                    var record = activate();
                    record.Read(i => pieces[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
