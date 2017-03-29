using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunson.Domain.Entities;
using Lunson.DAL.Repositories;
using Pharos.Framework;
using Lunson.Domain;
using Pharos.Framework.MVC;

namespace Lunson.BLL.Services
{
    public class ReportService
    {
        private ReportRepository DAL = new ReportRepository();

        /// <summary>
        /// 获取购票详细报表数据
        /// </summary>主数量来源：OrderDetail,备注暂为Ticket.Resume简述
        /// <typeparam name="T"></typeparam>
        /// <param name="fromTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<T> DetailReportData<T>(DateTime? fromTime, DateTime? endTime) where T : new()
        {
            var query = from detail in DAL.GetQueryInfo<OrderDetail>()
                        join order in DAL.GetQueryInfo<Order>() on detail.OrderID equals order.ID
                        join customer in DAL.GetQueryInfo<Customer>() on order.CustomerID equals customer.ID
                        where detail.OrderStatus == OrderStatus.HasPay && detail.BuyTime >= fromTime && detail.BuyTime <= endTime
                        orderby detail.BuyTime descending
                        select new
                        {
                            detail.OrderNo,
                            order.TradeNo,
                            //detail.Ticket.Name,
                            detail.Name,
                            customer.Mobile,
                            detail.BuyTime,
                            order.PayType,
                            detail.Num,
                            detail.CurPrice,
                            detail.AllPrice,
                            detail.Ticket.Resume
                        };
            return query.ToList<T>();
        }

        /// <summary>
        /// 获取已过检的票明细
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="usedFromTime"></param>
        /// <param name="usedEndTime"></param>
        /// <returns></returns>
        public List<T> DailyReportData<T>(DateTime? usedFromTime, DateTime? usedEndTime) where T : new()
        {
            var query = from detail in DAL.GetQueryIncludeInfo<TicketDetail>("OrderDetail")
                        join customer in DAL.GetQueryInfo<Customer>() on detail.CustomerID equals customer.ID
                        where detail.IsUsed == YesOrNo.Yes && detail.UsedTime != null && detail.UsedTime >= usedFromTime && detail.UsedTime <= usedEndTime
                        orderby detail.UsedTime
                        select new
                        {
                            detail.TicketNO,
                            detail.OrderDetail.OrderNo,
                            customer.Mobile,
                            detail.Name,
                            detail.BuyTime,
                            detail.UsedTime,
                            detail.CurPrice
                        };
            return query.OrderByDescending(a => a.UsedTime).ToList<T>();
        }

        /// <summary>
        /// 未使用的票 数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> UnusedTicketsData<T>() where T : new()
        {
            var query = from detail in DAL.GetQueryInfo<TicketDetail>()
                        join customer in DAL.GetQueryInfo<Customer>() on detail.CustomerID equals customer.ID
                        where detail.UsedTime == null
                        orderby detail.BuyTime descending
                        select new
                        {
                            detail.TicketNO,
                            detail.Name,
                            detail.BuyTime,
                            detail.UsedTime,
                            detail.CurPrice,
                            customer.Mobile
                        };
            return query.ToList<T>();
        }

        /// <summary>
        /// 获取票种类下拉框数据
        /// </summary>
        /// <param name="isAll">是否包含“全部”项</param>
        /// <returns></returns>
        public List<DropdownItem> GetAllTicket(bool? isAll = false)
        {
            List<DropdownItem> ddl = new List<DropdownItem>();
            if (isAll == true)
            {
                ddl.Add(new DropdownItem { Text = "全部", Value = "" });
            }
            var data = DAL.GetAllTicket();
            foreach (var item in data)
            {
                ddl.Add(new DropdownItem { Text = item.Name, Value = item.ID });
            }
            return ddl;
        }

        /// <summary>
        /// 购票统计数据报表
        /// </summary>数据来源主表：TicketDatail 票实体表
        /// <typeparam name="T"></typeparam>
        /// <param name="fromTime"></param>
        /// <param name="endTime"></param>
        /// <param name="ids">票种ID集合</param>
        /// <returns></returns>
        public List<T> StatisticsData<T>(DateTime? fromTime, DateTime? endTime, List<string> ids) where T : new()
        {
            var details = DAL.GetQueryInfo<TicketDetail>();
            var tickets = DAL.GetQueryInfo<Ticket>();
            var orders = DAL.GetQueryInfo<OrderDetail>();

            var result = from x in details
                         join y in orders on x.OrderDetailID equals y.ID
                         join z in tickets on y.TicketID equals z.ID
                         where x.UsedTime != null
                         select new
                         {
                             ticketID = z.ID,
                             time = x.UsedTime,
                             Name = x.Name,
                             SalePrice = x.CurPrice,
                             x.Resume
                         };

            if (ids.Any())
            {
                result = result.Where(a => ids.Contains(a.ticketID));
            }

            if (fromTime != null)
                result = result.Where(a => a.time >= fromTime);

            if (endTime != null)
                result = result.Where(a => a.time <= endTime);

            var temp = from x in result
                       group x by new { x.Name, x.SalePrice, x.Resume } into p
                       select new
                       {
                           Name = p.Key.Name,
                           Num = p.Count(),
                           SalePrice = p.Key.SalePrice,
                           Income = p.Key.SalePrice * p.Count(),
                           Resume = p.Key.Resume
                       };

            return temp.ToList<T>();
        }

        /// <summary>
        /// 退票详情数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<T> RefundedTicketsData<T>(DateTime? fromTime, DateTime? endTime) where T : new()
        {
            var query = from t in DAL.GetQueryIncludeInfo<TicketRefund>("TicketDetail")
                        join users in DAL.GetQueryInfo<Sys_User>() on t.CreatedBy equals users.ID
                        where t.CreatedOn >= fromTime && t.CreatedOn <= endTime
                        orderby t.CreatedOn descending
                        select new
                        {
                            t.TicketDetail.TicketNO,
                            t.TicketDetail.OrderDetail.OrderNo,
                            t.TicketDetail.Customer.Mobile,
                            t.TicketDetail.Name,
                            t.TicketDetail.CurPrice,
                            t.TicketDetail.BuyTime,
                            t.RefundPrice,
                            RefundedTime = t.CreatedOn,
                            Operator = users.DisplayName,
                            t.Description
                        };

            return query.ToList<T>();
        }

    }
}
