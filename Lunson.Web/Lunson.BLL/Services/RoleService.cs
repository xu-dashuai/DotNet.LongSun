using Lunson.DAL.Repositories;
using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.BLL.Services
{
    /// <summary>
    /// 角色服务类
    /// </summary>
    public class RoleService
    {
        private RoleRepository DAL = new RoleRepository();

        /// <summary>
        /// 获取状态集合项
        /// </summary>
        /// <returns></returns>
        public List<DropdownItem> GetActiveStates()
        {
            var activeStates = EnumHelper.GetList(typeof(ActiveState));
            return activeStates.Select(a => new DropdownItem {Text=a.Text,Value=a.Value}).ToList();
        }

        /// <summary>
        /// 获取角色集合项
        /// </summary>
        /// <returns></returns>
        public List<DropdownItem> GetDropdownRoles()
        {
            var roles = DAL.GetRoles();

            return roles.Select(a => new { a.ID, a.Name }).ToList().
                Select(a => new DropdownItem { Text = a.Name, Value = a.ID }).ToList();
        }

        /// <summary>
        /// 获取单个角色
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <returns></returns>
        public Sys_Role GetRole(string roleID,bool InClude=false)
        {
            return DAL.GetRole(roleID, InClude);
        }

        /// <summary>
        /// 获取角色集合
        /// </summary>
        /// <returns></returns>
        public IQueryable<Sys_Role> GetRoles(int pageIndex, int pageSize, out int totalCount)
        {
            return DAL.GetRoleList(pageIndex, pageSize, out totalCount);
        }

        public IQueryable<Sys_Role> GetRoles(bool isClude = false)
        {
            return DAL.GetRoles(isClude);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleID">角色ID</param>
        /// <param name="userID">操作者ID</param>
        /// <returns></returns>
        public bool DeleteRole(string roleID, string userID)
        {
            return DAL.DeleteRole(roleID, userID);
        }

        /// <summary>
        /// 保存角色，新增或修改
        /// </summary>
        /// <param name="entity">角色对象</param>
        /// <param name="userID">操作者ID</param>
        /// <returns></returns>
        public Object SaveRole(Sys_Role entity, string userID)
        {
            Sys_Role role;

            if (entity.ID.IsNullOrTrimEmpty())
            { //add
                role = DAL.AddRole(entity, userID);
                if (role == null)
                {
                    return new { validate = false, target = "Code", msg = "角色代码已存在" };
                }
            }
            else
            { //updata
                role = DAL.UpdateRole(entity, userID);
            }
            if (role == null)
            {
                return new { validate = false, target = "", msg = "找不到角色信息" };
            }
            else
            {
                return new { validate = true, target = "",msg="保存成功"};
            }
        }


        public void SavePermissions(string roleID, List<string> permissionIDs, string currentUserID)
        { 
            DAL.SavePermissions(roleID,permissionIDs,currentUserID);
        }


    }
}
