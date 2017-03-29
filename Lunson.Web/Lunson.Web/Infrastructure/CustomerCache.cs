using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lunson.Web
{
    public class CustomerCache
    {
        /// <summary>
        /// 是否已登录
        /// </summary>
        public bool IsLogin
        {
            get {
                if (CustomerID == string.Empty)
                    return false;
                return CheckCode == SetCheckCode(CustomerID);
            }
        }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string CustomerID
        { 
            get{
                return CookieHelper.GetCookieValue("lonsun_customer_a");
            }
        }

        public string DisplayName
        {
            get
            {
                return CookieHelper.GetCookieValue("lonsun_customer_c");
            }
        }
        private string CheckCode
        {
            get
            {
                return CookieHelper.GetCookieValue("lonsun_customer_b");
            }
        }
        private string SetCheckCode(string customerID)
        {
            return "LONSUN" + Math.Abs((customerID + DataHelper.GetIP() + "lonsun_cookie").GetHashCode());
        }
        /// <summary>
        /// 设置用户cookie
        /// </summary>
        /// <param name="customerID"></param>
        public void SetCookie(string customerID,string displayName,DateTime? time=null)
        {
            DateTime t = DateTime.Now.AddHours(1); ;
            if (time != null)
                t = (DateTime)time;
            CookieHelper.SetCookie("lonsun_customer_a", customerID, t);
            CookieHelper.SetCookie("lonsun_customer_b", SetCheckCode(customerID), t);
            CookieHelper.SetCookie("lonsun_customer_c",HttpUtility.UrlEncode(displayName), t);
        }
        public void SetDisplayName(string displayName)
        {
            CookieHelper.SetCookie("lonsun_customer_c", HttpUtility.UrlEncode(displayName), DateTime.Now.AddYears(1));
        }

        public void LogOut()
        {
            CookieHelper.ClearCookie("lonsun_customer_a");
            CookieHelper.ClearCookie("lonsun_customer_b");
            CookieHelper.ClearCookie("lonsun_customer_c");
        }
    }
}