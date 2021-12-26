﻿using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    public interface ICsvReader
    {
        IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new();
    }
}
