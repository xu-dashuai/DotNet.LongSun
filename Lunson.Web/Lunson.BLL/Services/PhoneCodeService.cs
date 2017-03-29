using Lunson.DAL.Repositories;
using Lunson.Domain;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.BLL.Services
{
    public class PhoneCodeService
    {
        public PhoneCodeRepository DAL = new PhoneCodeRepository();

        /// <summary>
        /// 取出验证码 2分钟发送间隔 1小时有效 取最近一个
        /// </summary>
        /// <param name="customerID">客户ID</param>
        /// <returns></returns>
        public PhoneCode GetPhoneCodeByCID(string customerID)
        {
            var validTime = 60; //有效期，单位为分钟
            var codes = DAL.GetQueryInfo<PhoneCode>().Where(a => a.CustomerID.Equals(customerID));
            if (codes.Any())
            {
                var code = codes.OrderByDescending(a => a.SendTime).First();
                if ((DateTime.Now - code.SendTime).TotalMinutes < validTime)
                {
                    return code;
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 取出验证码 60分钟有效(客户无登录)
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <returns></returns>
        public PhoneCode GetPhoneCodeByMobile(string mobile)
        {
            var validTime = 60; //有效期，单位为分钟
            var codes = DAL.GetQueryInfo<PhoneCode>().Where(a => a.Mobile.Equals(mobile));
            if (codes.Any())
            {
                var code = codes.OrderByDescending(a => a.SendTime).First();
                if ((DateTime.Now - code.SendTime).TotalMinutes < validTime)
                {
                    return code;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 验证手机验证码是否正确
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckMobileCode(string mobile, string code)
        {
            var mc = GetPhoneCodeByMobile(mobile);
            if (mc != null)
            {
                return mc.Code.Equals(code);
            }
            return false;
        }

        /// <summary>
        /// 使用验证码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public int UsePhoneCode(string customerID)
        {
            var codes = DAL.GetQueryInfo<PhoneCode>().Where(a => a.CustomerID.Equals(customerID)).ToList();
            foreach (var x in codes)
            {
                DAL.RemoveInfo<PhoneCode>(x, customerID);
            }
            return DAL.Submit();
        }
        /// <summary>
        /// 存储手机验证码记录
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <param name="customerID"></param>
        public void CreatePhoneCode(string mobile, string code, string customerID)
        {
            var pc = new PhoneCode
            {
                Code = code,
                CustomerID = customerID,
                SendTime = DateTime.Now,
                Mobile = mobile,
            };
            DAL.SaveInfo<PhoneCode>(pc, customerID);
            DAL.Submit();
        }
        /// <summary>
        /// 判断手机是否绑定过 存储手机验证码记录（客户无登录）
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <returns>是否存在 已用该手机号绑定的客户</returns>
        public bool PhoneBinded(string mobile, string code)
        {
            var customer = DAL.GetQueryInfo<Customer>().SingleOrDefault(a => a.Mobile.Equals(mobile) && a.IsMobileCheck == YesOrNo.Yes);
            if (customer == null)
                return false;
            CreatePhoneCode(mobile, code, customer.ID);
            return true;
        }

        /// <summary>
        /// 获得手机验证码重新发送倒计时
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns>剩余时间 2分钟额度</returns>
        public double CountDown(string customerID)
        {
            var codes = DAL.GetQueryInfo<PhoneCode>().Where(a => a.CustomerID.Equals(customerID));
            if (codes.Any())
            {
                var code = codes.OrderByDescending(a => a.SendTime).First();
                var countDown = (DateTime.Now - code.SendTime).TotalSeconds;
                if (countDown < 120)
                {
                    return 120 - countDown;
                }
            }
            return -1;
        }
    }
}
