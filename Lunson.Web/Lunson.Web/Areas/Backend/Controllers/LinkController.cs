using Lunson.BLL.Services;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Lunson.Web.Areas.Backend.Controllers
{
    public class LinkController : BaseController
    {
        LinkService lsv = new LinkService();
        AnnexService asv = new AnnexService();

        /// <summary>
        /// 网格显示页面
        /// </summary>
        /// <returns></returns>
        [PermissionCheck("view","index")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetLinks()
        {
            return Json(lsv.GetObjectLinks());
        }

        [PermissionCheck("edit", "index")]
        public ActionResult LinkManageForm(string id)
        {
            Link link = new Link();

            if (id != null && id != "null")
            {
                link = lsv.GetLinkInclude(id);
            }

            return View(link);
        }

        #region 添加修改

        [HttpPost]
        [PermissionCheck("edit", "index")]
        public ActionResult SaveLink(Link link)
        {
            Regex reg = new Regex(@"((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?");
            if (link.LinkUrl.IsNullOrTrimEmpty() || reg.IsMatch(link.LinkUrl) == false)
                return Json(new { validate = false, target = "LinkUrl", msg = "URL格式不正确，需以http://开头" });
            if (link.Name.IsNullOrTrimEmpty() && link.ImgID.IsNullOrTrimEmpty()) //名称和图片至少有一个
                return Json(new { validate = false, target = "Name", msg = "网站名称不能为空" });

            var result = lsv.SaveLink(link, UserCache.CurrentUser.Id);
            return Json(result);
        }

        #endregion

        #region 删除
        [PermissionCheck("delete", "index")]
        [HttpPost]
        public ActionResult DeleteLink(string id)
        {
            lsv.DeleteLink(id, UserCache.CurrentUser.Id);
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

                var annew = asv.SaveLinkLogo(file, UserCache.CurrentUser.Id);

                Session["Logo"] = new { validate = true, id = annew.ID, url = annew.Url };
            }
            catch (Exception ex)
            {
                Session["Logo"] = new { validate = false, msg = ex.Message };
            }
            return Json("", "text/html");
        }

        [HttpPost]
        public ActionResult GetThumbMsg()
        {
            var msg = Session["Logo"];
            Session["Logo"] = null;
            return Json(msg);
        }

        #endregion

    }
}
