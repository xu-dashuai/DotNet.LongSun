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
    public class FeedTypeController : BaseController
    {
        FeedTypeService fts = new FeedTypeService();
        AnnexService asv = new AnnexService();

        [PermissionCheck("view", "index")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 文章类型Grid数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public ActionResult GetFeedTypes()
        {
            var result = fts.GetFeedTypes();
            return Json(result);
        }

        /// <summary>
        /// 文章类型编辑Form
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PermissionCheck("edit", "index")]
        public ActionResult FeedTypeManageForm(string id)
        {
            FeedType feedType = new FeedType();

            if(id != null && id != "null")
                feedType = fts.GetFeedType(id);

            return View(feedType);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [PermissionCheck("delete", "index")]
        public bool DeleteData(string id)
        {
            return fts.DeleteFeedType(id, UserCache.CurrentUser.Id);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="feedType">角色对象</param>
        /// <returns></returns>
        [PermissionCheck("edit", "index")]
        public ActionResult SaveData(FeedType feedType)
        {
            Regex reg = new Regex(@"^[a-zA-Z]{1}[0-9a-zA-Z_]{1,19}$");
            if (feedType.Name.IsNullOrTrimEmpty())
            {
                return Json(new { validate = false, target = "Name", msg = "角色名称不能为空" });
            }
            if (feedType.Code.IsNullOrTrimEmpty() || reg.IsMatch(feedType.Code) == false)
            {
                return Json(new { validate = false, target = "Code", msg = "以字母开头，只能含有字母数字下划线，长度2-20位" });
            }

            var result = fts.SaveFeedType(feedType, UserCache.CurrentUser.Id);
            return Json(result);
        }

    }
}
