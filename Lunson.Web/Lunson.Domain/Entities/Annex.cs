using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    /// <summary>
    /// 附件
    /// </summary>
    public class Annex:BaseEntity
    {
        /// <summary>
        /// 原名
        /// </summary>
        [DataMember]
        public string OldName { get; set; }
        /// <summary>
        /// 当前地址
        /// </summary>
        [DataMember]
        public string Url { get; set; }
        /// <summary>
        /// 大小  字节
        /// </summary>
        [DataMember]
        public int Size { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        [DataMember]
        public AnnexType Type { get; set; }
    }
}
