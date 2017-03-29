using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class Sys_UserRole : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public string UserID { get; set; }
        /// <summary>
        /// 角色ID
        /// </summary>
        [DataMember]
        public string RoleID { get; set; }

        public virtual Sys_User User { get; set; }
        public virtual Sys_Role Role { get; set; }
    }
}
