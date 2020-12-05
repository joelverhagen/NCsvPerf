using System;

namespace Knapcode.NCsvPerf.CsvReadable
{
    public interface ICsvReadable
    {
        public int GetColumnCount();
        public void Read(Func<int, string> getField);
    }
}
