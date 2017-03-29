using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lunson.Web
{
    public class CheckLoginAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var customer = new CustomerCache();

            if(customer.IsLogin==false)
                filterContext.Result = new RedirectResult("/account/login"); 
        }
    }
}