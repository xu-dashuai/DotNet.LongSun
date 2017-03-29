using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class PermissionMap : EntityTypeConfiguration<Permission>
    {
        public PermissionMap()
        {
            this.ToTable("Permissions");
            HasOptional(ur => ur.Menu).WithMany().HasForeignKey(u => u.MenuID);
        }
    }
}
