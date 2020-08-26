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
    public interface IProductProviderService : IEntityService<ProductProvider> { }
    public class ProductProviderService : EntityService<ProductProvider>, IProductProviderService
    {
        public ProductProviderService() : base() { }
        public ProductProviderService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT * FROM View_App_ProductProvider WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ProductProvider>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);

            var resultData = new List<RsProductProvider>();
            foreach (var item in dtList)
            {
                resultData.Add(new RsProductProvider(item.ID, item.Title, item.Summary, item.Alias, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate));
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
                        Alias = Helper.Library.Uni2NONE(model.Title),
                        Summary = model.Summary,
                        LanguageID = Current.LanguageID,
                        Enabled = model.Enabled,
                    }, transaction: transaction);
                    string temp = string.Empty;

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
        public ActionResult Update(ProductProviderUpdateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var productProviderService = new ProductProviderService(_connection);
                    string Id = model.ID.ToLower();
                    var productProviders = productProviderService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(Id), transaction: transaction).FirstOrDefault();
                    if (productProviders == null)
                        return Notifization.NotFound(NotifizationText.NotFound);

                    string title = model.Title;
                    var dpm = productProviderService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !productProviders.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (dpm.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    productProviders.Title = title;
                    productProviders.Alias = Helper.Library.Uni2NONE(title);
                    productProviders.Summary = model.Summary;
                    productProviders.Enabled = model.Enabled;
                    productProviderService.Update(productProviders, transaction: transaction);
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
        public ProductProvider UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_ProductProvider WHERE ID = @Query";
                return _connection.Query<ProductProvider>(sqlQuery, new { Query = Id }).FirstOrDefault();
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
                    var productProviderService = new ProductProviderService(_connection);
                    var productProviders = productProviderService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(Id), transaction: transaction).FirstOrDefault();
                    if (productProviders == null)
                        return Notifization.NotFound();
                    productProviderService.Remove(productProviders.ID, transaction: transaction);
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
                string sqlQuery = @"SELECT * FROM View_App_ProductProvider WHERE ID = @ID";
                var item = _connection.Query<ProductProvider>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(NotifizationText.NotFound);
                RsProductProvider result = new RsProductProvider(item.ID, item.Title, item.Summary, item.Alias, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate);
                return Notifization.DATALIST(NotifizationText.Success, data: result, role: null, paging: null);
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

                            if (!string.IsNullOrEmpty(id) && item.ID.ToLower().Equals(id.ToLower()))
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
                string sqlQuery = @"SELECT * FROM View_App_ProductProvider_Option ORDER BY Title ASC";
                return _connection.Query<ProductProviderOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<ProductProviderOption>();
            }
        }
    }
}
