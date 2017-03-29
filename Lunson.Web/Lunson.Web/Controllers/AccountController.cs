using Lunson.BLL.Services;
using Lunson.Domain.Entities;
using Lunson.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Pharos.Framework;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using Pharos.Framework.MVC;
using Lonsun.API.EChi;

namespace Lunson.Web.Controllers
{
    public class AccountController : BaseController
    {
        private CustomerService csv = new CustomerService();

        #region 注册

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterVM register)
        {
            if (ModelState.IsValid)
            {
                var check = false;

                if (!register.Mobile.IsNullOrTrimEmpty())
                {
                    if (new Regex(@"^1[3|5|7|8|][0-9]{9}$").IsMatch(register.Mobile) == false)
                    {
                        ModelState.AddModelError("Mobile", "手机格式不正确");
                        return View(register);
                    }
                    if (csv.CheckMobile(register.Mobile, null) == false)
                    {
                        var dbCustomer = csv.GetCustomerByMobile(register.Mobile);
                        var defaultPwd = register.Mobile.Substring(5, 6);
                        if (dbCustomer.LoginTimes == 0 && dbCustomer.Password.Equals(SecurityHelper.GetMD5String(defaultPwd)))
                        {
                            ModelState.AddModelError("Mobile", "该手机有购票记录!密码初始化为手机后六位,请登录");
                            return View(register);
                        }
                        else
                        {
                            ModelState.AddModelError("Mobile", "该手机已注册!");
                            return View(register);
                        }
                    }
                    if (register.MobileCode.IsNullOrTrimEmpty() == false)
                    {
                        var mc = new PhoneCodeService().GetPhoneCodeByMobile(register.Mobile);
                        var match = false;
                        if (mc != null)
                        {
                            match = mc.Mobile.Equals(register.Mobile) && mc.Code.Equals(register.MobileCode);
                        }
                        if (match == false)
                        {
                            ModelState.AddModelError("MobileCode", "验证码不正确");
                            return View(register);
                        }
                    }
                    check = true;
                }

                if (check)
                {
                    var customer = csv.CreateCustomer(register.Mobile.ToEmptyString(), register.Password.ToEmptyString());
                    if (customer == null)
                        ModelState.AddModelError("", "您已注册，请登录！");
                    else
                    {
                        customerCache.SetCookie(customer.ID, customer.Mobile);
                        return RedirectToAction("RegisterSuccess");
                    }
                }
            }
            return View(register);
        }
        public ActionResult RegisterSuccess()
        {
            return View();
        }

        public ActionResult License()
        {
            return View();
        }
        public ActionResult Policy()
        {
            return View();
        }

        #endregion

        #region 登录
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(CustomerLoginVM login)
        {
            if (ModelState.IsValid)
            {
                // 是否为手机购票用户初次登陆（初始化密码为手机后六位）
                var dbCustomer = csv.GetCustomerByMobile(login.Mobile);
                if (dbCustomer != null)
                {
                    var defaultPwd = login.Mobile.Substring(5, 6);
                    if (dbCustomer.LoginTimes == 0 && dbCustomer.Password.Equals(SecurityHelper.GetMD5String(defaultPwd)))
                    {
                        return RedirectToAction("Forget");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "手机号或密码错误！");
                }


                var customer = csv.Login(login.Mobile, login.Password);
                if (customer != null)
                {

                    if (login.IsRememberMe)
                    {
                        customerCache.SetCookie(customer.ID, customer.DisplayName, DateTime.Now.AddYears(1));
                    }
                    else
                    {
                        customerCache.SetCookie(customer.ID, customer.DisplayName);
                    }
                    return RedirectToAction("Index", "BookLet");
                }
                else
                {
                    ModelState.AddModelError("Password", "手机号或密码错误！");
                }
            }
            return View(login);
        }
        public ActionResult LogOut()
        {
            customerCache.LogOut();
            return RedirectToAction("Login");
        }
        #endregion

        #region 我的账户
        [CheckLogin]
        public ActionResult Index()
        {
            var customer = csv.GetCustomerByID(customerCache.CustomerID);
            AccountVM accountVM = new AccountVM()
            {
                DisplayName = customer.DisplayName,
                Mobile = customer.Mobile,
                Email = customer.Email,
                Sex = customer.Sex,
                IsMobileCheck = customer.IsMobileCheck
            };
            ViewBag.CountDown = new PhoneCodeService().CountDown(customerCache.CustomerID);
            return View(accountVM);
        }

        /// <summary>
        /// 保存客户信息
        /// </summary>
        /// <param name="cusInfo"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckLogin]
        public ActionResult Index(AccountVM cusInfo)
        {
            Customer user;

            if (ModelState.IsValid)
            {
                if (!cusInfo.Mobile.IsNullOrTrimEmpty())
                {
                    if (new Regex(@"^1[3|5|7|8|][0-9]{9}$").IsMatch(cusInfo.Mobile) == false)
                    {
                        ModelState.AddModelError("Mobile", "手机格式不正确");
                        user = csv.GetCustomerByID(customerCache.CustomerID);
                        return View(user.ToObject<AccountVM>());
                    }
                    if (cusInfo.MobileCode.IsNullOrTrimEmpty() == false && csv.CheckMobileCode(cusInfo.Mobile, cusInfo.MobileCode, customerCache.CustomerID) == false)
                    {
                        ModelState.AddModelError("Mobile", "手机验证不通过");
                        user = csv.GetCustomerByID(customerCache.CustomerID);
                        return View(user.ToObject<AccountVM>());
                    }
                    if (csv.CheckMobile(cusInfo.Mobile, customerCache.CustomerID) == false)
                    {
                        ModelState.AddModelError("Mobile", "手机号码已存在");
                        user = csv.GetCustomerByID(customerCache.CustomerID);
                        return View(user.ToObject<AccountVM>());
                    }
                }
                Customer customer = new Customer()
                {
                    ID = customerCache.CustomerID,
                    DisplayName = cusInfo.DisplayName,
                    Mobile = cusInfo.Mobile,
                    Sex = cusInfo.Sex
                };
                if (csv.SaveCustomer(customer, cusInfo.MobileCode))
                {
                    ViewBag.msg = "保存成功";
                    customerCache.SetDisplayName(customer.DisplayName);
                }
            }

            user = csv.GetCustomerByID(customerCache.CustomerID);
            return View(user.ToObject<AccountVM>());
        }

        #region 修改密码

        /// <summary>
        /// 注册时获取手机验证码
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMobileCode(string mobile)
        {
            var code = new Random().Next(100000, 999999);
            if (new Regex(@"^1[3|5|7|8|][0-9]{9}$").IsMatch(mobile) == false)
            {
                return Json(new { msg = "手机格式不正确", success = false });
            }
            if (csv.CheckMobile(mobile, null) == false)
            {
                return Json(new { msg = "该手机已注册", success = false });
            }

            var pcs = new PhoneCodeService();

            var result = EChiHelper.SendSMSResult(mobile, string.Format("尊敬的用户，您的验证码为 {0},本验证码一小时内有效。", code), FormatType.MobileCheckCode);
            if (result)
            {
                pcs.CreatePhoneCode(mobile, code + "", "Unknown");
                return Json(new { success = true });
            }

            return Json(new { msg = "短信发送失败", success = false });
        }

        #endregion


        [CheckLogin]
        public ActionResult ModifyPwd(string curPwd, string newPwd, string ConfirmPwd)
        {
            Regex regex = new Regex(@"^[^ ]{6,20}$");
            if (csv.CheckPassword(customerCache.CustomerID, curPwd) == false)
            {

                return Json(new { success = false, message = "原密码不正确", target = "CurPwdMsg" });
            }
            else
            {
                if (newPwd != ConfirmPwd)
                {
                    return Json(new { success = false, message = "两次输入的密码不一致", target = "ConfirmPwdMsg" });
                }
                else
                {
                    if (regex.IsMatch(newPwd) == false)
                    {
                        return Json(new { success = false, message = "不能含有空格，长度6-20位", target = "NewPwdMsg" });
                    }
                    else
                    {
                        var result = csv.ModifyPassword(customerCache.CustomerID, newPwd);
                        return Json(new { success = result, message = (result == true) ? "修改成功" : "修改失败", targer = "" });
                    }
                }
            }
        }
        public ActionResult Forget()
        {
            return View();
        }

        /// <summary>
        /// 发送手机验证码（用于前台 忘记密码页）
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public ActionResult SendMobileCode(string mobile)
        {
            mobile = mobile.Trim();
            var code = new Random().Next(100000, 999999);
            if (new Regex(@"^1[3|5|7|8|][0-9]{9}$").IsMatch(mobile) == false)
            {
                return Json(new { msg = "手机格式不正确", success = false });
            }

            var pcsv = new PhoneCodeService();
            var phoneCode = pcsv.GetPhoneCodeByMobile(mobile);
            if (phoneCode != null && (DateTime.Now - phoneCode.SendTime).TotalMinutes < 1)
            {
                return Json(new { msg = "短信已发送，发送时间为：" + phoneCode.SendTime.ToString("yyyy-MM-dd HH:mm:ss") + "，请间隔1分钟再发送", success = false });
            }

            var exist = pcsv.PhoneBinded(mobile, code + "");
            if (!exist)
            {
                return Json(new { msg = "该手机号未绑定账户，请确认您的手机号", success = false });
            }
            else
            {
                var result = EChiHelper.SendSMSResult(mobile, string.Format("尊敬的用户，您的验证码为 {0},本验证码一小时内有效。", code), FormatType.MobileCheckCode);

                //var result = true; //测试用

                if (result)
                {
                    Session["SendTime"] = DateTime.Now;

                    return Json(new { msg = "短信发送成功，一小时内有效", success = true });
                }
            }
            return Json(new { msg = "短信发送失败", success = false });
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="code"></param>
        /// <param name="newPwd"></param>
        /// <param name="confirmPwd"></param>
        /// <returns></returns>
        public ActionResult ResetPassword(string mobile, string code, string newPwd, string confirmPwd)
        {
            Regex regex = new Regex(@"^[^ ]{6,20}$");

            if (newPwd != confirmPwd)
            {
                return Json(new { success = false, message = "两次输入的密码不一致" });
            }
            else
            {
                if (regex.IsMatch(newPwd) == false)
                {
                    return Json(new { success = false, message = "密码不能含有空格，长度6-28位" });
                }
                else
                {
                    var result = csv.ResetPassword(mobile.Trim(), code, newPwd);
                    if (result.Successed == false)
                    {
                        return Json(new { success = false, message = result.Message });
                    }
                }
            }
            return Json(new { success = true });
        }
        #endregion

        #region 客户交易记录

        [CheckLogin]
        public ActionResult TradeLog()
        {
            var sortExpression = Request.Params["sort"];
            switch (sortExpression)
            {
                case "buy":
                    sortExpression = "购买";
                    break;
                case "refund":
                    sortExpression = "退款";
                    break;
                default:
                    sortExpression = "";
                    break;
            }
            var model = csv.GetTradeLogByCustomerID("D5E3F7C66855462580D78A9271AEF08B", sortExpression);
            return View(model);
        }

        #endregion
    }
}
