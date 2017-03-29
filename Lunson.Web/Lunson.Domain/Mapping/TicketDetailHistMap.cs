using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class TicketDetailHistMap : EntityTypeConfiguration<TicketDetailHist>
    {
        public TicketDetailHistMap()
        {
            this.ToTable("TicketDetailHists");
            HasRequired(t => t.Customer).WithMany().HasForeignKey(t => t.CustomerID);
            HasRequired(t => t.OrderDetail).WithMany().HasForeignKey(t => t.OrderDetailID);
            HasRequired(t => t.TicketDetail).WithMany().HasForeignKey(t => t.TicketDetailID);
        }
    }
}
