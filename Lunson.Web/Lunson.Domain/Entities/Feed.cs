using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class Feed : BaseEntity
    { 
        /// <summary>
        /// 文章类型ID
        /// </summary>
        [DataMember]
        public string FeedTypeID { get; set; } 
        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int ShowOrder { get; set; } 
        /// <summary>
        /// 状态(审核状态)
        /// </summary>
        [DataMember]
        public ActiveState ActiveState { get; set; } 
        /// <summary>
        /// 标题
        /// </summary>
        [DataMember]
        public string Title { get; set; } 
        /// <summary>
        /// 原名
        /// </summary>
        [DataMember]
        public string Author { get; set; } 
        /// <summary>
        /// 内容
        /// </summary>
        [DataMember]
        public string Content { get; set; } 
        /// <summary>
        /// 点击量
        /// </summary>
        [DataMember]
        public int ClickNum { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        [DataMember]
        public string Original { get; set; }
        /// <summary>
        /// 发布日期（可定时发布）
        /// </summary>
        [DataMember]
        public DateTime? PublishOn { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        [DataMember]
        public int IsRecommend { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        [DataMember]
        public int IsTop { get; set; }
        /// <summary>
        /// 索引代码
        /// </summary>
        [DataMember]
        public int? Code { get; set; }

        public virtual FeedType FeedType { get; set; }
    }
}
