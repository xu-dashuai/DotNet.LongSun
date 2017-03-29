using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    /// <summary>
    /// 前台注册用户登录历史
    /// </summary>
    public class CustomerLoginLog : BaseEntity
    {
        /// <summary>
        /// 注册用户ID
        /// </summary>
        [DataMember]
        public string CustomerID { get; set; }

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
