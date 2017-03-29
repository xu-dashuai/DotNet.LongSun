using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lunson.Web.Models
{
    public class CustomerLoginVM
    {
        [Required(ErrorMessage = "手机号不能为空")]
        [Display(Name = "手机号")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "记住密码?")]
        public bool IsRememberMe { get; set; }
    }
}