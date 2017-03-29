using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Pharos.Framework
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public int TotalPageCount { get; private set; }
        public int StartRecordIndex { get; private set; }
        public int EndRecordIndex { get; private set; }

        public PagedList(IList<T> items, int pageIndex, int pageSize)
        {
            PageSize = pageSize;
            TotalItemCount = items.Count;
            TotalPageCount = (int)Math.Ceiling(TotalItemCount / (double)PageSize);
            CurrentPageIndex = pageIndex;
            StartRecordIndex = (CurrentPageIndex - 1) * PageSize + 1;
            EndRecordIndex = TotalItemCount > pageIndex * pageSize ? pageIndex * pageSize : TotalItemCount;
            for (int i = StartRecordIndex - 1; i < EndRecordIndex; i++)
            {
                Add(items[i]);
            }
        }

        public PagedList(IEnumerable<T> items, int pageIndex, int pageSize, int totalItemCount)
        {
            AddRange(items);
            TotalItemCount = totalItemCount;
            TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
            CurrentPageIndex = pageIndex;
            PageSize = pageSize;
            StartRecordIndex = (pageIndex - 1) * pageSize + 1;
            EndRecordIndex = TotalItemCount > pageIndex * pageSize ? pageIndex * pageSize : totalItemCount;
        }
    }

    public static class PageLinqExtensions
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> allItems, int pageIndex, int pageSize)
        {
            if (pageIndex < 1)
                pageIndex = 1;
            var itemIndex = (pageIndex - 1) * pageSize;
            var pageOfItems = allItems.Skip(itemIndex).Take(pageSize);
            var totalItemCount = allItems.Count();
            return new PagedList<T>(pageOfItems, pageIndex, pageSize, totalItemCount);
        }
    }

    public static class PagerHelper
    {

        public static string Pager(this HtmlHelper helper, int pageSize, int currentPageIndex, int totalPages, int totalItemCount)
        {
            bool hasPreviousPage = (currentPageIndex > 1);
            bool hasNextPage = (currentPageIndex < totalPages);

            StringBuilder sb = new StringBuilder();
            string reqUrl = helper.ViewContext.HttpContext.Request.RawUrl;
            string link = "";

            Regex re = new Regex(@"page=(\d+)|page=", RegexOptions.IgnoreCase);

            MatchCollection results = re.Matches(reqUrl);

            if (results.Count > 0)
            {
                link = reqUrl.Replace(results[0].ToString(), "page={0}");
            }
            else if (reqUrl.IndexOf("?") < 0)
            {
                link = reqUrl + "?page={0}";
            }
            else
            {
                link = reqUrl + "&page={0}";
            }

            if (totalItemCount > pageSize)
            {
                sb.Append("<div class=\"pager\">");

                sb.AppendFormat("<span>共{0}页 {1}条记录，当前为第{2}页</span>",totalPages,totalItemCount,currentPageIndex);

                if (hasPreviousPage)
                {
                    sb.Append("<a href=\"" + string.Format(link, (currentPageIndex - 1).ToString()) + "\">上一页</a>");
                }

                int beginPage = currentPageIndex - 2, endPage = currentPageIndex + 2;

                beginPage = beginPage < 1 ? 1 : beginPage;
                endPage = endPage > totalPages ? totalPages : endPage;

                if (beginPage >= 2)
                {
                    sb.Append("<a href=\"?" + string.Format(link, "1") + "\">1</a>");
                    if (beginPage > 2)
                        sb.Append("...");
                }



                for (int i = beginPage; i <= endPage; i++)
                {
                    if (i == currentPageIndex)
                        sb.Append("<span class=cur>").Append(i.ToString()).Append("</span>");
                    else
                        sb.Append("<a href=\"" + string.Format(link, i) + "\">" + i + "</a>");
                }

                if (endPage <= totalPages - 1)
                {
                    if (endPage < totalPages - 1)
                        sb.Append("...");
                    sb.Append("<a href=\"" + string.Format(link, totalPages) + "\">" + totalPages + "</a>");
                }

                if (hasNextPage)
                {
                    sb.Append("<a href=\"" + string.Format(link, (currentPageIndex + 1).ToString()) + "\">下一页</a>");
                }

                sb.Append("</div>");
            }

            return sb.ToString();
        }

        public static string Pager<T>(this HtmlHelper helper, PagedList<T> pagedList)
        {
            if (pagedList == null)
                return "";
            return Pager(helper, pagedList.PageSize, pagedList.CurrentPageIndex, pagedList.TotalPageCount, pagedList.TotalItemCount);
        }

    }
}
