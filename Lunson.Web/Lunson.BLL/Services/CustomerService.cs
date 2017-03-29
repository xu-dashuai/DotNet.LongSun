using Lunson.DAL.Repositories;
using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using Pharos.Framework.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.BLL.Services
{
    public class CustomerService
    {
        private CustomerRepository DAL = new CustomerRepository();

        public Customer GetCustomerByID(string id)
        {
            return DAL.GetCustomer(id);
        }
        public Customer GetCustomer(string mobile, string password)
        {
            return DAL.GetCustomerByPassword(mobile, password);
        }
        public Customer GetCustomerByName(string name)
        {
            return DAL.GetCustomerByName(name);
        }
        public Customer GetCustomerByMobile(string mobile)
        {
            return DAL.GetCustomerByMobile(mobile);
        }

        public Customer CreateCustomer(string mobile, string password)
        {
            var customer = GetCustomerByMobile(mobile);
            if (customer != null)
                return null;

            customer = new Customer
            {
                IsDisabled = YesOrNo.No,
                Status = ActiveState.Normal,
                Mobile = mobile,
                IP = DataHelper.GetIP(),
                IsEmailCheck = YesOrNo.No,
                IsMobileCheck = YesOrNo.Yes,
                LoginTimes = 0,
                Password = SecurityHelper.GetMD5String(password),
                Sex = Sex.Secret,
                DisplayName = mobile
            };

            DAL.SaveInfo(customer, "CreateUser");
            DAL.Submit();

            return customer;
        }

        public Customer Login(string mobile, string password)
        {
            var customer = GetCustomer(mobile, SecurityHelper.GetMD5String(password));
            if (customer != null)
                SaveLoginLog(customer);
            return customer;
        }
        public void SaveLoginLog(Customer customer)
        {
            customer.IP = DataHelper.GetIP();
            customer.LastLoginTime = DateTime.Now;
            customer.LoginTimes = (customer.LoginTimes ?? 0) + 1;

            var log = new CustomerLoginLog
            {
                CustomerID = customer.ID,
                IP = DataHelper.GetIP(),
                LoginTime = DateTime.Now
            };

            DAL.SaveInfo(log, customer.ID);
            DAL.Submit();
        }

        public bool SaveCustomer(Customer customer, string mobileCode)
        {
            var cus = GetCustomerByID(customer.ID);

            if (cus.Mobile.Equals(customer.Mobile) == false)
                cus.IsMobileCheck = YesOrNo.No;

            if (customer.Mobile.IsNullOrTrimEmpty() == false)
            {
                if (CheckMobile(customer.Mobile, customer.ID) == false)
                    return false;
            }

            cus.DisplayName = customer.DisplayName;
            cus.Mobile = customer.Mobile;
            cus.Sex = customer.Sex;

            if (mobileCode.IsNullOrTrimEmpty() == false)
            {
                var pcs = new PhoneCodeService();
                var code = pcs.GetPhoneCodeByCID(customer.ID);
                if (code != null)
                {
                    if (customer.Mobile.Equals(code.Mobile) && mobileCode.Equals(code.Code))
                    {
                        cus.IsMobileCheck = YesOrNo.Yes;
                        pcs.UsePhoneCode(customer.ID);
                    }
                }
            }

            DAL.SaveInfo(cus, customer.ID);
            return DAL.Submit() > 0;
        }

        /// <summary>
        /// 验证手机唯一
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public bool CheckMobile(string mobile, string customerID)
        {
            if (customerID.IsNullOrTrimEmpty())
                return DAL.GetQueryInfo<Customer>().Any(a => a.Mobile.Equals(mobile)) == false;
            else
                return DAL.GetQueryInfo<Customer>().Any(a => a.Mobile.Equals(mobile)&&a.ID.Equals(customerID)==false) == false;
        }
        public bool CheckMobileCode(string mobile, string code, string customerID)
        {
            var mc = new PhoneCodeService().GetPhoneCodeByCID(customerID);
            if (mc != null)
            {
                return mc.Code.Equals(code) && mc.Mobile.Equals(mobile);
            }
            return false;
        }
        public bool ModifyPassword(string customerID, string newPwd)
        {
            DAL.Modify<Customer>(customerID, c =>
            {
                c.Password = SecurityHelper.GetMD5String(newPwd);
            }, customerID);

            return DAL.Submit() > 0;
        }
        public bool CheckPassword(string customerID, string pwd)
        {
            var customer = GetCustomerByID(customerID);
            if (customer != null)
                return customer.Password.Equals(SecurityHelper.GetMD5String(pwd));
            return false;
        }

        /// <summary>
        /// 忘记密码，重置密码
        /// </summary>
        /// <param name="newPwd"></param>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public OpResult ResetPassword(string mobile, string code, string newPwd)
        {
            var mc = new PhoneCodeService().GetPhoneCodeByMobile(mobile);
            var match = false;
            if (mc != null)
            {
                match = mc.Mobile.Equals(mobile) && mc.Code.Equals(code);
            }
            if (match == false)
            {
                return new OpResult { Successed = false, Message="验证码不匹配，请重新确认"};
            }
            else
            {
                var result = DAL.ResetPassword(SecurityHelper.GetMD5String(newPwd), mobile);
                if (result)
                {
                    return new OpResult { Successed = true };
                }
            }
            return new OpResult { Successed = false, Message = "重置密码失败" };
        }


        public List<TradeLog> GetTradeLogByCustomerID(string id, string sortExpression)
        {
            return DAL.GetTradeLogByCustomerID(id, sortExpression).ToList();
        }

    }
}
