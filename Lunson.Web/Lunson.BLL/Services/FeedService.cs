using Lunson.DAL.Repositories;
using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Lunson.BLL.Services
{
    public class FeedService
    {
        public FeedRepository DAL = new FeedRepository();

        public IQueryable<Feed> GetFeeds()
        {
            return DAL.GetFeeds();
        }
        public IQueryable<Feed> GetFeeds(string feedTypeID)
        {
            return DAL.GetFeeds(feedTypeID);
        }

        public Feed GetFeed(string id)
        {
            return DAL.GetFeed(id);
        }
        public void RemoveFeed(string id, string currentUserID)
        {
            DAL.RemoveFeed(id, currentUserID);
        }
        public List<DropdownItem> GetFeedType(string id)
        {
            List<DropdownItem> result=new List<DropdownItem>();
            var feedTypes = new FeedTypeRepository().GetFeedTypes();
            foreach (var x in feedTypes)
            {
                DropdownItem item = new DropdownItem();
                item.Text = x.Code;
                item.Value = x.ID;
                item.IsSelected = x.ID.Equals(id);
                result.Add(item);
            }
            return result;
        }

        public object SaveFeed(Feed feed,string currentUserID)
        {
            var result = DAL.SaveFeed(feed, currentUserID);
            return new { validate = result, target = "", msg = "保存失败" };
        }
        public Annex SaveFeedPicture(HttpPostedFileBase pic, string currentUserID)
        {
            var url = "/upload/feed/";
            if (Directory.Exists(HttpContext.Current.Server.MapPath(url)) == false)
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(url));
            }

            string fileExtension = pic.FileName.GetExtension().ToLower();
            var name = DataHelper.GetSystemID();

            pic.SaveAs(HttpContext.Current.Server.MapPath(url + name + fileExtension));

            var annex = new Annex();
            annex.OldName = pic.FileName;
            annex.Size = pic.ContentLength;
            annex.Type = AnnexType.FeedImg;
            annex.Url = url + name + fileExtension;

            return new AnnexRepository().CreateAnnex(annex, currentUserID);
        }
    }
}
