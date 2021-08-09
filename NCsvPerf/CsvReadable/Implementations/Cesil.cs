using Cesil;
using Knapcode.NCsvPerf.CsvReadable.TestCases;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Cesil/
    /// Source: https://github.com/kevin-montrose/cesil
    /// </summary>
    public class Cesil : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public Cesil(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            // specialize for T == PackageAsset
            return GetAssets(stream) as List<T>;
        }

        public List<PackageAsset> GetAssets(MemoryStream stream) {
            var allRecords = new List<PackageAsset>();
            using (var reader = new StreamReader(stream))
            {
                var config = Configuration.For<PackageAsset>();
                var csv = config.CreateReader(reader);
                foreach (var row in csv.EnumerateAll())
                {
                    allRecords.Add(row);
                    // for some reason this final field is null for certain records
                    // which was causing tests to fail
                    if (row.PlatformVersion == null)
                        row.PlatformVersion = "";
                }
            }

            return allRecords;
        }
    }
}
