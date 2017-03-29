using Lunson.Domain;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pharos.Framework;

namespace Lunson.DAL.Repositories
{
    public class MenuRepository
    {
        private EFDbContext context = ContextFactory.GetCurrentContext();

        public bool CheckExist(string id)
        { 
            return context.Sys_Menus.Any(a=>a.IsDeleted==YesOrNo.No&&a.ID.Equals(id));                
        }
        public bool CheckExistChildren(string id)
        {
            return context.Sys_Menus.Any(a => a.IsDeleted == YesOrNo.No && a.ParentID.Equals(id));    
        }
        public bool CheckLegal(string id,string pid)
        {
            if (id.IsNullOrTrimEmpty())
                return true;

            if (pid.Equals(id))
                return false;
            var menus = context.Sys_Menus.Where(a => a.IsDeleted == YesOrNo.No).ToList();

            var children = new List<Sys_Menu>();
            var curChildren = menus.Where(a =>id.Equals(a.ParentID)).ToList();
            while (curChildren.Any())
            {
                if (curChildren.Any(a => a.ID.Equals(pid)))
                    return false;
                curChildren = menus.Where(a => curChildren.Select(b => b.ID).Contains(a.ParentID)).ToList();
            }
            return true;
        }
        public bool RemoveMenu(string id, string userID)
        {
            var menu = context.Sys_Menus.SingleOrDefault(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(id));
            if (menu == null)
                return false;
            menu.IsDeleted = YesOrNo.Yes;
            menu.ModifiedOn = DateTime.Now;
            menu.ModifiedBy = userID;
            return context.SaveChanges()>0;
        }
        public bool UpMenu(string id, string userID)
        {
            var menu = context.Sys_Menus.SingleOrDefault(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(id));
            if (menu == null)
                return false;
            menu.ParentID = "";
            menu.ModifiedOn = DateTime.Now;
            menu.ModifiedBy = userID;
            return context.SaveChanges() > 0;
        }
        public bool Save(Sys_Menu menu,string userID)
        {
            try
            {
                if (menu.ID.IsNullOrTrimEmpty())
                {
                    var m = new Sys_Menu();
                    m.CopyProperty(menu);
                    m.CreatedBy = userID;
                    m.CreatedOn = DateTime.Now;
                    m.ID = DataHelper.GetSystemID();
                    m.IsDeleted = YesOrNo.No;

                    context.Sys_Menus.Add(m);
                }
                else
                {
                    var m = context.Sys_Menus.Find(menu.ID);
                    m.CopyProperty(menu);
                    m.ModifiedBy = userID;
                    m.ModifiedOn = DateTime.Now;
                }
                return context.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Sys_Menu GetMenu(string id, bool inClude=false)
        {
            if (inClude)
                return context.Sys_Menus.Include("Permissions").SingleOrDefault(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(id));

            return context.Sys_Menus.SingleOrDefault(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(id));
        }

        public IQueryable<Sys_Menu> GetMenus()
        {
            return context.Sys_Menus.Where(a => a.IsDeleted == YesOrNo.No);
        }
    }
}
