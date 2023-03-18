﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/CsvHelper/
    /// Source: https://github.com/JoshClose/CsvHelper
    /// </summary>
    public class CsvHelper : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public CsvHelper(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public IEnumerable<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);

            using (var reader = new StreamReader(stream))
            {
                var config = new global::CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    CacheFields = true,
                };
                var csvParser = new global::CsvHelper.CsvParser(reader, config);
                while (csvParser.Read())
                {
                    var record = activate();
                    record.Read(i => csvParser[i]);
                    yield return record;
                }
            }
        }
    }
}
