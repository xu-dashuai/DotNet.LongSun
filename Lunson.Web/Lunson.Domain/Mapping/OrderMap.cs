using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            this.ToTable("Orders");
            HasMany(u => u.OrderDetails).WithOptional(ur => ur.Order).HasForeignKey(r => r.OrderID);
            HasRequired(o => o.Customer).WithMany().HasForeignKey(o => o.CustomerID);
        }
    }
}
