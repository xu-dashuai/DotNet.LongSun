using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lunson.Web.Controllers
{
    public class BaseController : Controller
    {
        public CustomerCache customerCache = new CustomerCache();
    }
}
