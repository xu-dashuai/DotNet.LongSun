using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pharos.Framework
{
    public static class LinqHelper
    {
        public static List<T> ToList<T>(this IEnumerable<object> source) where T : new()
        {
            List<T> result = new List<T>();
            var outType = typeof(T);
            var typeInfos = outType.GetProperties();
            var typeName = typeInfos.Select(a => a.Name);

            foreach (var item in source)
            {
                try
                {
                    T outItem = new T();
                    var classType = item.GetType();
                    var proInfos = classType.GetProperties();
                    foreach (var x in proInfos.Where(p => typeName.Contains(p.Name)))
                    {
                        var p = typeInfos.SingleOrDefault(a => a.Name.Equals(x.Name));
                        p.SetValue(outItem, x.GetValue(item, null), null);
                    }
                    result.Add(outItem);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return result;
        }
    }
}
