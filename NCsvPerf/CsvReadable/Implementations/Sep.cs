using System;
using System.Collections.Generic;
using System.IO;
using nietras.SeparatedValues;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Sep/
    /// Source: https://github.com/nietras/Sep
    /// </summary>
    public class Sep : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public Sep(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            using var reader = nietras.SeparatedValues.Sep.Reader(o => o with
            {
                HasHeader = false,
                CreateToString = SepToString.PoolPerCol(maximumStringLength: 128),
            })
            .From(stream);

            // Due to how NCsvPerf code is factored and that Sep uses ref
            // structs, the code below is different from normal Sep usage.
            Func<int, string> toString = reader.ToString;
            foreach (var row in reader)
            {
                var record = activate();
                record.Read(toString);
                allRecords.Add(record);
            }

            return allRecords;
        }
    }
}
