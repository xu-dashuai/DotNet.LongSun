using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunson.Domain.Entities;
using Lunson.Domain;

namespace Lunson.DAL.Repositories
{
    public class ReportRepository : BaseRepository
    {
        //EFDbContext context = ContextFactory.GetCurrentContext();

        public ReportRepository() : base() { }
        public ReportRepository(EFDbContext context) : base(context) { }



        public IQueryable<T> GetDetailReportData<T>() where T : new()
        {
            return null;
        }

        /// <summary>
        /// 获取所有的票数据，填充下拉框
        /// </summary>
        /// <returns></returns>
        public List<Ticket> GetAllTicket()
        {
            var data = context.Tickets.Where(p => p.IsDeleted != YesOrNo.Yes);
            return data.ToList();
        }

    }
}
