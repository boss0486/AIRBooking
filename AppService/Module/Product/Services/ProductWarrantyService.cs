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
    public interface IProductWarrantyService : IEntityService<ProductWarranty> { }
    public class ProductWarrantyService : EntityService<ProductWarranty>, IProductWarrantyService
    {
        public ProductWarrantyService() : base() { }
        public ProductWarrantyService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT * FROM View_App_ProductWarranty WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ProductWarranty>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);

            var resultData = new List<RsProductWarranty>();
            foreach (var item in dtList)
            {
                resultData.Add(new RsProductWarranty(item.ID, item.Title, item.Summary, item.Alias, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate));
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
        public ActionResult Create(ProductWarrantyCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    ProductWarrantyService productWarrantyService = new ProductWarrantyService(_connection);
                    var ProductWarrantys = productWarrantyService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                    if (ProductWarrantys.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    var Id = productWarrantyService.Create<string>(new ProductWarranty()
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
        public ActionResult Update(ProductWarrantyUpdateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var productWarrantyService = new ProductWarrantyService(_connection);
                    string Id = model.ID.ToLower();
                    var ProductWarranty = productWarrantyService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(Id), transaction: transaction).FirstOrDefault();
                    if (ProductWarranty == null)
                        return Notifization.NotFound(NotifizationText.NotFound);

                    string title = model.Title;
                    var dpm = productWarrantyService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !ProductWarranty.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (dpm.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    ProductWarranty.Title = title;
                    ProductWarranty.Alias = Helper.Library.Uni2NONE(title);
                    ProductWarranty.Summary = model.Summary;
                    ProductWarranty.Enabled = model.Enabled;
                    productWarrantyService.Update(ProductWarranty, transaction: transaction);
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
        public ProductWarranty UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_ProductWarranty WHERE ID = @Query";
                return _connection.Query<ProductWarranty>(sqlQuery, new { Query = Id }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(ProductWarrantyIDModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string id = model.ID;
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound();
            //
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    id = id.ToLower();
                    var productWarrantyService = new ProductWarrantyService(_connection);
                    var ProductWarranty = productWarrantyService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(id), transaction: transaction).FirstOrDefault();
                    if (ProductWarranty == null)
                        return Notifization.NotFound();
                    productWarrantyService.Remove(ProductWarranty.ID, transaction: transaction);
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
                string sqlQuery = @"SELECT * FROM View_App_ProductWarranty WHERE ID = @ID";
                var item = _connection.Query<ProductWarranty>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(NotifizationText.NotFound);
                RsProductWarranty result = new RsProductWarranty(item.ID, item.Title, item.Summary, item.Alias, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate);
                return Notifization.DATALIST(NotifizationText.Success, data: result, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLProductWarranty(string id)
        {
            try
            {
                string result = string.Empty;
                using (var ProductWarrantyService = new ProductWarrantyService())
                {
                    var dtList = ProductWarrantyService.DataOption(id);
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
        public List<ProductWarrantyOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_App_ProductWarranty_Option ORDER BY Title ASC";
                return _connection.Query<ProductWarrantyOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<ProductWarrantyOption>();
            }
        }
    }
}
