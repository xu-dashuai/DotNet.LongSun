using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunson.Domain.Entities;
using Pharos.Framework.MVC;
using Lunson.Domain;
using Pharos.Framework;
using System.Data.SqlClient;

namespace Lunson.DAL.Repositories
{
    public class OrderRepository : BaseRepository
    {
        #region 数据处理
        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        public Order CreateOrder(Dictionary<string, int> dic, string currentUserID)
        {
            var curTime = DateTime.Now;
            var tickets = GetQueryInfo<Ticket>(true).Where(a => a.Status == OnOffState.On && dic.Keys.Contains(a.ID)).ToList();
            if (tickets.Any())
            {
                //订单
                var order = new Order();
                order.OrderNo = CreateOrderNo();
                order.OrderStatus = OrderStatus.WaitPay;
                order.AllPrice = tickets.Sum(a => a.CurPrice * dic[a.ID]);
                order.Num = tickets.Sum(a => dic[a.ID]);
                order.CustomerID = currentUserID;
                order.BuyTime = curTime;
                order.Day = curTime.ToString("yyyyMMdd");
                SaveInfo(order, "webcreateorder");

                //明细
                foreach (var ticket in tickets)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.CopyProperty(ticket);
                    orderDetail.OrderID = order.ID;
                    orderDetail.OrderNo = order.OrderNo;

                    orderDetail.TicketID = ticket.ID;
                    orderDetail.Num = dic[ticket.ID];
                    orderDetail.AllPrice = ticket.CurPrice * dic[ticket.ID];
                    orderDetail.BuyTime = curTime;
                    orderDetail.Day = curTime.ToString("yyyyMMdd");
                    orderDetail.OrderStatus = OrderStatus.WaitPay;

                    SaveInfo(orderDetail, "webcreateorder");
                }

                return order;
            }

            return null;
        }
        public List<TicketDetail> CreateTicketDetails(IQueryable<OrderDetail> orderDetails, string currentUserID)
        {
            List<TicketDetail> result = new List<TicketDetail>();

            foreach (var x in orderDetails.ToList())
            {
                var ticket = GetInfoByID<Ticket>(x.TicketID);
                for (int i = 0; i < x.Num; i++)
                {
                    var detail = new TicketDetail();
                    detail.CopyProperty(ticket);
                    detail.BuyTime = DateTime.Now;
                    detail.CustomerID = currentUserID;
                    detail.OrderDetailID = x.ID;
                    detail.IsUsed = YesOrNo.No;
                    detail.Status = TicketStatus.HasPay;
                    detail.TicketNO = GetTicketNo();//票号
                    SaveInfo<TicketDetail>(detail, currentUserID);

                    result.Add(detail);
                }
            }

            return result;
        }
        /// <summary>
        /// 保存订单历史 
        /// </summary>
        /// <param name="order"></param>
        public void CreateOrderHist(Order order)
        {
            var hist = new OrderHist();
            hist.CopyProperty(order);
            SaveInfo<OrderHist>(hist);

            hist.CreatedBy = order.CreatedBy;
            hist.CreatedOn = order.CreatedOn;
            hist.ModifiedBy = order.ModifiedBy;
            hist.ModifiedOn = order.ModifiedOn;
            hist.IsDeleted = order.IsDeleted;
            hist.OrderID = order.ID;
        }
        /// <summary>
        /// 订单支付完成
        /// </summary>
        /// <param name="order"></param>
        /// <param name="tradeNo"></param>
        /// <param name="paytype"></param>
        //public void FinishOrder(Order order, string tradeNo, PayType payType)
        //{
        //    var time = DateTime.Now;
        //    string orderNo = order.OrderNo;
        //    var details = GetQueryInfo<OrderDetail>().Where(a => a.OrderNo.Equals(orderNo));
        //    var customer = GetInfoByID<Customer>(order.CustomerID);

        //    //存订单历史
        //    CreateOrderHist(order);
        //    //生成票
        //    #region
        //    var tickets = CreateTicketDetails(details, order.CustomerID);
        //    #endregion
        //    //交易记录
        //    #region
        //    TradeLog tralog = new TradeLog();
        //    tralog.CustomerID = order.CustomerID;
        //    tralog.OrderNo = orderNo;
        //    tralog.TradeNo = tradeNo;
        //    tralog.Message = string.Format("{0} {1}通过{2}付款,购买{3}张门票,共计{4}元，订单号：{5}，交易单号：{6}",
        //        time.ToString("yyyy年MM月dd日"), customer.DisplayName, EnumHelper.GetDescription(payType), order.Num, order.AllPrice, order.OrderNo, tradeNo);
        //    tralog.Content = string.Join("", tickets.Select(a => "<p>票名：" + a.Name + "，票号：" + a.TicketNO + "，价格：" + a.CurPrice + "</p>"));
        //    SaveInfo<TradeLog>(tralog, order.CustomerID);
        //    #endregion

        //    //订单支付完成
        //    order.OrderStatus = OrderStatus.HasPay;//已支付
        //    order.TradeNo = tradeNo;
        //    order.PayType = payType;

        //    //订单详细的状态
        //    foreach (var x in details)
        //    {
        //        x.OrderStatus = OrderStatus.HasPay;
        //        x.ModifiedBy = "alipay";
        //        x.ModifiedOn = time;
        //    }

        //    SaveInfo<Order>(order, order.CustomerID);
        //}

        /// <summary>
        /// 订单支付完成并生成票
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="tradeNo">交易平台返回交易号</param>
        /// <param name="payType">交易类型</param>
        /// <returns>事务成功返回1，失败返回-1，已生成过票不再生成返回0</returns>
        public int FinishOrder(string orderNo, string tradeNo, PayType payType)
        {
            SqlParameter[] para = new SqlParameter[]{
                new SqlParameter("@orderNo", orderNo),
                new SqlParameter("@tradeNo", tradeNo),
                new SqlParameter("@payType", payType)
            };
            return context.Database.ExecuteSqlCommand("EXEC FinishOrderThenGenerateTickets @orderNo,@tradeNo,@payType", para);
        }
        #endregion

        /// <summary>
        /// 支付成功的时候将订单信息插入支付历史
        /// </summary>
        public void InsertTradeLogs(string currentUserId, string orderNo, string currentName)
        {
            var data = GetQueryIncludeInfo<Order>("OrderDetails").Where(a => a.OrderNo.Equals(orderNo)).SingleOrDefault();
            if (data != null)
            {
                TradeLog tralog = new TradeLog();
                tralog.CustomerID = currentUserId;
                tralog.Message = string.Format("{0} {1}通过支付宝付款,购买{2}张门票,共计{3}元", DateTime.Now.ToString("yyyy年MM月dd日"), currentName, data.Num, data.AllPrice);

                StringBuilder sb = new StringBuilder();
                foreach (var item in data.OrderDetails)
                {
                    var ticketInfo = GetQueryInfo<TicketDetail>().Where(a => a.OrderDetailID.Equals(item.ID));
                    foreach (var detail in ticketInfo)
                    {
                        if (detail.Description != null)//票的备注为空
                        {
                            tralog.Content += string.Format("<p>票号:{0},价格:{1}</p>", detail.TicketNO, detail.CurPrice);
                        }
                        else
                        {
                            tralog.Content += string.Format("<p>票号:{0},价格:{1},备注:{2}</p>", detail.TicketNO, detail.CurPrice, detail.Description);
                        }
                    }

                }
                SaveInfo<TradeLog>(tralog, currentUserId);
            }
        }
        /// <summary>
        /// 生成订单号
        /// </summary>
        /// <returns></returns>
        public string CreateOrderNo()
        {
            string maxid = GetMaxOrderIdInToday();
            int maxorder;
            int.TryParse(maxid, out maxorder);
        roclback:
            maxorder = maxorder + 1;
            System.Text.StringBuilder currentOrderId = new System.Text.StringBuilder();
            string id = string.Format("{0:D5}", maxorder);
            currentOrderId.Append("LS").Append(DateTime.Now.ToString("yyMMdd")).Append(id).Append(Guid.NewGuid().ToString().ToUpper().Substring(0, 3));

            string no = currentOrderId.ToString();

            var results = (context.Orders.Where(p => p.OrderNo == no && p.IsDeleted != YesOrNo.Yes));
            if (results.Count() > 0)
            {
                goto roclback;
            }

            return no;
        }

        /// <summary>
        /// 查询当日最大订单号
        /// </summary>
        /// <returns></returns>
        public string GetMaxOrderIdInToday()
        {
            string time1 = DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00";
            string time2 = DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59";
            DateTime beginDate = new DateTime();
            DateTime.TryParse(time1, out beginDate);
            DateTime endDate = new DateTime();
            DateTime.TryParse(time2, out endDate);
            var maxorderid = context.Orders.Where(p => p.IsDeleted != YesOrNo.Yes && p.CreatedOn > beginDate && p.CreatedOn < endDate).OrderByDescending(p => p.CreatedOn).FirstOrDefault();
            if (maxorderid == null)
                return "0000";
            else
                return maxorderid.OrderNo.ToString().Remove(0, 8);
        }

        /// <summary>
        /// 根据订单ID查询订单及详情的相关信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Order GetOrderInfoByNo(string orderNo, bool isInclude = true)
        {
            if (isInclude)
                return context.Orders.Include("OrderDetails").Where(p => p.OrderNo.Equals(orderNo) && p.IsDeleted == YesOrNo.No).FirstOrDefault();
            else
                return context.Orders.Where(p => p.OrderNo.Equals(orderNo) && p.IsDeleted == YesOrNo.No).FirstOrDefault();
        }
        /// <summary>
        /// 更新订单状态
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public bool UpdateOrderState(string orderNo, OrderStatus state)
        {
            //读订单号相等，未删除，未支付的单
            var data = context.Orders.Where(p => p.OrderNo.Equals(orderNo) && p.IsDeleted == YesOrNo.No && p.OrderStatus == OrderStatus.WaitPay).SingleOrDefault();
            if (data != null)
            {
                data.OrderStatus = state;
                var temp = context.OrderDetails.Where(p => p.OrderNo.Equals(orderNo) && p.IsDeleted == YesOrNo.No);
                foreach (var item in temp)
                {
                    item.OrderStatus = state;
                }
            }
            return context.SaveChanges() > 0;
        }
        /// <summary>
        /// 订单数据发生变动的时候插入数据
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="currentUserID">用户ID</param>
        public void InsertOrderHist(string orderNo, string currentUserID)
        {
            var order = GetOrderInfoByNo(orderNo, false);
            if (order != null)
            {
                var hist = new OrderHist();
                hist.CopyProperty(order);
                hist.OrderID = order.ID;
                hist.CreatedBy = currentUserID;
                hist.CreatedOn = DateTime.Now;
                hist.ID = DataHelper.GetSystemID();
                hist.IsDeleted = YesOrNo.No;
                context.OrderHists.Add(hist);
                //context.SaveChanges();
            }
        }
        //所有流程完成后调用这个保存
        public int SaveChanges()
        {
            return context.SaveChanges();
        }
        /// <summary>
        /// 根据订单详情将订单所有数据分开插入
        /// </summary>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public bool InsertTicketDetails(string orderNo, string currentUserID)
        {
            bool result = false;
            try
            {
                var data = context.OrderDetails.Where(p => p.OrderNo.Equals(orderNo) && p.IsDeleted == YesOrNo.No && p.OrderStatus == OrderStatus.HasPay).ToList();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.Num > 0)
                        {
                            for (int i = 0; i < item.Num; i++)
                            {
                                var ticketid = item.TicketID;
                                TicketDetail td = new TicketDetail();
                                var tt = context.Tickets.FirstOrDefault();
                                var ticket = context.Tickets.SingleOrDefault(p => p.ID.Equals(ticketid) && p.IsDeleted == YesOrNo.No);
                                td.CopyProperty(ticket);
                                td.ID = DataHelper.GetSystemID();
                                td.CreatedOn = DateTime.Now;
                                td.CreatedBy = currentUserID;
                                td.OrderDetailID = item.ID;
                                td.TicketNO = GetTicketNo();
                                td.Status = TicketStatus.HasPay;
                                td.IsUsed = YesOrNo.No;
                                td.BuyTime = DateTime.Now;
                                td.CustomerID = currentUserID;
                                context.TicketDetails.Add(td);
                            }
                        }
                    }
                }
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }
        /// <summary>
        /// 订单状态发生改变，插入订单历史
        /// </summary>
        /// <param name="ticketNo">票号</param>
        /// <param name="Status"></param>
        /// <param name="currentId"></param>
        public void InsertTicketDetailHist(string ticketNo, TicketStatus Status, string currentId)
        {
            var data = context.TicketDetails.SingleOrDefault(p => p.TicketNO.Equals(ticketNo) && p.IsDeleted == YesOrNo.No);
            if (data != null)
            {
                TicketDetailHist tdh = new TicketDetailHist();
                tdh.CopyProperty(data);
                tdh.ID = DataHelper.GetSystemID();
                tdh.TicketDetailID = data.ID;
                tdh.Status = Status;
                context.TicketDetailHists.Add(tdh);
                //context.SaveChanges();
            }
        }
        /// <summary>
        /// 生成票号：暂时是"ABC 150601 0000":五个随机大写字母+6位年月+四位递增序号 共15位
        /// 生成票号："ABC 150601 0000":五个随机数+6位年月+四位递增序号 共15位
        /// </summary>
        /// <returns></returns>
        public string GetTicketNo()
        {
            string tNo = string.Empty;
            //生成随机
            var randomStr = Guid.NewGuid().ToString().ToUpper().Substring(0, 3);

            //Random r = new Random();
            //string s = string.Empty;
            //string str = string.Empty;
            //for (int i = 0; i < 5; i++)
            //{
            //    s = ((char)r.Next(65, 91)).ToString();
            //    str += s;
            //}

            string maxNo;
            var tickets = context.TicketDetails.Where(t => t.IsDeleted == YesOrNo.No && t.CreatedOn >= DateTime.Today).OrderByDescending(t => t.CreatedOn).FirstOrDefault();
            if (tickets == null)
                maxNo = "0";
            else
                maxNo = tickets.TicketNO.Remove(0, 9); //"ABC 150601 0000"

            int maxNoInt;
            int.TryParse(maxNo, out maxNoInt);
            maxNoInt = maxNoInt + 1;

            tNo = randomStr + DateTime.Today.ToString("yyMMdd") + string.Format("{0:D4}", maxNoInt);

            return tNo;
        }
    }
}
