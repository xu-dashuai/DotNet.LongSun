/********************************************
 * Create By：zsj
 * Create Time：2015/5/11 16:44:55
 * Function：易驰技术短信供应商[3分钟内限制5条]，联系人：小吴，wrk@echisoft.com，QQ：1154426180，企业QQ800082810，联系电话：4006592001。
 * Modify Course：
 2015/5/11 16:44:55   zsj    创建文件
 *******************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using Lonsun.API.EChi.Interface;
using Lonsun.API.EChi.Utils;
namespace Lonsun.API.EChi
{
    public class YiChiSMS : ISMS
    {

        #region ISMS 成员

        #region 验证码通道
        /// <summary>
        /// 验证码通道url
        /// </summary>
        private string indentifying_url = "http://115.29.47.151:8860/";

        /// <summary>
        /// 验证码通道用户账号
        /// </summary>
        private string indentifying_cust_code = "600025";
        /// <summary>
        /// 验证码通道密码
        /// </summary>
        private string indentifying_pwd = "9AKIOLFH7A";
        #endregion


        #region 正常通道
        /// <summary>
        /// 正常通道url
        /// </summary>
        private string normal_url = "http://api.sojisms.com:8082/";
        /// <summary>
        /// 正常通道账户
        /// </summary>
        private string normal_userName = "600025";
        /// <summary>
        /// 正常通道密码
        /// </summary>
        private string normal_pwd = "9AKIOLFH7A";
        #endregion

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="mobile">接收手机，多个用都号隔开</param>
        /// <param name="content">发送内容</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public bool Send_IdentifyingCode(string mobiles, string content, out string errorMessage)
        {
            bool status = false;
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(content)) { return false; }
            if (!string.IsNullOrEmpty(mobiles))
            {
                content = Common.SMSSignContent + content;

                string sendContent_UrlEndcode = HttpContext.Current.Server.UrlEncode(content);
                //cust_code=570061&sp_code=1234&content=%E5%8F%91%E9%80%81%E7%9F%AD%E4%BF%A1%E4%B8%8B%E8%A1%8C%E6%B5%8B%E8%AF%&destMobiles=手机号码,手机号码,手机号码&sign=fa246d0262c3925617b0c72bb20eeb1d
                //签名内容根据 “短信内容URL编码后+客户密码”进行MD5编码后获得。
                string sign = Common.MD5(sendContent_UrlEndcode + indentifying_pwd);
                string requestUrl = indentifying_url + "?cust_code=" + indentifying_cust_code + "&content=" + sendContent_UrlEndcode + "&destMobiles=" + mobiles + "&sign=" + sign;
                string return_content = Common.GetPageResponseContent(requestUrl, "get");
                return_content = HttpContext.Current.Server.UrlDecode(return_content);
                if (!string.IsNullOrEmpty(return_content))
                {
                    //成功时返回：SUCCESS:提交成功！\r\n13860119615:59106105111751472327:0\r\n15280212623:59106105111751472328:0\r\n
                    //失败时返回：ERROR:签名验证不通过！
                    if (return_content.StartsWith("SUCCESS"))
                    {
                        status = true;
                    }
                }
            }
            return status;
        }
        /// <summary>
        /// 获取短信通道余额
        /// </summary>
        /// <returns></returns>
        public string GetBalance_IdentifyingCode()
        {
            string result = "";
            string requestTokenUrl = indentifying_url + "?action=GetToken&cust_code=" + indentifying_cust_code + "";
            string tokenResult = Common.GetPageResponseContent(requestTokenUrl, "get");
            if (!string.IsNullOrEmpty(tokenResult))
            {
                string[] tokenResultArray = tokenResult.Split(',');
                if (tokenResultArray.Length > 0)
                {
                    //2015-05-20 改成返回如下测试， TOKEN_ID:v35mfowvqg,TOKEN:q5w2iq9hujv2a9t5apqd
                    string[] tokenIdArray = tokenResultArray[0].Split(':');
                    string tokenId = tokenIdArray[1];
                    string[] tokenValueArray = tokenResultArray[1].Split(':');
                    string tokenValue = tokenValueArray[1];
                    string sign = Common.MD5(tokenValue + indentifying_pwd);
                    string requestUrl = indentifying_url + "?action=QueryAccount&cust_code=" + indentifying_cust_code + "&token_id=" + tokenId + "&sign=" + sign + "";
                    result = Common.GetPageResponseContent(requestUrl, "get");
                    result = HttpContext.Current.Server.UrlDecode(result);
                }
                if (string.IsNullOrEmpty(result))
                {
                    result = "获取余额失败";
                }
                else
                {
                    //结果：cust_code:600003,status:1,sms_balance:191
                    string[] result_Array = result.Split(',');
                    if (result_Array != null)
                        foreach (string resultValue in result_Array)
                        {
                            string[] keyValue = resultValue.Split(':');
                            if (keyValue.Length > 1)
                            {
                                if (string.Compare(keyValue[0], "sms_balance", true) == 0)
                                {
                                    result = keyValue[1];
                                    break;
                                }
                            }
                        }
                }
            }
            else
            {
                result = "获取token失败";
            }
            return result;
        }
        #endregion

        #region 正常通道(营销+关怀短信)
        /// <summary>
        /// 发送正常短信[营销+关怀短信]
        /// </summary>
        /// <param name="mobiles">要发送的手机号列表，多于100个自动分批次发送</param>
        /// <param name="content"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool Send_NormalSMS(List<string> mobiles, string content, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (mobiles != null && mobiles.Count > 0)
            {
                //手机号码，80个一组，用英文状态下的逗号分隔。一次提交最多100个手机，这里就限制一次只提交80个
                string strMobiles = string.Empty;
                int sendOneTime = 80;//一次发送几个号码
                int errorCount = 0;
                for (int i = 0; i < mobiles.Count; i++)
                {
                    if (!string.IsNullOrEmpty(strMobiles))
                    {
                        strMobiles += ",";
                    }
                    strMobiles += mobiles[i].Trim();
                    if ((i + 1) % sendOneTime == 0)
                    {
                        bool isSuccess = Send_NormalSMS(strMobiles, content, out errorMessage);
                        //bool isSuccess = true;
                        if (!isSuccess)
                        {
                            errorCount++;
                        }
                        strMobiles = string.Empty;
                    }
                }
                if (!string.IsNullOrEmpty(strMobiles))
                {
                    bool isSuccess = Send_NormalSMS(strMobiles, content, out errorMessage);
                    //bool isSuccess = true;
                    if (!isSuccess)
                    {
                        errorCount++;
                    }
                }
                if (errorCount <= 0) { return true; }
            }
            return false;
        }
        /// <summary>
        /// 发送正常短信[营销+关怀短信]
        /// </summary>
        /// <param name="mobiles">要发送的手机号，多个用,隔开，不能超过100个</param>
        /// <param name="content"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool Send_NormalSMS(string mobiles, string content, out string errorMessage)
        {
            //todo:mobiles超过100要分批次发。
            errorMessage = "";
            bool status = false;
            content += Common.SMSSignContent;//加后缀签名文字
            //string sendContent_UrlEndcode = HttpContext.Current.Server.UrlEncode(content);
            string sendContent_UrlEndcode = System.Web.HttpUtility.UrlEncode(content, System.Text.Encoding.GetEncoding("gb2312"));//短信内容gb2312编码
            string requestUrl = normal_url + "sendsms.aspx?suser=" + normal_userName + "&spass=" + normal_pwd + "&telnum=" + mobiles + "&nr=" + sendContent_UrlEndcode + "";
            string return_content = Common.GetPageResponseContent(requestUrl, "get");
            return_content = HttpContext.Current.Server.UrlDecode(return_content);
            switch (return_content)
            {
                case "0":
                    status = true;
                    break;
                case "-1":
                    errorMessage = "没有用户存在，用户名或密码错误";
                    break;
                case "-2":
                    errorMessage = "金额不足";
                    break;
                case "-4":
                    errorMessage = "号码个数超过100个";
                    break;
                case "-5":
                    errorMessage = "账号密码参数不完整";
                    break;
                case "-6":
                    errorMessage = "没有此项功能";
                    break;
                case "-7":
                    errorMessage = "网络故障";
                    break;
                case "-8":
                    errorMessage = "内容含有关键字";
                    break;
                case "-9":
                    errorMessage = "通道要求内容加签名，签名格式：【***】，（短信内容以【***】做结尾）。";
                    break;
            }
            return status;

        }
        /// <summary>
        /// 获取正常通道余额
        /// </summary>
        /// <returns></returns>
        public string GetBalance_Normal()
        {
            string requestUrl = normal_url + "apisms.aspx?suser=" + normal_userName + "&spass=" + normal_pwd + "&kind=getprice";
            string return_content = Common.GetPageResponseContent(requestUrl, "get");
            return_content = HttpContext.Current.Server.UrlDecode(return_content);
            return return_content;
        }
        #endregion
    }
}
