using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    /// <summary>
    /// 票
    /// </summary>
    public class Ticket:BaseEntity
    {
        /// <summary>
        /// 票名
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 简述
        /// </summary>
        [DataMember]
        public string Resume { get; set; }
        /// <summary>
        /// 详细描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
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
        /// 地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public OnOffState Status { get; set; }
        /// <summary>
        /// 过期天数
        /// </summary>
        [DataMember]
        public int? OverDueDay { get; set; }

        //缩略图
        public virtual Annex Annex { get; set; }
    }
}
