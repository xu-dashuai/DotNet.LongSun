using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lunson.Web.Models
{
    public class UserData
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string OfficeId { get; set; }
        public bool IsAdmin { get; set; }

        public List<RoleData> Roles { get; set; }
    }

    public class RoleData
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}