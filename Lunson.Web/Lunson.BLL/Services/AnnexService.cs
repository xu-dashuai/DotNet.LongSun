using Lunson.DAL.Repositories;
using Lunson.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using Pharos.Framework;
using Lunson.Domain;

namespace Lunson.BLL.Services
{
    public class AnnexService
    {
        private AnnexRepository DAL = new AnnexRepository();

        public Annex GetAnnex(string id)
        {
            return DAL.GetAnnex(id);
        }
        public Annex GetAnnex(string id,AnnexType type)
        {
            return DAL.GetAnnex(id,type);
        }
        public IQueryable<Annex> GetAnnexes(AnnexType type)
        {
            return DAL.GetAnnexes(type);
        }

        public void RemoveAnnex(string id,string currentUserID,AnnexType type)
        {
            var annex= DAL.RemoveAnnex(id,currentUserID,type);

            if (annex != null)
            {
                switch (annex.Type)
                { 
                    case AnnexType.Swf:
                        File.Delete(HttpContext.Current.Server.MapPath(annex.Url));
                        break;
                    case AnnexType.Crocodile:
                        File.Delete(HttpContext.Current.Server.MapPath(annex.Url));
                        File.Delete(HttpContext.Current.Server.MapPath(annex.Url+"_thumb.jpg"));
                        break;
                }
            }

        }

        public Annex SaveTicketThumb(HttpPostedFileBase pic,string currentUserID)
        {
            var url = "/upload/ticket/"+DateTime.Now.ToString("yyyy-MM/dd/");
            if (Directory.Exists(HttpContext.Current.Server.MapPath(url)) == false)
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(url));
            }

            string fileExtension =pic.FileName.GetExtension().ToLower();
            var name = DataHelper.GetSystemID();

            pic.SaveAs(HttpContext.Current.Server.MapPath(url+name+fileExtension));
            PictureHelper.MakeThumbnail(HttpContext.Current.Server.MapPath(url + name + fileExtension), HttpContext.Current.Server.MapPath(url + name + fileExtension) + "_thumb.jpg", 800, 400, "Cut");
            PictureHelper.MakeThumbnail(HttpContext.Current.Server.MapPath(url + name + fileExtension), HttpContext.Current.Server.MapPath(url + name + fileExtension) + "_thumb_for_mobile.jpg", 150, 150, "Cut");
            PictureHelper.MakeThumbnail(HttpContext.Current.Server.MapPath(url + name + fileExtension), HttpContext.Current.Server.MapPath(url + name + fileExtension) + "_large.jpg", 1000, 600, "Cut");

            var annex = new Annex();
            annex.OldName = pic.FileName;
            annex.Size = pic.ContentLength;
            annex.Type = AnnexType.Image;
            annex.Url = url + name + fileExtension;

           return DAL.CreateAnnex(annex, currentUserID);
        }
        public Annex SaveLinkLogo(HttpPostedFileBase pic, string curUserID)
        {
            var url = "/upload/link/";
            if (Directory.Exists(HttpContext.Current.Server.MapPath(url)) == false)
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(url));
            }

            string fileExtension = pic.FileName.GetExtension().ToLower();
            var name = DataHelper.GetSystemID();

            pic.SaveAs(HttpContext.Current.Server.MapPath(url + name + fileExtension));
            //PictureHelper.MakeThumbnail(HttpContext.Current.Server.MapPath(url + name + fileExtension), HttpContext.Current.Server.MapPath(url + name + fileExtension) + "_thumb.jpg", 0, 31, "H");

            var annex = new Annex();
            annex.OldName = pic.FileName;
            annex.Size = pic.ContentLength;
            annex.Type = AnnexType.Link;
            annex.Url = url + name + fileExtension;

            return DAL.CreateAnnex(annex, curUserID);
        }
        public void SaveFlash(HttpPostedFileBase flash, string curUserID)
        {
            var url = "/upload/flash/";
            if (Directory.Exists(HttpContext.Current.Server.MapPath(url)) == false)
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(url));
            }

            string fileExtension = flash.FileName.GetExtension().ToLower();
            var name = DataHelper.GetSystemID();

            flash.SaveAs(HttpContext.Current.Server.MapPath(url + name + fileExtension));

            var annex = new Annex();
            annex.OldName = flash.FileName;
            annex.Size = flash.ContentLength;
            annex.Type = AnnexType.Swf;
            annex.Url = url + name + fileExtension;

            DAL.CreateAnnex(annex, curUserID);
        }
        public void SaveCrocodile(HttpPostedFileBase crocodile, string curUserID)
        {
            var url = "/upload/crocodile/";
            if (Directory.Exists(HttpContext.Current.Server.MapPath(url)) == false)
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(url));
            }

            string fileExtension = crocodile.FileName.GetExtension().ToLower();
            var name = DataHelper.GetSystemID();

            crocodile.SaveAs(HttpContext.Current.Server.MapPath(url + name + fileExtension));
            PictureHelper.MakeThumbnail(HttpContext.Current.Server.MapPath(url + name + fileExtension), HttpContext.Current.Server.MapPath(url + name + fileExtension) + "_thumb.jpg", 600, 450, "Cut");

            var annex = new Annex();
            annex.OldName = crocodile.FileName;
            annex.Size = crocodile.ContentLength;
            annex.Type = AnnexType.Crocodile;
            annex.Url = url + name + fileExtension;

            DAL.CreateAnnex(annex, curUserID);
        }
        public object EditAnnexName(string id, string name, string currentUserID)
        {
            DAL.EditAnnexName(id,name,currentUserID);
            return new { validate=true};
        }

        /// <summary>
        /// xheditor上传附件
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        public Object UploadContentAnnex(HttpPostedFileBase fileData, string curUserID)
        {
            var result = "0";
            string folder = "/upload/content/";
            string errMsg = string.Empty;
            string fileName = string.Empty;
            if (fileData != null)
            {
                try
                {
                    if (Directory.Exists(HttpContext.Current.Server.MapPath(folder)) == false)
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folder));
                    }

                    string fileExtension = fileData.FileName.GetExtension().ToLower();
                    var name = DataHelper.GetSystemID();

                    fileData.SaveAs(HttpContext.Current.Server.MapPath(folder + name + fileExtension));
                    
                    result = string.Format("{0}/{1}{2}", folder, name, fileExtension);//返回图片路径以显示

                    Annex upload = new Annex();
                    upload.OldName = fileData.FileName;
                    upload.Size = fileData.ContentLength;
                    upload.Type = AnnexType.FeedImg;
                    upload.Url = folder + name + fileExtension;

                    DAL.CreateAnnex(upload, curUserID);
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
            }
            else
            {
                errMsg = "请选择要上传的文件!";
            }
            return new { err = errMsg, msg = result };
        }
    }
}
