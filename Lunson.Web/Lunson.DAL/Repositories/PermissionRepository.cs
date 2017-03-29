using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.DAL.Repositories
{
    public class PermissionRepository
    {
        private EFDbContext context = ContextFactory.GetCurrentContext();

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public object GetSelectPermissions()
        {
            return (from x in context.Permissions
                    join y in context.Sys_Menus on x.MenuID equals y.ID
                    where y.Address != null && y.Address.Trim() != ""
                    && x.IsDeleted==YesOrNo.No
                    && y.IsDeleted==YesOrNo.No
                    select new
                    {
                        x.ID,
                        x.Name,
                        x.Code,
                        MenuName=y.Name,
                        MenuID=x.MenuID
                    }).OrderBy(a=>a.MenuName).ThenBy(a=>a.Name);
        }
        public Permission GetPermission(string id)
        {
            return context.Permissions.SingleOrDefault(a => a.ID.Equals(id) && a.IsDeleted == YesOrNo.No);
        }
        public void RemovePermission(string id, string currentUserID)
        {
            var permission = GetPermission(id);
            if (permission != null)
            {
                permission.IsDeleted = YesOrNo.Yes;
                permission.ModifiedBy = currentUserID;
                permission.ModifiedOn = DateTime.Now;
            }
        }

        public void AddPermission(Permission permission, string currentUserID)
        {
            permission.ID = DataHelper.GetSystemID();
            permission.CreatedBy = currentUserID;
            permission.CreatedOn = DateTime.Now;
            context.Permissions.Add(permission);
        }

        public void EditPermission(Permission permission, string currentUserID)
        {
            var p = GetPermission(permission.ID);
            if (p != null)
            {
                p.CopyProperty(permission);
                p.ModifiedOn = DateTime.Now;
                p.ModifiedBy = currentUserID;
            }
        }
    }
}
