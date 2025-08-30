using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;


namespace FNMES.Utility.Core
{
    public static class JsonHelper
    {
        /// <summary>
        /// 对象序列化成JSON字符串。
        /// </summary>
        /// <param name="obj">序列化对象</param>
        /// <param name="ignoreProperties">设置需要忽略的属性</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            if (obj == null)
                return string.Empty;
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return JsonConvert.SerializeObject(obj, timeConverter);
        }

        /// <summary>
        /// JSON字符串序列化成对象。
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns></returns>
        public static T ToObject<T>(this string json)
        {
            //var setting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return json == null ? default(T) : JsonConvert.DeserializeObject<T>(json);//, setting);
        }


        /// <summary>
        /// JSON字符串序列化成集合。
        /// </summary>
        /// <typeparam name="T">集合类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string json)
        {
            //var setting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return json == null ? null : JsonConvert.DeserializeObject<List<T>>(json);//, setting);
        }


        /// <summary>
        /// JSON字符串序列化成DataTable。
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns></returns>
        public static DataTable ToTable(this string json)
        {
            return json == null ? null : JsonConvert.DeserializeObject<DataTable>(json);
        }

        /// <summary>
        /// 将JSON字符串反序列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseEntity"></param>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T Json2Obj<T>(T baseEntity, string strJson)
        {
            return JsonConvert.DeserializeAnonymousType(strJson, baseEntity);
        }

        /// <summary>
        /// 将对象转换层JSON字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Obj2Json<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }


        public static List<T> JsonToList<T>(string strJson)
        {
            T[] list = JsonConvert.DeserializeObject<T[]>(strJson);
            return list.ToList();
        }

        public static T Json2Obj<T>(string strJson)
        {
            return JsonConvert.DeserializeObject<T>(strJson);
        }

        public static DataTable ToDataTable(this string json)
        {
            return json.ToTable();
        }

        public static string FormatJson(this string json)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(json);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return json;
            }
        }

        public static string ToJson(this object obj, string dateFormat = "yyyy/MM/dd HH:mm:ss")
        {
            return obj == null ? string.Empty : JsonConvert.SerializeObject(obj, new IsoDateTimeConverter { DateTimeFormat = dateFormat });
        }
    }
}
