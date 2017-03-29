using Lunson.BLL.Services;
using Lunson.Domain;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lunson.Web.Areas.Backend.Controllers
{
    public class MenuController : BaseController
    {
        private MenuService msv = new MenuService();

        [PermissionCheck("view","index")]
        public ActionResult Index()
        {
            return View();
        }

        #region 取树型菜单
        [HttpPost]
        public ActionResult GetMenus()
        {
            return Json(msv.GetMenusTreeGrid());
        }

        [HttpPost]
        public ActionResult GetNavMenus()
        {
            return Json(msv.GetMenusTreeGrid(PermissionCache.GetCurrentMenus().Where(a => a.Type == MenuType.Show).ToList()));
        }
        #endregion

        /// <summary>
        /// 保存菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [HttpPost]
        [PermissionCheck("edit", "index")]
        public ActionResult SaveMenu(Sys_Menu menu)
        {
            var result = msv.SaveMenu(menu, UserCache.CurrentUser.Id);
            //重置权限缓存
            PermissionCache.Reset();
            return Json(result);
        }
        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [PermissionCheck("delete", "index")]
        public ActionResult RemoveMenu(string id)
        {
            var result = msv.RemoveMenu(id, UserCache.CurrentUser.Id);
            //重置权限缓存
            PermissionCache.Reset();
            return Json(result);
        }
        /// <summary>
        /// 菜单置顶
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [PermissionCheck("top", "index")]
        public ActionResult UpMenu(string id)
        {
            var result = msv.UpMenu(id, UserCache.CurrentUser.Id);
            //重置权限缓存
            PermissionCache.Reset();
            return Json(result);
        }
    }
}
