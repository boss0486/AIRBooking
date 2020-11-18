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
using WebCore.Model.Services;
using WebCore.Model.Enum;
using Helper.File;

namespace WebCore.Services
{
    public interface IArticleService : IEntityService<Article> { }
    public class ArticleService : EntityService<Article>, IArticleService
    {
        public ArticleService() : base() { }
        public ArticleService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_Article WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ViewArticle>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);

            var resultData = new List<ViewArticle>();
            foreach (var item in dtList)
            {
                resultData.Add(new ViewArticle(item.ID, item.CategoryID, item.CategoryName, item.CategoryAlias, item.Title, item.Alias, item.TextID, item.ImageFile, item.Summary, item.HtmlNote, item.HtmlText, item.Tag, item.ViewTotal, item.ViewDate, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate));
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
        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(ArticleCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    ArticleService articleService = new ArticleService(_connection);
                    var articles = articleService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction).FirstOrDefault();
                    if (articles == null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    string categoryId = model.CategoryID.ToLower();
                    ArticleCategoryService articleCategoryService = new ArticleCategoryService(_connection);
                    var articleCategory = articleCategoryService.GetAlls(m => m.ID == categoryId, transaction: transaction).FirstOrDefault();
                    if (articleCategory == null)
                        return Notifization.NotFound();
                    //
                    string alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
                    string imgFile = model.ImageFile;
                    var Id = articleService.Create<string>(new Article()
                    {
                        CategoryID = categoryId,
                        Title = model.Title,
                        Alias = alias,
                        Summary = model.Summary,
                        ImageFile = imgFile,
                        HtmlNote = string.Empty,
                        HtmlText = model.HtmlText,
                        Tag = model.Tag,
                        ViewTotal = model.ViewTotal,
                        ViewDate = model.ViewDate,
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
                    _connection.Close();
                    return Notifization.Success(MessageText.CreateSuccess);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Notifization.TEST("::" + ex.ToString());
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(ArticleUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    ArticleService articleService = new ArticleService(_connection);
                    string id = model.ID.ToLower();
                    var article = articleService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (article == null)
                        return Notifization.NotFound(MessageText.NotFound);

                    string title = model.Title;
                    article = articleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && article.ID != id, transaction: _transaction).FirstOrDefault();
                    if (article != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    string categoryId = model.CategoryID.ToLower();
                    ArticleCategoryService articleCategoryService = new ArticleCategoryService(_connection);
                    var articleCategory = articleCategoryService.GetAlls(m => m.ID == categoryId, transaction: _transaction).FirstOrDefault();
                    if (articleCategory == null)
                        return Notifization.NotFound();
                    //
                    string alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
                    string imgFile = model.ImageFile;
                    //
                    article.CategoryID = categoryId;
                    article.TextID = model.TextID;
                    article.Title = model.Title;
                    article.Alias = alias;
                    article.Summary = model.Summary;
                    article.ImageFile = imgFile;
                    article.HtmlNote = model.HtmlNote;
                    article.HtmlText = model.HtmlText;
                    article.Tag = model.Tag;
                    article.ViewTotal = model.ViewTotal;
                    article.ViewDate = model.ViewDate;
                    article.LanguageID = Helper.Current.UserLogin.LanguageID;
                    article.Enabled = model.Enabled;
                    articleService.Update(article, transaction: _transaction);
                    IList<string> photoFile = model.Photos;
                    Helper.Page.MetaSEO meta = new Helper.Page.MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    var _controllerText = Helper.Page.MetaSEO.ControllerText.ToLower();
                    // get all frorm db
                    IList<string> lstPhotoDb = attachmentIngredientService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ForID) && m.ForID.ToLower() == id && m.CategoryID.ToLower() == _controllerText, transaction: _transaction).Select(m => m.FileID).ToList();
                    // add new
                    var lstAddNewFile = photoFile;
                    if (lstPhotoDb != null && lstPhotoDb.Count > 0)
                        lstAddNewFile = photoFile.Except(lstPhotoDb).ToList();
                    if (lstAddNewFile != null && lstAddNewFile.Count > 0)
                    {
                        foreach (var item in lstAddNewFile)
                        {
                            var attachmentIngredient = attachmentIngredientService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ForID) && m.FileID.ToLower() == item.ToLower() && m.ForID.ToLower() == id && m.CategoryID.ToLower() == _controllerText, transaction: _transaction).FirstOrDefault();
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
                            var attachmentIngredient = attachmentIngredientService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ForID) && m.FileID.ToLower() == item.ToLower() && m.ForID.ToLower() == id && m.CategoryID.ToLower() == _controllerText, transaction: _transaction).FirstOrDefault();
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
                    return Notifization.TEST("::" + ex);
                }
            }
        }
        public ViewArticle UpdateForm(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_Article WHERE ID = @Query";
                var item = _connection.Query<ViewArticle>(sqlQuery, new { Query = id }).FirstOrDefault();
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
                        ArticleService articleService = new ArticleService(_connectDb);
                        var article = articleService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                        if (article == null)
                            return Notifization.NotFound();
                        // delete
                        AttachmentFile.DeleteFile(article.ImageFile, dbTransaction: _transaction);
                        articleService.Remove(article.ID, transaction: _transaction);
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
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM App_Article WHERE ID = @ID";
                var item = _connection.Query<Article>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(MessageText.NotFound);

                return Notifization.Data(MessageText.Success, data: item, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLArticle(string id)
        {
            try
            {
                string result = string.Empty;
                using (var articleService = new ArticleService())
                {
                    var dtList = articleService.DataOption(id);
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
        public List<ArticleOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_Article ORDER BY Title ASC";
                return _connection.Query<ArticleOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<ArticleOption>();
            }
        }
    }
    //##############################################################################################################################################################################################################################################################
}