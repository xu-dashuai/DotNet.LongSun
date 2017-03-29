using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{

    /// <summary>
    /// 菜单
    /// </summary>
    public class Sys_Menu : BaseEntity
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 菜单地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 上层菜单ID
        /// </summary>
        [DataMember]
        public string ParentID { get; set; }
        /// <summary>
        /// 菜单类型
        /// </summary>
        [DataMember]
        public MenuType Type { get; set; }
        /// <summary>
        /// 简短地址
        /// </summary>
        [DataMember]
        public string BriefUrl { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int ShowOrder { get; set; }

        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
