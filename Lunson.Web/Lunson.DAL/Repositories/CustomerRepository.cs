using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.DAL.Repositories
{
    public class CustomerRepository :BaseRepository
    {
        public CustomerRepository() : base() { }
        public CustomerRepository(EFDbContext context) : base(context) { }

        #region
        public Customer GetCustomerByPassword(string mobile, string password)
        {
            var customers = GetQueryInfo<Customer>();
            return customers.SingleOrDefault(a => a.Mobile.Equals(mobile) && a.Password.Equals(password));
        }
        public Customer GetCustomerByName(string name)
        {
            var customers = GetQueryInfo<Customer>();
            return customers.SingleOrDefault(a => a.UserName.Equals(name));
        }
        public Customer GetCustomerByMobile(string mobile)
        {
            var customers = GetQueryInfo<Customer>();
            return customers.SingleOrDefault(a => a.Mobile.Equals(mobile));
        }
        public Customer GetCustomer(string id)
        {
            return GetInfoByID<Customer>(id);
        }

        #endregion

        /// <summary>
        /// 忘记密码，重设密码
        /// </summary>
        /// <param name="newPwd"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool ResetPassword(string newPwd, string mobile)
        {
            var customer = context.Customers.SingleOrDefault(c => c.Mobile.Equals(mobile) && c.IsDeleted == YesOrNo.No);
            customer.Password = newPwd;
            customer.ModifiedOn = DateTime.Now;
            return context.SaveChanges() > 0;
        }


        public IQueryable<TradeLog> GetTradeLogByCustomerID(string id, string sortExpression)
        {
            sortExpression = sortExpression == null ? "" : sortExpression;
            return context.TradeLogs.Where(t => t.CustomerID.Equals(id) && t.IsDeleted == YesOrNo.No && t.Message.Contains(sortExpression)).OrderByDescending(t => t.CreatedOn);
        }



    }
}
