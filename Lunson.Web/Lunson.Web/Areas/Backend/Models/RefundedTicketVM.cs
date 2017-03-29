using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lunson.Web.Areas.Backend.Models
{
    public class RefundedTicketVM
    {
        [Display(Name = "票号")]
        public string TicketNO { get; set; }
        [Display(Name = "商户订单号")]
        public string OrderNo { get; set; }
        [Display(Name = "客户手机")]
        public string Mobile { get; set; }
        [Display(Name = "门票名称")]
        public string Name { get; set; }
        [Display(Name = "售价/元")]
        public double CurPrice { get; set; }
        [Display(Name = "购买时间")]
        public DateTime BuyTime { get; set; }
        [Display(Name = "退款金额/元")]
        public double RefundPrice { get; set; }
        [Display(Name = "退票时间")]
        public DateTime RefundedTime { get; set; }
        [Display(Name = "操作人")]
        public string Operator { get; set; }
        [Display(Name="描述说明")]
        public string Description { get; set; }
    }
}