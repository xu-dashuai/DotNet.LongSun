using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class Order:BaseEntity
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        public string OrderNo { get; set; }
        /// <summary>
        /// 交易单号
        /// </summary>
        [DataMember]
        public string TradeNo { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public OrderStatus OrderStatus { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        [DataMember]
        public double AllPrice { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        [DataMember]
        public int Num { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        [DataMember]
        public string CustomerID { get; set; }
        /// <summary>
        /// 购买时间
        /// </summary>
        [DataMember]
        public DateTime BuyTime { get; set; }
        /// <summary>
        /// 天
        /// </summary>
        [DataMember]
        public string Day { get; set; }
        /// <summary>
        /// 支付类型
        /// </summary>
        [DataMember]
        public PayType? PayType { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
