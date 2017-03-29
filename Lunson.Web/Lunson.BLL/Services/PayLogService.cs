using Lunson.DAL.Repositories;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.BLL.Services
{
    public class PayLogService
    {
        public PayLogRepository DAL = new PayLogRepository();
        public bool AddLog(PayLog entity)
        {
            return DAL.Add(entity);
        }
    }
}
