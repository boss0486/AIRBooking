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
using Helper.Page;

namespace WebCore.Services
{
    public interface IProductWarrantyService : IEntityService<ProductWarranty> { }
    public class ProductWarrantyService : EntityService<ProductWarranty>, IProductWarrantyService
    {
        public ProductWarrantyService() : base() { }
        public ProductWarrantyService(System.Data.IDbConnection db) : base(db) { }
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
            string sqlQuery = @"SELECT * FROM App_ProductWarranty WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ProductWarrantyResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
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
            Helper.Model.RoleDefaultModel roleDefault = new Helper.Model.RoleDefaultModel
            {
                Create = true,
                Update = true,
                Details = true,
                Delete = true
            };
            return Notifization.Data(MessageText.Success, data: result, role: roleDefault, paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(ProductWarrantyCreateModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
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
            ProductWarrantyService productWarrantyService = new ProductWarrantyService(_connection);
            ProductWarranty productWarrantys = productWarrantyService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower()).FirstOrDefault();
            if (productWarrantys != null)
                return Notifization.Invalid("Tiêu đề đã được sử dụng");

            var productId = productWarrantyService.Create<string>(new ProductWarranty()
            {
                Title = model.Title,
                Alias = Helper.Page.Library.FormatToUni2NONE(model.Title),
                Summary = model.Summary,
                LanguageID = Helper.Current.UserLogin.LanguageID,
                Enabled = model.Enabled,
            });
            return Notifization.Success(MessageText.CreateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(ProductWarrantyUpdateModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);

            string id = model.ID.ToLower();
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
            ProductWarrantyService productWarrantyService = new ProductWarrantyService(_connection);
            var productWarranty = productWarrantyService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (productWarranty == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            productWarranty = productWarrantyService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && productWarranty.ID.ToLower() != id).FirstOrDefault();
            if (productWarranty != null)
                return Notifization.Invalid("Tiêu đề đã được sử dụng");
            // update user information
            productWarranty.Title = title;
            productWarranty.Alias = Helper.Page.Library.FormatToUni2NONE(title);
            productWarranty.Summary = summary;
            productWarranty.Enabled = model.Enabled;
            productWarrantyService.Update(productWarranty);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public ProductWarranty UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_ProductWarranty WHERE ID = @Query";
                return _connection.Query<ProductWarranty>(sqlQuery, new { Query = Id }).FirstOrDefault();
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
                return Notifization.Invalid(MessageText.Invalid);
            //
            id = id.ToLower();
            ProductWarrantyService productWarrantyService = new ProductWarrantyService(_connection);
            var productWarranty = productWarrantyService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (productWarranty == null)
                return Notifization.NotFound();
            //
            productWarrantyService.Remove(productWarranty.ID);
            return Notifization.Success(MessageText.DeleteSuccess);
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
        public List<ProductWarrantyOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_ProductWarranty ORDER BY Title ASC";
                return _connection.Query<ProductWarrantyOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<ProductWarrantyOption>();
            }
        }
    }
}
