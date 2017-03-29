using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class TicketRefund : BaseEntity
    {
        /// <summary>
        /// 票ID
        /// </summary>
        [DataMember]
        public string TicketDetailID { get; set; }
        /// <summary>
        /// 退款额
        /// </summary>
        [DataMember]
        public double RefundPrice { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        public virtual TicketDetail TicketDetail { get; set; }
    }
}
