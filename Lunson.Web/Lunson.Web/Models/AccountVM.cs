using Lunson.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lunson.Web.Models
{
    /// <summary>
    /// 会员账号信息视图
    /// </summary>
    public class AccountVM
    {
        [Required(ErrorMessage="必填")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "{1}到{0}个字符")]
        [Display(Name = "显示名")]
        public string DisplayName { get; set; }

        [Display(Name = "手机")]
        public string Mobile { get; set; }

        [Display(Name = "邮箱")]
        public string Email { get; set; }

        [Display(Name = "性别")]
        public Sex Sex { get; set; }

        [Display(Name = "手机验证码")]
        public string MobileCode { get; set; }

        [Display(Name = "邮箱验证码")]
        public string EmailCode { get; set; }

        [Display(Name = "手机验证")]
        public YesOrNo IsMobileCheck { get; set; }
    }
}