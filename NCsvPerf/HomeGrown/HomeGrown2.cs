using System;
using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.HomeGrown
{
    public delegate string StringPool(ReadOnlySpan<char> text);

    public class HomeGrown2
    {
        private readonly char[] _buffer;
        private readonly StringPool _stringPool;
        private int _index;

        public HomeGrown2(char[] buffer, StringPool stringPool)
        {
            _buffer = buffer;
            _stringPool = stringPool;
        }

        private enum State
        {
            BeforeField,
            InField,
            InQuotedField,
            LineEnd,
        }

        public bool TryReadLine(TextReader reader, List<string> fields)
        {
            _index = 0;
            fields.Clear();

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
                                _buffer[_index++] = (char)c;
                                state = State.InField;
                                break;
                        }
                        break;

                    case State.InField:
                        switch (c)
                        {
                            case ',':
                                AddField(fields);
                                state = State.BeforeField;
                                break;
                            case '\r':
                                AddField(fields);
                                if (reader.Peek() == '\n')
                                {
                                    reader.Read();
                                }
                                state = State.LineEnd;
                                break;
                            case '\n':
                                AddField(fields);
                                state = State.LineEnd;
                                break;
                            default:
                                _buffer[_index++] = (char)c;
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
                                        _buffer[_index++] = '"';
                                        reader.Read();
                                        break;
                                    case ',':
                                        reader.Read();
                                        AddField(fields);
                                        state = State.BeforeField;
                                        break;
                                    case '\r':
                                        reader.Read();
                                        AddField(fields);
                                        if (reader.Peek() == '\n')
                                        {
                                            reader.Read();
                                        }
                                        state = State.LineEnd;
                                        break;
                                    case '\n':
                                        reader.Read();
                                        AddField(fields);
                                        state = State.LineEnd;
                                        break;
                                    default:
                                        throw new InvalidDataException("Corrupt field found. A double quote is not escaped or there is extra data after a quoted field.");
                                }
                                break;
                            default:
                                _buffer[_index++] = (char)c;
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
                    var span = _buffer.AsSpan(0, _index);
                    var text = _stringPool != null
                        ? _stringPool(span)
                        : span.ToString();

                    fields.Add(text);
                    break;
                case State.InQuotedField:
                    throw new InvalidDataException("When the line ends with a quoted field, the last character should be an unescaped double quote.");
            }

            return fields.Count > 0;
        }

        private void AddField(List<string> fields)
        {
            if (_index == 0)
            {
                fields.Add(string.Empty);
            }
            else
            {
                var span = _buffer.AsSpan(0, _index);
                var text = _stringPool != null
                    ? _stringPool(span)
                    : span.ToString();

                fields.Add(text);
                _index = 0;
            }
        }
    }
}
