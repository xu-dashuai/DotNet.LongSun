using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LonsunTicket
{
    public class TicketDetail
    {
        public virtual string ID { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreatedOn { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual DateTime? ModifiedOn { get; set; }
        public virtual YesOrNo IsDeleted { get; set; }
        public string TicketNO { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double CurPrice { get; set; }
        public double? PrePrice { get; set; }
        public string MainImgID { get; set; }
        public TicketStatus Status { get; set; }
        public string Address { get; set; }
        public YesOrNo IsUsed { get; set; }
        public DateTime? UsedTime { get; set; }
        public DateTime? BuyTime { get; set; }
        public DateTime? RefundingTime { get; set; }
        public DateTime? RefundedTime { get; set; }
        public string CustomerID { get; set; }
        public int? OverDueDay { get; set; }
        public string OrderDetailID { get; set; }
    }

    public enum YesOrNo
    {
        /// <summary>
        /// 否 0
        /// </summary>
        No = 0,
        /// <summary>
        /// 是 1
        /// </summary>
        Yes = 1
    }

    /// <summary>
    /// 票状态
    /// </summary>
    public enum TicketStatus
    {
        /// <summary>
        /// 等待付款
        /// </summary>
        [Description("等待付款")]
        WaitPay = 0,
        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        HasPay = 1,
        /// <summary>
        /// 申请退款
        /// </summary>
        [Description("申请退款")]
        ApplyForRefund = 2,
        /// <summary>
        /// 退款中
        /// </summary>
        [Description("退款中")]
        Refunding = 3,
        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        Refunded = 4,
        /// <summary>
        /// 已使用
        /// </summary>
        [Description("已使用")]
        IsUsed = 5,
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Invalid = 6
    }
    public class TicketsResult
    {
        public bool Result;
        public List<TicketDetail> AllowTickets;
        public List<TicketDetail> InvalidateTickets;
        public string Message;
    }
}
