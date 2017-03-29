using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lunson.Web.Models
{
    public class ReportVM
    {
        /// <summary>
        /// 票名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 票类型
        /// </summary>
        public string T_Type { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public double? AllPrice { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int? Num { get; set; }

    }
}