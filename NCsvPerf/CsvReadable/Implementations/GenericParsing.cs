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
        private readonly ActivationMethod _activationMethod;

        public GenericParsing(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            using (var parser = new GenericParser(reader))
            {
                parser.FirstRowHasHeader = false;

                while (parser.Read())
                {
                    var record = activate();
                    record.Read(i => parser[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
