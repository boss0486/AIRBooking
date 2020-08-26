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
    public interface IProductService : IEntityService<Product> { }
    public class ProductService : EntityService<Product>, IProductService
    {
        public ProductService() : base() { }
        public ProductService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(ProductSearchModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            string query = model.Query;
            //
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
            int state = model.State;
            if (state > 0)
            {
                whereCondition += " AND State = @State";
            }
            //
            int status = model.Status;
            if (status > 0)
            {
                whereCondition += " AND Enabled = @Enabled";
            }
            int page = model.Page;
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM View_App_Product 
                WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'" + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ViewProduct>(sqlQuery, new { Query = query, CategoryId = categoryId, State = state, Enabled = status }).ToList();
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

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(ProductCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    ProductService productService = new ProductService(_connection);
                    var products = productService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                    if (products.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    ProductCategoryService ProductCategoryService = new ProductCategoryService(_connection);
                    var productCategory = ProductCategoryService.GetAlls(m => m.ID.Equals(model.CategoryID.ToLower()), transaction: transaction).FirstOrDefault();
                    if (productCategory == null)
                        return Notifization.NotFound();
                    //
                    string alias = model.Alias;
                    if (string.IsNullOrEmpty(alias))
                        alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
                    else
                        alias = Helper.Page.Library.FormatToUni2NONE(model.Alias);
                    var aliasData = productService.GetAlls(m => m.Alias.ToLower().Equals(alias.ToLower()), transaction: transaction).ToList();
                    if (aliasData.Count > 0)
                        return Notifization.Invalid("Đường dẫn đã được sử dụng");
                    //
                    string imgFile = model.ImageFile;
                    var Id = productService.Create<string>(new Product()
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
                        Warranty = model.Warranty,
                        Originate = model.Originate,
                        MadeIn = model.MadeIn,
                        Price = model.Price,
                        PriceListed = model.PriceListed,
                        PriceText = model.PriceText,
                        State = model.State,
                        LanguageID = Helper.Current.UserLogin.LanguageID,
                        Enabled = model.Enabled,
                    }, transaction: transaction);
                    // photo : save, insert product file
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
                catch
                {
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(ProductUpdateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    ProductService productService = new ProductService(_connection);
                    string Id = model.ID.ToLower();
                    var product = productService.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                    if (product == null)
                        return Notifization.NotFound(MessageText.NotFound);

                    string title = model.Title;
                    var dpm = productService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !product.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (dpm.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    string alias = model.Alias;
                    if (string.IsNullOrEmpty(alias))
                        alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
                    else
                        alias = Helper.Page.Library.FormatToUni2NONE(model.Alias);
                    var aliasData = productService.GetAlls(m => m.Alias.ToLower().Equals(alias.ToLower()) && !product.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (aliasData.Count > 0)
                        return Notifization.Invalid("Đường dẫn đã được sử dụng");
                    //
                    string imgFile = model.ImageFile;
                    product.CategoryID = model.CategoryID;
                    product.TextID = model.TextID;
                    product.Title = model.Title;
                    product.Alias = alias;
                    product.Summary = model.Summary;
                    product.ImageFile = imgFile;
                    product.HtmlNote = model.HtmlNote;
                    product.HtmlText = model.HtmlText;
                    product.Tag = model.Tag;
                    product.ViewTotal = model.ViewTotal;
                    product.ViewDate = model.ViewDate;
                    product.Warranty = model.Warranty;
                    product.Originate = model.Originate;
                    product.MadeIn = model.MadeIn;
                    product.Price = model.Price;
                    product.PriceListed = model.PriceListed;
                    product.PriceText = model.PriceText;
                    product.State = model.State;
                    product.LanguageID = Helper.Current.UserLogin.LanguageID;
                    product.Enabled = model.Enabled;
                    productService.Update(product, transaction: transaction);
                    // file
                    IList<string> photoFile = model.Photos;
                    Helper.Page.MetaSEO meta = new Helper.Page.MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    var _controllerText = Helper.Page.MetaSEO.ControllerText.ToLower();
                    // get all frorm db
                    IList<string> lstPhotoDb = attachmentIngredientService.GetAlls(m => m.ForID.ToLower().Equals(Id) && m.CategoryID.ToLower().Equals(_controllerText), transaction: transaction).Select(m => m.FileID).ToList();
                    // add new
                    var lstAddNewFile = photoFile;
                    if (lstPhotoDb != null && lstPhotoDb.Count > 0)
                        lstAddNewFile = photoFile.Except(lstPhotoDb).ToList();
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
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        public ViewProduct UpdateForm(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_Product WHERE ID = @Query";
                var item = _connection.Query<ViewProduct>(sqlQuery, new { Query = id }).FirstOrDefault();
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
            if (Id == null)
                return Notifization.NotFound();
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    ProductService ProductService = new ProductService(_connection);
                    var Product = ProductService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (Product == null)
                        return Notifization.NotFound();
                    // delete
                    AttachmentFile.DeleteFile(Product.ImageFile, transaction: transaction);
                    ProductService.Remove(Product.ID, transaction: transaction);
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
        public ActionResult Detail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM View_App_Product WHERE ID = @ID";
                var item = _connection.Query<ViewProduct>(sqlQuery, new { ID = id }).FirstOrDefault();
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
        public static string DDLProduct(string id)
        {
            try
            {
                string result = string.Empty;
                using (var ProductService = new ProductService())
                {
                    var dtList = ProductService.DataOption(id);
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
        public List<ProductOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_App_Product ORDER BY Title ASC";
                return _connection.Query<ProductOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<ProductOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLProductState(int Id)
        {
            try
            {
                var productStateModels = new List<ProductStateModel>{
                    new ProductStateModel(1, "Còn hàng"),
                    new ProductStateModel(2, "Hết hàng")
                };
                string result = string.Empty;
                foreach (var item in productStateModels)
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