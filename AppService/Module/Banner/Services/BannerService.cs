using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using Helper;
using System.Web;
using WebCore.Entities;
using WebCore.Model.Enum;

namespace WebCore.Services
{
    public interface IBannerService : IEntityService<Banner> { }
    public class BannerService : EntityService<Banner>, IBannerService
    {
        public BannerService() : base() { }
        public BannerService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT * FROM View_App_Banner WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ViewBanner>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(NotifizationText.NotFound);
            var result = dtList.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
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
        public ActionResult Create(BannerCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var bannerService = new BannerService(_connection);
                    var banners = bannerService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                    if (banners.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    var Id = bannerService.Create<string>(new Banner()
                    {
                        Title = model.Title,
                        Alias = Helper.Library.Uni2NONE(model.Title),
                        Summary = model.Summary,

                        LocationID = model.LocationID,
                        LanguageID = Current.LanguageID,
                        Enabled = model.Enabled,
                    }, transaction: transaction);
                    string temp = string.Empty;
                    // photo : save, insert product file
                    IEnumerable<string> photoFile = model.Photos;
                    MetaSEO meta = new MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    var _controllerText = MetaSEO.ControllerText.ToLower();
                    if (photoFile != null && photoFile.Count() > 0)
                    {
                        foreach (var item in photoFile)
                        {
                            string guid = attachmentIngredientService.Create<string>(new Entities.AttachmentIngredient()
                            {
                                ForID = Id,
                                FileID = item,
                                CategoryID = _controllerText,
                                TypeID = (int)ModelEnum.FileType.MULTI
                            }, transaction: transaction);
                        }
                    }
                    //sort
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
        public ActionResult Update(BannerUpdateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string Id = model.ID;
                    if (string.IsNullOrEmpty(Id))
                        return Notifization.NotFound(NotifizationText.NotFound);
                    Id = Id.ToLower();
                    var bannerService = new BannerService(_connection);
                    var banner = bannerService.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                    if (banner == null)
                        return Notifization.NotFound(NotifizationText.NotFound);

                    string title = model.Title;
                    var dpm = bannerService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !banner.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (dpm.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    banner.Title = title;
                    banner.Alias = Helper.Library.Uni2NONE(title);
                    banner.Summary = model.Summary;
                    banner.LocationID = model.LocationID;
                    banner.Enabled = model.Enabled;
                    bannerService.Update(banner, transaction: transaction);
                    IList<string> photos = model.Photos;
                    MetaSEO meta = new MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    var _controllerText = MetaSEO.ControllerText.ToLower();
                    // get all frorm db
                    IList<string> lstPhotoDb = attachmentIngredientService.GetAlls(m => m.ForID.ToLower().Equals(Id) && m.CategoryID.ToLower().Equals(_controllerText), transaction: transaction).Select(m => m.FileID).ToList();
                    // add new
                    var lstAddNewFile = photos;
                    if (lstPhotoDb != null && lstPhotoDb.Count > 0)
                        lstAddNewFile = photos.Except(lstPhotoDb).ToList();
                    if (lstAddNewFile != null && lstAddNewFile.Count > 0)
                    {
                        foreach (var item in lstAddNewFile)
                        {
                            var attachmentIngredient = attachmentIngredientService.GetAlls(m => m.FileID.ToLower().Equals(item.ToLower()) && m.ForID.ToLower().Equals(Id) && m.CategoryID.ToLower().Equals(_controllerText), transaction: transaction).FirstOrDefault();
                            if (attachmentIngredient == null)
                            {
                                string guid = attachmentIngredientService.Create<string>(new Entities.AttachmentIngredient()
                                {
                                    ForID = Id,
                                    FileID = item,
                                    CategoryID = _controllerText,
                                    TypeID = (int)ModelEnum.FileType.MULTI
                                }, transaction: transaction);
                            }
                        }
                    }
                    // delete
                    var lstDeleteFile = new List<string>();
                    if (photos != null && photos.Count > 0)
                        lstDeleteFile = lstPhotoDb.Except(photos).ToList();
                    if (lstDeleteFile.Count > 0)
                    {
                        foreach (var item in lstDeleteFile)
                        {
                            var attachmentIngredient = attachmentIngredientService.GetAlls(m => m.FileID.ToLower().Equals(item.ToLower()) && m.ForID.ToLower().Equals(Id) && m.CategoryID.ToLower().Equals(_controllerText), transaction: transaction).FirstOrDefault();
                            if (attachmentIngredient != null)
                            {
                                attachmentIngredientService.Remove(attachmentIngredient.ID, transaction: transaction);
                            }
                        }
                    }
                    //
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
        public ViewBanner UpdateForm(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;
                string query = string.Empty;
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_Banner WHERE ID = @Query";
                var dtItem = _connection.Query<ViewBanner>(sqlQuery, new { Query = id }).FirstOrDefault();
                // get attachment
                var attachmentService = new AttachmentService(_connection);
                List<ViewAttachment> lstPhoto = attachmentService.AttachmentrListByForID(id);
                if (lstPhoto.Count > 0)
                    dtItem.Photos = lstPhoto;
                return dtItem;
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string Id)
        {
            if (Id == null)
                return Notifization.NotFound();
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var bannerService = new BannerService(_connection);
                    var banner = bannerService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (banner == null)
                        return Notifization.NotFound();
                    bannerService.Remove(banner.ID, transaction: transaction);
                    // remover seo
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
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(NotifizationText.Invalid);
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT * FROM View_App_Banner WHERE ID = @ID";
                var dtItem = _connection.Query<ViewBanner>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (dtItem == null)
                    return Notifization.NotFound(NotifizationText.NotFound);
                return Notifization.DATALIST(NotifizationText.Success, data: dtItem, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLBanner(string id)
        {
            try
            {
                string result = string.Empty;
                using (var BannerService = new BannerService())
                {
                    var dtList = BannerService.DataOption(id);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (item.ID.Equals(id.ToLower()))
                                select = "selected";
                            result += "<option value='" + item.ID + "'" + select + ">" + item.Title + "</option>";
                        }
                    }
                    return result;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        public List<BannerOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_App_Banner_Option ORDER BY Title ASC";
                return _connection.Query<BannerOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<BannerOption>();
            }
        }
        public static string DDLBannerLoaction(int Id)
        {
            try
            {
                var bannerLocationModels = new List<BannerLocationModel>{
                    new BannerLocationModel(1, "Banner chính"),
                    new BannerLocationModel(2, "Nội dung trên"),
                    new BannerLocationModel(3, "Nội dung dưới"),
                    new BannerLocationModel(4, "Nội dung trái"),
                    new BannerLocationModel(5, "Nội dung phải"),
                    new BannerLocationModel(6, "Dọc bên trái"),
                    new BannerLocationModel(7, "Dọc bên phải"),
                };
                string result = string.Empty;
                foreach (var item in bannerLocationModels)
                {
                    string selected = string.Empty;
                    if (item.ID == Id)
                        selected = "selected";
                    result += "<option value='" + item.ID + "' " + selected + ">" + item.Title + "</option>";
                }
                return result;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
