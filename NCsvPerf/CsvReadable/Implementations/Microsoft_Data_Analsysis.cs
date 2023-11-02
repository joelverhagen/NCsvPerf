using Microsoft.Data.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Microsoft.Data.Analysis/
    /// Source: https://github.com/dotnet/MachineLearning
    /// </summary>
    public class Microsoft_Data_Analysis : ICsvReader
    {
        static Type[] types = Enumerable.Range(0, 25).Select(i => typeof(string)).ToArray();

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            // This only works for data with exactly 25 columns.
            // You must either provide the column types, or the types will be
            // guessed. Can't allow guessing, because the round-trip back to string doesn't preserve the exact text.
            // Must know the number of columns to provide the schema, so this isn't general-purpose for any <T>.
            DataFrame frame;
            try
            {
                frame = DataFrame.LoadCsv(stream, header: false, guessRows: 0, dataTypes: types);
            } 
            catch(FormatException e)
            {
                if (e.Message == "Empty file")
                    return allRecords;
                throw;
            }
            foreach (var row in frame.Rows)
            {
                var record = new T();
                record.Read(i => row[i].ToString());
                allRecords.Add(record);
            }

            return allRecords;
        }
    }
}
