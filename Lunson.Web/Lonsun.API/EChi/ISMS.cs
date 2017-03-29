/********************************************
 * Create By：zsj
 * Create Time：2015/5/11 19:08:47
 * Function：功能说明
 * Modify Course：
 2015/5/11 19:08:47   zsj    创建文件
 *******************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lonsun.API.EChi.Interface
{
    public interface ISMS
    {
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="mobile">接收手机，多个用都号隔开</param>
        /// <param name="content">发送内容</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        bool Send_IdentifyingCode(string mobiles, string content, out string errorMessage);
        /// <summary>
        /// 获取验证码通道余额
        /// </summary>
        /// <returns></returns>
        string GetBalance_IdentifyingCode();

        /// <summary>
        /// 发送正常短信[营销+关怀短信]
        /// </summary>
        /// <param name="mobiles"></param>
        /// <param name="content"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool Send_NormalSMS(List<string> mobiles, string content, out string errorMessage);
        /// <summary>
        /// 获取正常通道余额
        /// </summary>
        /// <returns></returns>
        string GetBalance_Normal();
    }
}
