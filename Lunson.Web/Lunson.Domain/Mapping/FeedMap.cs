using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Lunson.Domain.Mapping
{
    public class FeedMap : EntityTypeConfiguration<Feed>
    {
        public FeedMap()
        {
            this.ToTable("Feeds");
            HasOptional(t => t.FeedType).WithMany().HasForeignKey(t => t.FeedTypeID);
        }
    }
}
