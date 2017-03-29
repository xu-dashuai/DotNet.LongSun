using Lunson.DAL.Repositories;
using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.BLL.Services
{
    public class FeedTypeService
    {
        private FeedTypeRepository DAL = new FeedTypeRepository();

        /// <summary>
        /// 获取状态集合项
        /// </summary>
        /// <returns></returns>
        public List<DropdownItem> GetActiveStates()
        {
            var activeStates = EnumHelper.GetList(typeof(ActiveState));
            return activeStates.Select(a => new DropdownItem { Text = a.Text, Value = a.Value }).ToList();
        }

        public List<DropdownItem> GetFlashList(string id = "")
        {
            var flashList = new AnnexService().GetAnnexes(AnnexType.Swf);
            return flashList.Select(a => new DropdownItem { Text = a.OldName, Value = a.ID, IsSelected = (a.ID.Equals(id)) }).ToList();
        }

        /// <summary>
        /// 获取单个文章类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FeedType GetFeedType(string id, bool includeAnnex = true)
        {
            return DAL.GetFeedType(id, includeAnnex);
        }

        /// <summary>
        /// 获取单个文章类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FeedType GetFeedTypeByCode(string code, bool includeAnnex = true)
        {
            return DAL.GetFeedTypeByCode(code, includeAnnex);
        }

        /// <summary>
        /// 获取文章类型集合
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public object GetFeedTypes()
        {
            var feedTypes = DAL.GetFeedTypes();

            return feedTypes.ToList().Select(a => new
            {
                a.ActiveState,
                a.Annex,
                a.Code,
                a.Description,
                a.ID,
                a.Name,
                SwfName = a.Annex == null ? "" : a.Annex.OldName
            });
        }

        /// <summary>
        /// 保存文章类型，新增或修改
        /// </summary>
        /// <param name="feedType">角色对象</param>
        /// <param name="curUserID">操作者ID</param>
        /// <returns></returns>
        public Object SaveFeedType(FeedType feedType, string curUserID)
        {
            if (DAL.CheckCodeUnique(feedType.Code, feedType.ID))
                return new { validate = false, target = "Code", msg = "文章代码已存在" };

            FeedType dbFeedType;

            if (feedType.ID.IsNullOrTrimEmpty())
            { //add
                dbFeedType = DAL.AddFeedType(feedType, curUserID);
            }
            else
            { //updata
                dbFeedType = DAL.UpdateFeedType(feedType, curUserID);
                if (dbFeedType == null)
                {
                    return new { validate = false, target = "", msg = "找不到条目信息" };
                }
            }
            return new { validate = true, target = "", msg = "保存成功" };
        }

        /// <summary>
        /// 删除文章类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        public bool DeleteFeedType(string id, string curUserID)
        {
            return DAL.DeleteFeedType(id, curUserID);
        }

        /// <summary>
        /// 文章类型下拉数据
        /// </summary>
        /// <returns></returns>
        public List<DropdownItem> FeedTypesItem(bool all)
        {
            List<DropdownItem> ddi = new List<DropdownItem>();
            if (all == true)
                ddi.Add(new DropdownItem { Text = "全部", Value = "" });
            var data = DAL.GetFeedTypes();
            foreach (var item in data)
            {
                ddi.Add(new DropdownItem { Text = item.Name, Value = item.ID });
            }
            return ddi;
        }
    }
}
