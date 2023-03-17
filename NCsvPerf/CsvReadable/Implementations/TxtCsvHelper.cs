using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/TxtCsvHelper/
    /// Source: https://github.com/camdrudge/TxtCsvHelper
    /// </summary>
    class TxtCsvHelper : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public TxtCsvHelper(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();
            using (var reader = new StreamReader(stream))
            using (var txt = new global::TxtCsvHelper.Parser())
            {

                while (reader.Peek() >= 0)
                {

                    var strings = txt.MixedSplit(reader.ReadLine()).ToList();
                    var record = activate();
                    record.Read(i => strings[i]);
                    allRecords.Add(record);
                }
            }
            return allRecords;
        }
    }
}