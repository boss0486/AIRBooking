using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using WebCore.Entities;
using Helper;
using System.Web;
using System.Web.Configuration;
using System.Data;

namespace WebCore.Services
{
    public interface ISiteInfoService : IEntityService<SiteInfo> { }
    public class SiteInfoService : EntityService<SiteInfo>, ISiteInfoService
    {
        public SiteInfoService() : base() { }
        public SiteInfoService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT * FROM View_SiteInfor 
                                     WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                     ORDER BY [CreatedDate]";
            var dtList = _connection.Query<SiteInfo>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);
            var resultData = new List<RsSiteInfo>();
            foreach (var item in dtList)
            {
                resultData.Add(new RsSiteInfo(item.ID, item.SiteName, item.Domain, item.ParentID, item.Title, item.Alias, item.IconFile, item.ImageFile, item.Summary, item.Email, item.Fax, item.Phone, item.Tel, item.Address, item.Gmaps, item.GoogleAnalytic, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, Helper.Library.FormatDate(item.CreatedDate)));
            }
            var result = resultData.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = resultData.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);

            Library.PagingModel pagingModel = new Library.PagingModel
            {
                PageSize = Library.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            Helper.Model.RoleAccountModel roleAccountModel = new Helper.Model.RoleAccountModel
            {
                Create = true,
                Update = true,
                Details = true,
                Delete = true,
                Block = true,
                Active = true,
            };
            return Notifization.DATALIST(NotifizationText.Success, data: result, role: roleAccountModel, paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(SiteInfoCreateFormModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    SiteInfoService siteInfoService = new SiteInfoService(_connection);
                    string title = model.Title;
                    var SiteInfo = siteInfoService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()), transaction: transaction).ToList();
                    if (SiteInfo.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    //
                    string imageFile = string.Empty;
                    HttpPostedFileBase file = model.DocumentFile;
                    try
                    {
                        string _fileExt = System.IO.Path.GetExtension(file.FileName);
                        if (!Helper.Validate.FormatImageFile(_fileExt))
                            return Notifization.Invalid("File không hợp lệ");

                        imageFile = WebCore.AttachmentFile.SaveFile(file, "SITEID", "USERID", true, 0, 0, transaction: transaction);
                    }
                    catch
                    {
                        return Notifization.Invalid("Không thể upload file");
                    }

                    //
                    string imageIcon = string.Empty;
                    HttpPostedFileBase IconFile = model.DocumentIconFile;
                    try
                    {
                        string _fileExt = System.IO.Path.GetExtension(file.FileName);
                        if (!Helper.Validate.FormatImageFile(_fileExt))
                            return Notifization.Invalid("File không hợp lệ");
                        imageIcon = WebCore.AttachmentFile.SaveFile(IconFile, "SITEID", "USERID", true, 0, 0, transaction: transaction);
                    }
                    catch
                    {
                        return Notifization.Invalid("Không thể upload file");
                    }
                    // 
                    if (string.IsNullOrWhiteSpace(imageIcon))
                        imageIcon = "no-img.gif";

                    // create
                    var ID = siteInfoService.Create<string>(new SiteInfo()
                    {
                        Title = model.Title,
                        Alias = Helper.Library.Uni2NONE(model.Title),
                        SiteID = "",
                        ImageFile = imageFile,
                        IconFile = imageIcon,
                        Summary = model.Summary,
                        Email = model.Email,
                        Fax = model.Fax,
                        Phone = model.Phone,
                        Tel = model.Tel,
                        Address = model.Address,
                        Gmaps = model.Gmaps,
                        GoogleAnalytic = model.GoogleAnalytic,
                        Enabled = model.Enabled,
                    }, transaction: transaction);
                    // site id
                    string strPath = string.Empty;
                    transaction.Commit();
                    return Notifization.Success(NotifizationText.CREATE_SUCCESS);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(SiteInfoUpdateFormModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    SiteInfoService siteInfoService = new SiteInfoService(_connection);
                    string title = model.Title;
                    var SiteInfo = siteInfoService.GetAlls(m => m.ID.Equals(model.ID.ToLower()), transaction: transaction).FirstOrDefault();
                    if (SiteInfo == null)
                        return Notifization.NotFound();
                    string menuId = SiteInfo.ID;
                    //
                    var modelTitle = siteInfoService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !m.ID.Equals(model.ID.ToLower()), transaction: transaction).ToList();
                    if (modelTitle.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    HttpPostedFileBase file = model.DocumentFile;
                    string imgFile = SiteInfo.ImageFile;
                    if (file != null)
                    {
                        // update file
                        imgFile = WebCore.AttachmentFile.SaveFile(file, null, null, true, 0, 0, transaction: transaction);
                        if (!Helper.Validate.FormatGuid(imgFile) && !string.IsNullOrEmpty(imgFile))
                            return Notifization.NotFound(imgFile);
                        // delete file
                        var strDelete = WebCore.AttachmentFile.DeleteFile(SiteInfo.ImageFile, transaction: transaction);
                        if (!string.IsNullOrEmpty(strDelete))
                            return Notifization.Invalid(strDelete);
                    }
                    //
                    file = model.DocumentFile;
                    string iconFile = SiteInfo.ImageFile;
                    if (file != null)
                    {
                        // update file
                        iconFile = WebCore.AttachmentFile.SaveFile(file, null, null, true, 0, 0, transaction: transaction);
                        if (!Helper.Validate.FormatGuid(iconFile) && !string.IsNullOrEmpty(iconFile))
                            return Notifization.NotFound(iconFile);
                        // delete file
                        var strDelete = WebCore.AttachmentFile.DeleteFile(SiteInfo.ImageFile, transaction: transaction);
                        if (!string.IsNullOrEmpty(strDelete))
                            return Notifization.Invalid(strDelete);
                    }
                    string strParent = string.Empty;
                    string modelParentId = model.ParentID;
                    string parentId = SiteInfo.ParentID;

                    // update content
                    SiteInfo.ParentID = "";
                    SiteInfo.ImageFile = imgFile;
                    SiteInfo.Title = title;
                    SiteInfo.Summary = model.Summary;
                    SiteInfo.Enabled = model.Enabled;
                    siteInfoService.Update(SiteInfo, transaction: transaction);
                    transaction.Commit();
                    return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        public SiteInfoModel UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_SiteInfor                                 
                                     WHERE ID = @Query";
                var model = _connection.Query<SiteInfoModel>(sqlQuery, new { Query = Id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(SiteInfoIDModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    SiteInfoService siteInfoService = new SiteInfoService(_connection);
                    var SiteInfo = siteInfoService.GetAlls(m => m.ID.Equals(model.ID.ToLower()), transaction: transaction).FirstOrDefault();
                    if (SiteInfo == null)
                        return Notifization.NotFound();
                    // remote menu, children menu
                    siteInfoService.Remove(SiteInfo.ID, transaction: transaction);
                    transaction.Commit();
                    return Notifization.Success(NotifizationText.DELETE_SUCCESS);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public SiteInfo SiteKeyID(IDbTransaction transaction = null)
        {
            try
            {
                string _siteKeyId = WebConfigurationManager.AppSettings["SiteKeyID"];
                var siteInforService = new SiteInfoService(_connection);
                SiteInfo siteInfo = siteInforService.GetAlls(m => m.SiteID.ToLower().Equals(_siteKeyId.ToLower()), transaction).FirstOrDefault();
                if (siteInfo != null)
                    return siteInfo;
                return null;
            }
            catch
            {
                return null;
            }
        }

        //##############################################################################################################################################################################################################################################################

    }
}