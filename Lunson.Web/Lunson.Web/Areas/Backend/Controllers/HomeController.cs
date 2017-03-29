using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lunson.Web.Areas.Backend.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Backend/Home/
        public ActionResult Index()
        {
            return View();
        }

    }
}
