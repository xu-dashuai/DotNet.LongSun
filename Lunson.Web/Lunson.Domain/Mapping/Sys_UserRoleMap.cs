using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class Sys_UserRoleMap : EntityTypeConfiguration<Sys_UserRole>
    {
        public Sys_UserRoleMap()
        {
            this.ToTable("Sys_UserRoles");
            HasOptional(ur => ur.Role).WithMany().HasForeignKey(u => u.RoleID);
            HasOptional(ur => ur.User).WithMany().HasForeignKey(u => u.UserID);
        }
    }
}
