using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class LinkMap : EntityTypeConfiguration<Link>
    {
        public LinkMap()
        {
            this.ToTable("Links");
            HasOptional(l => l.Annex).WithMany().HasForeignKey(l => l.ImgID);
        }
    }
}
