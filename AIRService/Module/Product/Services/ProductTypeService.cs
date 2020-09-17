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
using WebCore.ENM;
using Helper.Page;

namespace WebCore.Services
{
    public interface IProductTypeService : IEntityService<ProductType> { }
    public class ProductTypeService : EntityService<ProductType>, IProductTypeService
    {
        public ProductTypeService() : base() { }
        public ProductTypeService(System.Data.IDbConnection db) : base(db) { }
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
            string sqlQuery = @"SELECT * FROM App_ProductType WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ProductTypeResult>(sqlQuery, new { Query = query }).ToList();
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
            // return
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(ProductTypeCreateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();

                    string title = model.Title;
                    string summary = model.Summary;
                    if (string.IsNullOrEmpty(title))
                        return Notifization.Invalid("Không được để trống tiêu đề");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tiêu đề không hợp lệ");
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
                    // summary valid               
                    if (!string.IsNullOrEmpty(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }
                    ProductTypeService productTypeService = new ProductTypeService(_connection);
                    var productType = productTypeService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == model.Title.ToLower(), transaction: _transaction);
                    if (productType.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    var Id = productTypeService.Create<string>(new ProductType()
                    {
                        Title = model.Title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(model.Title),
                        Summary = model.Summary,
                        LanguageID = Helper.Current.UserLogin.LanguageID,
                        Enabled = model.Enabled,
                    }, transaction: _transaction);
                    string temp = string.Empty;

                    //sort
                    _transaction.Commit();
                    return Notifization.Success(MessageText.CreateSuccess);
                }
                catch
                {
                    _transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(ProductTypeUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();

                    string title = model.Title;
                    string summary = model.Summary;
                    if (string.IsNullOrEmpty(title))
                        return Notifization.Invalid("Không được để trống tiêu đề");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tiêu đề không hợp lệ");
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
                    // summary valid               
                    if (!string.IsNullOrEmpty(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }
                    var productTypeService = new ProductTypeService(_connection);
                    string id = model.ID.ToLower();
                    var productType = productTypeService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (productType == null)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    productType = productTypeService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && productType.ID != id, transaction: _transaction).FirstOrDefault();
                    if (productType != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    productType.Title = title;
                    productType.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    productType.Summary = model.Summary;
                    productType.Enabled = model.Enabled;
                    productTypeService.Update(productType, transaction: _transaction);
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
        public ProductType UpdateForm(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_ProductType WHERE ID = @Query";
                return _connection.Query<ProductType>(sqlQuery, new { Query = id }).FirstOrDefault();
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
            id = id.ToLower();
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    var productTypeService = new ProductTypeService(_connection);
                    var _productType = productTypeService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (_productType == null)
                        return Notifization.NotFound();
                    productTypeService.Remove(_productType.ID, transaction: _transaction);
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
        public ActionResult Details(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM App_ProductType WHERE ID = @ID";
                var item = _connection.Query<ProductType>(sqlQuery, new { ID = id }).FirstOrDefault();
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
        public static string DDLProductType(string id)
        {
            try
            {
                string result = string.Empty;
                using (var ProductTypeService = new ProductTypeService())
                {
                    var dtList = ProductTypeService.DataOption(id);
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
        public List<ProductTypeOption> DataOption(string langId)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_ProductType ORDER BY Title ASC";
                return _connection.Query<ProductTypeOption>(sqlQuery, new { LangID = langId }).ToList();
            }
            catch
            {
                return new List<ProductTypeOption>();
            }
        }
    }
}
