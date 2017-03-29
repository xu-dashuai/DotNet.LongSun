using Lunson.BLL.Services;
using Lunson.Web.Models;
using Pharos.Framework;
using Pharos.Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lunson.Web.Controllers
{
    [CheckLogin]
    public class BookLetController : BaseController
    {
        TicketService tsv = new TicketService();

        public ActionResult Index()
        {
            var availableTickets = tsv.GetTicketDetailByUser(customerCache.CustomerID);
            //var availableTicketPage = availableTickets.OrderBy(a=>a.CreatedOn).ToPagedList(p_available ?? 1, 1);
            return View(availableTickets.OrderBy(p=>p.CreatedOn));
        }

        public ImageResult QR(string id)
        {
            var detailNos = id.Split(',').Where(a => a.IsNullOrTrimEmpty() == false).Distinct().ToList();

            detailNos = tsv.GetAvailableTicketNos(detailNos, customerCache.CustomerID);
            if (detailNos.Any())
            {
                QRCodeHelper.GetQRCode(string.Join(",", detailNos.Take(5)), Response);
            }

            return new ImageResult();
        }

    }
}
