using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.DAL.Repositories
{
    public class FeedTypeRepository
    {
        EFDbContext context = ContextFactory.GetCurrentContext();

        /// <summary>
        /// 获取单个文章类型对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeAnnex">是否包含附件</param>
        /// <returns></returns>
        public FeedType GetFeedType(string id, bool includeAnnex = true)
        {
            if (includeAnnex == false)
                return context.FeedTypes.SingleOrDefault(t => t.ID.Equals(id) && t.IsDeleted == YesOrNo.No);
            return context.FeedTypes.Include("Annex").SingleOrDefault(t => t.ID.Equals(id) && t.IsDeleted == YesOrNo.No);
        }
        public FeedType GetFeedTypeByCode(string code, bool includeAnnex = true)
        {
            if (includeAnnex == false)
                return context.FeedTypes.SingleOrDefault(t => t.Code.Equals(code,StringComparison.OrdinalIgnoreCase) && t.IsDeleted == YesOrNo.No);
            return context.FeedTypes.Include("Annex").SingleOrDefault(t => t.Code.Equals(code, StringComparison.OrdinalIgnoreCase) && t.IsDeleted == YesOrNo.No);
        }

        /// <summary>
        /// 获取所有文章类型
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public IQueryable<FeedType> GetFeedTypes()
        {
            var result = context.FeedTypes.Include("Annex").Where(t => t.IsDeleted == YesOrNo.No);

            return result;
        }

        /// <summary>
        /// 添加文章类型
        /// </summary>
        /// <param name="feedType"></param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        public FeedType AddFeedType(FeedType feedType, string curUserID)
        {
            var isExist = context.Sys_Roles.Any(r => r.Code.Equals(feedType.Code) && r.IsDeleted == YesOrNo.No);
            if (isExist == true)
            { //角色代码已存在，返回空对象
                return feedType = null;
            }

            feedType.ID = DataHelper.GetSystemID();
            feedType.CreatedBy = curUserID;
            feedType.CreatedOn = DateTime.Now;

            context.FeedTypes.Add(feedType);
            context.SaveChanges();
            return feedType;
        }

        public bool CheckCodeUnique(string code,string id = "")
        {
            return context.FeedTypes.Any(a => a.IsDeleted == YesOrNo.No && a.ID.Equals(id) == false && a.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 修改文章类型
        /// </summary>
        /// <param name="feedType"></param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        public FeedType UpdateFeedType(FeedType feedType, string curUserID)
        {
            var dbEntity = context.FeedTypes.SingleOrDefault(r => r.ID.Equals(feedType.ID) && r.IsDeleted == YesOrNo.No);
            if (dbEntity != null)
            {
                dbEntity.CopyProperty(feedType);

                dbEntity.ModifiedBy = curUserID;
                dbEntity.ModifiedOn = DateTime.Now;
                context.SaveChanges();
            }
            return feedType;
        }

        /// <summary>
        /// 删除文章类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        public bool DeleteFeedType(string id, string curUserID)
        {
            var entity = GetFeedType(id, false);
            if (entity != null)
            {
                entity.IsDeleted = YesOrNo.Yes;
                entity.ModifiedBy = curUserID;
                entity.ModifiedOn = DateTime.Now;
            }
            return context.SaveChanges() > 0;
        }
    }
}
