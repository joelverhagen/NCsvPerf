using System.Collections.Generic;
using System.IO;
using Dsv;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Dsv
    /// Source: https://github.com/atifaziz/Dsv
    /// </summary>
    public class Dsv : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public Dsv(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);

            using (var reader = new StreamReader(stream))
            {
                var lines = EnumerateLines(reader);
                foreach (var row in lines.ParseCsv())
                {
                    var record = activate();
                    record.Read(i => row[i]);
                    yield return record;
                }
            }
        }

        static IEnumerable<string> EnumerateLines(TextReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}
