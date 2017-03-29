using Lunson.Domain.Entities;
using Lunson.Domain.Mapping;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;

namespace Lunson.DAL
{
    public partial class EFDbContext : DbContext
    {
        public EFDbContext()
        {
            ///Leo: disable the Lazy Loading the WCF will not be able to serilize the entities.
            //是否启用延迟加载:  
            //  true:   延迟加载（Lazy Loading）：获取实体时不会加载其导航属性，一旦用到导航属性就会自动加载  
            //  false:  直接加载（Eager loading）：通过 Include 之类的方法显示加载导航属性，获取实体时会即时加载通过 Include 指定的导航属性  
            this.Configuration.LazyLoadingEnabled = false;

            this.Configuration.ProxyCreationEnabled = false;

            //UseLegacyPreserveChangesBehavior
            //确定是否使用旧的行为， true 使用，false 不使用；
            this.Configuration.AutoDetectChangesEnabled = true;  //自动监测变化，默认值为 true 
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ///移除EF映射默认给表名添加“s“或者“es”
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            ///配置关系
            modelBuilder.Configurations
                .Add(new TicketMap())
                .Add(new TicketHistMap())
                .Add(new AnnexMap())
                .Add(new Sys_UserMap())
                .Add(new Sys_RoleMap())
                .Add(new Sys_MenuMap())
                .Add(new UserLoginLogMap())
                .Add(new OrderMap())
                .Add(new OrderHistMap())
                .Add(new OrderDetailMap())
                .Add(new Sys_UserRoleMap())
                .Add(new LinkMap())
                .Add(new FeedTypeMap())
                .Add(new FeedMap())
                .Add(new CustomerMap())
                .Add(new CustomerLoginLogMap())
                .Add(new TicketDetailMap())
                .Add(new TicketDetailHistMap())
                .Add(new PermissionMap())
                .Add(new RolePermissionMap())
                .Add(new TicketRefundMap())
                .Add(new PhoneCodeMap())
                .Add(new TradeLogMap())
                .Add(new PayLogMap());

            base.OnModelCreating(modelBuilder);
        }

        //表空间
        public DbSet<Ticket> Tickets { get; set; }  //票
        public DbSet<TicketHist> TicketHists { get; set; }  //票历史
        public DbSet<Annex> Annexes { get; set; }   //附件
        public DbSet<Sys_User> Sys_Users { get; set; }  //用户表
        public DbSet<Sys_Role> Sys_Roles { get; set; } //角色
        public DbSet<Sys_UserRole> Sys_UserRoles { get; set; } //用户角色
        public DbSet<Sys_Menu> Sys_Menus { get; set; }  //菜单 
        public DbSet<UserLoginLog> UserLoginLogs { get; set; }  //用户登录历史
        public DbSet<Order> Orders { get; set; } //订单
        public DbSet<OrderHist> OrderHists { get; set; }  //订单历史 
        public DbSet<OrderDetail> OrderDetails { get; set; }  //订单明细
        public DbSet<TicketRefund> TicketRefunds { get; set; }  //退票记录
        public DbSet<Link> Links { get; set; }  //友情链接
        public DbSet<Feed> Feeds { get; set; }  //文章
        public DbSet<FeedType> FeedTypes { get; set; }  //文章类型
        public DbSet<Customer> Customers { get; set; }  //前台注册用户
        public DbSet<CustomerLoginLog> CustomerLoginLogs { get; set; }  //前台注册用户登录历史
        public DbSet<TicketDetail> TicketDetails { get; set; }  //单张票明细
        public DbSet<TicketDetailHist> TicketDetailHists { get; set; }  //单张票明细历史记录
        public DbSet<TradeLog> TradeLogs { get; set; }  //客户交易历史
        public DbSet<Permission> Permissions { get; set; }  //权限
        public DbSet<RolePermission> RolePermissions { get; set; }  //角色权限
        public DbSet<PhoneCode> PhoneCodes { get; set; }  //手机验证码
        public DbSet<PayLog> PayLogs { get; set; }  //支付日志类

    }
}
