using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    public class Sys_User : BaseEntity
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [DataMember]
        public string Email { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [DataMember]
        public Sex? Sex { get; set; }
        /// <summary>
        /// 用户状态
        /// </summary>
        [DataMember]
        public ActiveState Status { get; set; }
        /// <summary>
        /// 是否为超级管理员
        /// </summary>
        [DataMember]
        public int? IsAdmin { get; set; }
        /// <summary>
        /// 最后登录IP
        /// </summary>
        [DataMember]
        public string IP { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        [DataMember]
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// 登录次数
        /// </summary>
        [DataMember]
        public int? LoginTimes { get; set; }


        public virtual ICollection<Sys_UserRole> UserRoles { get; set; }
    }
}
