using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class AnnexMap : EntityTypeConfiguration<Annex>
    {
        public AnnexMap()
        {
            this.ToTable("Annexes");
        }
    }
}
