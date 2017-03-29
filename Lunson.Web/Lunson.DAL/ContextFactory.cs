using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunson.DAL
{
    public class ContextFactory
    {
        public static EFDbContext GetCurrentContext()
        {
            return new EFDbContext();
        }
    }
}
