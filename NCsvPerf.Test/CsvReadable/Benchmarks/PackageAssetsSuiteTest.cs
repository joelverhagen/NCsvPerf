using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
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

        [Theory]
        [MemberData(nameof(TestData))]
        public void AllBenchmarksHaveSameOutput(int lineCount)
        {
            // Arrange
            var benchmarks = new PackageAssetsSuite()
                .GetType()
                .GetMethods()
                .Where(x => x.GetCustomAttributes(typeof(BenchmarkAttribute), inherit: false).Any())
                .ToList();
            var results = new ConcurrentDictionary<string, List<PackageAsset>>();

            // Act
            Parallel.ForEach(benchmarks, benchmark =>
            {
                var suite = new PackageAssetsSuite(saveResult: true);
                suite.LineCount = lineCount;
                suite.GlobalSetup();
                benchmark.Invoke(suite, null);
                results.TryAdd(benchmark.Name, suite.LatestResult);
            });

            // Assert
            var groups = results
                .GroupBy(p => JsonConvert.SerializeObject(p.Value, Formatting.Indented), p => p.Key)
                .OrderByDescending(x => x.Count())
                .ToList();
            var number = 0;
            foreach (var group in groups)
            {
                number++;
                _output.WriteLine($"Group #{number} (result JSON length = {group.Key.Length}):");
                File.WriteAllText($"group-{number}.json", group.Key);
                foreach (var benchmark in group.Order(StringComparer.OrdinalIgnoreCase))
                {
                    _output.WriteLine($"  - {benchmark}");
                }
                _output.WriteLine(string.Empty);
            }

            Assert.Single(groups);
        }

        public static IEnumerable<object[]> TestData
        {
            get {
                yield return new object[] { 0 };
                yield return new object[] { 1 };
                yield return new object[] { 100 };
                yield return new object[] { 10_000 };
                yield return new object[] { 100_000 };
            }
        }
    }
}
