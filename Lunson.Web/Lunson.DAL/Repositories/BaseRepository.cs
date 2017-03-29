using Lunson.Domain;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using Pharos.Framework;

namespace Lunson.DAL.Repositories
{
    public class BaseRepository
    {
        public EFDbContext context { get; set; }


        #region 数据处理
        public T GetInfoByID<T>(string id) where T : BaseEntity
        {
            var dbSet = GetDbSet<T>();
            var info = dbSet.SingleOrDefault(a => a.ID.Equals(id) && a.IsDeleted == YesOrNo.No);
            return info;
        }
        public T GetInfoIncludeByID<T>(string id, params string[] includes) where T : BaseEntity
        {
            var dbSet = GetDbSet<T>();

            DbQuery<T> dbQuery = null;

            foreach (var x in includes)
            {
                if (dbQuery == null)
                    dbQuery = dbSet.Include(x);
                else
                    dbQuery = dbQuery.Include(x);
            }

            if (dbQuery == null)
                return dbSet.SingleOrDefault(a => a.ID.Equals(id) && a.IsDeleted == YesOrNo.No);

            return dbQuery.SingleOrDefault(a => a.ID.Equals(id) && a.IsDeleted == YesOrNo.No);
        }

        public IQueryable<T> GetQueryInfo<T>(bool includeDeleted = false) where T : BaseEntity
        {
            var dbSet = GetDbSet<T>();
            if (includeDeleted)
                return dbSet;
            return dbSet.Where(a => a.IsDeleted == YesOrNo.No);
        }
        public IQueryable<T> GetQueryIncludeInfo<T>(params string[] includes) where T : BaseEntity
        {
            var dbSet = GetDbSet<T>();

            DbQuery<T> dbQuery = null;
            foreach (var x in includes)
            {
                if (dbQuery == null)
                    dbQuery = dbSet.Include(x);
                else
                    dbQuery = dbQuery.Include(x);
            }

            if (dbQuery == null)
                return dbSet.Where(a => a.IsDeleted == YesOrNo.No);

            return dbQuery.Where(a => a.IsDeleted == YesOrNo.No);
        }

        public void SaveInfo<T>(T entity, string currentUserID = "") where T : BaseEntity
        {
            //添加
            if (entity.ID.IsNullOrTrimEmpty())
            {
                #region
                entity.ID = DataHelper.GetSystemID();
                entity.CreatedBy = currentUserID;
                entity.CreatedOn = DateTime.Now;
                entity.IsDeleted = YesOrNo.No;

                var dbSet = GetDbSet<T>();
                dbSet.Add(entity);
                #endregion
            }
            //修改
            else
            {
                var info = GetInfoByID<T>(entity.ID);
                if (info != null)
                {
                    info.CopyProperty(entity);
                    info.ModifiedBy = currentUserID;
                    info.ModifiedOn = DateTime.Now;
                }
            }
        }

        public void RemoveInfo<T>(string id, string currentUserID = "") where T : BaseEntity
        {
            var info = GetInfoByID<T>(id);
            if (info != null)
            {
                info.IsDeleted = YesOrNo.Yes;
                info.ModifiedBy = currentUserID;
                info.ModifiedOn = DateTime.Now;
            }
        }
        public void RemoveInfo<T>(T entity, string currentUserID = "") where T : BaseEntity
        {
            if (entity != null)
            {
                entity.IsDeleted = YesOrNo.Yes;
                entity.ModifiedBy = currentUserID;
                entity.ModifiedOn = DateTime.Now;
            }
        }

        public void Modify<T>(string id, Action<T> action, string currentUserID) where T : BaseEntity
        {
            var entity = GetInfoByID<T>(id);
            if (entity != null)
            {
                action(entity);
                SaveInfo(entity, currentUserID);
            }
        }

        private DbSet<T> GetDbSet<T>() where T : BaseEntity
        {
            var type = context.GetType();
            var setType = typeof(DbSet<>).MakeGenericType(typeof(T));

            dynamic dbSet = null;

            foreach (var x in type.GetProperties())
            {
                if (setType.IsAssignableFrom(x.PropertyType))
                {
                    dbSet = x.GetValue(context, null);
                    break;
                }
            }

            return dbSet;
        }
        #endregion

        #region 数据提交
        /// <summary>
        /// 提交数据库
        /// </summary>
        /// <returns></returns>
        public int Submit()
        {
            return context.SaveChanges();
        }
        #endregion

        #region 构造函数
        public BaseRepository()
        {
            this.context = ContextFactory.GetCurrentContext();
        }
        public BaseRepository(EFDbContext Context)
        {
            this.context = Context;
        }
        #endregion
    }
}
