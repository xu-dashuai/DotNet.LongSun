using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunson.DAL;
using Pharos.Framework.MVC;
using Lunson.DAL.Repositories;
using Lunson.Domain.Entities;
using Lunson.Domain;
using Lonsun.API.EChi;
using Pharos.Framework;

namespace Lunson.BLL.Services
{
    public class OrderService
    {
        OrderRepository DAL = new OrderRepository();

        #region 数据获取
        public Order GetOrderByOrderNo(string orderNo)
        {
            var orders = DAL.GetQueryInfo<Order>();

            return orders.SingleOrDefault(a => a.OrderNo.Equals(orderNo, StringComparison.OrdinalIgnoreCase));
        }
        public Order GetOrderIncludeByOrderNo(string orderNo, params string[] includes)
        {
            var orders = DAL.GetQueryIncludeInfo<Order>(includes);

            return orders.SingleOrDefault(a => a.OrderNo.Equals(orderNo, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region 数据处理
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        public OpResult CreateOrder(Dictionary<string, int> dic, string currentUserID)
        {
            var order = DAL.CreateOrder(dic, currentUserID);
            var saveResult = DAL.Submit() > 0;

            OpResult result = new OpResult { Successed = false, Message = "数据异常，添加失败" };
            if (order != null && saveResult)
            {
                result.Successed = true;
                result.Code = order.OrderNo;
                result.Message = "订单添加成功";
            }
            return result;
        }
        /// <summary>
        /// 订单支付完成
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="tradeNo">交易平台返回的交易号</param>
        /// <param name="payType">交易类型</param>
        public void FinishOrder(string orderNo, string tradeNo, PayType payType)
        {
            var order = GetOrderIncludeByOrderNo(orderNo, "Customer");
            if (order != null && order.OrderStatus == OrderStatus.WaitPay)
            {
                //更新订单状态并生成票
                var result = DAL.FinishOrder(orderNo, tradeNo, payType);

                var code = new PhoneCode()
                {
                    Code = result.ToString(),
                    Mobile = order.Customer.Mobile,
                    SendTime = DateTime.Now
                };
                DAL.SaveInfo<PhoneCode>(code);
                DAL.SaveChanges();

                if (result > 0)
                {
                    try
                    {
                        //存订单历史
                        DAL.CreateOrderHist(order);

                        //记录交易记录
                        DAL.InsertTradeLogs(order.CustomerID, orderNo, order.Customer.DisplayName);
                        DAL.SaveChanges();
                    }
                    catch (Exception)
                    {
                        
                    }
                    finally
                    {
                        var smsMsg = GetTicketsMsgByOrder(orderNo);
                        EChiHelper.SendSMS(order.Customer.Mobile, string.Format("您已成功购买 {0}，请持门票二维码于检票处刷票快捷入园。", order.Num + "张南顺鳄鱼园门票：" + smsMsg), FormatType.MobileCheckCode);
                    }
                }
            }
        }

        private string GetTicketsMsgByOrder(string orderNo)
        {
            string msg = string.Empty;
            var ticketDetails = DAL.GetQueryIncludeInfo<TicketDetail>("OrderDetail").Where(t => t.OrderDetail.OrderNo == orderNo).OrderBy(t => t.Name).ToList();
            if (ticketDetails != null && ticketDetails.Count > 0)
            {
                var lastName = string.Empty;
                foreach (var item in ticketDetails)
                {
                    var name = string.Empty;
                    if (lastName != item.Name)
                    {
                        name = item.Name + ":";
                        lastName = item.Name;
                    }
                    msg += name + item.TicketNO + ";";
                }
            }
            return msg;
        }
        #endregion

        /// <summary>
        /// 更改订单状态
        /// </summary>
        /// <param name="orderNo"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool UpdateOrderState(string orderNo, OrderStatus state, string currentUserId, string currentName)
        {
            bool result = DAL.UpdateOrderState(orderNo, state);
            if (result)
            {
                DAL.InsertOrderHist(orderNo, currentUserId);//订单历史
                DAL.InsertTicketDetails(orderNo, currentUserId);//
                DAL.SaveChanges();
                DAL.InsertTradeLogs(currentUserId, orderNo, currentName);//TODO://
                DAL.SaveChanges();
            }
            return result;
        }
        public Order GetOrderInfoById(string orderNo)
        {
            if (orderNo == null || orderNo == "")
            {
                return new Order();
            }
            else
            {
                Order order = new Order();
                order = DAL.GetOrderInfoByNo(orderNo);
                DAL.SaveChanges();
                return order;
            }

        }

    }
}
