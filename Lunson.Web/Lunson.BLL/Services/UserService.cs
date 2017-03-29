using Lunson.DAL.Repositories;
using Lunson.Domain.Entities;
using Pharos.Framework;
using Pharos.Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.BLL.Services
{
    public class UserService
    {
        private UserRepository DAL = new UserRepository();

        public string GetEncryPassword(string password, bool isCookiePwd = false)
        {
            var pwd = password;
            if (!isCookiePwd || password.Length < 21)
                pwd = GetEncryPwd(password);
            return pwd;
        }
        /// <summary>
        /// 验证用户登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public OpResult CheckLogin(string userName, string password)
        {
            var user = DAL.GetUser(userName, password);

            if (user == null)
                return OpResult.Fail(message: "用户名或密码错误");
            if (user.IsAdmin == 1 || (user.UserRoles != null && user.UserRoles.Count() > 0))
                return OpResult.Success(message: "用户验证成功", data: user);
            else
                return OpResult.Fail(message: "该用户尚未配置权限，请联系管理员！");
        }
        private string GetEncryPwd(string password)
        {
            return SecurityHelper.GetMD5String(password);
        }

        public object GetUsers()
        {
            var users = DAL.GetUsers();

            return users.ToList().Select(a => new
            {
                a.ID,
                a.IP,
                a.IsAdmin,
                a.UserName,
                a.DisplayName,
                a.Status,
                a.Sex,
                a.LoginTimes,
                a.LastLoginTime,
                Role=string.Join(",",a.UserRoles.Select(r=>r.Role.Name))
            });

        }

        public Sys_User GetUser(string id,bool inClude=true)
        {
            return DAL.GetUser(id, inClude);
        }

        public object SaveUser(Sys_User user,List<string> rolelist, string currentUserID)
        {
            if (DAL.CheckUserName(user.UserName))
                return new { validate = false, target = "UserName", msg = "用户名已存在" };
            var result = DAL.SaveUser(user, rolelist, currentUserID);
            return new { validate = result, target = "", msg = "保存失败" };
        }
        public object EditUserPassword(string userid, string password, string currentUserID)
        {
            DAL.EditUserPassword(userid,GetEncryPassword(password),currentUserID);
            return new { validate = true };
        }
        public object SaveUser(string userid, string displayName, List<string> rolelist, string currentUserID)
        {
            var user = DAL.GetUser(userid);
            if (user != null)
            {
                user.DisplayName = displayName;
                var result = DAL.SaveUser(user, rolelist, currentUserID);
                return new { validate = result, target = "", msg = "保存失败" };
            }
            return new { validate = false, target = "", msg = "找不到用户信息" };
        }
        public void RemoveUser(string userid, string currentUserID)
        {
            DAL.RemoveUser(userid,currentUserID);
        }
        public void SaveUserLoginLog(string currentUserID,string IP)
        {
            DAL.SaveUserLoginLog(currentUserID, IP);
        }
    }
}
