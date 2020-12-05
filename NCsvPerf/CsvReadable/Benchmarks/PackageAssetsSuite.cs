using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable.TestCases
{
    public class PackageAssetsSuite
    {
        private readonly byte[] _bytes;
        private readonly bool _saveResult;

        public PackageAssetsSuite() : this(saveResult: false)
        {
        }

        public PackageAssetsSuite(bool saveResult)
        {
            _bytes = File.ReadAllBytes(Path.Combine("TestData", "PackageAssets.csv"));
            _saveResult = saveResult;
        }

        public List<PackageAsset> LatestResult { get; private set; }

        [Benchmark] public void CsvHelperCsvReader() => Execute(new CsvHelperCsvReader());
        [Benchmark] public void FastCsvParserCsvReader() => Execute(new FastCsvParserCsvReader());
        [Benchmark] public void HomeGrownCsvReader() => Execute(new HomeGrownCsvReader());
        [Benchmark] public void LumenWorksCsvReader() => Execute(new LumenWorksCsvReader());
        [Benchmark] public void MgholamFastCsvReader() => Execute(new MgholamFastCsvReader());
        [Benchmark] public void NRecoCsvReader() => Execute(new NRecoCsvReader());
        [Benchmark] public void ServiceStackTextCsvReader() => Execute(new ServiceStackTextCsvReader());
        [Benchmark] public void TinyCsvReader() => Execute(new TinyCsvReader());

        private void Execute(ICsvReader reader)
        {
            using (var memoryStream = new MemoryStream(_bytes, writable: false))
            {
                var result = reader.GetRecords<PackageAsset>(memoryStream);
                if (_saveResult)
                {
                    LatestResult = result;
                }
            }
        }
    }
}
