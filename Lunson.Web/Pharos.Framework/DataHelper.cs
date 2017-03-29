using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.Framework
{
    public static class DataHelper
    {
        public static T ToObject<T>(this object source) where T : class, new()
        {
            var result = new T();

            var tsource = source.GetType();
            var tresult=typeof(T);

            foreach (var x in tresult.GetProperties())
            {
                var p = tsource.GetProperty(x.Name);
                if (p != null)
                {
                    try
                    {
                        x.SetValue(result, p.GetValue(source, null), null);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }

            return result;
        }
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static string ToString(this DateTime? time, string format = "")
        {
            if (time == null)
                return "";
            if (format.IsNullOrTrimEmpty())
                return ((DateTime)time).ToString();
            return ((DateTime)time).ToString(format);
        }
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrTrimEmpty(this string source)
        {
            return source == null || source.Trim() == "";
        }
        public static string GetExtension(this string source)
        {
            return source.IsNullOrTrimEmpty() ? "" : source.Substring(source.LastIndexOf('.'));
        }
        /// <summary>
        /// 返回GUID去横杆字符串大写
        /// </summary>
        /// <returns></returns>
        public static string GetSystemID()
        {
            var result = Guid.NewGuid().ToString("N");
            return result.ToUpper();
        }
        /// <summary>
        /// 如果值为NULL或空 返回空
        /// </summary>
        /// <returns></returns>
        public static string ToEmptyString(this string source)
        {
            return source.IsNullOrTrimEmpty() ? "" : source.Trim();
        }
        public static string GetIP()
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"]))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);
            if (string.IsNullOrEmpty(ip))
                ip = Convert.ToString(System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]);
            return ip;
        }
        public static Dictionary<string, T> ToDic<T>(string keys, string values, char split)
        {
            Dictionary<string, T> result = new Dictionary<string, T>();
            var k = keys.Split(split);
            var v = values.Split(split);

            for (int i = 0; i < k.Length && i < v.Length; i++)
            {
                var key = k[i];
                var value = v[i];

                if (key.IsNullOrTrimEmpty() == false && result.Keys.Contains(key) == false)
                {
                    try
                    {
                        result.Add(key, (T)Convert.ChangeType(value, typeof(T)));
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }
            }

            return result;
        }
    }
}
