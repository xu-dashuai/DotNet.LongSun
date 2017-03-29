using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

///这段代码可能存在引用的程序包的冲突，主要是System.Net相关的命名空间
namespace Pharos.Framework.MVC
{
    /// <summary>
    /// 为WEBAPI提供的专用的返回值格式配置工具
    /// </summary>
    public class JsonContentNegotiator : System.Net.Http.Formatting.IContentNegotiator
    {
        private readonly System.Net.Http.Formatting.JsonMediaTypeFormatter _jsonFormatter;

        public JsonContentNegotiator(System.Net.Http.Formatting.JsonMediaTypeFormatter formatter)
        {
            _jsonFormatter = formatter;
        }

        public System.Net.Http.Formatting.ContentNegotiationResult Negotiate(Type type, System.Net.Http.HttpRequestMessage request, IEnumerable<System.Net.Http.Formatting.MediaTypeFormatter> formatters)
        {
            var result = new System.Net.Http.Formatting.ContentNegotiationResult(_jsonFormatter, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"));
            return result;
        }
    }
}
