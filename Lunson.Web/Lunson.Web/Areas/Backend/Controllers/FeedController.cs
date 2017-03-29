using Lunson.BLL.Services;
using Lunson.Domain;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Pharos.Framework;
using Pharos.Framework.MVC;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Web.Script.Serialization;

namespace Lunson.Web.Areas.Backend.Controllers
{
    public class FeedController : BaseController
    {
        public FeedService fsv = new FeedService();

        [PermissionCheck("view", "index")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetFeeds(int? page, int? rows)
        {
            var feeds = fsv.GetFeeds();

            var pagelist = feeds.OrderByDescending(a => a.PublishOn).Skip(((page ?? 1) - 1) * (rows ?? 20)).Take(rows ?? 20).ToList()
                .Select(a => new
                {
                    a.ID,
                    a.ActiveState,
                    a.Author,
                    a.ClickNum,
                    FeedTypeName = a.FeedType == null ? "" : a.FeedType.Name,
                    a.ShowOrder,
                    a.Title,
                    a.Original,
                    a.IsRecommend,
                    a.IsTop,
                    a.PublishOn
                });

            //var json =  JsonConvert.SerializeObject(new { total = feeds.Count(), rows = pagelist });
            //return json;
            return Json(new { total = feeds.Count(), rows = pagelist });
        }
        [HttpPost]
        [PermissionCheck("delete", "index")]
        public ActionResult RemoveFeed(string id)
        {
            fsv.RemoveFeed(id, UserCache.CurrentUser.Id);
            return Json("");
        }

        [PermissionCheck("edit", "index")]
        public ActionResult FeedManageForm(string id)
        {
            var feed = fsv.GetFeed(id);
            if (feed == null)
                feed = new Feed { ActiveState = ActiveState.Frozen };

            ViewBag.feedtypes = fsv.GetFeedType(feed.FeedTypeID);

            return View(feed);
        }

        [HttpPost]
        [ValidateInput(false)]
        [PermissionCheck("edit", "index")]
        public ActionResult SaveData(Feed feed)
        {
            if (feed.Title.IsNullOrTrimEmpty())
            {
                return Json(new { validate = false, target = "Title", msg = "标题不能为空" });
            }

            var result = fsv.SaveFeed(feed, UserCache.CurrentUser.Id);
            return Json(result);
        }

        //// simditor
        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult Upload()
        //{
        //    try
        //    {
        //        if (Request.Files.Count == 0)
        //            throw new Exception("查无上传的文件");
        //        var file = Request.Files[0];


        //        if (PictureHelper.IsSecureUploadPhoto(file) == false)
        //            throw new Exception("图片格式不正确：gif png jpeg jpg bmp");

        //        var annew = fsv.SaveFeedPicture(file, UserCache.CurrentUser.Id);
        //        return Json(new { success = true, msg = "上传成功", file_path = annew.Url });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, msg = ex.Message, file_path = "" });
        //    }

        //}

        /// <summary>
        /// 文章查询
        /// </summary>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        public ActionResult SearchFeeds(string title, string type, DateTime? beginTime, DateTime? endTime, int? page, int? rows)
        {
            var feeds = fsv.GetFeeds();

            if (!title.IsNullOrTrimEmpty())
                feeds = feeds.Where(f => f.Title.Contains(title));
            if (!type.IsNullOrTrimEmpty())
                feeds = feeds.Where(f => f.FeedTypeID.Equals(type));
            if (beginTime != null)
                feeds = feeds.Where(f => f.PublishOn >= beginTime);
            if (endTime != null)
            {
                endTime = ((DateTime)endTime).AddDays(1);
                feeds = feeds.Where(f => f.PublishOn <= endTime);
            }

            var pagelist = feeds.OrderByDescending(f=>f.PublishOn).Skip(((page ?? 1) - 1) * (rows ?? 20)).Take(rows ?? 20).ToList()
                .Select(a => new
                {
                    a.ID,
                    a.ActiveState,
                    a.Author,
                    a.ClickNum,
                    FeedTypeName = a.FeedType == null ? "" : a.FeedType.Name,
                    a.ShowOrder,
                    a.Title,
                    a.Original,
                    a.IsRecommend,
                    a.IsTop,
                    a.PublishOn
                });
            //return Newtonsoft.Json.JsonConvert.SerializeObject(new { total = feeds.Count(), rows = pagelist });
            return Json(new { total = feeds.Count(), rows = pagelist });
        }

        #region 文章审核
        [PermissionCheck("view", "feedreview")]
        public ActionResult FeedReview()
        {
            return View();
        }

        [PermissionCheck("edit", "feedreview")]
        public ActionResult UpdateIsRecommend(string id, int? oldValue)
        {
            var feed = fsv.GetFeed(id);
            if (oldValue != 1) feed.IsRecommend = 1;
            else feed.IsRecommend = 0;
            var result = fsv.SaveFeed(feed, UserCache.CurrentUser.Id);
            return Json(result);
        }
        [PermissionCheck("edit", "feedreview")]
        public ActionResult UpdateIsTop(string id, int? oldValue)
        {
            var feed = fsv.GetFeed(id);
            if (oldValue != 1) feed.IsTop = 1;
            else feed.IsTop = 0;
            var result = fsv.SaveFeed(feed, UserCache.CurrentUser.Id);
            return Json(result);
        }

        [PermissionCheck("edit", "feedreview")]
        public ActionResult UpdateCheck(string id, int? oldValue)
        {
            var feed = fsv.GetFeed(id);
            if (oldValue == 1) feed.ActiveState = ActiveState.Normal;
            else feed.ActiveState = ActiveState.Frozen;
            var result = fsv.SaveFeed(feed, UserCache.CurrentUser.Id);
            return Json(result);
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
    }
}
