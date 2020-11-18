using Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;
using Helper.Page;
using System.Drawing;
using System.IO.Compression;

namespace Helper.File
{
    public class AttachmentFile
    {
        public static string SaveFile(HttpPostedFileBase file, DateTime dateTime, string siteId, string userId, bool isImage = false, int imgWidth = 0, int imgHeight = 0, IDbTransaction dbTransaction = null, IDbConnection dbConnection = null)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                    return string.Empty;
                string fileName = file.FileName;
                if (string.IsNullOrEmpty(fileName))
                    return "Tệp tin không hợp lệ";
                //Check allowed file
                string extension = System.IO.Path.GetExtension(fileName);
                if (isImage)
                {
                    if (!Validate.TestImageFile(extension))
                        return "Tệp tin  không hợp lệ";
                }
                else
                {
                    if (!IsAllowedFile(extension))
                        return "Tệp tin  không hợp lệ";
                }
                if (file.ContentLength / 1048576 > 5)
                    return "Tệp tin phải < 5M";
                //Save file info to database
                AttachmentService attachmentService = new AttachmentService(dbConnection);
                var guid = attachmentService.Create<string>(new Attachment()
                {
                    Title = null,
                    Extension = extension,
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType.ToLower(),
                    LanguageID = Helper.Language.LanguagePage.GetLanguageCode,

                }, transaction: dbTransaction);
                //Save file to system
                var year = dateTime.Year;
                var month = dateTime.Month;
                string fileFolderPath = HttpContext.Current.Server.MapPath("~/Files/Upload/" + year + "/" + month + "/");
                if (!System.IO.Directory.Exists(fileFolderPath))
                    System.IO.Directory.CreateDirectory(fileFolderPath);
                //
                if (imgWidth > 0)
                {
                    WebImage webImage = new WebImage(file.InputStream);
                    double imageWidth = webImage.Width;

                    if (imgHeight == 0)
                    {
                        int imageHeight = webImage.Height;
                        imgHeight = (int)(imgWidth * 1.0 * imageHeight / imageWidth);
                    }

                    webImage.Resize(imgWidth, imgHeight);
                    webImage.Save(fileFolderPath + guid + extension);
                }
                else
                {
                    file.SaveAs(fileFolderPath + guid + extension);
                }
                return guid;

            }
            catch (Exception ex)
            {
                return "Lỗi tải tệp tin" + ex;
            }
        }

        public static string DeleteFile(string guid, IDbTransaction dbTransaction = null, IDbConnection dbConnection = null)
        {
            try
            {
                AttachmentService attachmentService = new AttachmentService(dbConnection);
                if (!Validate.TestGuid(guid))
                    return string.Empty;

                var attachment = attachmentService.Find(guid, transaction: dbTransaction);
                if (attachment != null)
                {
                    var date = attachment.CreatedDate;
                    var year = date.Year;
                    var month = date.Month;
                    string fileFolderPath = HttpContext.Current.Server.MapPath("~/files/upload/" + year + "/" + month + "/" + guid + attachment.Extension);
                    if (System.IO.File.Exists(fileFolderPath))
                        System.IO.File.Delete(fileFolderPath);
                    attachmentService.Remove(guid, transaction: dbTransaction);
                    return string.Empty;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return "Lỗi xóa tệp tin";
            }
        }

        public static string DeleteMultiFile(List<string> idList, IDbTransaction dbTransaction = null, IDbConnection dbConnection = null)
        {
            try
            {
                AttachmentService attachmentService = new AttachmentService(dbConnection);
                foreach (var item in idList)
                {
                    string guid = item;
                    if (!Validate.TestGuid(guid))
                        continue;
                    //
                    var attachment = attachmentService.Find(guid, transaction: dbTransaction);
                    if (attachment != null)
                    {
                        var date = attachment.CreatedDate;
                        var year = date.Year;
                        var month = date.Month;
                        string fileFolderPath = HttpContext.Current.Server.MapPath("~/files/upload/" + year + "/" + month + "/" + guid + attachment.Extension);
                        if (System.IO.File.Exists(fileFolderPath))
                            System.IO.File.Delete(fileFolderPath);
                        //
                    }
                }
                string sqlQuery = "DELETE Attachment WHERE ID IN  ('" + String.Join("','", idList) + "')";
                attachmentService.Execute(sqlQuery, transaction: dbTransaction);
                return string.Empty;
            }
            catch (Exception)
            {
                return "Lỗi xóa tệp tin";
            }
        }
        public static List<AttachmentResultFilesModel> SaveFiles(IEnumerable<HttpPostedFileBase> files, DateTime dateTime, string siteId, string userId, bool isImage = false, int width = 0, int height = 0, IDbTransaction dbTransaction = null, IDbConnection dbConnection = null)
        {
            if (files.Count() > 0)
            {
                double totalSize = 0.0;
                // valid file
                foreach (var file in files)
                {
                    if (file == null || file.ContentLength == 0)
                        return new List<AttachmentResultFilesModel> {
                            new AttachmentResultFilesModel(400,"Dữ liệu không hợp lệ",null)
                        };


                    //
                    string fileName = file.FileName;
                    if (string.IsNullOrEmpty(fileName))
                        return new List<AttachmentResultFilesModel> {
                            new AttachmentResultFilesModel(404,"Tên tệp tin không hợp lệ",null)
                        };

                    //Check allowed file
                    if (!Validate.TestImageFile(fileName))
                        return new List<AttachmentResultFilesModel> {
                            new AttachmentResultFilesModel(400,"Tệp tin không hợp lệ",null)
                        };
                    totalSize += file.ContentLength;
                    //Check file size limit
                    if (totalSize / 1048576 > 10)
                        return new List<AttachmentResultFilesModel> {
                            new AttachmentResultFilesModel(400,"Tệp tin phải < 1M",null)
                        };
                }
            }

            List<string> lstFile = new List<string>();
            //Save file info to database
            var attachmentService = new AttachmentService(dbConnection);
            foreach (var file in files)
            {
                string fileName = file.FileName;
                string extension = System.IO.Path.GetExtension(fileName);
                string guid = attachmentService.Create<string>(new Attachment()
                {
                    Title = null,
                    Extension = extension,
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType.ToLower(),
                    LanguageID = Helper.Language.LanguagePage.GetLanguageCode,
                }, transaction: dbTransaction);

                //Save file to system
                var year = dateTime.Year;
                var month = dateTime.Month;
                string fileFolderPath = HttpContext.Current.Server.MapPath("~/files/upload/" + year + "/" + month + "/");
                if (!System.IO.Directory.Exists(fileFolderPath))
                    System.IO.Directory.CreateDirectory(fileFolderPath);

                file.SaveAs(fileFolderPath + guid + extension);
                lstFile.Add(guid);
            }
            if (lstFile.Count() == 0)
                return new List<AttachmentResultFilesModel> {
                    new AttachmentResultFilesModel(404,"ok",null)
                };

            return new List<AttachmentResultFilesModel> {
                    new AttachmentResultFilesModel(200,"ok",lstFile)
                };
        }
        //
        public static string SaveImageFile(Image imgFile, DateTime dateTime, string imgName, System.Drawing.Imaging.ImageFormat imgFormat, string siteId, string userId, IDbTransaction dbTransaction = null, IDbConnection dbConnection = null)
        {

            //Save file to system
            var year = dateTime.Year;
            var month = dateTime.Month;
            string fileFolderPath = HttpContext.Current.Server.MapPath("~/Files/Upload/Image/");
            if (!System.IO.Directory.Exists(fileFolderPath))
                System.IO.Directory.CreateDirectory(fileFolderPath);
            //  
            AttachmentService attachmentService = new AttachmentService(dbConnection);
            string contentType = "image/" + imgFormat;
            var guid = attachmentService.Create<string>(new Attachment()
            {
                Title = null,
                Extension = "." + imgFormat.ToString().ToLower(),
                ContentLength = 0,
                ContentType = contentType.ToLower(),
                LanguageID = Language.LanguagePage.GetLanguageCode,
            }, transaction: dbTransaction);

            Attachment attachment = attachmentService.GetAlls(m => m.ID == guid, transaction: dbTransaction).FirstOrDefault();
            string fullPath = fileFolderPath + imgName;
            imgFile.Save(fullPath, imgFormat);
            FileInfo file = new FileInfo(fullPath);
            attachment.ContentLength = file.Length;
            attachmentService.Update(attachment, transaction: dbTransaction);
            //

            return guid;

        }
        //
        public static string ZipDirectoryFile(string inputDirectory, string zipPath)
        {
            ZipFile.CreateFromDirectory(inputDirectory, zipPath);
            return zipPath;
        }

        private List<string> GetImagesInHTMLString(string htmlString)
        {
            List<string> images = new List<string>();
            string pattern = @"<(img)\b[^>]*>";

            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(htmlString);

            for (int i = 0, l = matches.Count; i < l; i++)
            {
                images.Add(matches[i].Value);
            }

            return images;
        }
        public static bool IsAllowedFile(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return false;

            extension = extension.ToLower();
            return AllowedExtensions.Contains(extension);
        }
        public static IList<string> AllowedExtensions = new List<string>
        {
            ".png", ".jpg", ".jpeg", ".svg", ".mp3", ".mp4", ".pdf", ".xls", ".xlsx", ".doc", ".docx", ".rar", ".zip", ".txt", ".ppt", ".pptx"
        };

        //* Get file *****************************************************************************************************************************************************************************************************************************************************************************************
        public static string GetFile(string id, string extension, DateTime createdDate)
        {
            var year = createdDate.Year;
            var month = createdDate.Month;
            return "/files/upload/" + year + "/" + month + "/" + id + "" + extension;
        }
        public static string GetFile(string guid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(guid))
                    return Helper.Page.Default.FileNoImange;
                //
                var files = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/files/upload/"), guid + ".*", System.IO.SearchOption.AllDirectories);
                if (files != null && files.Count() > 0)
                    return "/files/upload/" + files.First().Replace("\\", "/").Split(new string[] { "/files/upload/" }, StringSplitOptions.None)[1];
                else
                    return Helper.Page.Default.FileNoImange;
            }
            catch (Exception)
            {
                return Helper.Page.Default.FileNoImange;
            }
        }
        public string GetAttmFile(string guid)
        {
            try
            {
                var files = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/files/upload/"), guid + ".*", System.IO.SearchOption.AllDirectories);
                if (files != null && files.Count() > 0)
                    return "/files/upload/" + files.First().Replace("\\", "/").Split(new string[] { "/files/upload/" }, StringSplitOptions.None)[1];
                else
                    return Helper.Page.Default.FileNoImange;
            }
            catch (Exception)
            {
                return Helper.Page.Default.FileNoImange;
            }
        }
    }
    public class AttachmentResultFilesModel
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public List<string> FileList { get; set; }
        public AttachmentResultFilesModel(int status, string message, List<string> fileList)
        {
            Status = status;
            Message = message;
            FileList = fileList;
        }
    }
    public class AttachmentModel
    {
        public HttpPostedFileBase DocumentFile { get; set; }
    }
    public class AttachmentFolderModel : SearchModel
    {
        public string All { get; set; }
        public string Product { get; set; }
        public string News { get; set; }
    }

}
