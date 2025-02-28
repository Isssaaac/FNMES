using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    public static class CopyHelper
    {
        //不要拷贝list，只能拷贝简单类型
        public static void CopyField(this object des, params object[] sources)     //把源数据字段复制到本体  
        {
            foreach (var source in sources)
            {
                foreach (var srcfield in source.GetType().GetProperties())
                {
                    foreach (var tarfield in des.GetType().GetProperties())
                    {
                        if (srcfield.Name.ToLower() == tarfield.Name.ToLower() && srcfield.PropertyType.Name == tarfield.PropertyType.Name)
                        {
                            //不复制list
                            if (!srcfield.PropertyType.Name.Contains("List"))
                            {
                                tarfield.SetValue(des, srcfield.GetValue(source));
                            }
                        }
                    }
                }
            }
        }

        public static void CopyMatchingProperties<TSrc, TDes>(this TDes des, TSrc src)
        {
            // 将 Source 对象序列化为 JSON 字符串  
            string json = JsonConvert.SerializeObject(src);

            // 反序列化为 Destination 对象，仅复制相同字段  
            JsonConvert.PopulateObject(json, des);
        }

    }
}
