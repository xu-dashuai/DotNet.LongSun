using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lunson.Web.Models
{
    public class RegisterVM
    {
        //[Required(ErrorMessage = "必填")]
        //[RegularExpression(@"^[a-zA-Z_]{1}[0-9a-zA-Z_]{1,19}$", ErrorMessage = "以字母或下划线开头，只能含有字母数字下划线，长度2-20位")]
        //[Display(Name = "用户名")]
        //public string UserName { get; set; }

        [Required(ErrorMessage = "必填")]
        [RegularExpression(@"^1[3|5|7|8|][0-9]{9}$", ErrorMessage = "手机格式不正确")]
        [Display(Name = "手机号码")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "必填")]
        [Display(Name = "手机验证码")]
        public string MobileCode { get; set; }

        [Required(ErrorMessage = "必填")]
        [RegularExpression(@"^[^ ]{6,20}$", ErrorMessage = "不能含有空格，长度6-20位")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required(ErrorMessage = "必填")]
        [Compare("Password", ErrorMessage = "两次输入的密码不一致")]
        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        public string ConfirmPassword { get; set; }

        //[Required(ErrorMessage = "必填")]
        //[StringLength(20, MinimumLength = 1, ErrorMessage = "{1}到{0}个字符")]
        //[Display(Name = "显示")]
        //public string DisplayName { get; set; }
    }
}