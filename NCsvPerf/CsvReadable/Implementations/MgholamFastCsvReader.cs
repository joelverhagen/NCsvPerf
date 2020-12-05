using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    public class MgholamFastCsvReader : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            using (var reader = new StreamReader(stream))
            {
                var colCount = new T().GetColumnCount();
                return fastCSV.ReadStream<T>(reader, colCount, ',', (obj, cols) =>
                {
                    obj.Read(i => cols[i]);
                    return true;
                });
            }
        }
    }
}
