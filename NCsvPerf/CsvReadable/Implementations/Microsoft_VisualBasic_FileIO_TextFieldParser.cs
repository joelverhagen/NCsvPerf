using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: N/A
    /// Source: https://github.com/dotnet/runtime/blob/master/src/libraries/Microsoft.VisualBasic.Core/src/Microsoft/VisualBasic/FileIO/TextFieldParser.vb
    /// </summary>
    public class Microsoft_VisualBasic_FileIO_TextFieldParser : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var parser = new TextFieldParser(stream))
            {
                parser.Delimiters = new[] { "," };

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    var record = new T();
                    record.Read(i => fields[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
