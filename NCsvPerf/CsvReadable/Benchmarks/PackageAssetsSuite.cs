using System.Collections.Generic;
using System.IO;
using System.Linq;
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

                if (_saveResult)
                {
                    result = LatestResult = result.ToList();
                }

                var count = result.Count();

                if (count != LineCount)
                {
                    throw new InvalidDataException($"ICsvReader '{reader.GetType().FullName}' failed to produce correct number of rows. Expected: {LineCount}, actual: {count}.");
                }
            }
        }

        [Benchmark]
        public void Angara_Table()
        {
            Execute(new Angara_Table(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Cesil()
        {
            Execute(new Cesil(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void ChoETL()
        {
            Execute(new ChoETL(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void CommonLibrary_NET()
        {
            Execute(new CommonLibrary_NET(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Csv()
        {
            Execute(new Csv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void CSVFile()
        {
            Execute(new CSVFile(ActivationMethod.ILEmit));
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
        public void CsvTools()
        {
            Execute(new CsvTools(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Ctl_Data()
        {
            Execute(new Ctl_Data(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Cursively()
        {
            Execute(new Cursively(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Dsv()
        {
            Execute(new Dsv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void FastCsvParser()
        {
            Execute(new FastCsvParser(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void FileHelpers()
        {
            Execute(new FileHelpers(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void FlatFiles()
        {
            Execute(new FlatFiles(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void FluentCSV()
        {
            Execute(new FluentCSV(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void HomeGrown()
        {
            Execute(new HomeGrown(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void KBCsv()
        {
            Execute(new KBCsv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void LinqToCsv()
        {
            Execute(new LinqToCsv(ActivationMethod.ILEmit));
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
        public void Microsoft_ML()
        {
            Execute(new Microsoft_ML(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Microsoft_Data_Analysis()
        {
            Execute(new Microsoft_Data_Analysis(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Microsoft_VisualBasic_FileIO_TextFieldParser()
        {
            Execute(new Microsoft_VisualBasic_FileIO_TextFieldParser(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void NReco_Csv()
        {
            Execute(new NReco_Csv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Open_Text_CSV()
        {
            Execute(new Open_Text_CSV(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void RecordParser()
        {
            Execute(new RecordParser(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void ServiceStack_Text()
        {
            Execute(new ServiceStack_Text(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void Sky_Data_Csv()
        {
            Execute(new Sky_Data_Csv(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void StackOverflowRegex()
        {
            Execute(new StackOverflowRegex(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void string_Split()
        {
            Execute(new string_Split(ActivationMethod.ILEmit));
        }

        [Benchmark]
        public void SoftCircuits_CsvParser()
        {
            Execute(new SoftCircuits_CsvParser(ActivationMethod.ILEmit));
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

        [Benchmark]
        public void TxtCsvHelper()
        {
            Execute(new TxtCsvHelper(ActivationMethod.ILEmit));
        }
    }
}
