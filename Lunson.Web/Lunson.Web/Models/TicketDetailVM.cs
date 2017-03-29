using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lunson.Web.Models
{
    public class TicketDetailVM
    {
        /// <summary>
        /// 票ID
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImgUrl { get; set; }
        /// <summary>
        /// 票名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 简单描述
        /// </summary>
        public string Resume { get; set; }
        /// <summary>
        /// 票单价
        /// </summary>
        public double CurPrice { get; set; }
        /// <summary>
        /// 单票总价
        /// </summary>
        public double  AllPrice{ get; set; }
        /// <summary>
        /// 票数量
        /// </summary>
        public int? Num { get; set; }
    }
}