using Lunson.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Pharos.Framework;
using Lunson.Domain;

namespace Lunson.Web.Controllers
{
    public class FeedController : Controller
    {
        public FeedTypeService ftsv = new FeedTypeService();
        public FeedService fsv = new FeedService();

        public ActionResult Index(string id, int? page)
        {
            id = id.IsNullOrTrimEmpty() ? "Introduction" : id;
            var feedType = ftsv.GetFeedTypeByCode(id);
            if (feedType == null)
                return View("404");

            var feeds = fsv.GetFeeds(feedType.ID);
            if (feeds != null)
            {
                if (feeds.Count() == 1)
                    return Redirect("/"+id+"/show?fid="+feeds.First().ID);

                ViewBag.feeds = feeds.OrderByDescending(a => a.IsTop).ThenByDescending(a => a.PublishOn).ToPagedList(page ?? 1, 20);
            }
            ViewBag.id = id;
            return View(feedType);
        }

        public ActionResult Show(string id, string fid)
        {
            var feedType = ftsv.GetFeedTypeByCode(id);
            if (feedType == null)
                return View("404");

            var feed = fsv.GetFeed(fid);
            if (feed == null || feed.FeedTypeID.Equals(feedType.ID) == false||feed.ActiveState==ActiveState.Frozen )
                return View("404");

            ViewBag.feed = feed;

            ViewBag.id = id;
            return View(feedType);

        }

    }
}
