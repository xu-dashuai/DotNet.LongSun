using Lunson.DAL.Repositories;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pharos.Framework;

namespace Lunson.BLL.Services
{
    public class PermissionService
    {
        private PermissionRepository DAL = new PermissionRepository();

        public object GetSelectPermissions()
        {
            return DAL.GetSelectPermissions();
        }
        public Permission GetPermission(string id)
        {
            return DAL.GetPermission(id);
        }
        public object SavePermission(Permission permission,string currentUerID)
        {
            if (permission.Name.IsNullOrTrimEmpty())
            {
                return new { validate = false, target = "Name", msg = "权限名称不能为空" };
            }
            if (permission.Code.IsNullOrTrimEmpty())
            {
                return new { validate = false, target = "Code", msg = "权限代码不能为空" };
            }

            permission.Name = permission.Name.Trim();
            permission.Code = permission.Code.Trim();

            if (permission.ID.IsNullOrTrimEmpty())
            {
                DAL.AddPermission(permission,currentUerID);
            }
            else
            {
                DAL.EditPermission(permission,currentUerID);
            }
            DAL.SaveChanges();
            return new { validate = true, target = "", msg = "保存成功" };
        }
        public int RemovePermission(string id, string currentUserID)
        {
            DAL.RemovePermission(id,currentUserID);
            return DAL.SaveChanges();
        }
    }
}
