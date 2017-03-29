using Lunson.DAL.Repositories;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.BLL.Services
{
    /// <summary>
    /// 友情链接服务类
    /// </summary>
    public class LinkService
    {
        private LinkRepository DAL = new LinkRepository();

        public Link GetLinkInclude(string id)
        {
            var link = DAL.GetInfoIncludeByID<Link>(id, "Annex");
            return link;
        }
        public List<Link> GetLinks()
        {
            var links = DAL.GetQueryIncludeInfo<Link>("Annex");
            return links.OrderBy(a => a.ShowOrder).ToList();
        }
        public object GetObjectLinks()
        {
            var links = DAL.GetQueryIncludeInfo<Link>("Annex").ToList();

            return links.Select(a => new
            {
                a.ID,
                a.Name,
                a.LinkUrl,
                a.ShowOrder,
                a.ActiveState,
                LogoUrl = a.Annex == null ? "" : a.Annex.Url
            });
        }
        public object SaveLink(Link link, string currentUserID)
        {
            DAL.SaveInfo<Link>(link, currentUserID);
            DAL.Submit();
            return new { validate = true, target = "", msg = "保存成功" };
        }
        public bool DeleteLink(string id, string currentUserID)
        {
            DAL.RemoveInfo<Link>(id, currentUserID);
            return DAL.Submit() > 0;
        }
    }
}
