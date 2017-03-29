using Lunson.BLL.Services;
using Lunson.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pharos.Framework;

namespace Lunson.Web.Areas.Backend.Controllers
{
    public class AnnexController : BaseController
    {
        private AnnexService asv = new AnnexService();

        #region flash
        [PermissionCheck("view","flash")]
        public ActionResult Flash()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetFlashs()
        {
            var flashs = asv.GetAnnexes(AnnexType.Swf);
            return Json(flashs);
        }
        [PermissionCheck("edit", "flash")]
        public ActionResult UploadFlash()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UploadFlash(string id)
        {
            try
            {
                if (Request.Files.Count == 0)
                    throw new Exception("查无上传的文件");
                var file = Request.Files[0];

                if (file.FileName.GetExtension().ToLower() != ".swf")
                    throw new Exception("flash格式不正确：swf");

               asv.SaveFlash(file, UserCache.CurrentUser.Id);

               Session["FlashMsg"] = new { validate = true };
            }
            catch (Exception ex)
            {
                Session["FlashMsg"] = new { validate = false, msg = ex.Message };
            }

            return Json("", "text/html");
        }
        public ActionResult GetFlashMsg()
        {
            var msg = Session["FlashMsg"];
            Session["FlashMsg"] = null;
            return Json(msg);
        }

        [PermissionCheck("edit", "flash")]
        public ActionResult EditFlash(string id)
        {
            var flash = asv.GetAnnex(id,AnnexType.Swf);
            if (flash == null)
            {
                ViewBag.msg = "找不到附件数据";
                return View("Error");
            }

            return View(flash);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditFlash(string id, string OldName)
        {
            if (OldName.IsNullOrTrimEmpty())
                return Json(new { validate = false, target = "OldName", msg = "名称不能为空" });

            var result = asv.EditAnnexName(id, OldName, UserCache.CurrentUser.Id);
            return Json(result);
        }

        [HttpPost]
        [PermissionCheck("delete", "flash")]
        public ActionResult RemoveFlash(string id)
        {
            asv.RemoveAnnex(id, UserCache.CurrentUser.Id, AnnexType.Swf);
            return Json("");
        }

        [PermissionCheck("view", "flash")]
        public ActionResult ShowFlash(string id)
        {
            var flash = asv.GetAnnex(id,AnnexType.Swf);
            if (flash == null)
            {
                ViewBag.msg = "找不到附件数据";
                return View("Error");
            }

            return View(flash);
        }
        #endregion

        #region Crocodile
        [PermissionCheck("view", "crocodile")]
        public ActionResult Crocodile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetCrocodiles()
        {
            var crocodiles = asv.GetAnnexes(AnnexType.Crocodile);
            return Json(crocodiles);
        }
        [PermissionCheck("edit", "crocodile")]
        public ActionResult UploadCrocodile()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UploadCrocodile(string id)
        {
            try
            {
                if (Request.Files.Count == 0)
                    throw new Exception("查无上传的文件");
                var file = Request.Files[0];

                if (PictureHelper.IsSecureUploadPhoto(file) == false)
                    throw new Exception("图片格式不正确：gif png jpeg jpg bmp");

                asv.SaveCrocodile(file, UserCache.CurrentUser.Id);

                Session["CrocodileMsg"] = new { validate = true };
            }
            catch (Exception ex)
            {
                Session["CrocodileMsg"] = new { validate = false, msg = ex.Message };
            }

            return Json("", "text/html");
        }
        public ActionResult GetCrocodileMsg()
        {
            var msg = Session["CrocodileMsg"];
            Session["CrocodileMsg"] = null;
            return Json(msg);
        }
        [HttpPost]
        [PermissionCheck("delete", "crocodile")]
        public ActionResult RemoveCrocodile(string id)
        {
            asv.RemoveAnnex(id, UserCache.CurrentUser.Id, AnnexType.Crocodile);
            return Json("");
        }
        #endregion
    }
}
