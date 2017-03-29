using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    /// <summary>
    /// 文章类型
    /// </summary>
    public class FeedType:BaseEntity
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        
        /// <summary>
        /// 类型编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// sef类型附件ID
        /// </summary>
        [DataMember]
        public string SwfID { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public ActiveState ActiveState { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public virtual Annex Annex { get; set; }
    }
}
