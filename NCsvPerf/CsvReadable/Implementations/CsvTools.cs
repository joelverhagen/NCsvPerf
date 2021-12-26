using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataAccess;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/CsvTools/
    /// Source: https://github.com/MikeStall/DataTable
    /// </summary>
    public class CsvTools : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public CsvTools(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            if (stream.Length > 0)
            {
                var table = DataTable.New.ReadLazy(stream);

                // Read header as a row.
                var r = activate();
                r.Read(i => table.ColumnNames.ElementAt(i));
                allRecords.Add(r);

                foreach (var row in table.Rows)
                {
                    var record = activate();
                    record.Read(i => row.Values[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
