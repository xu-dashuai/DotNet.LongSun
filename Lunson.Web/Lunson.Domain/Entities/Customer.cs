using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lunson.Domain.Entities
{
    /// <summary>
    /// 前台注册用户
    /// </summary>
    public class Customer : BaseEntity
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// 显示昵称
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// 手机是否已验证
        /// </summary>
        [DataMember]
        public YesOrNo IsMobileCheck { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [DataMember]
        public string Email { get; set; }
        /// <summary>
        /// 邮箱是否已验证
        /// </summary>
        [DataMember]
        public YesOrNo IsEmailCheck { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [DataMember]
        public Sex Sex { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        [DataMember]
        public ActiveState Status { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [DataMember]
        public YesOrNo IsDisabled { get; set; }

        /// <summary>
        /// 固定电话
        /// </summary>
        [DataMember]
        public string Tel { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        [DataMember]
        public string IP { get; set; }
        
        /// <summary>
        /// 上次登录时间
        /// </summary>
        [DataMember]
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        [DataMember]
        public int? LoginTimes { get; set; }


    }
}
