using System;
using System.Globalization;

namespace Knapcode.NCsvPerf.CsvReadable.TestCases
{
    public class PackageAsset : ICsvReadable
    {
        public Guid? ScanId { get; set; }
        public DateTimeOffset? ScanTimestamp { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
        public DateTimeOffset Created { get; set; }
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

        private static Guid? ParseNullableGuid(string input)
        {
            return input.Length > 0 ? Guid.Parse(input) : null;
        }

        private static DateTimeOffset? ParseNullableDateTimeOffset(string input)
        {
            return input.Length > 0 ? ParseDateTimeOffset(input) : null;
        }

        private static DateTimeOffset ParseDateTimeOffset(string input)
        {
            return DateTimeOffset.ParseExact(input, "O", CultureInfo.InvariantCulture);
        }

        public void Read(Func<int, string> getField)
        {
            ScanId = ParseNullableGuid(getField(0));
            ScanTimestamp = ParseNullableDateTimeOffset(getField(1));
            Id = getField(2);
            Version = getField(3);
            Created = ParseDateTimeOffset(getField(4));
            ResultType = getField(5);
            PatternSet = getField(6);
            PropertyAnyValue = getField(7);
            PropertyCodeLanguage = getField(8);
            PropertyTargetFrameworkMoniker = getField(9);
            PropertyLocale = getField(10);
            PropertyManagedAssembly = getField(11);
            PropertyMSBuild = getField(12);
            PropertyRuntimeIdentifier = getField(13);
            PropertySatelliteAssembly = getField(14);
            Path = getField(15);
            FileName = getField(16);
            FileExtension = getField(17);
            TopLevelFolder = getField(18);
            RoundTripTargetFrameworkMoniker = getField(19);
            FrameworkName = getField(20);
            FrameworkVersion = getField(21);
            FrameworkProfile = getField(22);
            PlatformName = getField(23);
            PlatformVersion = getField(24);
        }

        public int GetColumnCount()
        {
            return 25;
        }
    }
}
