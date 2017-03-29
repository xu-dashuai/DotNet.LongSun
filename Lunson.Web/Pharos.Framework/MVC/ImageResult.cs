using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Pharos.Framework.MVC
{
    public class ImageResult : ActionResult
    {
        //private Stream _imgStream;
        //private string _contentType;

        //public ImageResult(Stream imgStream, string contentType)
        //{
        //    _imgStream = imgStream;
        //    _contentType = contentType;
        //}
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentException("context");
            //if (_imgStream == null)
            //    throw new ArgumentException("imgStream is null");
            //if (_contentType == null)
            //    throw new ArgumentException("contentType is null");

            HttpResponseBase response = context.HttpContext.Response;

            response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
    }
}
