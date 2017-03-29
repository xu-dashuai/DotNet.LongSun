using Lunson.BLL.Services;
using Lunson.Domain.Entities;
using Lunson.Web.Models;
using Pharos.Framework;
using Pharos.Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Lunson.Web
{
    public class FormAuthProvider
    {
        public OpResult Authenticate(LoginVM model, bool isCookie, string urlHost)
        {
            var result = OpResult.Fail("用户验证失败");
            var userService = new UserService();
            var pwd = model.Password;
            if (isCookie)
            {
                var cookieValue = CookieHelper.GetCookieValue(Constants.LOGIN_COOKIE_INFO);
                pwd = cookieValue.Split(',')[1];
            }
            else
                pwd = userService.GetEncryPassword(pwd, false);
            //验证是否有登录权限
            result = userService.CheckLogin(model.UserName, pwd);
            if (result.Successed)
            {

                var entity = (Sys_User)result.Data;
                string userData = string.Format("{0},{1},{2}", entity.ID, entity.UserName, entity.DisplayName);
                CookieHelper.SaveTicketCookie(Constants.LOGIN_COOKIE_NAME, userData, 0, urlHost);
                UserHelper.SetUserSession(UserHelper.ToUserData(entity));
                SetLoginCookie(model.UserName, entity.Password, model.IsRememberMe, urlHost);
                //记录登录信息
                new UserService().SaveUserLoginLog(entity.ID, DataHelper.GetIP());
            }
            return result;
        }
        /// <summary>
        /// 设置cookie
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="rememberMe"></param>
        /// <param name="urlHost"></param>
        /// <returns></returns>
        private bool SetLoginCookie(string userName, string password, bool rememberMe, string urlHost)
        {
            if (rememberMe)
            {
                var cookiePwd = CookieHelper.GetCookie(Constants.LOGIN_COOKIE_INFO);
                var resetCookie = false;
                if (string.IsNullOrEmpty(cookiePwd))
                    resetCookie = true;
                if (!string.IsNullOrEmpty(cookiePwd))
                {
                    if (cookiePwd.Split(',')[0] != userName)
                    {
                        resetCookie = true;
                    }
                }
                if (resetCookie)
                {
                    var cookieValue = string.Format("{0},{1}", userName, password);
                    CookieHelper.SaveCookie(Constants.LOGIN_COOKIE_INFO, cookieValue, urlHost, 43200, false);
                }
            }
            else
            {
                CookieHelper.SaveCookie(Constants.LOGIN_COOKIE_INFO, "", urlHost, 0, false);
            }
            return true;
        }

    }
}