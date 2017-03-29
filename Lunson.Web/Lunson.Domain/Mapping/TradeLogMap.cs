using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class TradeLogMap : EntityTypeConfiguration<TradeLog>
    {
        public TradeLogMap()
        {
            this.ToTable("TradeLogs");
            HasRequired(t => t.Customer).WithMany().HasForeignKey(t => t.CustomerID);
        }
    }
}
