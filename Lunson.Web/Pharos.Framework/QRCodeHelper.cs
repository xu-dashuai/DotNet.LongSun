using Gma.QrCodeNet.Encoding;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.Web;

namespace Pharos.Framework
{
    public static class QRCodeHelper
    {
        public static void GetQRCode(string word,HttpResponseBase Response)
        {
            QrEncoder coder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode code = new QrCode();
            coder.TryEncode(word, out code);

            using (MemoryStream ms=new MemoryStream())
            {
                var renderer=new GraphicsRenderer(new FixedModuleSize(4,QuietZoneModules.Two));
                renderer.WriteToStream(code.Matrix,ImageFormat.Png,ms);
                Response.ContentType = "image/png";
                Response.OutputStream.Write(ms.GetBuffer(),0,(int)ms.Length);
            }
            
        }
    }
}
