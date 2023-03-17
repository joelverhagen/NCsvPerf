using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: N/A
    /// Source: https://stackoverflow.com/a/39939559
    /// </summary>
    public class StackOverflowRegex : ICsvReader
    {
        private static readonly Regex CSVParser = new Regex(
            @"(?<=\r|\n|^)(?!\r|\n|$)(?:(?:""(?<Value>(?:[^""]|"""")*)""|(?<Value>(?!"")[^,\r\n]+)|""(?<OpenValue>(?:[^""]|"""")*)(?=\r|\n|$)|(?<Value>))(?:,|(?=\r|\n|$)))+?(?:(?<=,)(?<Value>))?(?:\r\n|\r|\n|$)",
            RegexOptions.Compiled);

        private readonly ActivationMethod _activationMethod;

        public StackOverflowRegex(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();
            var fields = new List<string>();
            var builder = new StringBuilder();

            // Same regex from before shortened to one line for brevity

            using (var CSVReader = new StreamReader(stream))
            {
                String CSVLine = CSVReader.ReadLine();
                StringBuilder RecordText = new StringBuilder();
                Int32 RecordNum = 0;

                while (CSVLine != null)
                {
                    RecordText.AppendLine(CSVLine);
                    MatchCollection RecordsRead = CSVParser.Matches(RecordText.ToString());
                    Match Record = null;

                    for (Int32 recordIndex = 0; recordIndex < RecordsRead.Count; recordIndex++)
                    {
                        Record = RecordsRead[recordIndex];

                        if (Record.Groups["OpenValue"].Success && recordIndex == RecordsRead.Count - 1)
                        {
                            // We're still trying to find the end of a muti-line value in this record
                            // and it's the last of the records from this segment of the CSV.
                            // If we're not still working with the initial record we started with then
                            // prep the record text for the next read and break out to the read loop.
                            if (recordIndex != 0)
                                RecordText.AppendLine(Record.Value);

                            break;
                        }

                        // Valid record found or new record started before the end could be found
                        RecordText.Clear();
                        RecordNum++;

                        for (Int32 valueIndex = 0; valueIndex < Record.Groups["Value"].Captures.Count; valueIndex++)
                        {
                            Capture c = Record.Groups["Value"].Captures[valueIndex];
                            if (c.Length == 0 || c.Index == Record.Index || Record.Value[c.Index - Record.Index - 1] != '\"')
                                fields.Add(c.Value);
                            else
                                fields.Add(c.Value.Replace("\"\"", "\""));
                        }

                        if (Record.Groups["OpenValue"].Captures.Count > 0)
                        {
                            throw new InvalidDataException("R" + RecordNum + ":ERROR - Open ended quoted value: " + Record.Groups["OpenValue"].Captures[0].Value);
                        }

                        var record = activate();
                        record.Read(i => fields[i]);
                        fields.Clear();
                        allRecords.Add(record);
                    }

                    CSVLine = CSVReader.ReadLine();

                    if (CSVLine == null && Record != null)
                    {
                        RecordNum++;

                        //End of file - still working on an open value?
                        if (Record.Groups["OpenValue"].Captures.Count > 0)
                        {
                            throw new InvalidDataException("R" + RecordNum + ":ERROR - Open ended quoted value: " + Record.Groups["OpenValue"].Captures[0].Value);
                        }
                    }
                }
            }

            return allRecords;
        }
    }
}
