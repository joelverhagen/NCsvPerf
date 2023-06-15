﻿using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Microsoft.ML
    /// Source: https://github.com/dotnet/machinelearning
    /// </summary>
    public class Microsoft_ML : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public Microsoft_ML(ActivationMethod activationMethod, string id)
        {
            _activationMethod = activationMethod;
            this.file = "data" + id + ".csv";
        }

        string file;

        public List<T> GetRecords<T>(MemoryStream stream)
            where T : ICsvReadable, new()
        {
            // this library only allows loading from a file.
            // so write to a local file, use the length of the memory stream
            // to write to a different file based on the input data
            // this will be executed during the first "warmup" run

            if (!File.Exists(file))
            {
                using var data = File.Create(file);
                stream.CopyTo(data);
            }

            return GetRecordsFromFile<T>(file);
        }

        public List<T> GetRecordsFromFile<T>(string file)
            where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();
            var mlc = new MLContext();

            var schema = new TextLoader.Column[25];
            for (int i = 0; i < schema.Length; i++)
            {
                schema[i] = new TextLoader.Column("" + i, DataKind.String, i);
            }

            var opts = new TextLoader.Options()
            {
                HasHeader = false,
                Separators = new[] { ',' },
                Columns = schema,
                AllowQuoting = true,
            };
            var l = mlc.Data.LoadFromTextFile(file, opts);
            var rc = l.GetRowCursor(l.Schema);
            var cols = l.Schema.ToArray();
            var getters = cols.Select(c => rc.GetGetter<ReadOnlyMemory<char>>(c)).ToArray();
            while (rc.MoveNext())
            {
                var record = activate();
                record.Read(i => { ReadOnlyMemory<char> s = null; getters[i](ref s); return s.ToString(); });
                allRecords.Add(record);
            }

            return allRecords;
        }
    }
}
