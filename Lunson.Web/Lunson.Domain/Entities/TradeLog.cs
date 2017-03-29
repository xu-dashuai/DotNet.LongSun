using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    /// <summary>
    /// 交易历史表
    /// </summary>
    public class TradeLog : BaseEntity
    {
        /// <summary>
        /// 描述文字
        /// </summary>
        [DataMember]
        public string Message { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        [DataMember]
        public string CustomerID { get; set; }
        /// <summary>
        /// 交易历史票详情
        /// </summary>
        [DataMember]
        public string Content { get; set; }
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

        public virtual Customer Customer { get; set; }
    }
}
