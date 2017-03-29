using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class TicketRefundMap : EntityTypeConfiguration<TicketRefund>
    {
        public TicketRefundMap()
        {
            this.ToTable("TicketRefunds");
            this.HasRequired(ur => ur.TicketDetail).WithMany().HasForeignKey(u => u.TicketDetailID);
        }
    }
}
