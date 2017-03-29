using Lunson.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Lunson.Web.Areas.Backend.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(LoginVM model, bool hidden1, string returnUrl)
        {
            FormAuthProvider authProvider = new FormAuthProvider();
            if (ModelState.IsValid)
            {
                var loginResult = authProvider.Authenticate(model, hidden1, Request.Url.Host);
                if (loginResult.Successed)
                {
                    return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                }
                else
                {
                    ModelState.AddModelError("", loginResult.Message);
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult NoPermission()
        {
            return Json(new { validate=false,msg=TempData["msg"]},JsonRequestBehavior.AllowGet);
        }
    }
}
