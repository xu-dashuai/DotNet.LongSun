using Lunson.Domain;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pharos.Framework;

namespace Lunson.DAL.Repositories
{
    public class FeedRepository
    {
        private EFDbContext context = ContextFactory.GetCurrentContext();
        /// <summary>
        /// 后台获取文章
        /// </summary>
        /// <returns>返回所有未删除的文章</returns>
        public IQueryable<Feed> GetFeeds()
        {
            return context.Feeds.Include("FeedType").Where(a => a.IsDeleted == YesOrNo.No);
        }

        /// <summary>
        /// 前端按类型获取文章
        /// </summary>
        /// <param name="feedTypeID"></param>
        /// <returns>返回所有审核通过的、发布时间小于当前的文章</returns>
        public IQueryable<Feed> GetFeeds(string feedTypeID)
        {
            if (feedTypeID.IsNullOrTrimEmpty())
                return null;
            return context.Feeds.Where(a => a.IsDeleted == YesOrNo.No&&a.ActiveState==ActiveState.Normal && a.FeedTypeID.Equals(feedTypeID) && a.PublishOn <= DateTime.Now);
        }
        public void RemoveFeed(string id, string currentUserID)
        {
            var feed = GetFeed(id);
            if (feed != null)
            {
                feed.IsDeleted = YesOrNo.Yes;
                feed.ModifiedBy = currentUserID;
                feed.ModifiedOn = DateTime.Now;
                context.SaveChanges();
            }
        }
        public Feed GetFeed(string id)
        {
            return context.Feeds.SingleOrDefault(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(id));
        }
        public bool SaveFeed(Feed feed, string currentUserID)
        {
            if (feed.ID.IsNullOrTrimEmpty())
            {
                feed.ID = DataHelper.GetSystemID();
                feed.CreatedBy = currentUserID;
                feed.CreatedOn = DateTime.Now;
                feed.ActiveState = ActiveState.Frozen;
                context.Feeds.Add(feed);
            }
            else
            {
                var t = GetFeed(feed.ID);

                t.CopyProperty(feed);
                t.ModifiedOn = DateTime.Now;
                t.ModifiedBy = currentUserID;
            }

            return context.SaveChanges() > 0;
        }
    }
}
