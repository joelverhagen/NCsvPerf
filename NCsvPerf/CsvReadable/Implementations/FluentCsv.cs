using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/FluentCSV/
    /// Source: https://github.com/aboudoux/FluentCSV/
    /// </summary>
    public class FluentCSV : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public FluentCSV(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var splitter = new global::FluentCsv.CsvParser.Splitters.Rfc4180DataSplitter();
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var row = splitter.SplitColumns(line, ",");
                    var record = activate();
                    record.Read(i => row[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
