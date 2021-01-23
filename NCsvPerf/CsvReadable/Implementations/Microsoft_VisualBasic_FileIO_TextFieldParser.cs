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
        private readonly ActivationMethod _activationMethod;

        public Microsoft_VisualBasic_FileIO_TextFieldParser(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var parser = new TextFieldParser(stream))
            {
                parser.Delimiters = new[] { "," };

                while (!parser.EndOfData)
                {
                    var fields = parser.ReadFields();
                    var record = activate();
                    record.Read(i => fields[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
