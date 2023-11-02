using System.Collections.Generic;
using System.IO;
using GenericParsing;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/GenericParsing/
    /// Source: https://github.com/AndrewRissing/GenericParsing
    /// </summary>
    public class GenericParsing : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            using (var parser = new GenericParser(reader))
            {
                parser.FirstRowHasHeader = false;

                while (parser.Read())
                {
                    var record = new T();
                    record.Read(i => parser[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
