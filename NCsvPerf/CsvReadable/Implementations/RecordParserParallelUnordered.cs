namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/recordparser
    /// Source: https://github.com/leandromoh/RecordParser
    /// </summary>
    public class RecordParserParallelUnordered : RecordParser
    {
        public RecordParserParallelUnordered() : base(parallel: true, ensureOriginalOrdering: false)
        {
        }
    }
}