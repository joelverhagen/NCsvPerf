namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/recordparser
    /// Source: https://github.com/leandromoh/RecordParser
    /// </summary>
    public class RecordParserParallel : RecordParser
    {
        public RecordParserParallel() : base(parallel: true, ensureOriginalOrdering: true)
        {
        }
    }
}