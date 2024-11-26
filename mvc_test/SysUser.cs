using SqlSugar;
namespace FNMES.TEST
{
    /// <summary>
    /// 
    ///</summary>
    [SplitTable(SplitType.Day)]//指定按照时间分表
    [SugarTable("UserSplit{year}{month}{day}")]
    public class SysUser
    {
        /// <summary>
        /// 主键 
        ///</summary>
        [Newtonsoft.Json.JsonConverter(typeof(ValueToStringConverter))]
        [SugarColumn(ColumnName="id" ,IsPrimaryKey = true   )]
         public long Id { get; set; }
        /// <summary>
        /// 卡号 
        ///</summary>
         [SugarColumn(ColumnName="cardNo",IsNullable =true)]
         public string? CardNo { get; set; }
        /// <summary>
        /// 用户名 
        ///</summary>
         [SugarColumn(ColumnName="userNo" , IsNullable = true)]
         public string? UserNo { get; set; }
       
        /// <summary>
        /// 用户名 
        ///</summary>
         [SugarColumn(ColumnName="name")]
         public string? Name { get; set; }

        [SplitField] //分表以当前这个属性  对应的数据库表字段数据为维度来分表
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
