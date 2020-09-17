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
using Helper.Page;
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface IBannerService : IEntityService<Banner> { }
    public class BannerService : EntityService<Banner>, IBannerService
    {
        public BannerService() : base() { }
        public BannerService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(SearchModel model)
        {
            #region
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            int page = model.Page;
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            //
            string whereCondition = string.Empty;
            //
            SearchResult searchResult = WebCore.Model.Services.ModelService.SearchDefault(new SearchModel
            {
                Query = model.Query,
                Status = model.Status,
                TimeExpress = model.TimeExpress,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Page = model.Page,
                AreaID = model.AreaID,
                TimeZoneLocal = model.TimeZoneLocal
            });
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            #endregion
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_Banner WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ViewBanner>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query) }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);

            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
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
                    var banners = bannerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                    if (banners.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    var Id = bannerService.Create<string>(new Banner()
                    {
                        Title = model.Title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(model.Title),
                        Summary = model.Summary,

                        LocationID = model.LocationID,
                        LanguageID = Helper.Current.UserLogin.LanguageID,
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
                    return Notifization.Success(MessageText.CreateSuccess);
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
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    string id = model.ID;
                    if (string.IsNullOrWhiteSpace(id))
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    id = id.ToLower();
                    var bannerService = new BannerService(_connection);
                    var banner = bannerService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (banner == null)
                        return Notifization.NotFound(MessageText.NotFound);

                    string title = model.Title;
                    banner = bannerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && banner.ID != id, transaction: _transaction).FirstOrDefault();
                    if (banner != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    banner.Title = title;
                    banner.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    banner.Summary = model.Summary;
                    banner.LocationID = model.LocationID;
                    banner.Enabled = model.Enabled;
                    bannerService.Update(banner, transaction: _transaction);
                    IList<string> photos = model.Photos;
                    MetaSEO meta = new MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    var _controllerText = MetaSEO.ControllerText.ToLower();
                    // get all frorm db
                    IList<string> lstPhotoDb = attachmentIngredientService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ForID) && m.ForID.ToLower() == id && m.CategoryID.ToLower() == _controllerText, transaction: _transaction).Select(m => m.FileID).ToList();
                    // add new
                    var lstAddNewFile = photos;
                    if (lstPhotoDb != null && lstPhotoDb.Count > 0)
                        lstAddNewFile = photos.Except(lstPhotoDb).ToList();
                    if (lstAddNewFile != null && lstAddNewFile.Count > 0)
                    {
                        foreach (var item in lstAddNewFile)
                        {
                            var attachmentIngredient = attachmentIngredientService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.FileID.ToLower() == item.ToLower() && m.ForID.ToLower() == id && m.CategoryID.ToLower() == _controllerText, transaction: _transaction).FirstOrDefault();
                            if (attachmentIngredient == null)
                            {
                                string guid = attachmentIngredientService.Create<string>(new Entities.AttachmentIngredient()
                                {
                                    ForID = id,
                                    FileID = item,
                                    CategoryID = _controllerText,
                                    TypeID = (int)ModelEnum.FileType.MULTI
                                }, transaction: _transaction);
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
                            var attachmentIngredient = attachmentIngredientService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.FileID.ToLower() == item.ToLower() && m.ForID.ToLower() == id && m.CategoryID.ToLower() == _controllerText, transaction: _transaction).FirstOrDefault();
                            if (attachmentIngredient != null)
                            {
                                attachmentIngredientService.Remove(attachmentIngredient.ID, transaction: _transaction);
                            }
                        }
                    }
                    //
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch
                {
                    _transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        public ViewBanner UpdateForm(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_Banner WHERE ID = @Query";
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
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound();
            //
            id = id.ToLower();
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var bannerService = new BannerService(_connection);
                    var banner = bannerService.GetAlls(m => m.ID == id, transaction: transaction).FirstOrDefault();
                    if (banner == null)
                        return Notifization.NotFound();
                    bannerService.Remove(banner.ID, transaction: transaction);
                    // remover seo
                    transaction.Commit();
                    return Notifization.Success(MessageText.DeleteSuccess);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
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
                    var dtList = BannerService.DataOption();
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
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
        public List<BannerOption> DataOption()
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_Banner ORDER BY Title ASC";
                return _connection.Query<BannerOption>(sqlQuery, new { }).ToList();
            }
            catch
            {
                return new List<BannerOption>();
            }
        }
        public static string DDLBannerLoaction(int id)
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
                    if (item.ID == id)
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
