using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class Permission:BaseEntity
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        [DataMember]
        public string MenuID { get; set; }
        /// <summary>
        /// 权限名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 权限代码
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 权限正则
        /// </summary>
        [DataMember]
        public string Regex { get; set; }

        public virtual Sys_Menu Menu { get; set; }
    }
}
