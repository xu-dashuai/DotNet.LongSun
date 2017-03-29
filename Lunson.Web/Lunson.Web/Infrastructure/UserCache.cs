using Lunson.BLL.Services;
using Lunson.Web.Models;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lunson.Web
{
    public class UserCache
    {
        private static UserData _currentUser = null;

        public static UserData CurrentUser
        {

            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.Session != null && HttpContext.Current.Session["CurrentUser"] != null)
                    {
                        _currentUser = (UserData)HttpContext.Current.Session["CurrentUser"];
                    }
                    else
                    {
                        var userId = CookieHelper.GetUserDataFromCookie(Constants.LOGIN_COOKIE_NAME);
                        if (!string.IsNullOrEmpty(userId))
                        {
                            var user = (new UserService()).GetUser(userId.Split(',')[0], true);
                            if (user != null)
                            {
                                UserHelper.SetUserSession(UserHelper.ToUserData(user));
                            }
                        }
                    }
                }
                else
                {
                    HttpContext.Current.Session["CurrentUser"] = null;
                }
                _currentUser = HttpContext.Current.Session["CurrentUser"] as UserData;
                return _currentUser;
            }
        }

    }
}