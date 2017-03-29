using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class OrderDetailMap : EntityTypeConfiguration<OrderDetail>
    {
        public OrderDetailMap()
        {
            this.ToTable("OrderDetails");
            HasOptional(ur => ur.Order).WithMany().HasForeignKey(u => u.OrderID);
            HasOptional(ur => ur.Ticket).WithMany().HasForeignKey(u => u.TicketID);
        }
    }
}
