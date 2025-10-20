using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace FNMES.Utility.Core
{
    public class MultiCultureDateTimeConverter : DateTimeConverterBase
    {
        // 定义需要兼容的所有日期时间格式
        private readonly string[] _dateFormats = new[]
        {
        "yyyy-MM-dd HH:mm:ss",   // 中文/英文（冒号分隔时间）
        "MM/dd/yyyy h:mm:ss tt", // 英文（如 10/09/2025 2:21:14 PM）
        "yyyy/MM/dd HH:mm:ss",   // 中文（如 2025/10/09 14:23:03）
        "dd/MM/yyyy HH.mm.ss",   // 印尼（如 09/10/2025 14.25.18）
        "yyyy-MM-dd HH.mm.ss",   // 印尼（如 2025-10-09 12.36.45）
        "yyyyMMddHHmmss",        // 紧凑格式（如 20251009143045）
        "o"                      // ISO 8601 格式（如 2025-10-09T14:30:45.123Z）
    };

        // 解析 JSON 时调用（字符串 -> DateTime）
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            string dateStr = reader.Value.ToString();
            // 尝试用所有预定义格式解析，使用不变文化避免区域影响
            if (DateTime.TryParseExact(
                dateStr,
                _dateFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime result))
            {
                return result;
            }

            // 解析失败时抛出异常（或返回默认值）
            throw new JsonSerializationException($"无法解析日期时间字符串: {dateStr}，支持的格式：{string.Join("；", _dateFormats)}");
        }

        // 序列化时调用（DateTime -> 字符串，可选实现）
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime dateTime)
            {
                // 序列化时统一输出为 ISO 格式（可自定义）
                writer.WriteValue(dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}