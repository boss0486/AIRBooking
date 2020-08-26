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

namespace WebCore.Services
{
    public interface IAppServiceCategoryService : IEntityService<AppServiceCategory> { }
    public class AppServiceCategoryService : EntityService<AppServiceCategory>, IAppServiceCategoryService
    {
        public AppServiceCategoryService() : base() { }
        public AppServiceCategoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            try
            {
                string query = string.Empty;
                if (string.IsNullOrEmpty(strQuery))
                    query = "";
                else
                    query = strQuery;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM View_App_ServiceCategory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
                var dtList = _connection.Query<AppServiceCategory>(sqlQuery, new { Query = query }).ToList();
                if (dtList.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);

                var resultData = new List<RsAppServiceCategory>();
                foreach (var item in dtList)
                {
                    resultData.Add(new RsAppServiceCategory(item.ID, item.Title, item.Summary, item.Alias, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate));
                }
                var result = resultData.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
                if (result.Count <= 0 && page > 1)
                {
                    page -= 1;
                    result = resultData.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
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
            catch
            {
                return Notifization.NotService;
            }
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(AppServiceCategoryCreateModel model)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        AppServiceCategoryService AppServiceCategoryService = new AppServiceCategoryService(_connection);
                        var AppServiceCategorys = AppServiceCategoryService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                        if (AppServiceCategorys.Count > 0)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");

                        var Id = AppServiceCategoryService.Create<string>(new AppServiceCategory()
                        {
                            Title = model.Title,
                            Alias = Helper.Page.Library.FormatToUni2NONE(model.Title),
                            Summary = model.Summary,
                            LanguageID = Helper.Current.UserLogin.LanguageID,
                            Enabled = model.Enabled,
                        }, transaction: transaction);
                        string temp = string.Empty;

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
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(AppServiceCategoryUpdateModel model)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        var AppServiceCategoryService = new AppServiceCategoryService(_connection);
                        string Id = model.ID.ToLower();
                        var AppServiceCategory = AppServiceCategoryService.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                        if (AppServiceCategory == null)
                            return Notifization.NotFound(MessageText.NotFound);

                        string title = model.Title;
                        var dpm = AppServiceCategoryService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !AppServiceCategory.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                        if (dpm.Count > 0)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");
                        // update user information
                        AppServiceCategory.Title = title;
                        AppServiceCategory.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                        AppServiceCategory.Summary = model.Summary;
                        AppServiceCategory.Enabled = model.Enabled;
                        AppServiceCategoryService.Update(AppServiceCategory, transaction: transaction);
                        transaction.Commit();
                        return Notifization.Success(MessageText.UpdateSuccess);
                    }
                    catch
                    {
                        transaction.Rollback();
                        return Notifization.NotService;
                    }
                }
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        public AppServiceCategory UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_ServiceCategory WHERE ID = @Query";
                return _connection.Query<AppServiceCategory>(sqlQuery, new { Query = Id }).FirstOrDefault();
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
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        var AppServiceCategoryService = new AppServiceCategoryService(_connection);
                        var AppServiceCategory = AppServiceCategoryService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                        if (AppServiceCategory == null)
                            return Notifization.NotFound();
                        AppServiceCategoryService.Remove(AppServiceCategory.ID, transaction: transaction);
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
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Detail(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM View_App_ServiceCategory WHERE ID = @ID";
                var item = _connection.Query<AppServiceCategory>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(MessageText.NotFound);
                RsAppServiceCategory result = new RsAppServiceCategory(item.ID, item.Title, item.Summary, item.Alias, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate);
                return Notifization.Data(MessageText.Success, data: result, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLAppServiceCategory(string id)
        {
            try
            {
                string result = string.Empty;
                using (var AppServiceCategoryService = new AppServiceCategoryService())
                {
                    var dtList = AppServiceCategoryService.DataOption(id);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrEmpty(id) && item.ID.Equals(id.ToLower()))
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
        public List<AppServiceCategoryOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_App_ServiceCategory ORDER BY Title ASC";
                return _connection.Query<AppServiceCategoryOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<AppServiceCategoryOption>();
            }
        }
    }
}
