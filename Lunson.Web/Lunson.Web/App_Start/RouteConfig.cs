using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lunson.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //首页
            #region
            routes.MapRoute(
                name: "Home_Empty",
                url: "",
                defaults: new { controller = "Home", action = "Flash", id = "" },
                //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Lunson.Web.Controllers" }
            );
            routes.MapRoute(
                name: "Home/{action}/{id}",
                url: "Home",
                defaults: new { controller = "Feed", action = "Index", id = "Introduction" },
                //defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Lunson.Web.Controllers" }
            );
            #endregion
            //前台用户中心
            #region
            routes.MapRoute(
                name: "Account",
                url: "Account/{action}/{id}",
                defaults: new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Lunson.Web.Controllers" }
            );
            #endregion
            //网上购票
            #region
            routes.MapRoute(
                name: "Ticket",
                url: "Ticket/{action}/{id}",
                defaults: new { controller = "Ticket", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Lunson.Web.Controllers" }
            );
            #endregion
            //订单
            #region
            routes.MapRoute(
                name: "Order",
                url: "Order/{action}/{id}",
                defaults: new { controller = "Order", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Lunson.Web.Controllers" }
            );
            #endregion
            //BookLet
            #region
            routes.MapRoute(
                name: "BookLet",
                url: "BookLet/{action}/{id}",
                defaults: new { controller = "BookLet", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Lunson.Web.Controllers" }
            );
            #endregion
            //移动端购票
            #region
            routes.MapRoute(
                name: "MobilePay",
                url: "MobilePay/{action}/{id}",
                defaults: new { controller = "MobilePay", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Lunson.Web.Controllers" }
            );
            #endregion
            //Feed
            #region
            routes.MapRoute(
                name: "Feed",
                url: "{id}/{action}",
                defaults: new { controller = "Feed", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "Lunson.Web.Controllers" }
            );
            #endregion
        }
    }
}