using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace FNMES.Entity.Param
{
    /// <summary>
    /// 
    ///</summary>
    public class CopyAble
    {
       
        public CopyAble CopyField(params object[] sources)     //把源数据字段复制到本体  
        {
            foreach (var source in sources)
            {
                foreach (var sorfield in source.GetType().GetProperties())
                {
                    foreach (var tarfield in this.GetType().GetProperties())
                    {
                        if (sorfield.Name.ToLower() == tarfield.Name.ToLower() && sorfield.PropertyType.Name == tarfield.PropertyType.Name)
                        {
                            if (!sorfield.PropertyType.Name.Contains("List"))
                            {
                                tarfield.SetValue(this, sorfield.GetValue(source));
                            }
                        }
                    }
                }
            }
            return this;
        }

    }
}
