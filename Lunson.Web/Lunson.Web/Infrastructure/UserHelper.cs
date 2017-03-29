using Lunson.Domain.Entities;
using Lunson.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lunson.Web
{
    public class UserHelper
    {
        /// <summary>
        /// 构造登录信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static UserData ToUserData(Sys_User user)
        {
            var roleList = user.UserRoles.Select(ur => ur.Role).ToList();
            var supperRoleCount = roleList.Where(r => r.Name.ToLower() == "supperadmin").Count();
            var isAdmin = false;
            if (user.IsAdmin.HasValue)
                isAdmin = user.IsAdmin.Value == 1;
            if (supperRoleCount == 1)
                isAdmin = true;
            UserData userData = new UserData()
            {
                Id = user.ID,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                IsAdmin = isAdmin,
                Roles = (from r in roleList select new RoleData() { RoleId = r.ID, RoleName = r.Name }).ToList()
            };
            return userData;
        }
        public static void SetUserSession(UserData userData)
        {
            HttpContext.Current.Session["CurrentUser"] = userData;
        }
    }
}