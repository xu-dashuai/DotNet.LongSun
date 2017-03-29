/********************************************
 * Create By：zsj
 * Create Time：2015/5/11 16:51:40
 * Function：功能说明
 * Modify Course：
 2015/5/11 16:51:40   zsj    创建文件
 *******************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Lonsun.API.EChi.Utils
{
    public class Common
    {
        /// <summary>
        /// 签名内容
        /// </summary>
        public const string SMSSignContent = "【南顺鳄鱼园】";

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns></returns>
        public static string MD5(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();

        }

        #region 判断远程请求文件是否存在HttpWebRequest

        public static string GetPageResponseContent(string url, string method)
        {
            string result = null;
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);

            req.Method = method;
            req.ContentType = "text/html;charset=utf-8";
            string return_content = GetPageResponseContent(req, Encoding.UTF8);
            if (!string.IsNullOrEmpty(Convert.ToString(return_content)))
            {
                result = return_content;
            }
            else
            {
                result = "";
            }
            return result;
        }
        /// <summary>
        /// 取得页面输出内容
        /// </summary>
        /// <param name="url">远程地址</param>
        /// <returns></returns>
        public static string GetPageReponseContent(string url, System.Text.Encoding encoding)
        {
            WebRequest req = WebRequest.Create(url);
            return GetPageResponseContent(req, encoding);
        }

        /// <summary>
        /// 获取远程页面内容
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetPageResponseContent(WebRequest request, System.Text.Encoding encoding)
        {
            return GetPageResponseContent(request, encoding, null);
        }
        /// <summary>
        /// 获取远程页面内容，带参数
        /// </summary>
        /// <param name="request"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetPageResponseContent(WebRequest request, System.Text.Encoding encoding, byte[] param)
        {
            //定义页面输出内容变量
            string content = null;
            WebResponse response = null;
            if (UrlExisted(request, param, ref response))
            {
                try
                {
                    //if (response != null && response.ContentLength > 0)
                    if (response != null)
                    {
                        Stream resStream = null;
                        try
                        {
                            resStream = response.GetResponseStream();
                        }
                        catch (Exception ex) { }
                        if (resStream != null)
                        {
                            StreamReader sr = new StreamReader(resStream, encoding);
                            content = sr.ReadToEnd();
                            resStream.Close();
                            sr.Close();
                        }
                        if (response != null)
                        {
                            response.Close();
                        }
                    }
                }
                catch (Exception ex) { }
            }
            return content;
        }
        /// <summary>
        /// 判断远程文件是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool UrlExisted(WebRequest request)
        {
            WebResponse response = null;
            return UrlExisted(request, null, ref response);
        }
        /// <summary>
        /// 判断远程文件是否存在
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool UrlExisted(WebRequest request, byte[] param, ref WebResponse response)
        {
            bool result = false;
            try
            {

                if (param == null)
                {
                    param = new byte[] { };
                    request.ContentLength = param.Length;
                }
                switch (request.Method.ToLower())
                {
                    case "get":

                        break;
                    case "post":
                        if (param.Length > 0)
                        {
                            Stream reqStream = null;
                            request.ContentLength = param.Length;
                            reqStream = request.GetRequestStream();
                            reqStream.Write(param, 0, param.Length);
                            reqStream.Close();
                        }
                        break;
                }
                response = request.GetResponse();
                result = response == null ? false : true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                //if (response != null)
                //{
                //    response.Close();
                //}
            }
            return result;
        }
        #endregion
    }
}
