using Addax.Formats.Tabular;
using System;
using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Addax.Formats.Tabular
    /// Source: https://github.com/alexanderkozlenko/addax
    /// </summary>
    public class Addax_Formats_Tabular : ICsvReader
    {
        private readonly TabularDialect _dialect;

        public Addax_Formats_Tabular()
        {
            _dialect = new TabularDialect(Environment.NewLine, ',', '\"');
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var allRecords = new List<T>();

            using (var reader = new TabularReader(stream, _dialect))
            {
                while (reader.TryPickRecord())
                {
                    // workaround for https://github.com/alexanderkozlenko/addax/issues/21
                    string firstValue;
                    if (!reader.TryReadField() || !reader.TryGetString(out firstValue))
                    {
                        break;
                    }

                    string secondValue;
                    if (!reader.TryReadField() || !reader.TryGetString(out secondValue))
                    {
                        break;
                    }

                    var record = new T();
                    record.Read(i =>
                    {
                        if (i == 0)
                        {
                            return firstValue;
                        }
                        else if (i == 1)
                        {
                            return secondValue;
                        }
                        else if (reader.TryReadField() && reader.TryGetString(out var value))
                        {
                            return value;
                        }

                        return null;
                    });
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
