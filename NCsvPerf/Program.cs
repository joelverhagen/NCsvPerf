using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Knapcode.NCsvPerf.CsvReadable.TestCases;

namespace Knapcode.NCsvPerf
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfig config = null;
#if DEBUG
            config = new DebugInProcessConfig();
#endif
            BenchmarkRunner.Run<PackageAssetsSuite>(config);
        }
    }
}
