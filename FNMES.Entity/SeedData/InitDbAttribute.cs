using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FNMES.Entity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IgnoreInitTableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SystemTableInitAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class LineTableInitAttribute : Attribute
    {
    }

    public class IgnoreSeedDataUpdateAttribute : Attribute
    {
    }
}
