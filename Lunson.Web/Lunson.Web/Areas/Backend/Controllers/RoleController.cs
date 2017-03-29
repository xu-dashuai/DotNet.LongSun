using Lunson.BLL.Services;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Lunson.Web.Areas.Backend.Controllers
{
    public class RoleController : BaseController
    {
        private RoleService rsv = new RoleService();

        [PermissionCheck("view","index")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 角色Grid数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public ActionResult GetDataAll(int page = 1, int rows = 20)
        {
            int totalCount;
            var result = rsv.GetRoles(page, rows, out totalCount);
            return Json(new { total = totalCount, rows = result });
        }

        /// <summary>
        /// 角色编辑Form
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        [PermissionCheck("edit", "index")]
        public ActionResult RoleManageForm(string roleID)
        {
            Sys_Role model = new Sys_Role();
            var activeStates = rsv.GetActiveStates();
            if (roleID != null && roleID != "null")
            {
                model = rsv.GetRole(roleID);
                foreach (var x in activeStates)
                {
                    if ((int)model.Status == int.Parse(x.Value))
                    {
                        x.IsSelected = true;
                    }
                    else
                    {
                        x.IsSelected = false;
                    }
                }
            }
            else
            { //add时，状态勾选默认值
                activeStates.First(a => a.Value.Equals("0")).IsSelected = true;
            }
            ViewBag.activeStates = activeStates;
            //重置权限缓存
            PermissionCache.Reset();
            return View(model);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        [PermissionCheck("delete", "index")]
        public bool DeleteData(string roleID)
        {
            var result= rsv.DeleteRole(roleID, UserCache.CurrentUser.Id);

            //重置权限缓存
            PermissionCache.Reset();
            return result;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model">角色对象</param>
        /// <returns></returns>
        [PermissionCheck("edit", "index")]
        public ActionResult SaveData(Sys_Role model)
        {
            Regex reg = new Regex(@"^[a-zA-Z]{1}[0-9a-zA-Z_]{1,9}$");
            if (model.Name.IsNullOrTrimEmpty())
            {
                return Json(new { validate = false, target = "Name", msg = "角色名称不能为空" });
            }
            if (model.Code.IsNullOrTrimEmpty() || reg.IsMatch(model.Code) == false)
            {
                return Json(new { validate = false, target = "Code", msg = "以字母开头，只能含有字母数字下划线，长度2-10位" });
            }

            var result = rsv.SaveRole(model, UserCache.CurrentUser.Id);
            //重置权限缓存
            PermissionCache.Reset();
            return Json(result);
        }

    }
}
