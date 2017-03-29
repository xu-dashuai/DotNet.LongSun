using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class Sys_UserMap : EntityTypeConfiguration<Sys_User>
    {
        public Sys_UserMap()
        {
            this.ToTable("Sys_Users");
            HasMany(u => u.UserRoles).WithOptional(ur => ur.User).HasForeignKey(r => r.UserID);
        }
    }
}
