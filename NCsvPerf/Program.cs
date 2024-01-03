using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Knapcode.NCsvPerf.CsvReadable.TestCases;

namespace Knapcode.NCsvPerf
{
    public class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            var config = new DebugInProcessConfig();
#else
            // Remove time units from column values for csv exporter as per:
            // https://github.com/dotnet/BenchmarkDotNet/issues/1207
            var csvExporter = new CsvExporter(
                CsvSeparator.CurrentCulture,
                new SummaryStyle(
                    cultureInfo: System.Globalization.CultureInfo.InvariantCulture,
                    printUnitsInHeader: true,
                    printUnitsInContent: false,
                    timeUnit: Perfolizer.Horology.TimeUnit.Millisecond,
                    sizeUnit: SizeUnit.KB
                ));
            var config = ManualConfig
                .CreateEmpty()
                .AddColumnProvider(DefaultColumnProviders.Instance)
                .AddLogger(ConsoleLogger.Default)
                .AddExporter(csvExporter)
                .AddExporter(HtmlExporter.Default)
                .AddExporter(MarkdownExporter.GitHub);
#endif
            BenchmarkRunner.Run<PackageAssetsSuite>(config, args);
        }
    }
}
