using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.DAL.Repositories
{
    public class PayLogRepository
    {
        EFDbContext context = ContextFactory.GetCurrentContext();

        public bool Add(PayLog entity)
        {
            context.PayLogs.Add(entity);
            return (context.SaveChanges() > 0);
        }
    }
}
