using System;
using System.Collections.Generic;
using System.Linq;
using FNMES.Utility;
using System.IO;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using FNMES.Utility.Logs;




namespace FNMES.Entity
{
    public class SeedDataBase<TEntity> 
    {
        public class FlexibleDateTimeConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                switch (reader.TokenType)
                {
                    case JsonToken.Null:
                        return objectType == typeof(DateTime?) ? (DateTime?)DateTime.Today : DateTime.Today;

                    case JsonToken.Date:
                        return reader.Value;

                    case JsonToken.String:
                        if (DateTime.TryParse(reader.Value?.ToString(), out var date))
                            return date;
                        return DateTime.Today;

                    default:
                        return DateTime.Today;
                }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(value);
            }
        }
        public List<TEntity> ReadCsvToDictionaries()
        {
            string name = this.GetType().Name.Replace("SeedData", "");
            string projectRoot = PathHelper.GetDataDirectoryPath();
            string csvFilePath = Path.Combine(projectRoot, name + ".csv");
            try
            {
                var result = new List<Dictionary<string, string>>();
                string[] headers = null;

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                };

                using (var reader = new StreamReader(csvFilePath))
                using (var csv = new CsvReader(reader, config))
                {
                    // 读取表头
                    csv.Read();
                    csv.ReadHeader();
                    headers = csv.HeaderRecord;

                    while (csv.Read())
                    {
                        var dict = new Dictionary<string, string>();
                        foreach (var header in headers)
                        {
                            if (csv.TryGetField(header, out string value))
                            {
                                dict[header] = value;
                            }
                        }
                        result.Add(dict);
                    }
                }

                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new FlexibleDateTimeConverter() }
                };
                var json = JsonConvert.SerializeObject(result);
                var record = JsonConvert.DeserializeObject<List<TEntity>>(json, settings);
                return record;
            }
            catch (Exception ex)
            {
                LogHelper.Error($"文件名:{name},读取csv种子文件出错", ex);
                return null;
            }
        }
            

    }
}
