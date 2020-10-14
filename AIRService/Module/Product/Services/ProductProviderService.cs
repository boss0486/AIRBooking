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
    public interface IProductProviderService : IEntityService<ProductProvider> { }
    public class ProductProviderService : EntityService<ProductProvider>, IProductProviderService
    {
        public ProductProviderService() : base() { }
        public ProductProviderService(System.Data.IDbConnection db) : base(db) { }
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
            string sqlQuery = @"SELECT * FROM App_ProductProvider WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ProductProviderResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query) }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //

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
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(ProductProviderCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var productProviderService = new ProductProviderService(_connection);
                    var productProviders = productProviderService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                    if (productProviders.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    var Id = productProviderService.Create<string>(new ProductProvider()
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
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(ProductProviderUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    var productProviderService = new ProductProviderService(_connection);
                    string id = model.ID.ToLower();
                    var productProvider = productProviderService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (productProvider == null)
                        return Notifization.NotFound(MessageText.NotFound);

                    string title = model.Title;
                    productProvider = productProviderService.GetAlls(m => m.Title.ToLower() == title.ToLower() && productProvider.ID != id, transaction: _transaction).FirstOrDefault();
                    if (productProvider != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    productProvider.Title = title;
                    productProvider.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    productProvider.Summary = model.Summary;
                    productProvider.Enabled = model.Enabled;
                    productProviderService.Update(productProvider, transaction: _transaction);
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
        public ProductProvider ProductProviderByID(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_ProductProvider WHERE ID = @Query";
                return _connection.Query<ProductProvider>(sqlQuery, new { Query = id }).FirstOrDefault();
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
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    var productProviderService = new ProductProviderService(_connection);
                    var productProviders = productProviderService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (productProviders == null)
                        return Notifization.NotFound();
                    //
                    productProviderService.Remove(productProviders.ID, transaction: _transaction);
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
        //##############################################################################################################################################################################################################################################################
        public ActionResult Detail(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM App_ProductProvider WHERE ID = @ID";
                var item = _connection.Query<ProductProvider>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                return Notifization.Data(MessageText.Success, data: item, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLProductProvider(string id)
        {
            try
            {
                string result = string.Empty;
                using (var productProviderService = new ProductProviderService())
                {
                    var dtList = productProviderService.DataOption(id);
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
        public List<ProductProviderOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_ProductProvider ORDER BY Title ASC";
                return _connection.Query<ProductProviderOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<ProductProviderOption>();
            }
        }
    }
}
