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
    public interface IArticleCategoryService : IEntityService<ArticleCategory> { }
    public class ArticleCategoryService : EntityService<ArticleCategory>, IArticleCategoryService
    {
        public ArticleCategoryService() : base() { }
        public ArticleCategoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_ArticleCategory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%'                                          
                                    ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ArticleCategoryResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
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
        public ActionResult Create(ArticleCategoryCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var articleCategoryService = new ArticleCategoryService(_connection);
                    var articleCategorys = articleCategoryService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                    if (articleCategorys.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    var Id = articleCategoryService.Create<string>(new ArticleCategory()
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
        public ActionResult Update(ArticleCategoryUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    string id = model.ID;
                    if (string.IsNullOrWhiteSpace(id))
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    id = id.ToLower();
                    var articleCategoryService = new ArticleCategoryService(_connection);
                    var articleCategory = articleCategoryService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (articleCategory == null)
                        return Notifization.NotFound(MessageText.NotFound);

                    string title = model.Title;
                    articleCategory = articleCategoryService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && articleCategory.ID != id, transaction: _transaction).FirstOrDefault();
                    if (articleCategory != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    articleCategory.Title = title;
                    articleCategory.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    articleCategory.Summary = model.Summary;
                    articleCategory.Enabled = model.Enabled;
                    articleCategoryService.Update(articleCategory, transaction: _transaction);
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
        public ArticleCategory UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_ArticleCategory WHERE ID = @Query";
                return _connection.Query<ArticleCategory>(sqlQuery, new { Query = Id }).FirstOrDefault();
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
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    var articleCategoryService = new ArticleCategoryService(_connection);
                    var articleCategory = articleCategoryService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (articleCategory == null)
                        return Notifization.NotFound();
                    articleCategoryService.Remove(articleCategory.ID, transaction: _transaction);
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
                string sqlQuery = @"SELECT * FROM App_ArticleCategory WHERE ID = @ID";
                var item = _connection.Query<ArticleCategoryResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
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
        public static string DDLArticleCategory(string id)
        {
            try
            {
                string result = string.Empty;
                using (var ArticleCategoryService = new ArticleCategoryService())
                {
                    var dtList = ArticleCategoryService.DataOption(id);
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
        public List<ArticleCategoryOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_ArticleCategory ORDER BY Title ASC";
                return _connection.Query<ArticleCategoryOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<ArticleCategoryOption>();
            }
        }
    }
}
