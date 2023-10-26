using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Knapcode.NCsvPerf.CsvReadable.TestCases
{
    public class PackageAssetsBenchmarkTest
    {
        private readonly ITestOutputHelper _output;

        public PackageAssetsBenchmarkTest(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> LineCounts
        {
            get
            {
                yield return new object[] { 0 };
                yield return new object[] { 1 };
                yield return new object[] { 100 };
                yield return new object[] { 10_000 };
            }
        }

        [Theory]
        [MemberData(nameof(LineCounts))]
        public void AllBenchmarksHaveSameOutput(int lineCount)
        {
            var data = TestData.PackageAssets.GetBytes(lineCount);
            AllBenchmarksAgree(data, lineCount.ToString());
        }

        public static IEnumerable<object[]> Files
        {
            get
            {
                yield return new[] { "Quoted.csv" };
                yield return new[] { "QuotedComma.csv" };
                yield return new[] { "QuotedNewLine.csv" };
                yield return new[] { "QuotedQuote.csv" };
            }
        }

        [Theory]
        [MemberData(nameof(Files))]
        public void AllBenchmarksHaveSameOutput2(string file)
        {
            var data = File.ReadAllBytes(file);
            AllBenchmarksAgree(data, Path.GetFileNameWithoutExtension(file));
        }

        void AllBenchmarksAgree(byte[] data, string id)
        {
            // Arrange
            var suite = new PackageAssetsSuite(saveResult: true);
            suite.fileId = id;
            var benchmarks = suite
                .GetType()
                .GetMethods()
                .Where(x => x.GetCustomAttributes(typeof(BenchmarkAttribute), inherit: false).Any())
                // the naive string.Split implementation is not expected
                // to produce correct results.
                .Where(m => m.Name != "string_Split")
                .ToList();
            var results = new Dictionary<string, List<PackageAsset>>();

            // Act
            foreach (var benchmark in benchmarks)
            {
                suite.SetData(data);
                try
                {
                    benchmark.Invoke(suite, null);
                    results.Add(benchmark.Name, suite.LatestResult);
                }
                catch
                {
                    results.Add(benchmark.Name, null);
                }
            }

            static string GetValue(List<PackageAsset> assets)
            {
                return assets == null
                    ? "Failed"
                    : assets.Count == 0
                    ? "No records"
                    : assets[0].Id;
            }

            // Assert
            var groups = results
                .GroupBy(p => JsonConvert.SerializeObject(p.Value, Formatting.Indented), p => new { Key = p.Key,  Id = GetValue(p.Value) })
                .OrderByDescending(x => x.Count())
                .ToList();

            var number = 0;
            foreach (var group in groups)
            {
                number++;
                var value = group.First().Id;
                _output.WriteLine($"Group #{number} (result JSON length = {group.Key.Length}, Output = {Escape(value)}):");
                File.WriteAllText($"group-{number}.json", group.Key);
                foreach (var benchmark in group)
                {

                    _output.WriteLine($"  - {benchmark.Key}");
                }
                _output.WriteLine(string.Empty);
            }

            Assert.Single(groups);
        }

        static string Escape(string s)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                var c = s[i];
                if (char.IsControl(c))
                {
                    sb.Append($"\\x{(int)c:x2}");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
