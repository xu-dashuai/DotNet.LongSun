using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class TicketDetailMap : EntityTypeConfiguration<TicketDetail>
    {
        public TicketDetailMap()
        {
            this.ToTable("TicketDetails");
            HasRequired(t => t.Customer).WithMany().HasForeignKey(t => t.CustomerID);
            HasRequired(t => t.OrderDetail).WithMany().HasForeignKey(t => t.OrderDetailID);
            HasOptional(ur => ur.Annex).WithMany().HasForeignKey(u => u.MainImgID);
        }
    }
}
