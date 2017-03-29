using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Lunson.Domain
{
    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 显示菜单
        /// </summary>
        [Description("显示菜单")]
        Show = 0,
        /// <summary>
        /// 隐藏菜单
        /// </summary>
        [Description("隐藏菜单")]
        Hide = 1
    }
    /// <summary>
    /// 性别
    /// </summary>
    public enum Sex
    {
        /// <summary>
        /// 男
        /// </summary>
        [Description("男")]
        Male = 0,
        /// <summary>
        /// 女
        /// </summary>
        [Description("女")]
        Female = 1,
        /// <summary>
        /// 保密
        /// </summary>
        [Description("保密")]
        Secret = 2
    }
    /// <summary>
    /// 活动状态
    /// </summary>
    public enum ActiveState
    {
        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        Normal = 0,
        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        Frozen = 1
    }
    /// <summary>
    /// 上下架状态
    /// </summary>
    public enum OnOffState
    {
        /// <summary>
        /// 上架
        /// </summary>
        [Description("上架")]
        On = 0,
        /// <summary>
        /// 下架
        /// </summary>
        [Description("下架")]
        Off = 1
    }
    /// <summary>
    /// 是或否
    /// </summary>
    public enum YesOrNo
    {
        /// <summary>
        /// 否 0
        /// </summary>
        [Description("否")]
        No = 0,
        /// <summary>
        /// 是 1
        /// </summary>
        [Description("是")]
        Yes = 1
    }
    /// <summary>
    /// 权限类型
    /// </summary>
    public enum PermissionType
    {
        /// <summary>
        /// 增加
        /// </summary>
        [Description("增加")]
        Create = 0,
        /// <summary>
        /// 读取
        /// </summary>
        [Description("读取")]
        Retrieve = 1,
        /// <summary>
        /// 更新
        /// </summary>
        [Description("更新")]
        Update = 2,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 3,
        /// <summary>
        /// 打印
        /// </summary>
        [Description("打印")]
        Print

    }
    /// <summary>
    /// 附件类型
    /// </summary>
    public enum AnnexType
    { 
        /// <summary>
        /// 图片
        /// </summary>
        [Description("图片")]
        Image = 0,
        /// <summary>
        /// 友情链接图片
        /// </summary>
        [Description("友情链接图片")]
        Link = 1,
        /// <summary>
        /// 前台动画
        /// </summary>
        [Description("前台动画Swf文件")]
        Swf = 2,
        /// <summary>
        /// 文章图片
        /// </summary>
        [Description("文章图片")]
        FeedImg = 3,
        /// <summary>
        /// 鳄鱼图片
        /// </summary>
        [Description("鳄鱼图片")]
        Crocodile = 4
    }
    /// <summary>
    /// 票状态
    /// </summary>
    public enum TicketStatus
    {
        /// <summary>
        /// 等待付款
        /// </summary>
        [Description("等待付款")]
        WaitPay = 0,
        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        HasPay = 1,
        ///// <summary>
        ///// 申请退款
        ///// </summary>
        //[Description("申请退款")]
        //ApplyForRefund = 2,
        /// <summary>
        /// 退款中
        /// </summary>
        [Description("退款中")]
        Refunding = 3,
        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        Refunded = 4,
        /// <summary>
        /// 已使用
        /// </summary>
        [Description("已使用")]
        IsUsed = 5,
        /// <summary>
        /// 已作废
        /// </summary>
        [Description("已作废")]
        Invalid = 6
    }
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum OrderStatus
    { 
        /// <summary>
        /// 等待付款
        /// </summary>
        [Description("等待付款")]
        WaitPay=0,
        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        HasPay=1,
        /// <summary>
        /// 退款中
        /// </summary>
        [Description("退款中")]
        Refunding=2,
        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        Refunded = 3,
        /// <summary>
        /// 订单作废
        /// </summary>
        [Description("订单作废")]
        Invalid=4
    }
    /// <summary>
    /// 支付类型
    /// </summary>
    public enum PayType
    {
        /// <summary>
        /// 支付宝
        /// </summary>
        [Description("支付宝")]
        Alipay=0
    }
}
