using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.DAL.Repositories
{
    public class RoleRepository
    {
        EFDbContext context = ContextFactory.GetCurrentContext();

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        public IQueryable<Sys_Role> GetRoles(bool inClude = false)
        {
            if (inClude)
                return context.Sys_Roles.Include("RolePermissions").Include("RolePermissions.Permission").Include("RolePermissions.Permission.Menu").Where(a => a.IsDeleted == YesOrNo.No);

            return context.Sys_Roles.Where(a => a.IsDeleted == YesOrNo.No);
        }

        public IQueryable<Sys_Role> GetRoleList(int pageIndex, int pageSize, out int totalCount)
        {
            var entityList = context.Sys_Roles.Where(r => r.IsDeleted == YesOrNo.No);
            totalCount = entityList.Count();
            entityList = entityList.OrderBy(r => r.Status).Skip((pageIndex - 1) * pageIndex).Take(pageSize);
            return entityList;
        }

        /// <summary>
        /// 获取单个角色对象
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        public Sys_Role GetRole(string roleID, bool InClude = false)
        {
            if (InClude)
                return context.Sys_Roles.Include("RolePermissions").SingleOrDefault(r => r.ID.Equals(roleID) && r.IsDeleted == YesOrNo.No);
            var entity = context.Sys_Roles.SingleOrDefault(r => r.ID.Equals(roleID) && r.IsDeleted == YesOrNo.No);
            return entity;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="entity">角色对象</param>
        /// <param name="userID">操作者ID</param>
        /// <returns></returns>
        public Sys_Role AddRole(Sys_Role entity, string userID)
        {
            var isExist = context.Sys_Roles.Any(r => r.Code.Equals(entity.Code) && r.IsDeleted == YesOrNo.No);
            if (isExist == true)
            { //角色代码已存在，返回空对象
                return entity = null;
            }

            entity.ID = DataHelper.GetSystemID();
            entity.CreatedBy = userID;
            entity.CreatedOn = DateTime.Now;

            context.Sys_Roles.Add(entity);
            context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="entity">新的角色对象</param>
        /// <param name="userID">操作者ID</param>
        /// <returns></returns>
        public Sys_Role UpdateRole(Sys_Role entity, string userID)
        {
            var dbEntity = context.Sys_Roles.SingleOrDefault(r => r.ID.Equals(entity.ID) && r.IsDeleted == YesOrNo.No);
            if (dbEntity != null)
            {
                dbEntity.CopyProperty(entity);

                dbEntity.ModifiedBy = userID;
                dbEntity.ModifiedOn = DateTime.Now;
                context.SaveChanges();
            }
            return dbEntity;
        }

        /// <summary>
        /// 删除一个角色
        /// </summary>
        /// <param name="roleID">角色对象</param>
        /// <param name="userID">操作者ID</param>
        /// <returns></returns>
        public bool DeleteRole(string roleID, string userID)
        {
            var dbEntity = context.Sys_Roles.SingleOrDefault(r => r.ID.Equals(roleID) && r.IsDeleted == YesOrNo.No);
            if (dbEntity != null)
            {
                dbEntity.IsDeleted = YesOrNo.Yes;
                dbEntity.ModifiedBy = userID;
                dbEntity.ModifiedOn = DateTime.Now;
            }
            return context.SaveChanges() > 0;
        }

        public void SavePermissions(string roleID, List<string> permissionIDs, string currentUserID)
        {
            var role = GetRole(roleID);
            if (role != null)
            {
                //删除
                var dels = context.RolePermissions.Where(a => a.RoleID.Equals(roleID) && permissionIDs.Contains(a.PermissionID) == false);
                foreach (var x in dels)
                {
                    context.RolePermissions.Remove(x);
                }
                //添加
                var temp = context.RolePermissions.Where(a => a.RoleID.Equals(roleID)).Select(a => a.PermissionID);
                var adds = permissionIDs.Where(a => temp.Contains(a) == false).ToList();
                adds = context.Permissions.Where(a => adds.Contains(a.ID)).Select(a => a.ID).ToList();
                foreach (var x in adds)
                {
                    context.RolePermissions.Add(new RolePermission
                    {
                        ID = DataHelper.GetSystemID(),
                        PermissionID = x,
                        RoleID = roleID,
                        CreatedBy = currentUserID,
                        CreatedOn = DateTime.Now
                    });
                }

                context.SaveChanges();
            }
        }

    }
}
