using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Record;
using System;

namespace FNMES.WebUI.Logic.Record
{
    public class RecordCellStartLogic : BaseLogic
    {
        public RecordCellStart GetCellInfo(string productCode, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                //业务逻辑强制走主库
                var cellStart = db.Queryable<RecordCellStart>().Where(it=>it.ProductCode == productCode).OrderBy(it=>it.CreateTime).First();
                return cellStart;
            }
            catch (Exception e)
            {
                Logger.ErrorInfo("查询出错", e);
                return null;
            }
        }
    }
}
