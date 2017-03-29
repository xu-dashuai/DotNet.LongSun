using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    /// <summary>
    /// 友情链接
    /// </summary>
    public class Link:BaseEntity
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 网站URL
        /// </summary>
        [DataMember]
        public string LinkUrl { get; set; }
        
        /// <summary>
        /// Logo图片ID
        /// </summary>
        [DataMember]
        public string ImgID { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int ShowOrder { get; set; }

        /// <summary>
        /// 链接Title
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// 活动状态
        /// </summary>
        [DataMember]
        public ActiveState ActiveState { get; set; }

        /// <summary>
        /// logo附件
        /// </summary>
        public virtual Annex Annex { get; set; }
    }
}
