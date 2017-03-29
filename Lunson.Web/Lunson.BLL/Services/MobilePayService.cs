using Lunson.DAL.Repositories;
using Lunson.Domain;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.BLL.Services
{
    public class MobilePayService
    {
        private TicketRepository DAL = new TicketRepository();

        #region 购买
        /// <summary>
        /// 临时购物车
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idAndNums"></param>
        /// <returns></returns>
        //public List<T> GetTicketCart<T>(Dictionary<string, int> idAndNums) where T : new()
        //{
        //    var ids = idAndNums.Select(a => a.Key);
        //    var tickets = DAL.GetQueryInfo<Ticket>().Where(a => ids.Contains(a.ID)).OrderByDescending(a => a.CurPrice).ToList();
        //    var result = tickets.Select(a => new
        //    {
        //        a.ID,
        //        a.Name,
        //        a.CurPrice,
        //        Num = idAndNums[a.ID],
        //        AllPrice = a.CurPrice * idAndNums[a.ID],
        //        a.Description,
        //        a.Resume,
        //        a.Address
        //    });

        //    return result.ToList<T>();
        //}

        #endregion

        #region 取票

        /// <summary>
        /// 由订单号获取对应门票
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public IQueryable<TicketDetail> GetTicketNosByOrderNo(string orderNo)
        {
            var tickets = DAL.GetQueryInfo<TicketDetail>().Where(t => t.OrderDetail.OrderNo.Equals(orderNo)).OrderByDescending(t => t.CurPrice);
            return tickets;
        }

        /// <summary>
        /// 由手机号获取对应门票
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public IQueryable<TicketDetail> GetTicketNosByMobile(string mobile, bool includeUsed = false)
        {
            var customer = new CustomerService().GetCustomerByMobile(mobile);

            var tickets = DAL.GetQueryInfo<TicketDetail>().Where(t => t.CustomerID.Equals(customer.ID));

            if (includeUsed == false)
            {
                tickets = tickets.Where(t => t.IsUsed == YesOrNo.No);
            }

            return tickets.OrderBy(t => t.IsUsed).ThenByDescending(t => t.CreatedOn);
        }

        #endregion
    }
}
