using Ben.Collections.Specialized;
using Knapcode.NCsvPerf.HomeGrown;
using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: N/A
    /// Source: see HomeGrownImproved.cs in this repository
    /// </summary>
    public class HomeGrown2 : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public HomeGrown2(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();
            var fields = new List<string>();

            using (var reader = new StreamReader(stream))
            {
                var parser = new HomeGrownImproved(buffer: new char[200], stringPool: new InternPool().Intern);

                while (parser.TryReadLine(reader, fields))
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
