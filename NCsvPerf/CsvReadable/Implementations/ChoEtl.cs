using Knapcode.NCsvPerf.CsvReadable.TestCases;
using System.Collections.Generic;
using System.IO;
using static Knapcode.NCsvPerf.CsvReadable.FileHelpers;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/ChoETL/
    /// Source: https://github.com/Cinchoo/ChoETL
    /// </summary>
    public class ChoETL : ICsvReader
    {
        public ChoETL(ActivationMethod _)
        {
        }

        public IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            return GetAssets(stream) as IEnumerable<T>;
        }

        IEnumerable<PackageAsset> GetAssets(MemoryStream stream)
        {
            var config = new global::ChoETL.ChoCSVRecordConfiguration
            {
                FileHeaderConfiguration = new global::ChoETL.ChoCSVFileHeaderConfiguration
                {
                    HasHeaderRecord = false,
                },
            };

            using (var reader = new StreamReader(stream))
                foreach (var record in new global::ChoETL.ChoCSVReader<PackageAssetData>(reader, config))
                {
                    var asset = new PackageAsset();
                    asset.Read(i => record.GetString(i));
                    yield return asset;
                }
        }
    }
}
