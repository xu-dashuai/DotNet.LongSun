using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class UserLoginLog:BaseEntity
    {
        /// <summary>
        /// 登录用户ID
        /// </summary>
        [DataMember]
        public string UserID { get; set; }
        /// <summary>
        /// 登录IP
        /// </summary>
        [DataMember]
        public string IP { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        [DataMember]
        public DateTime LoginTime { get; set; }
    }
}
