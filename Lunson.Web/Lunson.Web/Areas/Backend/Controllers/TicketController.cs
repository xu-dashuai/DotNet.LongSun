using Lunson.BLL.Services;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pharos.Framework;
using System.Web.Script.Serialization;
using Lunson.Web.Areas.Backend.Models;

namespace Lunson.Web.Areas.Backend.Controllers
{
    public class TicketController : BaseController
    {
        TicketService tsv = new TicketService();
        AnnexService asv = new AnnexService();
        /// <summary>
        /// 显示页面
        /// </summary>
        /// <returns></returns>
        [PermissionCheck("view", "index")]
        public ActionResult Index()
        {
            return View();
        }

        #region 取票数据
        public ActionResult GetTickets()
        {
            return Json(tsv.GetObjectTickets());
        }
        #endregion

        #region 添加修改
        /// <summary>
        /// 添加修改票
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PermissionCheck("edit", "index")]
        public ActionResult EditTicket(string id)
        {
            if (id.IsNullOrTrimEmpty())
                return View("CreateTicket");

            var ticket = tsv.GetTicket(id, "Annex");
            if (ticket == null)
            {
                ViewBag.msg = "找不到票数据";
                return View("Error");
            }

            return View("EditTicket", ticket);
        }
        /// <summary>
        /// 修改票
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [PermissionCheck("edit", "index")]
        public ActionResult EditTicket(Ticket ticket)
        {
            if (ticket.Name.IsNullOrTrimEmpty())
                return Json(new { validate = false, target = "Name", msg = "票名不能为空" });
            if (ticket.CurPrice <= 0)
                return Json(new { validate = false, target = "CurPrice", msg = "当前销售价不正确" });
            if (ticket.Status.ToString().IsNullOrTrimEmpty())
                return Json(new { validate = false, target = "Status", msg = "请选择一个状态" });

            var result = tsv.SaveTicket(ticket, UserCache.CurrentUser.Id);
            return Json(result);
        }
        #endregion

        #region 删除
        [HttpPost]
        [PermissionCheck("delete", "index")]
        public ActionResult RemoveTicket(string id)
        {
            tsv.RemoveTicket(id, UserCache.CurrentUser.Id);
            return Json("");
        }
        #endregion

        #region 图片上传
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UploadThumb()
        {
            try
            {
                if (Request.Files.Count == 0)
                    throw new Exception("查无上传的文件");
                var file = Request.Files[0];

                if (PictureHelper.IsSecureUploadPhoto(file) == false)
                    throw new Exception("图片格式不正确：gif png jpeg jpg bmp");

                var annex = asv.SaveTicketThumb(file, UserCache.CurrentUser.Id);

                Session["TicketThumb"] = new { validate = true, id = annex.ID, url = annex.Url };
            }
            catch (Exception ex)
            {
                Session["TicketThumb"] = new { validate = false, msg = ex.Message };
            }

            return Json("", "text/html");
        }

        [HttpPost]
        public ActionResult GetThumbMsg()
        {
            var msg = Session["TicketThumb"];
            Session["TicketThumb"] = null;
            return Json(msg);
        }

        #endregion

        #region xheditor
        /// <summary>xheditor上传附件
        /// 返回内容必需是标准的json字符串，结构可以是如下：{"err":"","msg":"fileUrlxxx"} 或者 {"err":"","msg":{"url":"fileUrlxxx","localfile":"test.jpg","id":"1"}} 注：若选择结构2，则url变量是必有
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload1(HttpPostedFileBase fileData)
        {
            var data = new AnnexService().UploadContentAnnex(fileData, UserCache.CurrentUser.Id);
            return this.Content(new JavaScriptSerializer().Serialize(data));
        }
        #endregion

        #region 确认退票
        [PermissionCheck("view", "index")]
        public ActionResult TicketRefund()
        {
            return View();
        }

        /// <summary>
        /// 获取申请退票数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public ActionResult GetRefundingData(string condition, int page = 1, int rows = 20)
        {
            var data = tsv.GetTicketDetailRefunding().Where(a => a.TicketNO.Contains(condition));
            var pageList = data.Skip((page - 1) * rows).Take(rows);

            return new JsonResult { Data = new { total = data.Count(), rows = pageList.ToList().Select(a => new { a, a.Customer, a.OrderDetail, time = a.RefundingTime.ToString("yyyy-MM-dd HH:mm:ss") }) } };
        }

        public ActionResult TicketRefundForm()
        {
            return View();
        }

        /// <summary>
        /// 确认退票
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult ComfirmRefund()
        {
            var description = Request.Params["description"];
            var ids = Request.Params["ids[]"].Split(',').ToList();
            var totalPrice = double.Parse(Request.Params["price"]);

            if (totalPrice <= 0)
                return Json(new { validate = false, target = "RefundPrice", msg = "非法的金额" });

            foreach (var id in ids)
            {
                TicketRefund record = new TicketRefund();
                record.TicketDetailID = id;
                record.Description = description;
                if (ids.Count == 1)
                { //单张票可编辑退还金额
                    record.RefundPrice = totalPrice;
                }
                else
                { //多张票默认按当前价退还
                    record.RefundPrice = tsv.GetTicketDetail(id).CurPrice;
                }

                var result = tsv.RefundTicket(record, UserCache.CurrentUser.Id);
                if (result == false)
                    return Json(new { validate = false, target = "", msg = "操作失败,请尝试先刷新数据" });
            }
            return Json(new { validate = true });
        }

        #endregion

        #region 未使用的票管理

        [PermissionCheck("view", "UnusedTickets")]
        public ActionResult UnusedTickets()
        {
            return View();
        }
        /// <summary>
        /// 未使用的门票数据
        /// </summary>
        /// <param name="usedFromTime"></param>
        /// <param name="usedEndTime"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        [PermissionCheck("view", "UnusedTickets")]
        [HttpPost]
        public ActionResult UnusedTicketsData()
        {
            var records = tsv.UnusedTicketsData<UnusedTicketVM>();
            var list = (new int[] { 1 }).Select(x => new { BuyTime = "预计收入", CurPrice = records.Sum(a => (decimal)a.CurPrice), Editor = "Editor" }).ToList();
            return Json(new { total = records.Count(), rows = records, footer = list });
        }

        [PermissionCheck("view", "UnusedTickets")]
        public ActionResult SearchUnusedTicket(string queryBy, string queryString)
        {
            var data = tsv.UnusedTicketsData<UnusedTicketVM>();
            if (queryString.IsNullOrTrimEmpty() == false)
            {
                queryString = queryString.Trim();
                switch (queryBy)
                {
                    case "ticketNo":
                        data = data.Where(a => a.TicketNO.Contains(queryString)).ToList();
                        break;
                    case "mobile":
                        data = data.Where(a => a.Mobile.Contains(queryString)).ToList();
                        break;
                    default:
                        break;
                }
            }
            return Json(new { total = data.Count, rows = data });
        }

        #endregion
    }
}
