using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/mgholam.fastCSV/
    /// Source: https://github.com/mgholam/fastCSV
    /// </summary>
    public class mgholam_fastCSV : ICsvReader
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
