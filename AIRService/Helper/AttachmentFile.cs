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

namespace Helper.File
{
    public class AttachmentFile
    {
        public static string SaveFile(HttpPostedFileBase file, string siteId, string userId, bool isImage = false, int width = 0, int height = 0, IDbTransaction transaction = null, IDbConnection connection = null)
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

                DateTime now = DateTime.Now;
                //Save file info to database
                AttachmentService attachmentService = new AttachmentService(connection);
                var guid = attachmentService.Create<string>(new Attachment()
                {
                    Title = "",
                    Extension = extension,
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    LanguageID = "vn",

                }, transaction: transaction);
                //Save file to system
                var year = now.Year;
                var month = now.Month;
                string fileFolderPath = HttpContext.Current.Server.MapPath("~/Files/Upload/" + year + "/" + month + "/");
                if (!System.IO.Directory.Exists(fileFolderPath))
                    System.IO.Directory.CreateDirectory(fileFolderPath);

                if (width > 0)
                {
                    WebImage webImage = new WebImage(file.InputStream);
                    double imageWidth = webImage.Width;

                    if (height == 0)
                    {
                        int imageHeight = webImage.Height;
                        height = (int)(width * 1.0 * imageHeight / imageWidth);
                    }

                    webImage.Resize(width, height);
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
        public static string SaveFile(HttpPostedFileBase file, string siteId, string userId, bool isImage = false, int width = 0, int height = 0)
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
                    if (!Helper.Page.Validate.TestImageFile(extension))
                        return "Tệp tin  không hợp lệ";
                }
                else
                {
                    if (!IsAllowedFile(extension))
                        return "Tệp tin  không hợp lệ";
                }
                if (file.ContentLength / 1048576 > 5)
                    return "Tệp tin phải < 5M";

                DateTime now = DateTime.Now;
                //Save file info to database
                AttachmentService attachmentService = new AttachmentService();
                var guid = attachmentService.Create<string>(new Attachment()
                {
                    Title = file.FileName,
                    Extension = extension,
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    LanguageID = "vn",
                });
                //Save file to system
                var year = now.Year;
                var month = now.Month;
                string fileFolderPath = HttpContext.Current.Server.MapPath("~/Files/Upload/" + year + "/" + month + "/");
                if (!System.IO.Directory.Exists(fileFolderPath))
                    System.IO.Directory.CreateDirectory(fileFolderPath);

                if (width > 0)
                {
                    WebImage webImage = new WebImage(file.InputStream);
                    double imageWidth = webImage.Width;

                    if (height == 0)
                    {
                        int imageHeight = webImage.Height;
                        height = (int)(width * 1.0 * imageHeight / imageWidth);
                    }

                    webImage.Resize(width, height);
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

        public static string DeleteFile(string guid, IDbTransaction transaction = null, IDbConnection connection = null)
        {
            try
            {
                DateTime now = DateTime.Now;
                //Save file info to database
                AttachmentService attachmentService = new AttachmentService(connection);
                if (!Validate.TestGuid(guid))
                    return string.Empty;

                var attachment = attachmentService.Find(guid, transaction: transaction);
                if (attachment != null)
                {
                    var date = attachment.CreatedDate;
                    var year = date.Year;
                    var month = date.Month;
                    string fileFolderPath = HttpContext.Current.Server.MapPath("~/files/upload/" + year + "/" + month + "/" + guid + attachment.Extension);
                    if (System.IO.File.Exists(fileFolderPath))
                        System.IO.File.Delete(fileFolderPath);
                    attachmentService.Remove(guid, transaction: transaction);
                    return string.Empty;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return "Lỗi xóa tệp tin";
            }
        }
        public static List<AttachmentResultFilesModel> SaveFiles(IEnumerable<HttpPostedFileBase> files, string siteId, string userId, bool isImage = false, int width = 0, int height = 0, IDbTransaction transaction = null, IDbConnection connection = null)
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
            DateTime now = DateTime.Now;
            //Save file info to database
            var attachmentService = new AttachmentService(connection);
            foreach (var file in files)
            {
                string fileName = file.FileName;
                string extension = System.IO.Path.GetExtension(fileName);
                string guid = attachmentService.Create<string>(new Attachment()
                {
                    Title = "",
                    Extension = extension,
                    ContentLength = file.ContentLength,
                    ContentType = file.ContentType,
                    LanguageID = "vn",
                }, transaction: transaction);

                //Save file to system
                var year = now.Year;
                var month = now.Month;
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
