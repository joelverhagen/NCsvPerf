using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knapcode.NCsvPerf.CsvReadable
{
	/// <summary>
	/// Package: https://www.nuget.org/packages/ChoETL/
	/// Source: https://github.com/Cinchoo/ChoETL
	/// </summary>
	public class ChoEtl : ICsvReader
	{
        private readonly ActivationMethod _activationMethod;

        public ChoEtl(ActivationMethod activationMethod)
        {
            _activationMethod = activationMethod;
        }

        public List<T> GetRecords<T>(MemoryStream stream) where T : ICsvReadable, new()
        {
            var activate = ActivatorFactory.Create<T>(_activationMethod);
            var allRecords = new List<T>();

            var config = new ChoETL.ChoCSVRecordConfiguration
            {
                FileHeaderConfiguration = new ChoETL.ChoCSVFileHeaderConfiguration
				{
                    HasHeaderRecord = false,
				},
            };
            using (var reader = new StreamReader(stream))
            using (var csvReader = new global::ChoETL.ChoCSVReader(reader, config).AsDataReader())
            {
                var count = 0;
                while (csvReader.Read())
                {
                    count++;
                    var record = activate();
                    record.Read(i => csvReader.GetString(i));
                    allRecords.Add(record);
                }
            }

            return allRecords;
        }
    }
}
