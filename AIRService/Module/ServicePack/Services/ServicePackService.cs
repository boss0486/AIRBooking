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
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface IAppServiceService : IEntityService<ServicePack> { }
    public class ServicePackService : EntityService<ServicePack>, IAppServiceService
    {
        public ServicePackService() : base() { }
        public ServicePackService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(ServicePackSearchModel model)
        {
            try
            {
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
                    TimeExpress = model.TimeExpress,
                    Status = model.Status,
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
                //
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
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM App_ServicePack WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'" + whereCondition + " ORDER BY [CreatedDate]";
                var dtList = _connection.Query<ServicePackResult>(sqlQuery, new { Query = query, CategoryId = categoryId, Enabled = status }).ToList();
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
        public ActionResult Create(ServicePackCreateModel model)
        {
            try
            {
                _connection.Open();
                using (var _transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        string categoryId = model.CategoryID.ToLower();
                        ServicePackService servicePackService = new ServicePackService(_connection);
                        var servicePack = servicePackService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: _transaction).FirstOrDefault();
                        if (servicePack != null)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");
                        //
                        ServicePackCategoryService servicePackCategoryService = new ServicePackCategoryService(_connection);
                        var servicePackCategory = servicePackCategoryService.GetAlls(m => m.ID == categoryId, transaction: _transaction).FirstOrDefault();
                        if (servicePackCategory == null)
                            return Notifization.NotFound();
                        //

                        string imgFile = model.ImageFile;
                        var Id = servicePackService.Create<string>(new ServicePack()
                        {
                            CategoryID = categoryId,
                            TextID = model.TextID,
                            Title = model.Title,
                            Alias = Helper.Page.Library.FormatToUni2NONE(model.Alias),
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
                        }, transaction: _transaction);
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
                                }, transaction: _transaction);
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
                            }, transaction: _transaction);
                        }
                        //sort
                        _transaction.Commit();
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
        public ActionResult Update(ServicePackUpdateModel model)
        {
            try
            {
                _connection.Open();
                using (var _transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        ServicePackService servicePackService = new ServicePackService(_connection);
                        string id = model.ID.ToLower();
                        var servicePack = servicePackService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                        if (servicePack == null)
                            return Notifization.NotFound(MessageText.NotFound);

                        string title = model.Title;
                        servicePack = servicePackService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && servicePack.ID != id, transaction: _transaction).FirstOrDefault();
                        if (servicePack != null)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");

                        string alias = model.Alias;
                        if (string.IsNullOrWhiteSpace(alias))
                            alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
                        else
                            alias = Helper.Page.Library.FormatToUni2NONE(model.Alias);
                        //
                        string imgFile = model.ImageFile;
                        servicePack.CategoryID = model.CategoryID;
                        servicePack.TextID = model.TextID;
                        servicePack.Title = model.Title;
                        servicePack.Alias = alias;
                        servicePack.Summary = model.Summary;
                        servicePack.ImageFile = imgFile;
                        servicePack.HtmlNote = model.HtmlNote;
                        servicePack.HtmlText = model.HtmlText;
                        servicePack.Tag = model.Tag;
                        servicePack.ViewTotal = model.ViewTotal;
                        servicePack.ViewDate = model.ViewDate;
                        servicePack.Price = model.Price;
                        servicePack.PriceListed = model.PriceListed;
                        servicePack.PriceText = model.PriceText;
                        servicePack.LanguageID = Helper.Current.UserLogin.LanguageID;
                        servicePack.Enabled = model.Enabled;
                        servicePackService.Update(servicePack, transaction: _transaction);
                        // file
                        IList<string> photoFile = model.Photos;
                        Helper.Page.MetaSEO meta = new Helper.Page.MetaSEO();
                        var attachmentIngredientService = new AttachmentIngredientService(_connection);
                        var _controllerText = Helper.Page.MetaSEO.ControllerText.ToLower();
                        // get all frorm db
                        var lstAddNewFile = photoFile;
                        IList<string> lstPhotoDb = attachmentIngredientService.GetAlls(m => m.ForID.ToLower() == id && m.CategoryID.ToLower() == _controllerText, transaction: _transaction).Select(m => m.FileID).ToList();
                        // add news                      
                        if (lstPhotoDb.Count > 0 && lstAddNewFile != null)
                            lstAddNewFile = photoFile.Except(lstPhotoDb).ToList();
                        //
                        if (lstAddNewFile != null && lstAddNewFile.Count > 0)
                        {
                            foreach (var item in lstAddNewFile)
                            {
                                var attachmentIngredient = attachmentIngredientService.GetAlls(m => m.FileID.ToLower() == item.ToLower() && m.ForID.ToLower() == id && m.CategoryID.ToLower() == _controllerText, transaction: _transaction).FirstOrDefault();
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
                        if (photoFile != null && photoFile.Count > 0)
                            lstDeleteFile = lstPhotoDb.Except(photoFile).ToList();
                        if (lstDeleteFile.Count > 0)
                        {
                            foreach (var item in lstDeleteFile)
                            {
                                var attachmentIngredient = attachmentIngredientService.GetAlls(m => !string.IsNullOrWhiteSpace(m.FileID) && m.FileID.ToLower() == item.ToLower() && m.ForID.ToLower() == id && m.CategoryID.ToLower() == _controllerText, transaction: _transaction).FirstOrDefault();
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
                    catch (Exception ex)
                    {
                        _transaction.Rollback();
                        return Notifization.TEST("::" + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex.ToString());
            }
        }
        public ServicePackResult UpdateForm(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_ServicePack WHERE ID = @Query";
                var item = _connection.Query<ServicePackResult>(sqlQuery, new { Query = id }).FirstOrDefault();
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
        public ActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Notifization.Invalid(MessageText.Invalid);
                //
                id = id.ToLower();
                using (var _connectDb = DbConnect.Connection.CMS)
                {
                    _connectDb.Open();
                    using (var _transaction = _connectDb.BeginTransaction())
                    {
                        try
                        {
                            ServicePackService servicePackService = new ServicePackService(_connectDb);
                            var servicePack = servicePackService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                            if (servicePack == null)
                                return Notifization.NotFound();
                            // delete
                            AttachmentFile.DeleteFile(servicePack.ImageFile, dbTransaction: _transaction);
                            servicePackService.Remove(servicePack.ID, transaction: _transaction);
                            // remover seo
                            _transaction.Commit();
                            return Notifization.Success(MessageText.DeleteSuccess);
                        }
                        catch
                        {
                            _transaction.Rollback();
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
                string sqlQuery = @"SELECT * FROM App_ServicePack WHERE ID = @ID";
                var item = _connection.Query<ServicePackResult>(sqlQuery, new { ID = id }).FirstOrDefault();
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
                using (var AppServiceService = new ServicePackService())
                {
                    var dtList = AppServiceService.DataOption(id);
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
        public List<ServicePackOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_ServicePack ORDER BY Title ASC";
                return _connection.Query<ServicePackOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<ServicePackOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLAppServiceState(int Id)
        {
            try
            {
                var AppServiceStateModels = new List<ServicePackStateModel>{
                    new ServicePackStateModel(1, "Còn hàng"),
                    new ServicePackStateModel(2, "Hết hàng")
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