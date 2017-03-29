using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lunson.Web
{
    public class UserAuthAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.HttpContext != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (UserCache.CurrentUser == null)
                    {
                        filterContext.Result = new RedirectResult("~/backend/Account/Login");
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/backend/Account/Login");
                }
            }
        }
    }
}