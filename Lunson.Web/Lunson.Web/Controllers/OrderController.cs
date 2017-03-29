using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lunson.BLL.Services;
using Pharos.Framework.MVC;
using Pharos.Framework;
using Lunson.Domain;
using Lonsun.API.AliDirectPay;
using System.Collections.Specialized;
using System.Configuration;
using Lonsun.API.EChi;


namespace Lunson.Web.Controllers
{
    public class OrderController : BaseController
    {
        OrderService osv = new OrderService();

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddOrder(string id, string num)
        {
            if (customerCache.IsLogin == false)
                return Json(new OpResult { Successed=false,Message="请登录后再购买" });

            var dic = DataHelper.ToDic<int>(id,num,',');

            var result = osv.CreateOrder(dic, customerCache.CustomerID);
            return Json(result);
        }

        /// <summary>
        /// 阿里支付
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [CheckLogin]
        public ActionResult Alipay(string id)
        {
            var order = osv.GetOrderIncludeByOrderNo(id, "OrderDetails");
            if (order != null && order.OrderStatus == OrderStatus.WaitPay&&order.CustomerID.Equals(customerCache.CustomerID))
            {
                #region 支付宝
                ////////////////////////////////////////////请求参数////////////////////////////////////////////

                //订单号
                string out_trade_no = order.OrderNo;
                //订单名称
                string subject = order.Num+"张鳄鱼园门票";
                //付款金额
                string total_fee = order.AllPrice + "";
                //订单描述        
                // string body = string.Join("\r\n", order.OrderDetails.Select(a => a.Name + "：" + a.Num + "张" + a.AllPrice + "元"));
                //商品展示地址
                string show_url = "";
                //防钓鱼时间戳
                string anti_phishing_key = Submit.Query_timestamp();


                ////////////////////////////////////////////////////////////////////////////////////////////////

                //把请求参数打包成数组
                SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();
                sParaTemp.Add("partner", Config.Partner);
                sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
                sParaTemp.Add("service", "create_direct_pay_by_user");
                sParaTemp.Add("payment_type", "1");
                sParaTemp.Add("notify_url", ConfigurationManager.AppSettings["alipay_notify"]);
                sParaTemp.Add("return_url", ConfigurationManager.AppSettings["alipay_return"]);
                sParaTemp.Add("seller_email", Config.Seller_Email);
                sParaTemp.Add("out_trade_no", out_trade_no);
                sParaTemp.Add("subject", subject);
                sParaTemp.Add("total_fee", total_fee);
                //sParaTemp.Add("body", body);
                sParaTemp.Add("show_url", show_url);
                sParaTemp.Add("anti_phishing_key", anti_phishing_key);
                sParaTemp.Add("exter_invoke_ip", ConfigurationManager.AppSettings["alipay_ip"]);

                //建立请求
                string sHtmlText = Submit.BuildRequest(sParaTemp, "post", "确认");

                ViewBag.html = sHtmlText;

                return View();



                #endregion
            }
            return Redirect("/ticket");
        }

        public ActionResult AlipayNotify()
        {
            SortedDictionary<string, string> sPara = GetRequestPost();

            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.Form["notify_id"], Request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    //商户订单号
                    string out_trade_no = Request.Form["out_trade_no"];
                    //支付宝交易号
                    string trade_no = Request.Form["trade_no"];
                    //交易状态
                    string trade_status = Request.Form["trade_status"];

                    if (Request.Form["trade_status"] == "TRADE_FINISHED")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序
                        osv.FinishOrder(out_trade_no, trade_no, PayType.Alipay);
                        //注意：
                        //该种交易状态只在两种情况下出现
                        //1、开通了普通即时到账，买家付款成功后。
                        //2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。
                    }
                    else if (Request.Form["trade_status"] == "TRADE_SUCCESS")
                    {
                        //判断该笔订单是否在商户网站中已经做过处理
                        //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                        //如果有做过处理，不执行商户的业务程序
                        osv.FinishOrder(out_trade_no, trade_no, PayType.Alipay);
                        //注意：
                        //该种交易状态只在一种情况下出现——开通了高级即时到账，买家付款成功后。
                    }

                    return Content("success");
                }
                else
                {
                    return Content("fail");
                }
            }
            else
            {
                return Content("无通知参数");
            }
        }
        public ActionResult AlipayReturn()
        {
            SortedDictionary<string, string> sPara = GetRequestGet();
            if (sPara.Count > 0)
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.Verify(sPara, Request.QueryString["notify_id"], Request.QueryString["sign"]);

                if (verifyResult)
                {
                    //订单号
                    string orderNo = Request.QueryString["out_trade_no"];
                    //支付宝交易号
                    string trade_no = Request.QueryString["trade_no"];
                    //交易状态
                    string trade_status = Request.QueryString["trade_status"];

                    if (Request.QueryString["trade_status"] == "TRADE_FINISHED" || Request.QueryString["trade_status"] == "TRADE_SUCCESS")
                    {
                        //交易成功
                        osv.FinishOrder(orderNo, trade_no, PayType.Alipay);
                        return RedirectToAction("BuyResult", new { orderNo = orderNo });
                    }
                }

            }
            ViewBag.msg = "付款验证失败，请与管理员联系";

            return View();
        }

        /// <summary>
        /// 支付结果页面
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [CheckLogin]
        public ActionResult BuyResult(string orderNo)
        {
            var order = osv.GetOrderIncludeByOrderNo(orderNo, "OrderDetails");
            if (order != null && order.CustomerID.Equals(customerCache.CustomerID) && order.OrderStatus == OrderStatus.HasPay)
            {
                return View(order);
            }

            return RedirectToAction("Index");
        }


        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        public SortedDictionary<string, string> GetRequestPost()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
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
        public SortedDictionary<string, string> GetRequestGet()
        {
            int i = 0;
            SortedDictionary<string, string> sArray = new SortedDictionary<string, string>();
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
    }
}
