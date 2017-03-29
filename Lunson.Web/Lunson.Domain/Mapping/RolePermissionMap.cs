using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class RolePermissionMap : EntityTypeConfiguration<RolePermission>
    {
        public RolePermissionMap()
        {
            this.ToTable("RolePermissions");
            HasOptional(ur => ur.Permission).WithMany().HasForeignKey(u => u.PermissionID);
            HasOptional(ur => ur.Role).WithMany().HasForeignKey(u => u.RoleID);
        }
    }
}
