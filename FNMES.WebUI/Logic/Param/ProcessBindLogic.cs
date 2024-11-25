using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Operator;
using FNMES.Utility.Security;
using FNMES.Utility.Extension;
using FNMES.Utility.Core;
using FNMES.Utility.Other;
using System.Drawing.Printing;
using Microsoft.VisualBasic;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Engineering;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using FNMES.Entity.Record;
using ServiceStack;
using FNMES.WebUI.API;
using FNMES.Entity.DTO.ApiParam;
using FNMES.Utility.Network;
using FNMES.Entity.DTO.ApiData;

namespace FNMES.WebUI.Logic.Param
{
    public class ProcessBindLogic : BaseLogic
    {


        public long Insert(ProcessBind model,string configId)
        {
            try
            {
                var db = GetInstance(configId);
                model.Id = SnowFlakeSingle.Instance.NextId();
                int res = 0;
                List<ProcessBind> oldagvbind = db.MasterQueryable<ProcessBind>().Where(it => it.PalletNo == model.PalletNo).ToList();
                List<ProcessBind> oldprocessBind = db.MasterQueryable<ProcessBind>().Where(it => it.ProductCode == model.ProductCode).ToList();
                if (oldagvbind != null && oldagvbind.Count != 0)
                {
                    try
                    {
                        Db.BeginTran();
                        if (string.IsNullOrWhiteSpace(model.PalletNo))
                        {
                            res = Update(model, configId);
                        }
                        else
                        {
                            foreach (var item in oldagvbind)
                            {
                                item.PalletNo = "";
                                res = Update(item, configId);
                            }
                            
                        }
                        Db.CommitTran();
                    }
                    catch (Exception)
                    {
                        Db.RollbackTran();
                        return 0L;
                        throw;
                    }
                    
                }
                if (oldprocessBind!= null && oldprocessBind.Count!=0)
                {
                    try
                    {
                        Db.BeginTran();
                        List<RecordBindHistory> histories = new List<RecordBindHistory>();
                        oldprocessBind.ForEach(it =>
                        {
                            RecordBindHistory history = new RecordBindHistory();
                            history.CopyField(it);
                            histories.Add(history);
                        });

                        db.Insertable<RecordBindHistory>(histories).SplitTable().ExecuteCommand();
                        db.Deleteable<ProcessBind>(oldprocessBind).ExecuteCommand();
                        res = db.Insertable<ProcessBind>(model).ExecuteCommand();
                        Db.CommitTran();
                        if (res != 0)
                        {
                            return model.Id;
                        }
                        return 0L;
                    }
                    catch (Exception)
                    {
                        Db.RollbackTran();
                        throw ;
                    }
                }
                else
                {
                    res = db.Insertable<ProcessBind>(model).ExecuteCommand();
                    if(res != 0)
                    {
                        return model.Id;
                    }
                    return 0L;
                }
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0L;
            }
        }

        //M500打包工位不直接删除绑定纪录， 不然打包工位可能删除后无法重复作业。
        //通过时间来删除，在出站的时候，直接删除超过30天旧数据
        public bool RemoveOldData(string configId)
        {
            var db = GetInstance(configId);
            List<ProcessBind> oldprocessBind = db.MasterQueryable<ProcessBind>().Where(it => it.CreateTime < DateTime.Now.AddDays(-30)).ToList();
            if (oldprocessBind != null && oldprocessBind.Count != 0)
            {
                try
                {
                    Db.BeginTran();
                    List<RecordBindHistory> histories = new List<RecordBindHistory>();
                    oldprocessBind.ForEach(it =>
                    {
                        RecordBindHistory history = new RecordBindHistory();
                        history.CopyField(it);
                        histories.Add(history);
                    });

                    db.Insertable<RecordBindHistory>(histories).SplitTable().ExecuteCommand();
                    db.Deleteable<ProcessBind>(oldprocessBind).ExecuteCommand();
                    Db.CommitTran();
                    return true;
                }
                catch (Exception e)
                {
                    Logger.ErrorInfo(e.Message);
                    Db.RollbackTran();
                    return false;
                }
            }
            return true;
        }


      
        public ProcessBind GetByPalletNo(string palletNo, string configId)
        {

            try
            {
                //业务逻辑，必须走主库
                var db = GetInstance(configId);
                return db.MasterQueryable<ProcessBind>().Where(it => it.PalletNo == palletNo).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }
        }
      
        public ProcessBind GetByProductCode(string productCode, string configId)
        {
            //业务逻辑，必须走主库
            try
            {
                var db = GetInstance(configId);
                return db.MasterQueryable<ProcessBind>().Where(it => it.ProductCode == productCode).First();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return null;
            }
        }

        /// <summary>
        /// 获得列表分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyWord"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<ProcessBind> GetList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount,string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ProcessBind> queryable = db.MasterQueryable<ProcessBind>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.PalletNo.Contains(keyWord) || it.ProductCode.Contains(keyWord));
                }
                if (index == "1")//正序   
                {
                    queryable = queryable.OrderBy(it => it.Id);
                }
                else //倒序   
                {
                    queryable = queryable.OrderByDescending(it => it.Id);
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }
        public List<RecordBindHistory> GetHistoryList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount, string index)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<RecordBindHistory> queryable = db.MasterQueryable<RecordBindHistory>();

                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.PalletNo.Contains(keyWord) || it.ProductCode.Contains(keyWord));
                }
                //查询当日
                if (index == "1")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today;
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近7天
                else if (index == "2")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-6);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近1月
                else if (index == "3")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-29);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //近3月
                else if (index == "4")
                {
                    DateTime today = DateTime.Today;
                    DateTime startTime = today.AddDays(-91);
                    DateTime endTime = today.AddDays(1);
                    queryable = queryable.Where(it => it.CreateTime >= startTime && it.CreateTime < endTime);
                }
                //按月分表三个月取3张表
                return queryable.SplitTable(tabs => tabs.Take(3)).ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return new List<RecordBindHistory>();
            }
        }

        public int Delete(string productCode, string configId)
        {
            var db = GetInstance(configId);
            try
            {
                return db.Deleteable<ProcessBind>().Where(it=> it.ProductCode == productCode).ExecuteCommand();
            }
            catch (Exception e)
            {
                Logger.ErrorInfo(e.Message);
                return 0;
            }
            
        }



        /// <summary>
        /// 更新用户基础信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(ProcessBind model, string configId)
        {
            try
            {
                var db = GetInstance(configId);

                return db.Updateable<ProcessBind>(model).IgnoreColumns(it => new
                {
                    it.CreateTime
                }).ExecuteCommand();
            }
            catch (Exception)
            {

                return 0;
            }
        }
        public int DeletePalletNo(string palletNo, string configId)
        {
            try
            {
                var db = GetInstance(configId);

                return db.Updateable<ProcessBind>().IgnoreColumns(it => new
                {
                    it.CreateTime
                }).ExecuteCommand();
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public int FastBinding(long id, string palletNo, string configId)
        {
            var linedb = GetInstance(configId);
            var bindData = linedb.Queryable<ProcessBind>().Where(it => it.Id == id).First();

            if (bindData == null)
            {
                return 0;
            }
            bindData.PalletNo = palletNo;

            //根据工站查询设备编码
            var db = GetInstance();
            SysLine sysline = db.Queryable<SysLine>().Where(e=>e.ConfigId == configId).First();
            SysEquipment equipment = db.Queryable<SysEquipment>()
                .Where(it => it.LineId == sysline.Id && it.BigProcedure == bindData.CurrentStation).First();
            if (equipment == null)
            {
                return 0;
            }

            //调用工厂MES 绑定，绑定成功后更新绑定表
            BindPalletParam bindPalletParam = new BindPalletParam
            {
                productCode = bindData.ProductCode,
                palletNo = bindData.PalletNo,
                stationCode = bindData.CurrentStation,
                smallStationCode = equipment.UnitProcedure,
                equipmentID = equipment.EnCode,
                productionLine = sysline.EnCode,
                operatorNo = OperatorProvider.Instance.Current.Account
            };
            var ret = APIMethod.Call(Url.BindPalletUrl, bindPalletParam, configId);
            var result = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<nullObject>>();

            if (result!=null && result.messageType=="S")
            {
                var v = linedb.Updateable<ProcessBind>(bindData).IgnoreColumns(it => new
                {
                    it.CreateTime
                }).ExecuteCommand();
                return 1;
            }
            return 0;
        }

        public int FastUnbinding(long id, string configId)
        {
            var linedb = GetInstance(configId);
            var bindData = linedb.Queryable<ProcessBind>().Where(it => it.Id == id).First();

            if (bindData == null)
            {
                return 0;
            }
            bindData.PalletNo = "";

            //根据工站查询设备编码
            var db = GetInstance();
            SysLine sysline = db.Queryable<SysLine>().Where(e => e.ConfigId == configId).First();
            SysEquipment equipment = db.Queryable<SysEquipment>()
                .Where(it => it.LineId == sysline.Id && it.BigProcedure == bindData.CurrentStation).First();
            if (equipment == null)
            {
                return 0;
            }

            //调用工厂MES 绑定，绑定成功后更新绑定表
            BindPalletParam bindPalletParam = new BindPalletParam
            {
                productCode = bindData.ProductCode,
                palletNo = bindData.PalletNo,
                stationCode = bindData.CurrentStation,
                smallStationCode = equipment.UnitProcedure,
                equipmentID = equipment.EnCode,
                productionLine = sysline.EnCode,
                operatorNo = OperatorProvider.Instance.Current.Account
            };
            var ret = APIMethod.Call(Url.UnBindPalletUrl, bindPalletParam, configId);
            var result = ret.IsNullOrEmpty() ? null : ret.ToObject<RetMessage<nullObject>>();

            if (result != null && result.messageType == "S")
            {
                var v = linedb.Updateable<ProcessBind>(bindData).IgnoreColumns(it => new
                {
                    it.CreateTime
                }).ExecuteCommand();
                return 1;
            }
            return 0;
        }

        //1125更改，仍在线的工站获取的是内控码的信息
        //这里面有个问题，返修要求可以五条线的都能返修？那查的时候，需要查5条线的数据库？
        public RecordBindHistory GetBindByProductCode(string productCode, string configId)
        {
            var db = GetInstance(configId);
            RecordBindHistory oldProcessBind = db.MasterQueryable<RecordBindHistory>().Where(it => it.ProductCode == productCode).First();
            return oldProcessBind;
        }
    }
}
