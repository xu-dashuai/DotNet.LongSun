using Lonsun.API.AliWapPay;
using Lunson.BLL.Services;
using Lunson.Domain;
using Lunson.Domain.Entities;
using Lunson.Web.Models;
using Pharos.Framework;
using Pharos.Framework.MVC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Lunson.Web.Controllers
{
    public class MobilePayController : Controller
    {
        MobilePayService mpsv = new MobilePayService();
        TicketService tsv = new TicketService();
        OrderService osv = new OrderService();

        #region 购票选择页
        public ActionResult Index()
        {
            var data = tsv.GetAvailableIncludeTickets().ToList();
            return View(data);
        }

        #endregion

        #region 购票确认页
        public ActionResult Confirm(string mobile, string ID, string Num = "1")
        {
            #region 测试专用代码
            if (mobile.EndsWith("dc2014", true, null))
            {
                mobile = mobile.Substring(0, 11);
                ViewBag.Mobile = mobile;
                return View(tsv.GetParticularTicketCart<TicketDetailVM>());
            }
            #endregion

            var dic = DataHelper.ToDic<int>(ID, Num, ',');
            if (dic.Any())
            {
                ViewBag.Mobile = mobile;
                return View(tsv.GetTicketCart<TicketDetailVM>(dic));
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region 添加订单
        /// <summary>
        /// 添加订单，统一为用户：lonsun.com.cn
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="nums"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddOrder(string ids, string nums, string mobile)
        {
            CustomerService csv = new CustomerService();
            var customer = csv.GetCustomerByMobile(mobile);
            if (customer == null)
            { //创建新账户。用户名：手机号；密码：初始为手机后六位
                customer = csv.CreateCustomer(mobile, mobile.Substring(5, 6));
            }
            //否则，使用注册过的账户

            var dic = DataHelper.ToDic<int>(ids, nums, ',');

            var result = osv.CreateOrder(dic, customer.ID);
            return Json(result);
        }
        #endregion

        #region 支付

        #region 支付宝
        public ActionResult AliPay(string no)
        {
            var order = osv.GetOrderByOrderNo(no);
            if (order != null && order.OrderStatus == OrderStatus.WaitPay)
            {
                #region 支付宝
                ////////////////////////////////////////////请求参数////////////////////////////////////////////
                //返回格式
                string format = "xml";
                //必填，不需要修改

                //返回格式
                string v = "2.0";
                //必填，不需要修改

                //请求号
                string req_id = DateTime.Now.ToString("yyyyMMddHHmmss");
                //必填，须保证每次请求都是唯一

                //支付宝网关地址
                string GATEWAY_NEW = "http://wappaygw.alipay.com/service/rest.htm?";
                //订单号
                string out_trade_no = order.OrderNo;
                //订单名称
                string subject = order.Num + "张鳄鱼园门票";
                //付款金额
                string total_fee = order.AllPrice + "";
                //订单描述        
                // string body = string.Join("\r\n", order.OrderDetails.Select(a => a.Name + "：" + a.Num + "张" + a.AllPrice + "元"));
                //商品展示地址
                string show_url = "";
                //防钓鱼时间戳
                string anti_phishing_key = Submit.Query_timestamp();
                //操作中断返回地址
                string merchant_url = "http://" + Request.Url.Authority + "/MobilePay";
                //用户付款中途退出返回商户的地址。需http://格式的完整路径，不允许加?id=123这类自定义参数

                //请求业务参数详细
                string req_dataToken = "<direct_trade_create_req><notify_url>" + ConfigurationManager.AppSettings["alipay_mobile_notify"] + "</notify_url><call_back_url>" + ConfigurationManager.AppSettings["alipay_mobile_return"] + "</call_back_url><seller_account_name>" + Config.Seller_Email + "</seller_account_name><out_trade_no>" + out_trade_no + "</out_trade_no><subject>" + subject + "</subject><total_fee>" + total_fee + "</total_fee><merchant_url>" + merchant_url + "</merchant_url></direct_trade_create_req>";
                //必填


                //把请求参数打包成数组
                Dictionary<string, string> sParaTempToken = new Dictionary<string, string>();
                sParaTempToken.Add("partner", Config.Partner);
                sParaTempToken.Add("_input_charset", Config.Input_charset.ToLower());
                sParaTempToken.Add("sec_id", Config.Sign_type.ToUpper());
                sParaTempToken.Add("service", "alipay.wap.trade.create.direct");
                sParaTempToken.Add("format", format);
                sParaTempToken.Add("v", v);
                sParaTempToken.Add("req_id", req_id);
                sParaTempToken.Add("req_data", req_dataToken);


                //建立请求
                string sHtmlTextToken = Submit.BuildRequest(GATEWAY_NEW, sParaTempToken);
                //URLDECODE返回的信息
                Encoding code = Encoding.GetEncoding(Config.Input_charset);
                sHtmlTextToken = HttpUtility.UrlDecode(sHtmlTextToken, code);

                //解析远程模拟提交后返回的信息
                Dictionary<string, string> dicHtmlTextToken = Submit.ParseResponse(sHtmlTextToken);

                //获取token
                string request_token = dicHtmlTextToken["request_token"];

                ////////////////////////////////////////////根据授权码token调用交易接口alipay.wap.auth.authAndExecute////////////////////////////////////////////


                //业务详细
                string req_data = "<auth_and_execute_req><request_token>" + request_token + "</request_token></auth_and_execute_req>";
                //必填

                //把请求参数打包成数组
                Dictionary<string, string> sParaTemp = new Dictionary<string, string>();
                sParaTemp.Add("partner", Config.Partner);
                sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
                sParaTemp.Add("sec_id", Config.Sign_type.ToUpper());
                sParaTemp.Add("service", "alipay.wap.auth.authAndExecute");
                sParaTemp.Add("format", format);
                sParaTemp.Add("v", v);
                sParaTemp.Add("req_data", req_data);

                //建立请求
                string sHtmlText = Submit.BuildRequest(GATEWAY_NEW, sParaTemp, "get", "确认");

                ViewBag.html = sHtmlText;

                return View();
                #endregion
            }
            return null;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AlipayNotify()
        {
            try
            {
                AddPayLog(true);
            }
            catch (Exception e)
            {
                PayLogService ps = new PayLogService();
                PayLog pl = new PayLog();
                pl = new PayLog();
                pl.ID = DataHelper.GetSystemID();
                pl.LogTime = DateTime.Now;
                pl.Message = e.Message;
                ps.AddLog(pl);
            }

            Dictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.VerifyNotify(sPara, Request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    //获取支付宝的通知返回参数，可参考技术文档中服务器异步通知参数列表

                    //解密（如果是RSA签名需要解密，如果是MD5签名则下面一行清注释掉）
                    //sPara = aliNotify.Decrypt(sPara);

                    //XML解析notify_data数据
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(sPara["notify_data"]);
                        //商户订单号
                        string out_trade_no = xmlDoc.SelectSingleNode("/notify/out_trade_no").InnerText;
                        //支付宝交易号
                        string trade_no = xmlDoc.SelectSingleNode("/notify/trade_no").InnerText;
                        //交易状态
                        string trade_status = xmlDoc.SelectSingleNode("/notify/trade_status").InnerText;

                        if (trade_status == "TRADE_FINISHED")
                        {
                            //判断该笔订单是否在商户网站中已经做过处理
                            //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                            //如果有做过处理，不执行商户的业务程序
                            osv.FinishOrder(out_trade_no, trade_no, PayType.Alipay);
                            //注意：
                            //该种交易状态只在两种情况下出现
                            //1、开通了普通即时到账，买家付款成功后。
                            //2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。

                            return Content("success");
                        }
                        else if (trade_status == "TRADE_SUCCESS")
                        {
                            //判断该笔订单是否在商户网站中已经做过处理
                            //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                            //如果有做过处理，不执行商户的业务程序
                            osv.FinishOrder(out_trade_no, trade_no, PayType.Alipay);
                            //注意：
                            //该种交易状态只在一种情况下出现——开通了高级即时到账，买家付款成功后。

                            return Content("success");
                        }
                        else
                        {
                            return Content(trade_status);
                        }

                    }
                    catch (Exception exc)
                    {
                        return Content(exc.ToString());
                    }

                }
                else//验证失败
                {
                    return Content("fail");
                }
            }
            else
            {
                return Content("无通知参数");
            }
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult AlipayReturn()
        {
            //记录返回url
            try
            {
                AddPayLog(false);
            }
            catch (Exception e)
            {
                PayLogService ps = new PayLogService();
                PayLog pl = new PayLog();
                pl = new PayLog();
                pl.ID = DataHelper.GetSystemID();
                pl.LogTime = DateTime.Now;
                pl.Message = e.Message;
                ps.AddLog(pl);
            }

            Dictionary<string, string> sPara = GetRequestGet();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.VerifyReturn(sPara, Request.QueryString["sign"]);

                if (verifyResult)//验证成功
                {
                    //获取支付宝的通知返回参数，可参考技术文档中页面跳转同步通知参数列表

                    //商户订单号
                    string orderNo = Request.QueryString["out_trade_no"];

                    //支付宝交易号
                    string trade_no = Request.QueryString["trade_no"];

                    //交易状态
                    string result = Request.QueryString["result"];
                    //result :判断支付结果及交易状态 result有且只有success一个交易状态

                    //判断是否在商户网站中已经做过了这次通知返回的处理
                    //如果没有做过处理，那么执行商户的业务程序
                    //如果有做过处理，那么不执行商户的业务程序

                    //交易成功
                    osv.FinishOrder(orderNo, trade_no, PayType.Alipay);
                    return RedirectToAction("MyTickets", new { orderNo = orderNo });
                }
                else//验证失败
                {
                    return Content("验证失败");
                }
            }
            else
            {
                return Content("无返回参数");
            }
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public Dictionary<string, string> GetRequestPost()
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public Dictionary<string, string> GetRequestGet()
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

        #endregion

        #region 微信
        #endregion

        #region 支付日志
        public void AddPayLog(bool isNotify)
        {
            PayLog log = new PayLog();
            log.ID = DataHelper.GetSystemID();
            log.OrderNo = Request.QueryString["out_trade_no"];
            if (isNotify)
            {
                log.Message = Request.Form.ToString();
            }
            else
            {
                log.Message = Request.Url.ToString(); ;
            }
            log.LogTime = DateTime.Now;
            PayLogService ps = new PayLogService();
            ps.AddLog(log);
        }
        #endregion

        #endregion

        #region 购买成功显示门票
        public ActionResult MyTickets(string orderNo)
        {
            var query = mpsv.GetTicketNosByOrderNo(orderNo);
            var result = query.Where(a => a.IsUsed == YesOrNo.No);
            ViewBag.UsedTickets = query.Where(a => a.IsUsed == YesOrNo.Yes);
            return View(result);
        }
        #endregion

        #region 查询购票历史
        public ActionResult History()
        {
            return View();
        }

        [HttpPost]
        public ActionResult History(string mobile, string code)
        {
            var result = mpsv.GetTicketNosByMobile(mobile, true);
            return View(result);
        }

        [HttpPost]
        public ActionResult CheckCode(string mobile, string code)
        {
            var customer = new CustomerService().GetCustomerByMobile(mobile);
            if (customer == null)
            {
                return Json(new { match = false, msg = "请确认您的手机" });
            }
            var match = new PhoneCodeService().CheckMobileCode(mobile, code);

            if (match == false)
            {
                // "验证码不正确";
                return Json(new { match = match, msg = "验证码不正确" });
            }
            var tickets = mpsv.GetTicketNosByMobile(mobile, true);
            if (tickets.Any() == false)
            {
                return Json(new { match = false, msg = "查无购票记录，快来买票吧！" });
            }

            return Json(new { match = true });
        }
        #endregion

        public ImageResult QR(string id)
        {
            var detailNos = id.Split(',').Where(a => a.IsNullOrTrimEmpty() == false).Distinct().ToList();

            if (detailNos.Any())
            {
                QRCodeHelper.GetQRCode(string.Join(",", detailNos.Take(5)), Response);
            }

            return new ImageResult();
        }


    }
}
