using System;
using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/FileHelpers/
    /// Source: https://github.com/MarcosMeli/FileHelpers
    /// </summary>
    public class FileHelpers : ICsvReader
    {
        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new StreamReader(stream))
            {
                // bit of a hack, since this only works for T == PackageAsset
                var engine = new global::FileHelpers.FileHelperAsyncEngine<PackageAssetData>();
                using (engine.BeginReadStream(reader))
                {
                    foreach (var item in engine)
                    {
                        // it seems like it would be slow to create a PackageAssetData
                        // and then subsequently copy all the fields to a PackageAsset
                        // but this approach is actually faster than having FileHelpers
                        // bind directly to the PackageAsset.
                        var record = new T();
                        record.Read(i => item.GetString(i));
                        allRecords.Add(record);
                    }
                }
            }

            return allRecords;
        }
        [global::FileHelpers.DelimitedRecord(",")]
        public class PackageAssetData
        {
            public string ScanId { get; set; }
            public string ScanTimestamp { get; set; }
            public string Id { get; set; }
            public string Version { get; set; }
            public string Created { get; set; }
            public string ResultType { get; set; }

            public string PatternSet { get; set; }
            public string PropertyAnyValue { get; set; }
            public string PropertyCodeLanguage { get; set; }
            public string PropertyTargetFrameworkMoniker { get; set; }
            public string PropertyLocale { get; set; }
            public string PropertyManagedAssembly { get; set; }
            public string PropertyMSBuild { get; set; }
            public string PropertyRuntimeIdentifier { get; set; }
            public string PropertySatelliteAssembly { get; set; }

            public string Path { get; set; }
            public string FileName { get; set; }
            public string FileExtension { get; set; }
            public string TopLevelFolder { get; set; }

            public string RoundTripTargetFrameworkMoniker { get; set; }
            public string FrameworkName { get; set; }
            public string FrameworkVersion { get; set; }
            public string FrameworkProfile { get; set; }
            public string PlatformName { get; set; }
            public string PlatformVersion { get; set; }

            public string GetString(int i)
            {
                switch (i)
                {
                    case 0: return ScanId;
                    case 1: return ScanTimestamp;
                    case 2: return Id;
                    case 3: return Version;
                    case 4: return Created;
                    case 5: return ResultType;
                    case 6: return PatternSet;
                    case 7: return PropertyAnyValue;
                    case 8: return PropertyCodeLanguage;
                    case 9: return PropertyTargetFrameworkMoniker;
                    case 10: return PropertyLocale;
                    case 11: return PropertyManagedAssembly;
                    case 12: return PropertyMSBuild;
                    case 13: return PropertyRuntimeIdentifier;
                    case 14: return PropertySatelliteAssembly;
                    case 15: return Path;
                    case 16: return FileName;
                    case 17: return FileExtension;
                    case 18: return TopLevelFolder;
                    case 19: return RoundTripTargetFrameworkMoniker;
                    case 20: return FrameworkName;
                    case 21: return FrameworkVersion;
                    case 22: return FrameworkProfile;
                    case 23: return PlatformName;
                    case 24: return PlatformVersion;
                }
                throw new ArgumentOutOfRangeException(nameof(i));
            }
        }
    }
}
