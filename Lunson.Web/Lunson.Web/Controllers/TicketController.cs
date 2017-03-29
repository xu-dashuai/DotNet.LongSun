using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lunson.BLL.Services;
using Lunson.Web.Models;
using Lunson.Domain;
using Pharos.Framework;

namespace Lunson.Web.Controllers
{
    public class TicketController : Controller
    {
        TicketService tsv = new TicketService();
        /// <summary>
        /// 票列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var data = tsv.GetAvailableIncludeTickets();
            return View(data.ToList());
        }
        /// <summary>
        /// 购买明细
        /// </summary>
        /// <param name="ID">票ID</param>
        /// <param name="Num">数量</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TicketDetails(string ID, string Num = "1")
        {
            var dic = DataHelper.ToDic<int>(ID, Num, ',');

            if (dic.Any())
            {
                return View(tsv.GetTicketCart<TicketDetailVM>(dic));
            }

            return RedirectToAction("Index");
        }
        /// <summary>
        /// 根据ID加载门票详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult TicketParticular(string id)
        {
            ViewBag.imgs = new AnnexService().GetAnnexes(AnnexType.Crocodile).OrderBy(a => Guid.NewGuid()).Take(6);
            var data = tsv.GetTicket(id, "Annex");
            //var data = tsv.TicketParticular(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }


        
        /// <summary>
        /// 退票
        /// </summary>
        /// <param name="nos"></param>
        /// <returns></returns>
        [CheckLogin]
        public ActionResult Refund(string nos)
        {
            CustomerCache cache = new CustomerCache();
            return Json(tsv.Refund(nos,cache.CustomerID));
        }

        public ActionResult Help()
        {
            return View();
        }
    }
}
