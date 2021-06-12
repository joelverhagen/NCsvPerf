using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/CommonLibrary.NET/
    /// Source: https://archive.codeplex.com/?p=commonlibrarynet
    /// </summary>
    public class CommonLibrary_Net : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public CommonLibrary_Net(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            string text;
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            if (!string.IsNullOrEmpty(text))
            {
                var doc = ComLib.CsvParse.Csv.LoadText(text, false);
                foreach (var row in doc.Parse())
                {
                    var record = activate();
                    record.Read(i => row[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
