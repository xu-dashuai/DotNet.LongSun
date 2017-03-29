using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class Sys_RoleMap : EntityTypeConfiguration<Sys_Role>
    {
        public Sys_RoleMap()
        {
            this.ToTable("Sys_Roles");
            HasMany(u => u.UserRoles).WithOptional(ur => ur.Role).HasForeignKey(r => r.RoleID);
            HasMany(u => u.RolePermissions).WithOptional(ur => ur.Role).HasForeignKey(r => r.RoleID);
        }
    }
}
