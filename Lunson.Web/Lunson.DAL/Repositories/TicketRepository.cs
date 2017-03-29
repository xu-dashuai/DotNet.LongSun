using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using Pharos.Framework.MVC;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace Lunson.DAL.Repositories
{
    /// <summary>
    /// 票券仓库
    /// </summary>
    public class TicketRepository : BaseRepository
    {
        public TicketRepository() : base() { }
        public TicketRepository(EFDbContext context) : base(context) { }

        #region 存历史
        /// <summary>
        /// 存票模型历史
        /// </summary>
        public void SaveTicketHist(Ticket ticket)
        {
            //初次添加数据不需要存历史
            if (ticket.ID.IsNullOrTrimEmpty() == false)
            {
                var hist = new TicketHist();
                hist.CopyProperty(ticket);
                hist.TicketID = ticket.ID;
                hist.CreatedBy = ticket.CreatedBy;
                hist.CreatedOn = ticket.CreatedOn;
                hist.ModifiedBy = ticket.ModifiedBy;
                hist.ModifiedOn = ticket.ModifiedOn;
                hist.ID = DataHelper.GetSystemID();
                context.TicketHists.Add(hist);
            }
        }
        /// <summary>
        /// 存票历史
        /// </summary>
        public void SaveTicketDetailHist(TicketDetail ticket)
        {
            //初次添加数据不需要存历史
            if (ticket.ID.IsNullOrTrimEmpty() == false)
            {
                var hist = new TicketDetailHist();
                hist.CopyProperty(ticket);
                hist.TicketDetailID = ticket.ID;
                hist.CreatedBy = ticket.CreatedBy;
                hist.CreatedOn = ticket.CreatedOn;
                hist.ModifiedBy = ticket.ModifiedBy;
                hist.ModifiedOn = ticket.ModifiedOn;
                hist.ID = DataHelper.GetSystemID();
                context.TicketDetailHists.Add(hist);
            }
        }
        #endregion

        #region 退票
        public void RefundTicket(TicketDetail ticket, string currentUserID)
        {
            if (ticket.Status == TicketStatus.Refunding || ticket.Status == TicketStatus.HasPay)
            {
                //存历史
                SaveTicketDetailHist(ticket);
                //变更票状态
                ticket.Status = TicketStatus.Refunded;
                ticket.RefundedTime = DateTime.Now;
                SaveInfo<TicketDetail>(ticket, currentUserID);
            }
        }
        #endregion


        /// <summary>
        /// 门票详情
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public Ticket TicketParticular(string id)
        {
            var data = context.Tickets.Include("Annex").Where(p => p.ID.Equals(id) && p.IsDeleted == YesOrNo.No).SingleOrDefault();
            return data;
        }

        /// <summary>
        /// 客户购买的票使用记录
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public IQueryable<TicketDetail> CustomerTicketRecords(string customerID)
        {
            var data = context.TicketDetails.Include("Annex").Where(t => t.IsDeleted == YesOrNo.No && t.CustomerID.Equals(customerID) && t.Status != TicketStatus.WaitPay && t.Status != TicketStatus.HasPay).OrderByDescending(t => t.CreatedOn);
            return data;
        }


        /// <summary>
        /// 获取门票，一张一张的
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        public IQueryable<TicketDetail> GetTicketDetailByUser(string currentUserId)
        {
            var data = context.TicketDetails.Include("Annex").Where(p => p.CustomerID.Equals(currentUserId) && p.IsDeleted == YesOrNo.No && p.IsUsed == YesOrNo.No && p.Status == TicketStatus.HasPay);
            return data;
        }

        /// <summary>
        /// 退票
        /// </summary>
        /// <param name="no">单张票</param>
        /// <returns></returns>
        public void Refund(string no, string currentUserId)
        {
            var data = context.TicketDetails.Where(p => p.TicketNO.Equals(no) && p.IsDeleted == YesOrNo.No && p.CustomerID.Equals(currentUserId)).SingleOrDefault();
            if (data != null)
            {
                data.Status = TicketStatus.Refunding;
                data.RefundingTime = DateTime.Now;
            }
        }
        /// <summary>
        /// 订单状态发生改变，插入订单历史
        /// </summary>
        /// <param name="ticketNo">票号</param>
        /// <param name="Status"></param>
        /// <param name="currentId"></param>
        public void InsertTicketDetailHist(string ticketNo, string currentId)
        {
            var data = context.TicketDetails.SingleOrDefault(p => p.TicketNO.Equals(ticketNo) && p.IsDeleted == YesOrNo.No);
            if (data != null)
            {
                TicketDetailHist tdh = new TicketDetailHist();
                tdh.CopyProperty(data);
                tdh.ID = DataHelper.GetSystemID();
                tdh.TicketDetailID = data.ID;
                tdh.CreatedOn = DateTime.Now;
                tdh.CreatedBy = currentId;
                //tdh.ID = DataHelper.GetSystemID();
                //tdh.CreatedBy = currentId;
                //tdh.CreatedOn = DateTime.Now;
                //tdh.IsDeleted = YesOrNo.No;
                //tdh.OrderDetailID = data.OrderDetailID;
                //tdh.TicketNO = data.TicketNO;
                //tdh.Name = data.Name;
                //tdh.Description = data.Description;
                //tdh.CurPrice = data.CurPrice;
                //tdh.PrePrice = data.PrePrice;
                //tdh.MainImgID = data.MainImgID;
                //tdh.Status = data.Status;
                //tdh.Address = data.Address;
                //tdh.IsUsed = data.IsUsed;
                //tdh.UsedTime = data.UsedTime;
                //tdh.BuyTime = data.BuyTime;
                //tdh.Name = data.Name;
                //tdh.Name = data.Name;
                //tdh.Name = data.Name;
                context.TicketDetailHists.Add(tdh);
            }
        }


        /// <summary>
        /// 验票
        /// </summary>
        /// <returns></returns>
        public bool ValidateTicket(string ticketNo, out  Domain.Entities.TicketDetail ticket)
        {
            //创建一个回传对象
            bool result;
            //根据参数门票号码从数据库查找门票号码相同，并且该门票属于已支付状态的记录
            var sourcesTicket = context.TicketDetails.FirstOrDefault(t => t.TicketNO.ToLower() == ticketNo.ToLower() && t.Status == TicketStatus.HasPay);
            ticket = sourcesTicket;
            //如果有这一条记录，则说明门票是合法有效的，如果没有，则门票是不合法的。
            if (sourcesTicket != null)
                result = true;
            else
                result = false;
            return result;
        }

        public bool HasTicketByTicketNo(string ticketNo, out  Domain.Entities.TicketDetail ticket)
        {
            //根据参数门票好吗从数据库查找门票号码相同的记录
            var sourcesTicket = context.TicketDetails.FirstOrDefault(t => t.TicketNO.ToLower() == ticketNo.ToLower());
            ticket = sourcesTicket;
            return sourcesTicket != null;
        }

        /// <summary>
        /// 验票通过，使用该票
        /// </summary>
        /// <param name="ticketNo"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UseTicket(string ticketNo, out string msg)
        {
            msg = string.Empty;
            var result = false;
            //根据ticketNo从数据库取得ticket记录并返回给一个实体
            var ticket = context.TicketDetails.FirstOrDefault(t => t.TicketNO.ToLower() == ticketNo.ToLower());
            if (ticket == null)
            {
                msg = "要使用的票不存在";
                result = false;
            }
            else
            {
                //保存历史
                SaveTicketHist(ticket);
                //修改该实体数据，设置ticket的状态为已使用状态
                ticket.IsUsed = YesOrNo.Yes;
                ticket.Status = TicketStatus.IsUsed;
                ticket.UsedTime = DateTime.Now;
                //将修改好的实体数据，保存回数据库
                try
                {
                    result = context.SaveChanges() > 0;
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 使用多张门票
        /// </summary>
        /// <param name="tickets"></param>
        /// <returns></returns>
        public bool UseTickets(List<TicketDetail> tickets)
        {
            //构造一个结果
            foreach (var x in tickets)
            {
                //保存历史
                SaveTicketHist(x);
                x.Status = TicketStatus.IsUsed;
                x.IsUsed = YesOrNo.Yes;
                x.UsedTime = DateTime.Now;
            }
            var result = context.SaveChanges() > 0;
            //返回结果
            return result;
        }

        /// <summary>
        /// 保存票历史
        /// </summary>
        /// <param name="ticket"></param>
        public void SaveTicketHist(TicketDetail ticket)
        {
            var hist = new TicketDetailHist();
            hist.CopyProperty(ticket);
            hist.CreatedBy = ticket.CreatedBy;
            hist.CreatedOn = ticket.CreatedOn;
            hist.ModifiedBy = ticket.ModifiedBy;
            hist.ModifiedOn = ticket.ModifiedOn;
            hist.IsDeleted = ticket.IsDeleted;
            hist.ID = DataHelper.GetSystemID();
            hist.TicketDetailID = ticket.ID;

            context.TicketDetailHists.Add(hist);
        }

        /// <summary>
        /// 测试用的特殊票
        /// </summary>
        /// <param name="includes"></param>
        /// <returns></returns>
        public IQueryable<Ticket> GetParticularTickets(params string[] includes)
        {
            DbQuery<Ticket> dbQuery = null;
            foreach (var x in includes)
	        {
                if (dbQuery == null)
                    dbQuery = context.Tickets.Include(x);
                else
                    dbQuery = dbQuery.Include(x);
	        }
            if (dbQuery == null)
                return dbQuery.Where(a => a.IsDeleted == YesOrNo.Yes && a.Name.Equals("测试数据"));

            return dbQuery.Where(a => a.IsDeleted == YesOrNo.Yes && a.Name.Equals("测试数据"));
        }

    }
}
