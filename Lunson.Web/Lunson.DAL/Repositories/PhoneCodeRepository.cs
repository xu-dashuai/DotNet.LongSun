using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.DAL.Repositories
{
    public class PhoneCodeRepository:BaseRepository
    {
        public PhoneCodeRepository() : base() { }
        public PhoneCodeRepository(EFDbContext context) : base(context) { }
    }
}
