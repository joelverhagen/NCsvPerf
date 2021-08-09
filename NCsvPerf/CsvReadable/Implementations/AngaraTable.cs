using Angara.Data;
using Angara.Data.DelimitedFile;
using Microsoft.FSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace Knapcode.NCsvPerf.CsvReadable
{
    /// <summary>
    /// Package: https://www.nuget.org/packages/Angara.Table
    /// Source: https://github.com/microsoft/Angara.Table
    /// </summary>
    public class AngaraTable : ICsvReader
    {
        private readonly ActivationMethod _activationMethod;

        public AngaraTable(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        class Types : FSharpFunc<Tuple<int, string>, FSharpOption<Type>>
        {
            static readonly FSharpOption<Type> StringType = new FSharpOption<Type>(typeof(string));

            public override FSharpOption<Type> Invoke(Tuple<int, string> func)
            {
                return StringType;
            }
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);

            using (var reader = new StreamReader(stream))
            {
                var cols = new FSharpOption<FSharpFunc<Tuple<int, string>, FSharpOption<Type>>>(new Types());
                var table = Table.Load(reader, new ReadSettings(Delimiter.Comma, false, false, FSharpOption<int>.None, cols));
                var allRecords = new List<T>(table.RowsCount);
                for (int r = 0; r < table.RowsCount; r++)
                {
                    var item = activate();
                    item.Read(i => table[i].Rows.Item(r).AsString);
                    allRecords.Add(item);
                }
                return allRecords;
            }
        }
    }
}
