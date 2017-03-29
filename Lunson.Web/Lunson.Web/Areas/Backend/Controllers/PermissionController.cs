using Lunson.BLL.Services;
using Lunson.Domain;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pharos.Framework;

namespace Lunson.Web.Areas.Backend.Controllers
{
    public class PermissionController : BaseController
    {
        private PermissionService psv = new PermissionService();
        private RoleService rsv = new RoleService();
        private MenuService msv = new MenuService();

        /// <summary>
        /// 角色权限绑定
        /// </summary>
        /// <returns></returns>
        [PermissionCheck("bind","menu")]
        public ActionResult Index(string id)
        {
            var role = rsv.GetRole(id, true);
            if (role == null)
                return View("ERROR");
            return View(role);
        }
        [HttpPost]
        public ActionResult GetPermissions(string id)
        {
            var result = psv.GetSelectPermissions();
            return Json(new { total = (result as IQueryable<dynamic>).Count(), rows = result });
        }

        [PermissionCheck("view", "menu")]
        public ActionResult Menu()
        {
            return View();
        }
        [PermissionCheck("view", "menu")]
        public ActionResult GetMenu(string id)
        {
            var menu = msv.GetMenu(id, false);
            if (menu == null)
                return View("Error");
            return View(menu);
        }

        [HttpPost]
        public ActionResult GetMenuPermissions(string id)
        {
            var menu = msv.GetMenu(id, true);
            if (menu == null)
                return Json(new { total=0,rows=""});
            var permissions = menu.Permissions.Where(a => a.IsDeleted == YesOrNo.No);
            return Json(new { total=permissions.Count(),rows=permissions});
        }

        [PermissionCheck("edit", "menu")]
        public ActionResult EditPermission(string menuid,string id="")
        {
            var permission = psv.GetPermission(id);
            if (permission == null)
            {
                permission = new Permission { MenuID=menuid};
            }
            return View(permission);
        }

        [HttpPost]
        [ValidateInput(false)]
        [PermissionCheck("edit", "menu")]
        public ActionResult SavePermission(Permission permission)
        {
            var result = psv.SavePermission(permission, UserCache.CurrentUser.Id);
            //重置权限缓存
            PermissionCache.Reset();
            return Json(result);
        }

        [HttpPost]
        [PermissionCheck("bind", "menu")]
        public ActionResult SavePermissions(string id, FormCollection c)
        {
            var rights = c["rightids[]"].IsNullOrTrimEmpty() ? "" : c["rightids[]"];
            var permissions = rights.Split(',').ToList();
            rsv.SavePermissions(id, permissions, UserCache.CurrentUser.Id);
            //重置权限缓存
            PermissionCache.Reset();
            return Json("");
        }

        [HttpPost]
        [PermissionCheck("delete", "menu")]
        public ActionResult RemovePermission(string id)
        {
            psv.RemovePermission(id, UserCache.CurrentUser.Id);
            //重置权限缓存
            PermissionCache.Reset();
            return Json("");
        }
    }
}
