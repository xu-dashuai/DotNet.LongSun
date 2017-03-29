using Lunson.DAL;
using Lunson.DAL.Repositories;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pharos.Framework;
using Lunson.Domain;
using Pharos.Framework.MVC;

namespace Lunson.BLL.Services
{
    /// <summary>
    /// 票券服务类
    /// </summary>
    public class TicketService
    {
        private TicketRepository DAL = new TicketRepository();

        #region 取模型数据
        /// <summary>
        /// 由ID 取出票模型数据 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        public Ticket GetTicket(string id, params string[] includes)
        {
            return DAL.GetInfoIncludeByID<Ticket>(id, includes);
        }
        /// <summary>
        /// 加载所有上架票模型列表含附件 
        /// </summary>
        /// <returns></returns>
        public IQueryable<Ticket> GetAvailableIncludeTickets()
        {
            var tickets = DAL.GetQueryIncludeInfo<Ticket>("Annex");

            return tickets.Where(a=>a.Status==OnOffState.On).OrderByDescending(a=>a.CurPrice);
        }
        /// <summary>
        /// 由ID 取出一张票实体数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public TicketDetail GetTicketDetail(string id)
        {
            return DAL.GetInfoByID<TicketDetail>(id);
        }
        #endregion

        #region 取object数据
        /// <summary>
        /// 传入票号(TicketNo)集合与用户ID 取回可用票的票号集合
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        public List<string> GetAvailableTicketNos(List<string> ticketNos, string currentUserID)
        {
            var tickets = DAL.GetQueryInfo<TicketDetail>();

            var availTicketNos = (from x in tickets
                                  where x.CustomerID.Equals(currentUserID)
                                  && ticketNos.Contains(x.TicketNO)
                                  && x.IsUsed == YesOrNo.No
                                  select x.TicketNO);

            return availTicketNos.ToList();
        }
        /// <summary>
        /// 取所有票模型数据 用于数据显示(easyui)
        /// </summary>
        /// <returns></returns>
        public object GetObjectTickets()
        {
            var tickets = DAL.GetQueryIncludeInfo<Ticket>("Annex");

            var result = tickets.OrderBy(a => a.Status).ThenByDescending(a => a.CurPrice)
                                            .Select(a => new
                                            {
                                                a.ID,
                                                a.CurPrice,
                                                a.Name,
                                                a.PrePrice,
                                                a.Status,
                                                Url = a.Annex == null ? "" : a.Annex.Url
                                            });
            return result;
        }
        /// <summary>
        /// 临时购物车
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idAndNums"></param>
        /// <returns></returns>
        public List<T> GetTicketCart<T>(Dictionary<string, int> idAndNums) where T : new()
        {
            var ids = idAndNums.Select(a => a.Key);
            var tickets = DAL.GetQueryIncludeInfo<Ticket>("Annex").Where(a=>ids.Contains(a.ID)).OrderByDescending(a=>a.CurPrice).ToList();
            var result = tickets.Select(a => new
            {
                a.ID,
                ImgUrl=a.Annex.Url,
                a.Name,
                a.CurPrice,
                Num=idAndNums[a.ID],
                AllPrice = a.CurPrice * idAndNums[a.ID],
                a.Description,
                a.Resume,
                a.Address
            });

            return result.ToList<T>();
        }
        /// <summary>
        /// 测试用数据购物车
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetParticularTicketCart<T>() where T : new()
        {
            var tickets = DAL.GetParticularTickets("Annex");
            var result = tickets.Select(a => new
            {
                a.ID,
                ImgUrl = a.Annex.Url,
                a.Name,
                a.CurPrice,
                Num = 1,
                AllPrice = a.CurPrice * 1,
                a.Description,
                a.Resume,
                a.Address
            });

            return result.ToList<T>();
        }
        #endregion

        #region 数据保存删除
        /// <summary>
        /// 保存票模型
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        public object SaveTicket(Ticket ticket, string currentUserID)
        {
            if (ticket.MainImgID.IsNullOrTrimEmpty())
                return new { validate = false, msg = "请上传一张图片", target = "" };
            var annex = new AnnexService().GetAnnex(ticket.MainImgID);
            if (annex == null)
                return new { validate = false, msg = "缩略图文件不存在", target = "" };

            //存历史
            DAL.SaveTicketHist(ticket);
            //存编辑
            DAL.SaveInfo(ticket,currentUserID);
            //存数据库
            var result = DAL.Submit() > 0;

            return new { validate = result, target = "", msg = "保存失败" };
        }
        /// <summary>
        /// 删除票模型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUserID"></param>
        /// <returns></returns>
        public bool RemoveTicket(string id, string currentUserID)
        {
            DAL.RemoveInfo<Ticket>(id,currentUserID);

            return DAL.Submit() > 0;
        }
        #endregion
        
        #region 退票
        /// <summary>
        /// 取出所有申请退票数据
        /// </summary>
        /// <returns></returns>
        public IQueryable<TicketDetail> GetTicketDetailRefunding()
        {
            var tickets = DAL.GetQueryIncludeInfo<TicketDetail>("Customer", "OrderDetail");

            var refunding = tickets.Where(a => a.Status == TicketStatus.Refunding && a.IsUsed == YesOrNo.No).OrderBy(a=>a.RefundingTime);
            return refunding;
        }
        /// <summary>
        /// 退票
        /// </summary>只对票实体操作，未操作OrderDetail
        /// <param name="id">(ID或票号)</param>
        /// <returns></returns>
        public bool RefundTicket(TicketRefund refund ,string currentUserID)
        {
            string id = refund.TicketDetailID;

            var tickets = DAL.GetQueryInfo<TicketDetail>();
            var ticket = tickets.SingleOrDefault(a => a.ID.Equals(id));
            if (ticket != null)
            {
                DAL.RefundTicket(ticket, currentUserID);
                DAL.SaveInfo<TicketRefund>(refund,currentUserID);
                return DAL.Submit() > 0;
            }
            return false;
        }
        #endregion


        /// <summary>
        /// 门票详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Ticket TicketParticular(string id)
        {
            return DAL.TicketParticular(id);
        }

        /// <summary>
        /// 客户购买的票使用记录
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public IQueryable<TicketDetail> CustomerTicketRecords(string customerID)
        {
            var result = DAL.CustomerTicketRecords(customerID);
            return result;
        }

        public IQueryable<TicketDetail> GetTicketDetailByUser(string userId)
        {
            return DAL.GetTicketDetailByUser(userId);
        }
        /// <summary>
        /// 用户申请退票
        /// </summary>
        /// <param name="nos"></param>
        /// <returns></returns>
        public OpResult Refund(string nos, string currenUserId)
        {
            OpResult result = new OpResult();
            result.Successed = false;
            try
            {
                if (nos.Contains(","))
                {
                    string[] refunsNos = nos.Split(',');
                    for (int i = 0; i < refunsNos.Length; i++)
                    {
                        DAL.InsertTicketDetailHist(refunsNos[i], currenUserId);
                        DAL.Refund(refunsNos[i], currenUserId);
                        DAL.Submit();
                    }
                }
                else
                {
                    DAL.InsertTicketDetailHist(nos, currenUserId);
                    DAL.Refund(nos, currenUserId);
                    DAL.Submit();
                }


                result.Successed = true;
            }
            catch (Exception e)
            {
                result.Message = "服务器抽搐中！请稍后重试！";
            }
            return result;
        }

        /// <summary>
        /// 多张门票的验票
        /// </summary>
        /// <returns></returns>
        public List<Domain.Entities.TicketDetail> ValidateTickets(List<string> ticketNos, out List<Domain.Entities.TicketDetail> unValidatedTickets)
        {
            //构造一个结果
            var result = new List<Domain.Entities.TicketDetail>();
            unValidatedTickets = new List<TicketDetail>();
            //调用多个门票
            foreach (var item in ticketNos)
            {
                //创建一个临时门票，用来保存并验证返回的门票数据
                var tempTicket = new TicketDetail();
                var tempResult = DAL.HasTicketByTicketNo(item, out tempTicket);
                if (tempResult)
                {
                    if (tempTicket.Status == TicketStatus.HasPay)
                    {
                        //如果门票数据是有的，那么我们将这个门票作为结果放到结果集合里面
                        result.Add(tempTicket);
                    }
                    else
                    {
                        //门票数据不可用的，那么我们将这个门票放在不可用结果集合里返回回去
                        unValidatedTickets.Add(tempTicket);
                    }
                }
                else
                {
                    //门票数据不可用的，那么我们将这个门票放在不可用结果集合里返回回去
                    tempTicket = new TicketDetail();
                    tempTicket.TicketNO = item;
                    tempTicket.Status = TicketStatus.Invalid;
                    tempTicket.Name = "未知类型的票";
                    unValidatedTickets.Add(tempTicket);
                }
            }
            //返回结果
            return result;
        }

        /// <summary>
        /// 单张门票的验票
        /// </summary>
        /// <param name="ticketNo"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public bool ValidateTicket(string ticketNo, out  Domain.Entities.TicketDetail ticket)
        {
            var result = DAL.ValidateTicket(ticketNo, out ticket);
            return result;
        }

        /// <summary>
        /// 使用单张门票
        /// </summary>
        /// <param name="ticketNo"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UseTicket(string ticketNo, out string msg)
        {
            var result = DAL.UseTicket(ticketNo, out msg);
            return result;
        }

        /// <summary>
        /// 使用多张门票
        /// </summary>
        /// <param name="ticketNos"></param>
        /// <param name="unUsedTickets"></param>
        /// <returns></returns>
        public bool UseTickets(List<Domain.Entities.TicketDetail> tickets)
        {
            //构造一个结果
            //var result = new List<Domain.Entities.TicketDetail>();
            //unUsedTickets = new List<TicketDetail>();

            //var tempresult = ValidateTickets(ticketNos, out unUsedTickets);
            return DAL.UseTickets(tickets);


        }

        /// <summary>
        /// 未使用的票数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> UnusedTicketsData<T>() where T : new()
        {
            var query = from detail in DAL.GetQueryIncludeInfo<TicketDetail>("OrderDetail")
                        join customer in DAL.GetQueryInfo<Customer>() on detail.CustomerID equals customer.ID
                        where detail.Status == TicketStatus.HasPay && detail.IsUsed == YesOrNo.No
                        orderby detail.BuyTime descending
                        select new
                        {
                            detail.ID,
                            detail.TicketNO,
                            detail.Name,
                            detail.BuyTime,
                            detail.UsedTime,
                            detail.CurPrice,
                            customer.Mobile,
                            detail.OrderDetail.OrderNo
                        };
            return query.ToList<T>();
        }

    }
}
