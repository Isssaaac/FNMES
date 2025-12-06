using Newtonsoft.Json;
using System;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Utility.Network;

namespace FNMES.Entity.DTO
{ 
    public static class ApiParser
    {
        /// <summary>
        /// 解析接口返回的JSON，支持code非00000时data为null的场景
        /// </summary>
        /// <typeparam name="T">Data的类型（BaseData或OkData）</typeparam>
        /// <param name="json">接口返回的JSON字符串</param>
        /// <returns>解析后的ApiResponse<T>对象</returns>
        /// <exception cref="Exception">解析失败时抛出</exception>
        public static MesRet<T> Parse<T>(string json) where T : ResultRet
        {
            try
            {
                
                // 先反序列化为动态对象，获取code和data是否为null
                dynamic temp = JsonConvert.DeserializeObject(json);
                string code = temp.code;
                bool isDataNull = temp.data == null;

                // 处理code非00000的场景（此时data可能为null）
                if (code != "00000")
                {
                    // 直接构造响应对象，避免因data为null导致的类型转换异常
                    return new MesRet<T>
                    {
                        code = code,
                        msg = temp.msg,
                        data = isDataNull ? null : JsonConvert.DeserializeObject<T>(temp.data.ToString())
                    };
                }

                // 处理code=00000的场景（此时data一定存在）
                if (isDataNull)
                {
                    throw new Exception("code为00000时，data不能为null");
                }

                // 反序列化为目标类型（OK/NG均适用）
                return JsonConvert.DeserializeObject<MesRet<T>>(json);
            }
            catch (JsonException ex)
            {
                throw new Exception($"JSON解析失败：{ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"解析处理失败：{ex.Message}");
            }
        }
    }
}