using FNMES.Entity.Sys;
using SqlSugar;
using System;
using System.Collections.Generic;
using FNMES.Utility.Core;
using FNMES.WebUI.Logic.Base;
using FNMES.Entity.Param;
using FNMES.Entity.DTO.ApiData;
using ParamItem = FNMES.Entity.Param.ParamItem;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using FNMES.Utility.Files;
using CCS.WebUI;
using System.Linq;

namespace FNMES.WebUI.Logic.Param
{
    public class ParamProductLogic : BaseLogic
    {

        public ParamRecipe Get(long primaryKey, string configId)
        {
            try
            {
                var db = GetInstance(configId);
                ParamRecipe paramRecipe = db.MasterQueryable<ParamRecipe>().Where(it => it.Id == primaryKey).First();
                return paramRecipe;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
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
       

        public List<ParamRecipe> GetList(int pageIndex, int pageSize, string keyWord, string configId, ref int totalCount)
        {
            try
            {
                var db = GetInstance(configId);
                ISugarQueryable<ParamRecipe> queryable = db.MasterQueryable<ParamRecipe>();
                if (!keyWord.IsNullOrEmpty())
                {
                    queryable = queryable.Where(it => it.ProductPartNo.Contains(keyWord) || it.ProductDescription.Contains(keyWord));
                }
                return queryable.ToPageList(pageIndex, pageSize, ref totalCount);
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        public List<ParamRecipe> GetList(string configId)
        {
            try
            {
                var db = GetInstance(configId);

                List<ParamRecipe> paramProducts = db.MasterQueryable<ParamRecipe>().ToList();
                return paramProducts;
            }
            catch (Exception E)
            {
                Logger.ErrorInfo(E.Message);
                return null;
            }
        }

        //同步配方参数，可以选中是否强制覆盖
        //ESOP文件需要复制到本地 TODO 不知道对方文件服务器内容
        public int insert(GetRecipeData data, string configId, bool force = false )
        {
            try {
                var db = GetInstance(configId);
                bool skip  = false;
                Db.BeginTran();
                ParamRecipe paramRecipe = db.MasterQueryable<ParamRecipe>().First(it => it.ProductPartNo == data.productPartNo);
               
                if (!paramRecipe.IsNullOrEmpty())
                {
                    skip = paramRecipe.ProductPartNo == data.productPartNo && paramRecipe.BomNo == data.bomNo && paramRecipe.BomVersion == data.bomVersion
                                    && paramRecipe.RouteNo == data.routeNo && paramRecipe.RouteVersion == data.routeVersion
                                    && paramRecipe.ProcessConfigName == data.processConfigName; 
                }
                List<string> files = new List<string>();
                string fileServer = "";
                foreach (var item in data.processParamItems)
                {
                    List<EsopItem> temp = new List<EsopItem>();
                    foreach (var esop in item.esopList)
                    {
                        if (esop.filePath.IsNullOrEmpty()) continue;
                        string[] urlParts = esop.filePath.Split(new string[] { ".pdf" }, StringSplitOptions.None);
                        string[] httpParts = urlParts[0].Split(new string[] { "http://" }, StringSplitOptions.None);
                        string httpAfter = httpParts[1] + ".pdf";
                        string[] strings = httpAfter.Split(new[] { '/' }, 2);

                        //string[] strings = esop.filePath.Split(new[] { '/' }, 2);
                        if (strings.Length > 1)
                        {
                            esop.filePath = strings[1];
                            if (!files.Contains(strings[1]))
                            {
                                //fileServer = strings[0];
                                fileServer = @"http://" + strings[0];
                                string filepath = strings[1];
                                files.Add(filepath);
                            }
                        }
                        temp.Add(esop);
                    }
                    item.esopList = temp;
                }
                //从文件服务器把esop文件下载并上传到本地ftp服务器
                foreach (var item in files)
                {
                    string url = fileServer + @"/" + item;
                    UploadFileIfNotExistsAsync(url, item);
                }

                //存在旧值判断怎么处理
                if (paramRecipe != null)
                {
                    if (force || !skip)
                    {
                        db.DeleteNav<ParamRecipe>(it => it.Id == paramRecipe.Id)
                            .Include(it => it.processParamItems).ThenInclude(it => it.ParamList)
                            .Include(it => it.processParamItems).ThenInclude(it => it.PartList).ThenInclude(it => it.AlternativePartList)
                            .Include(it => it.processParamItems).ThenInclude(it => it.EsopList)
                            .Include(it => it.processParamItems).ThenInclude(it => it.StepList)
                            .ExecuteCommand();
                        skip = false;
                    }
                }
                if (!skip)
                {
                    ParamRecipe model = new ParamRecipe();
                    model.CopyFromGetRecipeData(data);
                    db.InsertNav<ParamRecipe>(model)
                            .Include(it => it.processParamItems).ThenInclude(it => it.ParamList)
                            .Include(it => it.processParamItems).ThenInclude(it => it.PartList).ThenInclude(it => it.AlternativePartList)
                            .Include(it => it.processParamItems).ThenInclude(it => it.EsopList)
                            .Include(it => it.processParamItems).ThenInclude(it => it.StepList)
                            .ExecuteCommand();
                }
                Db.CommitTran();
                return 1;
            }
            catch (Exception E) {
                Db.RollbackTran();
                Logger.ErrorInfo(E.Message);
                return 0;
            }
        }


         void UploadFileIfNotExistsAsync(string sourceUrl, string ftpFilePath)
        {
            FTPparam fTPparam = AppSetting.FTPparam;
            FtpHelper ftpHelper = new FtpHelper(fTPparam.Host, fTPparam.Username, fTPparam.Password);
            // 异步执行文件上传，不等待其完成
            _ = Task.Run(() => ftpHelper.UploadServerFileToFtp(sourceUrl, ftpFilePath));

        }
    }
}
