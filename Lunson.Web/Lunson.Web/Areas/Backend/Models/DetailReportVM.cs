using Lunson.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lunson.Web.Areas.Backend.Models
{
    /// <summary>
    /// 购票详细报表
    /// </summary>
    public class DetailReportVM
    {
        [Display(Name="订单号")]
        public string OrderNo { get; set; }
        [Display(Name="交易号")]
        public string TradeNo { get; set; }
        [Display(Name = "门票名称")]        
        public string Name { get; set; }
        [Display(Name = "手机")]
        public string Mobile { get; set; }
        [Display(Name = "支付时间")]
        public DateTime BuyTime { get; set; }
        [Display(Name = "付款方式")]
        public PayType PayType { get; set; }
        [Display(Name = "数量")]
        public int Num { get; set; }
        [Display(Name = "售价/元")]
        public double CurPrice { get; set; }
        [Display(Name = "金额")]
        public double AllPrice { get; set; }
        /// <summary>
        /// 备注（票简述）
        /// </summary>
        [Display(Name = "备注")]
        public string Resume { get; set; }
    }
}