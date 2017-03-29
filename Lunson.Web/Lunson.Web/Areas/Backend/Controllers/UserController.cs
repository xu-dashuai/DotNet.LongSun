using Lunson.BLL.Services;
using Lunson.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pharos.Framework;
using System.Text.RegularExpressions;
using Lunson.Domain.Entities;
using Lunson.Domain;

namespace Lunson.Web.Areas.Backend.Controllers
{
    public class UserController : BaseController
    {
        private UserService usv = new UserService();
        private RoleService rsv = new RoleService();

        [PermissionCheck("view", "index")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUsers()
        {
            var users = usv.GetUsers();

            return Json(users);
        }

        [PermissionCheck("edit", "index")]
        public ActionResult CreateUser()
        {
            ViewBag.roles = rsv.GetDropdownRoles();
            return View();
        }
        [PermissionCheck("password", "index")]
        public ActionResult EditUserPassword(string id)
        {
            var user = usv.GetUser(id);
            if (user == null)
            {
                ViewBag.msg = "找不到用户";
                return View("Error");
            }
            return View(user);
        }
        [HttpPost]
        [ValidateInput(false)]
        [PermissionCheck("password", "index")]
        public ActionResult EditUserPassword(string id, string Password)
        {
            if (Password.IsNullOrTrimEmpty())
                return Json(new { validate = false, target = "Password", msg = "密码不能为空" });

            var result = usv.EditUserPassword(id, Password, UserCache.CurrentUser.Id);
            return Json(result);
        }
        [PermissionCheck("edit", "index")]
        public ActionResult EditUser(string id)
        {
            var user = usv.GetUser(id);
            if (user == null)
            {
                ViewBag.msg = "找不到用户";
                return View("Error");
            }
            var roles = rsv.GetDropdownRoles();
            foreach(var x in roles)
            {
                if(user.UserRoles.Select(r=>r.RoleID).Contains(x.Value))
                    x.IsSelected=true;
                else
                    x.IsSelected=false;
            }

            ViewBag.roles = roles;
            return View(user);
        }

        [HttpPost]
        [ValidateInput(false)]
        [PermissionCheck("edit", "index")]
        public ActionResult EditUser(string id,string DisplayName,string Role)
        {
            //用户名应以字母开头，只能含有字母数字下划线，长度大于2位小于10位
            Regex reg = new Regex(@"^[a-zA-Z]{1}[0-9a-zA-Z_]{1,9}$");
            if (DisplayName.IsNullOrTrimEmpty() || reg.IsMatch(DisplayName) == false)
                return Json(new { validate = false, target = "DisplayName", msg = "以字母开头，只能含有字母数字下划线，长度2-10位" });

            var rolelist = new List<string>();
            if (Role.IsNullOrTrimEmpty() == false)
            {
                rolelist = Role.Split(',').Where(a => a.IsNullOrTrimEmpty() == false).Distinct().ToList();
            }

            var result = usv.SaveUser(id, DisplayName, rolelist, UserCache.CurrentUser.Id);

            return Json(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        [PermissionCheck("delete", "index")]
        public ActionResult RemoveUser(string id)
        {
            usv.RemoveUser(id,UserCache.CurrentUser.Id);
            return Json("");
        }

        [HttpPost]
        [ValidateInput(false)]
        [PermissionCheck("edit", "index")]
        public ActionResult CreateUser(UserVM model)
        {
            //用户名应以字母开头，只能含有字母数字下划线，长度大于2位小于10位
            Regex reg = new Regex(@"^[a-zA-Z]{1}[0-9a-zA-Z_]{1,9}$");
            if (model.UserName.IsNullOrTrimEmpty() || reg.IsMatch(model.UserName) == false)
                return Json(new { validate = false, target = "UserName", msg = "以字母开头，只能含有字母数字下划线，长度2-10位" });
            if (model.DisplayName.IsNullOrTrimEmpty() || reg.IsMatch(model.DisplayName) == false)
                return Json(new { validate = false, target = "DisplayName", msg = "以字母开头，只能含有字母数字下划线，长度2-10位" });
            if (model.Password.IsNullOrTrimEmpty())
                return Json(new { validate = false, target = "Password", msg = "密码不能为空" });
            if (model.ConfirmPassword != model.Password)
                return Json(new { validate = false, target = "ConfirmPassword", msg = "密码不一至" });

            var user = new Sys_User
            {
                ID = "",
                DisplayName = model.DisplayName,
                UserName = model.UserName,
                IsDeleted = YesOrNo.No,
                Password = usv.GetEncryPassword(model.Password.Trim()),
                Status = ActiveState.Normal
            };

            var rolelist = new List<string>();
            if (model.Role.IsNullOrTrimEmpty() == false)
            {
                rolelist = model.Role.Split(',').Where(a => a.IsNullOrTrimEmpty() == false).Distinct().ToList();
            }

            var result = usv.SaveUser(user,rolelist,UserCache.CurrentUser.Id);

            return Json(result);
        }
    }
}
