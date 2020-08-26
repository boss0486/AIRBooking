using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using Helper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using WebCore.Entities;
using WebCore.ENM;
using WebCore.Model.Enum;
using Helper.File;

namespace WebCore.Services
{
    public interface IAppServiceService : IEntityService<AppService> { }
    public class AppServiceService : EntityService<AppService>, IAppServiceService
    {
        public AppServiceService() : base() { }
        public AppServiceService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(AppServiceSearchModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.Invalid();
                //
                string query = model.Query;
                if (!string.IsNullOrEmpty(query))
                    query = query.Trim();
                else
                    query = "";
                //
                string whereCondition = string.Empty;
                string categoryId = model.CategoryID;
                if (!string.IsNullOrEmpty(categoryId))
                {
                    whereCondition += " AND CategoryID = @CategoryID";
                }
                //
                int status = model.Status;
                if (status > 0)
                {
                    whereCondition += " AND Enabled = @Enabled";
                }
                int page = model.Page;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM View_App_Service 
                WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'" + whereCondition + " ORDER BY [CreatedDate]";
                var dtList = _connection.Query<ViewAppService>(sqlQuery, new { Query = query, CategoryId = categoryId, Enabled = status }).ToList();
                if (dtList.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound + sqlQuery);
                var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
                if (result.Count <= 0 && page > 1)
                {
                    page -= 1;
                    result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
                }
                if (result.Count <= 0)
                    return Notifization.NotFound(MessageText.NotFound);

                Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
                {
                    PageSize = Helper.Pagination.Paging.PAGESIZE,
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
                return Notifization.Data(MessageText.Success, data: result, role: roleAccountModel, paging: pagingModel);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(AppServiceCreateModel model)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        AppServiceService AppServiceService = new AppServiceService(_connection);
                        var AppServices = AppServiceService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                        if (AppServices.Count > 0)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");
                        //
                        AppServiceCategoryService AppServiceCategoryService = new AppServiceCategoryService(_connection);
                        var AppServiceCategory = AppServiceCategoryService.GetAlls(m => m.ID.Equals(model.CategoryID.ToLower()), transaction: transaction).FirstOrDefault();
                        if (AppServiceCategory == null)
                            return Notifization.NotFound();
                        //
                        string alias = model.Alias;
                        if (string.IsNullOrEmpty(alias))
                            alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
                        else
                            alias = Helper.Page.Library.FormatToUni2NONE(model.Alias);
                        var aliasData = AppServiceService.GetAlls(m => m.Alias.ToLower().Equals(alias.ToLower()), transaction: transaction).ToList();
                        if (aliasData.Count > 0)
                            return Notifization.Invalid("Đường dẫn đã được sử dụng");
                        //
                        string imgFile = model.ImageFile;
                        var Id = AppServiceService.Create<string>(new AppService()
                        {
                            CategoryID = model.CategoryID,
                            TextID = model.TextID,
                            Title = model.Title,
                            Alias = alias,
                            Summary = model.Summary,
                            ImageFile = imgFile,
                            HtmlNote = model.HtmlNote,
                            HtmlText = model.HtmlText,
                            Tag = model.Tag,
                            ViewTotal = model.ViewTotal,
                            ViewDate = model.ViewDate,
                            Price = model.Price,
                            PriceListed = model.PriceListed,
                            PriceText = model.PriceText,
                            LanguageID = Helper.Current.UserLogin.LanguageID,
                            Enabled = model.Enabled,
                        }, transaction: transaction);
                        // photo : save, insert AppService file
                        IEnumerable<string> photoFile = model.Photos;
                        Helper.Page.MetaSEO meta = new Helper.Page.MetaSEO();
                        var attachmentIngredientService = new AttachmentIngredientService(_connection);
                        var _controllerText = Helper.Page.MetaSEO.ControllerText.ToLower();
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
                        //
                        if (!string.IsNullOrEmpty(imgFile))
                        {
                            string guid = attachmentIngredientService.Create<string>(new Entities.AttachmentIngredient()
                            {
                                ForID = Id,
                                FileID = imgFile,
                                CategoryID = _controllerText,
                                TypeID = (int)ModelEnum.FileType.ALONE
                            }, transaction: transaction);
                        }
                        //sort
                        transaction.Commit();
                        return Notifization.Success(MessageText.CreateSuccess);
                    }
                    catch (Exception ex)
                    {
                        return Notifization.TEST("::" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex.ToString());
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(AppServiceUpdateModel model)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        AppServiceService AppServiceService = new AppServiceService(_connection);
                        string Id = model.ID.ToLower();
                        var AppService = AppServiceService.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                        if (AppService == null)
                            return Notifization.NotFound(MessageText.NotFound);

                        string title = model.Title;
                        var dpm = AppServiceService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !AppService.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                        if (dpm.Count > 0)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");

                        string alias = model.Alias;
                        if (string.IsNullOrEmpty(alias))
                            alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
                        else
                            alias = Helper.Page.Library.FormatToUni2NONE(model.Alias);
                        var aliasData = AppServiceService.GetAlls(m => m.Alias.ToLower().Equals(alias.ToLower()) && !AppService.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                        if (aliasData.Count > 0)
                            return Notifization.Invalid("Đường dẫn đã được sử dụng");
                        //
                        string imgFile = model.ImageFile;
                        AppService.CategoryID = model.CategoryID;
                        AppService.TextID = model.TextID;
                        AppService.Title = model.Title;
                        AppService.Alias = alias;
                        AppService.Summary = model.Summary;
                        AppService.ImageFile = imgFile;
                        AppService.HtmlNote = model.HtmlNote;
                        AppService.HtmlText = model.HtmlText;
                        AppService.Tag = model.Tag;
                        AppService.ViewTotal = model.ViewTotal;
                        AppService.ViewDate = model.ViewDate;
                        AppService.Price = model.Price;
                        AppService.PriceListed = model.PriceListed;
                        AppService.PriceText = model.PriceText;
                        AppService.LanguageID = Helper.Current.UserLogin.LanguageID;
                        AppService.Enabled = model.Enabled;
                        AppServiceService.Update(AppService, transaction: transaction);
                        // file
                        IList<string> photoFile = model.Photos;
                        Helper.Page.MetaSEO meta = new Helper.Page.MetaSEO();
                        var attachmentIngredientService = new AttachmentIngredientService(_connection);
                        var _controllerText = Helper.Page.MetaSEO.ControllerText.ToLower();
                        // get all frorm db
                        var lstAddNewFile = photoFile;
                        IList<string> lstPhotoDb = attachmentIngredientService.GetAlls(m => m.ForID.ToLower().Equals(Id) && m.CategoryID.ToLower().Equals(_controllerText), transaction: transaction).Select(m => m.FileID).ToList();
                        // add news                      
                        if (lstPhotoDb.Count > 0 && lstAddNewFile != null)
                            lstAddNewFile = photoFile.Except(lstPhotoDb).ToList();
                        //
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
                        if (photoFile != null && photoFile.Count > 0)
                            lstDeleteFile = lstPhotoDb.Except(photoFile).ToList();
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
                        return Notifization.Success(MessageText.UpdateSuccess);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Notifization.TEST("::" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex.ToString());
            }
        }
        public ViewAppService UpdateForm(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_Service WHERE ID = @Query";
                var item = _connection.Query<ViewAppService>(sqlQuery, new { Query = id }).FirstOrDefault();
                // get attachment
                var attachmentService = new AttachmentService(_connection);
                List<ViewAttachment> lstPhoto = attachmentService.AttachmentrListByForID(id);
                if (lstPhoto.Count > 0)
                    item.Photos = lstPhoto;
                return item;
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string Id)
        {
            try
            {
                if (Id == null)
                    return Notifization.NotFound();
                using (var _connectDb = DbConnect.Connection.CMS)
                {
                    _connectDb.Open();
                    using (var transaction = _connectDb.BeginTransaction())
                    {
                        try
                        {
                            AppServiceService AppServiceService = new AppServiceService(_connectDb);
                            var AppService = AppServiceService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                            if (AppService == null)
                                return Notifization.NotFound();
                            // delete
                            AttachmentFile.DeleteFile(AppService.ImageFile, transaction: transaction);
                            AppServiceService.Remove(AppService.ID, transaction: transaction);
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
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Detail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM View_App_Service WHERE ID = @ID";
                var item = _connection.Query<ViewAppService>(sqlQuery, new { ID = id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(MessageText.NotFound);
                // get attachment
                var attachmentService = new AttachmentService(_connection);
                List<ViewAttachment> lstPhoto = attachmentService.AttachmentrListByForID(id);
                if (lstPhoto.Count > 0)
                    item.Photos = lstPhoto;
                return Notifization.Data(MessageText.Success, data: item, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLAppService(string id)
        {
            try
            {
                string result = string.Empty;
                using (var AppServiceService = new AppServiceService())
                {
                    var dtList = AppServiceService.DataOption(id);
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
        public List<AppServiceOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_App_Service ORDER BY Title ASC";
                return _connection.Query<AppServiceOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<AppServiceOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLAppServiceState(int Id)
        {
            try
            {
                var AppServiceStateModels = new List<AppServiceStateModel>{
                    new AppServiceStateModel(1, "Còn hàng"),
                    new AppServiceStateModel(2, "Hết hàng")
                };
                string result = string.Empty;
                foreach (var item in AppServiceStateModels)
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
    //##############################################################################################################################################################################################################################################################

    //##############################################################################################################################################################################################################################################################
}