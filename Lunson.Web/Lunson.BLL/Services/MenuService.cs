using Lunson.DAL.Repositories;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pharos.Framework;

namespace Lunson.BLL.Services
{
    public class MenuService
    {
        private MenuRepository DAL = new MenuRepository();
        /// <summary>
        /// 获取菜单树型表格数据
        /// </summary>
        /// <returns></returns>
        public object GetMenusTreeGrid()
        {
            var menus = DAL.GetMenus().ToList();
            menus.ForEach(menu =>
            {
                if (menu.ParentID.IsNullOrTrimEmpty())
                    menu.ParentID = "";
            });

            return GetMenusTreeGrid(menus,"");
        }
        public object GetMenusTreeGrid(List<Sys_Menu> menus)
        {
            menus.ForEach(menu =>
            {
                if (menu.ParentID.IsNullOrTrimEmpty())
                    menu.ParentID = "";
            });

            return GetMenusTreeGrid(menus, "");
        }
        /// <summary>
        /// 递归获取菜单树形表格数据
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="parentID"></param>
        /// <returns></returns>
        private object GetMenusTreeGrid(List<Sys_Menu> menus, string parentID)
        {
            parentID = parentID.IsNullOrTrimEmpty() ? "" : parentID;
            return menus.Where(a => a.ParentID.Equals(parentID)).OrderBy(a => a.ShowOrder)
                .Select(a => new
                {
                    a.ID,
                    a.Name,
                    Address = a.Address.ToEmptyString(),
                    BriefUrl=a.BriefUrl.ToEmptyString(),
                    ParentID = a.ParentID.ToEmptyString(),
                    a.ShowOrder,
                    a.Type,
                    children = GetMenusTreeGrid(menus, a.ID),
                    text = a.Name,
                    id = a.ID,
                    attributes = GetMenuAttribute(a)
                });
        }
        private object GetMenuAttribute(Sys_Menu menu)
        {
            if (menu.Address.IsNullOrTrimEmpty())
                return new { };
            else
                return new { url=menu.Address};
        }
        public object RemoveMenu(string id, string userID)
        {
            //有子菜单不能删除
            if (DAL.CheckExistChildren(id))
                return new { validate = false, msg = "删除失败，有子菜单不能删除" };

            var result = DAL.RemoveMenu(id, userID);
            return new { validate=result,msg="删除失败，请刷新后重试"};
        }
        public object UpMenu(string id, string userID)
        {
            var result = DAL.UpMenu(id, userID);
            return new { validate = result, msg = "置顶失败，请刷新后重试" };
        }
        public object SaveMenu(Sys_Menu menu,string userID)
        {
            //菜单名称不能为空
            if (menu.Name.IsNullOrTrimEmpty())
                return new { validate = false, msg = "菜单名称：菜单名称不能为空" };
            //菜单不能为已删除或不存在
            if (menu.ID.IsNullOrTrimEmpty() == false && DAL.CheckExist(menu.ID) == false)
                return new { validate = false, msg = "菜单名称：该菜单不存在" };
            //验证上层菜单的合法性
            if (menu.ParentID.IsNullOrTrimEmpty() == false)
            {
                if (DAL.CheckExist(menu.ParentID) == false)
                    return new { validate = false, msg = "上层菜单：该菜单不存在" };
                if (DAL.CheckLegal(menu.ID,menu.ParentID) == false)
                    return new { validate = false, msg = "上层菜单：上层菜单不能为当前菜单或当前菜单的子菜单" };
            }
            var result = DAL.Save(menu, userID);
            return new { validate = result, msg = "保存失败" };
        }

        public Sys_Menu GetMenu(string id,bool inClude)
        {
            return DAL.GetMenu(id,inClude);
        }
        public IQueryable<Sys_Menu> GetMenus(bool inClude = false)
        {
            return DAL.GetMenus();
        }
    }
}
