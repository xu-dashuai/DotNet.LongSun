using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class TicketDetail: BaseEntity
    {
        /// <summary>
        /// 票号
        /// </summary>
        [DataMember]
        public string TicketNO { get; set; }

        /// <summary>
        /// 票类名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 简述
        /// </summary>
        [DataMember]
        public string Resume { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 当前价
        /// </summary>
        [DataMember]
        public double CurPrice { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        [DataMember]
        public double? PrePrice { get; set; }
        
        /// <summary>
        /// 票图ID
        /// </summary>
        [DataMember]
        public string MainImgID { get; set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public TicketStatus Status { get; set; }
        
        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        
        /// <summary>
        /// 是否已使用
        /// </summary>
        [DataMember]
        public YesOrNo IsUsed { get; set; }
        
        /// <summary>
        /// 使用时间
        /// </summary>
        [DataMember]
        public DateTime? UsedTime { get; set; }

        /// <summary>
        /// 购买时间
        /// </summary>
        [DataMember]
        public DateTime? BuyTime { get; set; }
        /// <summary>
        /// 申请退票时间
        /// </summary>
        [DataMember]
        public DateTime? RefundingTime { get; set; }

        /// <summary>
        /// 退票确认时间
        /// </summary>
        [DataMember]
        public DateTime? RefundedTime { get; set; }
        
        /// <summary>
        /// 客户ID
        /// </summary>
        [DataMember]
        public string CustomerID { get; set; }

        /// <summary>
        /// 过期天数
        /// </summary>
        [DataMember]
        public int? OverDueDay { get; set; }

        /// <summary>
        /// 订单明细
        /// </summary>
        [DataMember]
        public string OrderDetailID { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
        public virtual Annex Annex { get; set; }

    }
}
