using Knapcode.NCsvPerf.HomeGrown;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: N/A
    /// Source: see CsvUtility.cs in this repository
    /// </summary>
    public class HomeGrown : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public HomeGrown(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();
            var fields = new List<string>();
            var builder = new StringBuilder();

            using (var reader = new StreamReader(stream))
            {
                while (CsvUtility.TryReadLine(reader, fields, builder))
                {
                    var record = activate();
                    record.Read(i => fields[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
