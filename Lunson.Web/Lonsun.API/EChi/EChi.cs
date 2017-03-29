using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Lonsun.API.EChi
{
    public class EChiHelper
    {
        public static string SendSMS(string tel, string content)
        {
            return SendSMS(tel, content, FormatType.MobileCheckCode);
        }
        public static string SendSMS(string tel, string content, FormatType formatType)
        {
            var result = SendSMSResult(tel, content, formatType);
            return result ? "短信发送成功" : "短信发送失败";
        }
        public static bool SendSMSResult(string tel, string content)
        {
            var result = SendSMSResult(tel, content, FormatType.MobileCheckCode);
            return result;
        }
        public static bool SendSMSResult(string tel, string content, FormatType formatType)
        {
            var sms = new YiChiSMS();
            var errorMessage = string.Empty;
            bool result = false;
            if (formatType == FormatType.MobileCheckCode)
            {
                result = sms.Send_IdentifyingCode(tel, content, out errorMessage);
            }
            else if (formatType == FormatType.BuySuccess)
            {   //此通道无法收到短信，不知为何
                var list = new List<string>();
                list.Add(tel);
                result = sms.Send_NormalSMS(list, content, out errorMessage);
            }
            return result;
        }
    }

    /// <summary>
    /// 短信内容格式
    /// </summary>
    public enum FormatType
    {
        /// <summary>
        /// 手机验证码
        /// </summary>
        MobileCheckCode = 0,

        /// <summary>
        /// 购买成功
        /// </summary>
        BuySuccess = 1
    }
}
