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
    public interface IProductCategoryService : IEntityService<ProductCategory> { }
    public class ProductCategoryService : EntityService<ProductCategory>, IProductCategoryService
    {
        public ProductCategoryService() : base() { }
        public ProductCategoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM View_App_ProductCategory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ProductCategoryResult>(sqlQuery, new { Query = query }).ToList();
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

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(ProductCategoryCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    ProductCategoryService ProductCategoryService = new ProductCategoryService(_connection);
                    var ProductCategorys = ProductCategoryService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                    if (ProductCategorys.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    var Id = ProductCategoryService.Create<string>(new ProductCategory()
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
        public ActionResult Update(ProductCategoryUpdateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var productCategoryService = new ProductCategoryService(_connection);
                    string Id = model.ID.ToLower();
                    var productCategory = productCategoryService.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                    if (productCategory == null)
                        return Notifization.NotFound(MessageText.NotFound);

                    string title = model.Title;
                    var dpm = productCategoryService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !productCategory.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (dpm.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    productCategory.Title = title;
                    productCategory.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    productCategory.Summary = model.Summary;
                    productCategory.Enabled = model.Enabled;
                    productCategoryService.Update(productCategory, transaction: transaction);
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
        public ProductCategory UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_ProductCategory WHERE ID = @Query";
                return _connection.Query<ProductCategory>(sqlQuery, new { Query = Id }).FirstOrDefault();
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
                    var productCategoryService = new ProductCategoryService(_connection);
                    var productCategory = productCategoryService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (productCategory == null)
                        return Notifization.NotFound();
                    productCategoryService.Remove(productCategory.ID, transaction: transaction);
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
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM View_App_ProductCategory WHERE ID = @ID";
                var item = _connection.Query<ProductCategoryResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
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
        public static string DDLProductCategory(string id)
        {
            try
            {
                string result = string.Empty;
                using (var ProductCategoryService = new ProductCategoryService())
                {
                    var dtList = ProductCategoryService.DataOption(id);
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
        public List<ProductCategoryOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_App_ProductCategory ORDER BY Title ASC";
                return _connection.Query<ProductCategoryOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<ProductCategoryOption>();
            }
        }
    }
}
