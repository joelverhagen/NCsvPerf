using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Knapcode.NCsvPerf.CsvReadable.TestCases;

namespace Knapcode.NCsvPerf
{
    public class Program
    {
        private static void Main(string[] args)
        {
            IConfig config = null;
#if DEBUG
            config = new DebugInProcessConfig();
#else
            config = null;
#endif
            BenchmarkRunner.Run<PackageAssetsSuite>(config);
        }
    }
}
