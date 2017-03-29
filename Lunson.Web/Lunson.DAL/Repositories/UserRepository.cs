using Lunson.Domain;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pharos.Framework;

namespace Lunson.DAL.Repositories
{
    public class UserRepository
    {
        EFDbContext context = ContextFactory.GetCurrentContext();
        /// <summary>
        /// 由帐号密码获取USER
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Sys_User GetUser(string userName, string password)
        {
            var entity = context.Sys_Users.Include("UserRoles").Include("UserRoles.Role")
                .Where(u => u.UserName == userName
                && u.Password == password
                && u.IsDeleted == YesOrNo.No
                && u.Status == ActiveState.Normal
                ).FirstOrDefault();
            return entity;
        }
        public void EditUserPassword(string userid, string password, string currentUserID)
        {
            var user = GetUser(userid);
            user.Password = password;
            user.ModifiedBy = currentUserID;
            user.ModifiedOn = DateTime.Now;
            context.SaveChanges();
        }
        public Sys_User GetUser(string id, bool inClude = true)
        {
            if (inClude)
                return context.Sys_Users.Include("UserRoles").Include("UserRoles.Role").SingleOrDefault(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(id));
            return context.Sys_Users.SingleOrDefault(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(id));
        }

        public IQueryable<Sys_User> GetUsers()
        {
            return context.Sys_Users.Include("UserRoles").Include("UserRoles.Role").Where(a => a.IsDeleted == YesOrNo.No);
        }
        public void RemoveUser(string userid, string currentUserID)
        {
            var user = GetUser(userid);
            if (user.IsAdmin != 1)
            {
                user.IsDeleted = YesOrNo.Yes;
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = currentUserID;
                context.SaveChanges();
            }
        }
        public bool CheckUserName(string userName)
        {

            return context.Sys_Users.Any(a => a.IsDeleted == YesOrNo.No && a.UserName.Equals(userName));
        }

        public bool SaveUser(Sys_User user, List<string> rolelist, string currentUserID)
        {
            if (user.ID.IsNullOrTrimEmpty())
            {
                #region
                user.ID = DataHelper.GetSystemID();
                user.CreatedOn = DateTime.Now;
                user.CreatedBy = currentUserID;
                context.Sys_Users.Add(user);

                //权限
                var roleIDs = (from x in context.Sys_Roles
                               where rolelist.Contains(x.ID)
                               && x.IsDeleted == YesOrNo.No
                               select x.ID);
                foreach (var x in roleIDs)
                {
                    var userRole = new Sys_UserRole();
                    userRole.ID = DataHelper.GetSystemID();
                    userRole.CreatedBy = currentUserID;
                    userRole.CreatedOn = DateTime.Now;
                    userRole.IsDeleted = YesOrNo.No;
                    userRole.RoleID = x;
                    userRole.UserID = user.ID;
                    context.Sys_UserRoles.Add(userRole);
                }
                #endregion
            }
            else
            {
                var u = context.Sys_Users.SingleOrDefault(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(user.ID));
                u.CopyProperty(user);
                u.ModifiedBy = currentUserID;
                u.ModifiedOn = DateTime.Now;

                //权限
                var roles = (from x in context.Sys_UserRoles
                             where x.IsDeleted == YesOrNo.No
                             && x.UserID.Equals(u.ID)
                             select x);
                context.Sys_UserRoles.RemoveRange(roles);

                var roleIDs = (from x in context.Sys_Roles
                               where rolelist.Contains(x.ID)
                               && x.IsDeleted == YesOrNo.No
                               select x.ID);
                foreach (var x in roleIDs)
                {
                    var userRole = new Sys_UserRole();
                    userRole.ID = DataHelper.GetSystemID();
                    userRole.CreatedBy = currentUserID;
                    userRole.CreatedOn = DateTime.Now;
                    userRole.IsDeleted = YesOrNo.No;
                    userRole.RoleID = x;
                    userRole.UserID = u.ID;
                    context.Sys_UserRoles.Add(userRole);
                }
            }


            return context.SaveChanges() > 0;

        }

        public void SaveUserLoginLog(string currentUserID, string IP)
        {


            var u = context.Sys_Users.Find(currentUserID);
            u.IP = IP;
            u.LastLoginTime = DateTime.Now;
            u.LoginTimes = (u.LoginTimes == null ? 1 : (u.LoginTimes + 1));

            var log = new UserLoginLog();
            log.ID = DataHelper.GetSystemID();
            log.IP = IP;
            log.LoginTime = DateTime.Now;
            log.UserID = currentUserID;
            context.UserLoginLogs.Add(log);

            context.SaveChanges();
        }
    }
}
