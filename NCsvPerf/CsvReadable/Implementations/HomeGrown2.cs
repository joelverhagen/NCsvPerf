﻿using System.Collections.Generic;
using System.IO;
using Ben.Collections.Specialized;
using Knapcode.NCsvPerf.HomeGrown;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: N/A
    /// Source: see HomeGrown2.cs in this repository
    /// </summary>
    public class HomeGrown2 : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public HomeGrown2(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();
            var fields = new List<string>();

            using (var reader = new StreamReader(stream))
            {
#if ENABLE_STRING_POOLING
                StringPool stringPool = new InternPool().Intern;
#else
                StringPool stringPool = s => new string(s);
#endif

                var parser = new NCsvPerf.HomeGrown.HomeGrown2(buffer: new char[200], stringPool);

                while (parser.TryReadLine(reader, fields))
                {
                    var record = activate();
                    record.Read(i => fields[i]);
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
