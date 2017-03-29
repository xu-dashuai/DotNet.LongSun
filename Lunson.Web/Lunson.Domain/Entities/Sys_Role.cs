using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class Sys_Role : BaseEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 角色代码
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        /// 角色描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 活动状态
        /// </summary>
        [DataMember]
        public ActiveState Status { get; set; }

        public virtual ICollection<Sys_UserRole> UserRoles { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } 
    }
}
