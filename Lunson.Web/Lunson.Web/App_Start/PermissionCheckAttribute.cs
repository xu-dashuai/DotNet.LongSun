using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pharos.Framework;

namespace Lunson.Web
{
    public class PermissionCheckAttribute : ActionFilterAttribute
    {
        public bool IsRelease { get; set; }
        public string Code { get; set; }
        public string Action { get; set; }

        public PermissionCheckAttribute(bool isRelease=false)
        {
            this.IsRelease = isRelease;
            this.Code = isRelease ? "" : "view";
            this.Action = "";
        }
        public PermissionCheckAttribute(string code,string action)
        {
            this.Code = code;
            this.Action = action;
            this.IsRelease = false;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (IsRelease == true)
            {
                base.OnActionExecuting(filterContext);
                return;
            }
            var area = filterContext.RouteData.DataTokens["area"];
            string url = "";
            //域
            if (area != null)
                url += ("/" + area);
            //控制器
            url += ("/" + filterContext.RouteData.Values["controller"]);
            //action
            url += (Action.IsNullOrTrimEmpty()==false ? ("/"+Action) : ("/" + filterContext.RouteData.Values["action"]));


            bool right = PermissionCache.HavePermissionTo(url, filterContext.HttpContext.Request.Url.AbsoluteUri, Code);
            if (right == false)
            {
                filterContext.Controller.TempData["msg"] = "权限不足：" + Code + " " + url;
                filterContext.Result = new RedirectResult("/backend/account/nopermission/");
            }
        }
    }
}