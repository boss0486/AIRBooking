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
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface IServicePackCategoryService : IEntityService<ServicePackCategory> { }
    public class ServicePackCategoryService : EntityService<ServicePackCategory>, IServicePackCategoryService
    {
        public ServicePackCategoryService() : base() { }
        public ServicePackCategoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
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
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_ServiceCategory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ServicePackCategory>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
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
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
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
                        ServicePackCategoryService AppServiceCategoryService = new ServicePackCategoryService(_connection);
                        var AppServiceCategorys = AppServiceCategoryService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                        if (AppServiceCategorys.Count > 0)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");

                        var Id = AppServiceCategoryService.Create<string>(new ServicePackCategory()
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
                        var servicePackCategoryService = new ServicePackCategoryService(_connection);
                        string id = model.ID.ToLower();
                        var servicePackCategory = servicePackCategoryService.GetAlls(m => m.ID == id, transaction: transaction).FirstOrDefault();
                        if (servicePackCategory == null)
                            return Notifization.NotFound(MessageText.NotFound);
                        //
                        string title = model.Title;
                        servicePackCategory = servicePackCategoryService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && servicePackCategory.ID != id, transaction: transaction).FirstOrDefault();
                        if (servicePackCategory != null)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");
                        // update user information
                        servicePackCategory.Title = title;
                        servicePackCategory.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                        servicePackCategory.Summary = model.Summary;
                        servicePackCategory.Enabled = model.Enabled;
                        servicePackCategoryService.Update(servicePackCategory, transaction: transaction);
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
        public ServicePackCategory UpdateForm(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_ServiceCategory WHERE ID = @Query";
                return _connection.Query<ServicePackCategory>(sqlQuery, new { Query = id }).FirstOrDefault();
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
                    return Notifization.NotFound();
                //
                id = id.ToLower();
                _connection.Open();
                using (var _transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        var servicePackCategoryService = new ServicePackCategoryService(_connection);
                        var servicePackCategory = servicePackCategoryService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                        if (servicePackCategory == null)
                            return Notifization.NotFound();
                        servicePackCategoryService.Remove(servicePackCategory.ID, transaction: _transaction);
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
                using (var AppServiceCategoryService = new ServicePackCategoryService())
                {
                    var dtList = AppServiceCategoryService.DataOption(id);
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
        public List<AppServiceCategoryOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_ServiceCategory ORDER BY Title ASC";
                return _connection.Query<AppServiceCategoryOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<AppServiceCategoryOption>();
            }
        }
    }
}
