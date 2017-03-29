using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class PayLogMap: EntityTypeConfiguration<PayLog>
    {
        public PayLogMap()
        {
            this.ToTable("PayLogs");
        }
    }
}
