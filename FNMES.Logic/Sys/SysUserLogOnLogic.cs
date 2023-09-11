using FNMES.Entity.Sys;
using FNMES.Logic.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FNMES.Utility.Security;
using FNMES.Utility.Other;

namespace FNMES.Logic.Sys
{
    public class SysUserLogOnLogic : BaseLogic
    {

        /// <summary>
        /// 根据用户Id得到登录账号信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SysUserLogOn GetByAccount(string userId)
        {
            using (var db = GetInstance())
            {
                return db.Queryable<SysUserLogOn>().Where(it => it.UserId == userId).First();
            }
        }

        /// <summary>
        /// 更新用户登录账号信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateLogin(SysUserLogOn model)
        {
            using (var db = GetInstance())
            {
                model.IsOnLine = "1";
                model.LastVisitTime = DateTime.Now;
                model.PrevVisitTime = model.LastVisitTime;
                model.LoginCount += 1;
                model.ModifyTime = DateTime.Now;
                model.ModifyUserId = model.UserId;
                return db.Updateable<SysUserLogOn>(model).UpdateColumns(it => new
                {
                    it.IsOnLine,
                    it.PrevVisitTime,
                    it.LastVisitTime,
                    it.LoginCount,
                    it.ModifyTime,
                    it.ModifyUserId
                }).ExecuteCommand();
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userLoginEntity"></param>
        /// <returns></returns>
        public int ModifyPwd(SysUserLogOn userLoginEntity)
        {
            using (var db = GetInstance())
            {
                userLoginEntity.ChangePwdTime = DateTime.Now;
                userLoginEntity.ModifyUserId = userLoginEntity.UserId;
                userLoginEntity.ModifyTime = DateTime.Now;

                return db.Updateable<SysUserLogOn>(userLoginEntity).UpdateColumns(it => new
                {
                    it.Password,
                    it.ChangePwdTime,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        /// <summary>
        /// 删除用户登录信息
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public int Delete(List<string> userIds)
        {
            using (var db = GetInstance())
            {
                List<SysUserLogOn> list = db.Queryable<SysUserLogOn>().Where(it => userIds.Contains(it.UserId)).ToList();
                list.ForEach(it => { it.DeleteFlag = "Y"; });
                return db.Updateable<SysUserLogOn>(list).ExecuteCommand();
            }
        }

        /// <summary>
        /// 新增用户登录账号
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Insert(SysUserLogOn model)
        {
            using (var db = GetInstance())
            {
                model.Id = UUID.StrSnowId;
                model.SecretKey = model.Id.DESEncrypt().Substring(0, 8);
                model.Password = model.Password.MD5Encrypt().DESEncrypt(model.SecretKey).MD5Encrypt();
                model.LoginCount = 0;
                model.IsOnLine = "0";
                model.EnableFlag = "Y";
                model.DeleteFlag = "N";
                model.CreateTime = DateTime.Now;
                model.ModifyTime = DateTime.Now;
                model.ModifyUserId = model.UserId;
                model.CreateUserId = model.UserId;
                return db.Insertable<SysUserLogOn>(model).ExecuteCommand();
            }
        }

        /// <summary>
        /// 更新用户登录账号信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateInfo(SysUserLogOn model)
        {
            using (var db = GetInstance())
            {
                model.ModifyTime = DateTime.Now;
                model.ModifyUserId = model.UserId;

                return db.Updateable<SysUserLogOn>(model).UpdateColumns(it => new
                {
                    it.AllowMultiUserOnline,
                    it.Question,
                    it.AnswerQuestion,
                    it.CheckIPAddress,
                    it.Language,
                    it.Theme,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        public int UpdateTheme(SysUserLogOn model)
        {
            using (var db = GetInstance())
            {
                model.ModifyTime = DateTime.Now;
                model.ModifyUserId = model.UserId;
                return db.Updateable<SysUserLogOn>(model).UpdateColumns(it => new
                {
                    it.Theme,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }

        public int UpdatePassword(SysUserLogOn model)
        {
            using (var db = GetInstance())
            {
                model.ModifyTime = DateTime.Now;
                model.ModifyUserId = model.UserId;
                return db.Updateable<SysUserLogOn>(model).UpdateColumns(it => new
                {
                    it.Password,
                    it.ModifyUserId,
                    it.ModifyTime
                }).ExecuteCommand();
            }
        }
    }
}
