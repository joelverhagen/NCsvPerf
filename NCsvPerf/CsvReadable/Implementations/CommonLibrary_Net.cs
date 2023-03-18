using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Knapcode.CommonLibrary.NET/
    /// Source: https://archive.codeplex.com/?p=commonlibrarynet
    /// 
    /// I had to reference my own fork of this project because it did not support LF (Unix-style) line endings and did
    /// not seem to have an official distribution on NuGet.org.
    /// </summary>
    public class CommonLibrary_NET : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public CommonLibrary_NET(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);

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
                    yield return record;
                }
            }
        }
    }
}
