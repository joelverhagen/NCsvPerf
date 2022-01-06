using System.Collections.Generic;
using System.IO;
using ByteTerrace.Ouroboros.Core;

namespace Knapcode.NCsvPerf.CsvReadable.Implementations
{
    /// <summary>
    /// Package:
    /// Source: https://github.com/ByteTerrace/ByteTerrace.Ouroboros.Core
    /// </summary>
    public class ByteTerrace_Ouroboros_Core : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public ByteTerrace_Ouroboros_Core(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                var csvParser = new CsvReader(',', '"', reader);
                foreach (var rowMemory in csvParser)
                {
                    var record = activate();
                    record.Read(i => new string(rowMemory.Span[i].Span));
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
