using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable.TestCases
{
    [MemoryDiagnoser]
    public class PackageAssetsSuite
    {
        private byte[] _bytes;
        private readonly bool _saveResult;

        public PackageAssetsSuite() : this(saveResult: false)
        {
        }

        public PackageAssetsSuite(bool saveResult)
        {
            _saveResult = saveResult;
        }

        public List<PackageAsset> LatestResult { get; private set; }

        [ParamsSource(nameof(LineCountSource))]
        public int LineCount { get; set; }

        public static IReadOnlyList<int> LineCountSource { get; } = new[] { 0, 1, 10, 100, 1_000, 10_000, 100_000, 1_000_000 };

        [GlobalSetup]
        public void GlobalSetup()
        {
            _bytes = TestData.PackageAssets.GetBytes(LineCount);
        }

        private void Execute(ICsvReader reader)
        {
            using (var memoryStream = new MemoryStream(_bytes, writable: false))
            {
                var result = reader.GetRecords<PackageAsset>(memoryStream);
                if(result.Count != LineCount)
                {
                    throw new System.Exception("Failed to produce correct number of rows");
                }
                if (_saveResult)
                {
                    LatestResult = result;
                }
            }
        }

        [Benchmark(Baseline = true)]
        public void CsvHelperCsvReader()
        {
            Execute(new CsvHelper(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void CsvTextFieldParserCsvReader()
        {
            Execute(new CsvTextFieldParser(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void FastCsvParserCsvReader()
        {
            Execute(new FastCsvParser(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void HomeGrownCsvReader()
        {
            Execute(new Knapcode_Csv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void LumenWorksCsvReader()
        {
            Execute(new LumenWorksCsvReader(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void MgholamFastCsvReader()
        {
            Execute(new mgholam_fastCSV());
        }

        [Benchmark]
        public void NRecoCsvReader()
        {
            Execute(new NReco_Csv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void ServiceStackTextCsvReader()
        {
            Execute(new ServiceStack_Text(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void ReallySimpleCsvReader()
        {
            Execute(new Csv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void StringSplitCsvReader()
        {
            Execute(new string_Split(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void TinyCsvReader()
        {
            Execute(new TinyCsvParser(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void SylvanCsvReader()
        {
            Execute(new SylvanCsv(ActivationMethod.ILEmit));
        }
    }
}
