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
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
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
                    var record = new T();
                    record.Read(i => row[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
