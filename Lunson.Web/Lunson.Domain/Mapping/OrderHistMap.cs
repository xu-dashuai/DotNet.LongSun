using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class OrderHistMap : EntityTypeConfiguration<OrderHist>
    {
        public OrderHistMap()
        {
            this.ToTable("OrderHists");
        }
    }
}
