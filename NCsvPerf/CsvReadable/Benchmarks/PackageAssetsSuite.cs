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
                    throw new InvalidDataException($"ICsvReader '{reader.GetType().FullName}' failed to produce correct number of rows. Expected: {LineCount}, actual: {result.Count}.");
                }
                
                if (_saveResult)
                {
                    LatestResult = result;
                }
            }
        }

        [Benchmark]
        public void CsvHelper()
        {
            Execute(new CsvHelper(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void CsvTextFieldParser()
        {
            Execute(new CsvTextFieldParser(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Cursively()
        {
            Execute(new Cursively(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void FastCsvParser()
        {
            Execute(new FastCsvParser(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void FluentCsv()
        {
            Execute(new FluentCsv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void HomeGrown()
        {
            Execute(new HomeGrown(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void LumenWorksCsvReader()
        {
            Execute(new LumenWorksCsvReader(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void mgholam_fastCSV()
        {
            Execute(new mgholam_fastCSV());
        }

        [Benchmark]
        public void NReco_Csv()
        {
            Execute(new NReco_Csv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void ServiceStack_Text()
        {
            Execute(new ServiceStack_Text(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Csv()
        {
            Execute(new Csv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void string_Split()
        {
            Execute(new string_Split(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Sylvan_Data_Csv()
        {
            Execute(new Sylvan_Data_Csv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void TinyCsvReader()
        {
            Execute(new TinyCsvParser(ActivationMethod.ILEmit));
        }
    }
}
