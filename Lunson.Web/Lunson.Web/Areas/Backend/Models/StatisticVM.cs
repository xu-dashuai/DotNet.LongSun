using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lunson.Web.Areas.Backend.Models
{
    /// <summary>
    /// 购票统计报表
    /// </summary>
    public class StatisticVM
    {
        [Display(Name = "门票名称")]
        public string Name { get; set; }
        [Display(Name = "出售数量")]
        public int Num { get; set; }
        [Display(Name = "售价")]
        public double SalePrice { get; set; }
        [Display(Name = "总金额")]
        public double Income { get; set; }
        [Display(Name = "简述")]
        public string Resume { get; set; }
    }
}