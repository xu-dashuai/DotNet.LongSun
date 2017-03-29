using Lunson.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lunson.Web.Areas.Backend.Models
{
    /// <summary>
    /// 票使用日详细报表
    /// </summary>
    public class UnusedTicketVM
    {
        [Display(Name="ID")]
        public string ID { get; set; }
        [Display(Name = "票号")]
        public string TicketNO { get; set; }
        [Display(Name = "门票名称")]
        public string Name { get; set; }
        [Display(Name = "购买时间")]
        public DateTime BuyTime { get; set; }
        [Display(Name = "使用时间")]
        public DateTime? UsedTime { get; set; }
        [Display(Name = "售价/元")]
        public double CurPrice { get; set; }
        [Display(Name = "手机")]
        public string Mobile { get; set; }
        [Display(Name = "订单号")]
        public string OrderNo { get; set; }
    }
}