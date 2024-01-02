using System.Collections.Generic;
using System.IO;
using System.Linq;
using nietras.SeparatedValues;

namespace Knapcode.NCsvPerf.CsvReadable
{
    public class Sep_MT : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            using var reader = nietras.SeparatedValues.Sep.Reader(o => o with
            {
                HasHeader = false,
#if ENABLE_STRING_POOLING
                CreateToString = SepToString.PoolPerColThreadSafeFixedCapacity(maximumStringLength: 128),
#endif
            })
            .From(stream);

            return reader.ParallelEnumerate(row =>
            {
                var record = new T();
                record.Read(row.UnsafeToStringDelegate);
                return record;
            }).ToList();
        }
    }
}
