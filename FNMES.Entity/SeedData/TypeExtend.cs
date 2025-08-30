using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common.ReedSolomon;
using FNMES.Utility.Core;

namespace FNMES.Entity
{
    public static class TypeExtend
    {

        private static bool IsTheRawGenericType(System.Type type, System.Type generic)
        {
            return generic == (type.IsGenericType ? type.GetGenericTypeDefinition() : type);
        }
        /// <summary>
        /// 判断类型是否实现某个泛型
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="generic">泛型类型</param>
        /// <returns>bool</returns>
        public static bool HasImplementedRawGeneric(this Type type, System.Type generic)
        {
            // 检查接口类型
            var isTheRawGenericType = type.GetInterfaces().Any(i=> IsTheRawGenericType(i,generic));
            if (isTheRawGenericType) return true;

            // 检查类型
            while (type != null && type != typeof(object))
            {
                isTheRawGenericType = IsTheRawGenericType(type,generic);
                if (isTheRawGenericType) return true;
                type = type.BaseType;
            }

            return false;
        }

        private static bool IsIgnoreColumn(PropertyInfo pi)
        {
            var sc = pi.GetCustomAttributes<SugarColumn>(false).FirstOrDefault(u => u.IsIgnore == true);
            return sc != null;
        }

        private static bool IsJsonColumn(PropertyInfo pi)
        {
            var sc = pi.GetCustomAttributes<SugarColumn>(false).FirstOrDefault(u => u.IsJson == true);
            return sc != null;
        }
        public static DataTable ToDataTable_<T>(this List<T> list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                // result.TableName = list[0].GetType().Name; // 表名赋值
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    System.Type colType = pi.PropertyType;
                    if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    if (IsIgnoreColumn(pi))
                        continue;
                    if (IsJsonColumn(pi))//如果是json特性就是sting类型
                        colType = typeof(string);
                    result.Columns.Add(pi.Name, colType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (IsIgnoreColumn(pi))
                            continue;
                        object obj = pi.GetValue(list[i], null);
                        if (IsJsonColumn(pi))//如果是json特性就是转化为json格式
                            obj = obj?.ToJson();//如果json字符串是空就传null
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }
}
