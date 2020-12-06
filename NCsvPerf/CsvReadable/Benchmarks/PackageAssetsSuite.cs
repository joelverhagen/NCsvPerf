using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        [Benchmark]
        [ArgumentsSource(nameof(ActivationMethods))]
        public void CsvHelperCsvReader(ActivationMethod activationMethod)
        {
            Execute(new CsvHelperCsvReader(activationMethod));
        }

        [Benchmark]
        [ArgumentsSource(nameof(ActivationMethods))]
        public void FastCsvParserCsvReader(ActivationMethod activationMethod)
        {
            Execute(new FastCsvParserCsvReader(activationMethod));
        }

        [Benchmark]
        [ArgumentsSource(nameof(ActivationMethods))]
        public void HomeGrownCsvReader(ActivationMethod activationMethod)
        {
            Execute(new HomeGrownCsvReader(activationMethod));
        }

        [Benchmark]
        [ArgumentsSource(nameof(ActivationMethods))]
        public void LumenWorksCsvReader(ActivationMethod activationMethod)
        {
            Execute(new LumenWorksCsvReader(activationMethod));
        }

        [Benchmark]
        public void MgholamFastCsvReader()
        {
            Execute(new MgholamFastCsvReader());
        }

        [Benchmark]
        [ArgumentsSource(nameof(ActivationMethods))]
        public void NRecoCsvReader(ActivationMethod activationMethod)
        {
            Execute(new NRecoCsvReader(activationMethod));
        }

        [Benchmark]
        [ArgumentsSource(nameof(ActivationMethods))]
        public void ServiceStackTextCsvReader(ActivationMethod activationMethod)
        {
            Execute(new ServiceStackTextCsvReader(activationMethod));
        }

        [Benchmark]
        [ArgumentsSource(nameof(ActivationMethods))]
        public void StringSplitCsvReader(ActivationMethod activationMethod)
        {
            Execute(new StringSplitCsvReader(activationMethod));
        }

        [Benchmark]
        [ArgumentsSource(nameof(ActivationMethods))]
        public void TinyCsvReader(ActivationMethod activationMethod)
        {
            Execute(new TinyCsvReader(activationMethod));
        }

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

        public IEnumerable<ActivationMethod> ActivationMethods => Enum
            .GetValues(typeof(ActivationMethod))
            .Cast<ActivationMethod>();
    }
}
