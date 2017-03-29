using Lunson.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lunson.Web.Models
{
    public class UserVM
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }

        public UserVM()
        {
            this.ID = "";
            this.Password = "";
            this.UserName = "";
            this.DisplayName = "";
            this.Role = "";
            this.ConfirmPassword = "";
        }
    }
}