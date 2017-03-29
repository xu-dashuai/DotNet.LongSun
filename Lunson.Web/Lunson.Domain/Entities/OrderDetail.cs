using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class OrderDetail:BaseEntity
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [DataMember]
        public string OrderID { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 票ID
        /// </summary>
        [DataMember]
        public string TicketID { get; set; }
        /// <summary>
        /// 票名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 当前价格 
        /// </summary>
        [DataMember]
        public double CurPrice { get; set; }
        /// <summary>
        /// 原价
        /// </summary>
        [DataMember]
        public double? PrePrice { get; set; }
        /// <summary>
        /// 缩略图ID
        /// </summary>
        [DataMember]
        public string MainImgID { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int Num{ get; set; }
        /// <summary>
        /// 购买时间
        /// </summary>
        [DataMember]
        public DateTime BuyTime { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        [DataMember]
        public double AllPrice { get; set; }
        /// <summary>
        /// 天
        /// </summary>
        [DataMember]
        public string Day { get; set; }
        /// <summary>
        /// 简述
        /// </summary>
        [DataMember]
        public string Resume { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 过期天数
        /// </summary>
        [DataMember]
        public int? OverDueDay { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public OrderStatus OrderStatus { get; set; }
        public virtual Order Order { get; set; }
        public virtual Ticket Ticket { get; set; }
        //public virtual ICollection<TicketDetail> TicketDetails { get; set; }
    }
}
