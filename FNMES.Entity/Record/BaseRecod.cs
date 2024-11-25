using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FNMES.Entity.Record
{
    public abstract class BaseRecord
    {
        public BaseRecord CopyField(params object[] sources)     //把源数据字段复制到本体
        {
            foreach (var source in sources)
            {
                foreach (var sorfield in source.GetType().GetProperties())
                {
                    foreach (var tarfield in this.GetType().GetProperties())
                    {
                        if (sorfield.Name.ToLower()== tarfield.Name.ToLower() && sorfield.PropertyType.Name == tarfield.PropertyType.Name)
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
