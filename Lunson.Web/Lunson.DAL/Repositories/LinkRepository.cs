using Lunson.Domain;
using Lunson.Domain.Entities;
using Pharos.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.DAL.Repositories
{
    /// <summary>
    /// 友情链接仓储类
    /// </summary>
    public class LinkRepository:BaseRepository
    {
        public LinkRepository() : base() { }
        public LinkRepository(EFDbContext context) : base(context) { }
    }
}
