using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class Sys_MenuMap : EntityTypeConfiguration<Sys_Menu>
    {
        public Sys_MenuMap()
        {
            this.ToTable("Sys_Menus");
            HasMany(u => u.Permissions).WithOptional(ur => ur.Menu).HasForeignKey(r => r.MenuID);
        }
    }
}
