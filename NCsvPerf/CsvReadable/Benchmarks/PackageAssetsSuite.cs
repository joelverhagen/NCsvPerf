using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;

namespace Knapcode.NCsvPerf.CsvReadable.TestCases
{
    [MemoryDiagnoser]
    [SimpleJob(1, 2, 6, 1)]
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

        public static IReadOnlyList<int> LineCountSource { get; } =
            new[] { 
                //0, 1, 10, 100, 1_000, 10_000, 100_000, 
                1_000_000
            };

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

                if (result.Count != LineCount)
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
        public void Angara_Table()
        {
            Execute(new Angara_Table());
        }

        [Benchmark]
        public void Cesil()
        {
            Execute(new Cesil());
        }

        [Benchmark]
        public void ChoETL()
        {
            Execute(new ChoETL());
        }

        [Benchmark]
        public void CommonLibrary_NET()
        {
            Execute(new CommonLibrary_NET());
        }

        [Benchmark]
        public void Csv()
        {
            Execute(new Csv());
        }

        [Benchmark]
        public void CSVFile()
        {
            Execute(new CSVFile());
        }

        [Benchmark]
        public void CsvHelper()
        {
            Execute(new CsvHelper());
        }

        [Benchmark]
        public void CsvTextFieldParser()
        {
            Execute(new CsvTextFieldParser());
        }

        [Benchmark]
        public void CsvTools()
        {
            Execute(new CsvTools());
        }

        [Benchmark]
        public void Ctl_Data()
        {
            Execute(new Ctl_Data());
        }

        [Benchmark]
        public void Cursively()
        {
            Execute(new Cursively());
        }

        [Benchmark]
        public void Dsv()
        {
            Execute(new Dsv());
        }

        [Benchmark]
        public void FastCsvParser()
        {
            Execute(new FastCsvParser());
        }

        [Benchmark]
        public void FileHelpers()
        {
            Execute(new FileHelpers());
        }

        [Benchmark]
        public void FlatFiles()
        {
            Execute(new FlatFiles());
        }

        [Benchmark]
        public void FluentCSV()
        {
            Execute(new FluentCSV());
        }

        [Benchmark]
        public void GenericParsing()
        {
            Execute(new GenericParsing());
        }

        [Benchmark]
        public void HomeGrown()
        {
            Execute(new HomeGrown());
        }

        [Benchmark]
        public void HomeGrown2()
        {
            Execute(new HomeGrown2());
        }

        [Benchmark]
        public void KBCsv()
        {
            Execute(new KBCsv());
        }

        [Benchmark]
        public void LinqToCsv()
        {
            Execute(new LinqToCsv());
        }

        [Benchmark]
        public void LumenWorksCsvReader()
        {
            Execute(new LumenWorksCsvReader());
        }

        [Benchmark]
        public void mgholam_fastCSV()
        {
            Execute(new mgholam_fastCSV());
        }

        [Benchmark]
        public void Microsoft_ML()
        {
            Execute(new Microsoft_ML());
        }

        [Benchmark]
        public void Microsoft_Data_Analysis()
        {
            Execute(new Microsoft_Data_Analysis());
        }

        [Benchmark]
        public void Microsoft_VisualBasic_FileIO_TextFieldParser()
        {
            Execute(new Microsoft_VisualBasic_FileIO_TextFieldParser());
        }

        [Benchmark]
        public void NReco_Csv()
        {
            Execute(new NReco_Csv());
        }

        [Benchmark]
        public void Open_Text_CSV()
        {
            Execute(new Open_Text_CSV());
        }

        [Benchmark]
        public void RecordParser()
        {
            Execute(new RecordParser());
        }

        [Benchmark]
        public void RecordParserParallel()
        {
            Execute(new RecordParserParallel());
        }

        [Benchmark]
        public void Sep()
        {
            Execute(new Sep());
        }

        [Benchmark]
        public void ServiceStack_Text()
        {
            Execute(new ServiceStack_Text());
        }

        [Benchmark]
        public void Sky_Data_Csv()
        {
            Execute(new Sky_Data_Csv());
        }

        [Benchmark]
        public void StackOverflowRegex()
        {
            Execute(new StackOverflowRegex());
        }

        [Benchmark]
        public void string_Split()
        {
            Execute(new string_Split());
        }

        [Benchmark]
        public void SoftCircuits_CsvParser()
        {
            Execute(new SoftCircuits_CsvParser());
        }

        [Benchmark]
        public void Sylvan_Data_Csv()
        {
            Execute(new Sylvan_Data_Csv());
        }

        [Benchmark]
        public void TinyCsvReader()
        {
            Execute(new TinyCsvParser());
        }

        [Benchmark]
        public void TxtCsvHelper()
        {
            Execute(new TxtCsvHelper());
        }
    }
}
