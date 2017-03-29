using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lunson.Domain.Entities;
using Lunson.BLL.Services;
using Lunson.Domain;
using Pharos.Framework;
using System.Text.RegularExpressions;

namespace Lunson.Web
{
    public static class PermissionCache
    {
        /// <summary>
        /// 含有权限的角色数据
        /// </summary>
        public static List<Sys_Role> Roles { get; private set; }
        public static List<Sys_Menu> Menus { get; private set; }
        /// <summary>
        /// 重置权限
        /// </summary>
        public static void Reset()
        {
            RoleService rsv = new RoleService();
            MenuService msv = new MenuService();

            Roles = rsv.GetRoles(true).Where(a=>a.Status==ActiveState.Normal).ToList();
            Menus = msv.GetMenus(false).ToList();
        }
        public static List<Sys_Menu> GetCurrentMenus()
        {
            if (UserCache.CurrentUser.IsAdmin)
                return Menus;
            var menuIDs = GetCurrentPermissions().Where(a=>a.Code.Equals("view")).Select(a => a.MenuID).ToList();
            var menus= Menus.Where(a => menuIDs.Contains(a.ID)).ToList();

            var list = menus.Where(a => a.ParentID.IsNullOrTrimEmpty() == false && menus.Select(m => m.ID).Contains(a.ParentID) == false && Menus.Select(m => m.ID).Contains(a.ParentID)).Select(a => a.ParentID).Distinct().ToList();

            while (list.Any())
            {
                menus.AddRange(Menus.Where(a=>list.Contains(a.ID)));
                list = menus.Where(a => a.ParentID.IsNullOrTrimEmpty() == false && menus.Select(m => m.ID).Contains(a.ParentID) == false && Menus.Select(m => m.ID).Contains(a.ParentID)).Select(a => a.ParentID).Distinct().ToList();

            }
            return menus;
        }
        /// <summary>
        /// 获取当前用户所有权限
        /// </summary>
        public static List<Permission> GetCurrentPermissions()
        {
            var permissions = new List<Permission>();

            if (UserCache.CurrentUser != null)
            {
                var roleIDs = UserCache.CurrentUser.Roles.Select(a => a.RoleId);

                var tempPermissions = Roles.Where(a => roleIDs.Contains(a.ID)).SelectMany(a => a.RolePermissions).Select(a => a.Permission);
                foreach (var x in tempPermissions)
                {
                    if (permissions.Select(a => a.ID).Contains(x.ID) == false && x.IsDeleted == YesOrNo.No && x.Menu != null && x.Menu.IsDeleted == YesOrNo.No)
                    {
                        permissions.Add(x);
                    }
                }
            }

            return permissions;
        }
        /// <summary>
        /// 判断是否有权限
        /// </summary>
        /// <returns></returns>
        public static bool HavePermissionTo(string url, string fullUrl, string code)
        {
            //用户未登录
            if (UserCache.CurrentUser == null)
                return false;
            //用户为超级管理员
            if (UserCache.CurrentUser.IsAdmin)
                return true;

            var permissions = GetCurrentPermissions();

            Regex reg;

            //权限代码 权限判断地址一致
            foreach (var x in permissions.Where(a=>a.Code.Equals(code,StringComparison.OrdinalIgnoreCase)
                && a.Menu.BriefUrl.IsNullOrTrimEmpty()==false&& a.Menu.BriefUrl.Equals(url, StringComparison.OrdinalIgnoreCase)))
            {
                if (x.Regex.IsNullOrTrimEmpty())
                    return true;
                else
                {
                    reg = new Regex(x.Regex,RegexOptions.IgnoreCase);
                    if (reg.IsMatch(fullUrl))
                        return true;
                }
            }

            return false;
        }
        /// <summary>
        /// 获取某页面的权限代码
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fullUrl"></param>
        /// <returns></returns>
        public static List<string> GetPermissionCodes(string url,string fullUrl)
        {
            List<string> result = new List<string>();
            var permissions = GetCurrentPermissions();

            Regex reg;

            //权限代码 权限判断地址一致
            foreach (var x in permissions.Where(a => a.Menu.BriefUrl.Equals(url, StringComparison.OrdinalIgnoreCase)))
            {
                if (x.Regex.IsNullOrTrimEmpty())
                    result.Add(x.Code);
                else
                {
                    reg = new Regex(x.Regex, RegexOptions.IgnoreCase);
                    if (reg.IsMatch(fullUrl))
                        result.Add(x.Code);
                }
                
            }
            return result.Distinct().ToList();
        }
     }
}