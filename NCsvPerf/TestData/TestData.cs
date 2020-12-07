using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Knapcode.NCsvPerf
{
    public static class TestData
    {
        public static LineCache PackageAssets { get; }

        static TestData()
        {
            PackageAssets = new LineCache(Path.Combine("TestData", "PackageAssets.csv"));
        }

        public class LineCache
        {
            private readonly string[] _sourceLines;
            private readonly byte[] _sourceBytesAll;
            private readonly ConcurrentDictionary<int, byte[]> _lineCountToBytes;

            public LineCache(string filePath)
            {
                _sourceLines = File.ReadAllLines(filePath);
                _sourceBytesAll = EncodeLines(_sourceLines);
                _lineCountToBytes = new ConcurrentDictionary<int, byte[]>();
            }

            public byte[] GetBytes(int lineCount) => _lineCountToBytes.GetOrAdd(lineCount, GetBytesUncached);

            private byte[] GetBytesUncached(int lineCount)
            {
                if (lineCount == 0)
                {
                    return Array.Empty<byte>();
                }
                else if (lineCount <= _sourceLines.Length)
                {
                    return EncodeLines(_sourceLines.Take(lineCount));
                }
                else
                {
                    var repeatCount = lineCount / _sourceLines.Length;
                    var extraBytes = EncodeLines(_sourceLines.Take(lineCount % _sourceLines.Length));
                    using (var buffer = new MemoryStream())
                    {
                        for (var i = 0; i < repeatCount; i++)
                        {
                            buffer.Write(_sourceBytesAll, 0, _sourceBytesAll.Length);
                        }

                        buffer.Write(extraBytes, 0, extraBytes.Length);
                        return buffer.ToArray();
                    }
                }
            }

            private static byte[] EncodeLines(IEnumerable<string> sourceLines)
            {
                // We ensure there is a newline at the end since some CSV parser have bugs dealing with a trailing. We
                // want to catch that.
                return Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, sourceLines) + Environment.NewLine);
            }
        }
    }
}
