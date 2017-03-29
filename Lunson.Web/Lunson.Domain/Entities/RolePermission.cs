using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class RolePermission:BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [DataMember]
        public string RoleID { get; set; }
        /// <summary>
        /// 权限ID
        /// </summary>
        [DataMember]
        public string PermissionID { get; set; }

        public virtual Sys_Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
