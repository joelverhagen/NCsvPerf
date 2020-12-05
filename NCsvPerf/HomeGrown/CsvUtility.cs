using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Knapcode.NCsvPerf.HomeGrown
{
    public class CsvUtility
    {
        public static void WriteWithQuotes(TextWriter writer, string value)
        {
            if (value == null)
            {
                return;
            }

            if (value.StartsWith(' ')
                || value.EndsWith(' ')
                || value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) > -1)
            {
                writer.Write('"');
                writer.Write(value.Replace("\"", "\"\""));
                writer.Write('"');
            }
            else
            {
                writer.Write(value);
            }
        }

        private enum State
        {
            BeforeField,
            InField,
            InQuotedField,
            LineEnd,
        }

        public static bool TryReadLine(TextReader reader, List<string> fields, StringBuilder builder)
        {
            fields.Clear();
            builder.Clear();

            var state = State.BeforeField;
            int c;
            while ((c = reader.Read()) > -1)
            {
                switch (state)
                {
                    case State.BeforeField:
                        switch (c)
                        {
                            case '"':
                                state = State.InQuotedField;
                                break;
                            case ',':
                                fields.Add(string.Empty);
                                break;
                            case '\r':
                                fields.Add(string.Empty);
                                if (reader.Peek() == '\n')
                                {
                                    reader.Read();
                                }
                                state = State.LineEnd;
                                break;
                            case '\n':
                                fields.Add(string.Empty);
                                state = State.LineEnd;
                                break;
                            default:
                                builder.Append((char)c);
                                state = State.InField;
                                break;
                        }
                        break;

                    case State.InField:
                        switch (c)
                        {
                            case ',':
                                AddField(fields, builder);
                                state = State.BeforeField;
                                break;
                            case '\r':
                                AddField(fields, builder);
                                if (reader.Peek() == '\n')
                                {
                                    reader.Read();
                                }
                                state = State.LineEnd;
                                break;
                            case '\n':
                                AddField(fields, builder);
                                state = State.LineEnd;
                                break;
                            default:
                                builder.Append((char)c);
                                break;
                        }
                        break;

                    case State.InQuotedField:
                        switch (c)
                        {
                            case '"':
                                var nc = reader.Peek();
                                switch (nc)
                                {
                                    case '"':
                                        builder.Append('"');
                                        reader.Read();
                                        break;
                                    case ',':
                                        reader.Read();
                                        AddField(fields, builder);
                                        state = State.BeforeField;
                                        break;
                                    case '\r':
                                        reader.Read();
                                        AddField(fields, builder);
                                        if (reader.Peek() == '\n')
                                        {
                                            reader.Read();
                                        }
                                        state = State.LineEnd;
                                        break;
                                    case '\n':
                                        reader.Read();
                                        AddField(fields, builder);
                                        state = State.LineEnd;
                                        break;
                                    default:
                                        throw new InvalidDataException("Corrupt field found. A double quote is not escaped or there is extra data after a quoted field.");
                                }
                                break;
                            default:
                                builder.Append((char)c);
                                break;
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }

                if (state == State.LineEnd)
                {
                    break;
                }
            }

            switch (state)
            {
                case State.InField:
                    fields.Add(builder.ToString());
                    break;
                case State.InQuotedField:
                    throw new InvalidDataException("When the line ends with a quoted field, the last character should be an unescaped double quote.");
            }

            return fields.Count > 0;
        }

        private static void AddField(List<string> fields, StringBuilder builder)
        {
            if (builder.Length == 0)
            {
                fields.Add(string.Empty);
            }
            else
            {
                fields.Add(builder.ToString());
                builder.Clear();
            }
        }
    }
}
