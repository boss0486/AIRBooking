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
    public interface IMenuCategoryService : IEntityService<MenuCategory> { }
    public class MenuCategoryService : EntityService<MenuCategory>, IMenuCategoryService
    {
        public MenuCategoryService() : base() { }
        public MenuCategoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(SearchModel model)
        {
            string query = model.Query;
            int page = model.Page;
            //
            if (!string.IsNullOrWhiteSpace(query))
                query = model.Query;
            else
                query = "";
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM View_MenuCategory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE('" + query + "') +'%' ORDER BY [CreatedDate]";
            var dtList = _connection.Query<MenuCategoryResult>(sqlQuery, new { Query = query }).ToList();
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
            return Notifization.Data(MessageText.Success + "::" + sqlQuery, data: result, role: roleAccountModel, paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(MenuCategoryCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    MenuCategoryService MenuCategoryService = new MenuCategoryService(_connection);
                    var MenuCategorys = MenuCategoryService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                    if (MenuCategorys.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    var Id = MenuCategoryService.Create<string>(new MenuCategory()
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
        public ActionResult Update(MenuCategoryUpdateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var MenuCategoryService = new MenuCategoryService(_connection);
                    string Id = model.ID.ToLower();
                    var menuCategory = MenuCategoryService.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                    if (menuCategory == null)
                        return Notifization.NotFound(MessageText.NotFound);

                    string title = model.Title;
                    var dpm = MenuCategoryService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !menuCategory.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (dpm.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    menuCategory.Title = title;
                    menuCategory.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    menuCategory.Summary = model.Summary;
                    menuCategory.Enabled = model.Enabled;
                    MenuCategoryService.Update(menuCategory, transaction: transaction);
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
        public MenuCategoryResult UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_MenuCategory WHERE ID = @Query";
                return _connection.Query<MenuCategoryResult>(sqlQuery, new { Query = Id }).FirstOrDefault();
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
                    var MenuCategoryService = new MenuCategoryService(_connection);
                    var menuCategory = MenuCategoryService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (menuCategory == null)
                        return Notifization.NotFound();
                    MenuCategoryService.Remove(menuCategory.ID, transaction: transaction);
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
                string sqlQuery = @"SELECT * FROM View_MenuCategory WHERE ID = @ID";
                var item = _connection.Query<MenuCategoryResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
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
        public static string DropdownList(string id)
        {
            try
            {
                string result = string.Empty;
                using (var service = new MenuCategoryService())
                {
                    var dtList = service.DataOption(id);
                    if (dtList.Count > 0)
                    {
                        int cnt = 0;
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrEmpty(id) && item.ID.Equals(id.ToLower()))
                                select = "selected";
                            else if (cnt == 0)
                                select = "selected";
                            //
                            result += "<option value='" + item.ID + "'" + select + ">" + item.Title + "</option>";
                            cnt++;
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
        public List<MenuCategoryOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_MenuCategory ORDER BY Title ASC";
                return _connection.Query<MenuCategoryOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<MenuCategoryOption>();
            }
        }
    }
}
