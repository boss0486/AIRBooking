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
    public interface IAppAreaService : IEntityService<AppArea> { }
    public class AppAreaService : EntityService<AppArea>, IAppAreaService
    {
        public AppAreaService() : base() { }
        public AppAreaService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT * FROM View_App_Area WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
            var dtList = _connection.Query<RsAppArea>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);
            //    
            var result = dtList.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
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
        public ActionResult Create(AppAreaCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    AppAreaService appAreaService = new AppAreaService(_connection);
                    var appAreas = appAreaService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                    if (appAreas.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    var id = appAreaService.Create<string>(new AppArea()
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
        public ActionResult Update(AppAreaUpdateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var appAreaService = new AppAreaService(_connection);
                    string id = model.ID.ToLower();
                    var appArea = appAreaService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(id), transaction: transaction).FirstOrDefault();
                    if (appArea == null)
                        return Notifization.NotFound(NotifizationText.NotFound);

                    string title = model.Title;
                    var dpm = appAreaService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !appArea.ID.ToLower().Equals(id), transaction: transaction).ToList();
                    if (dpm.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    appArea.Title = title;
                    appArea.Alias = Helper.Library.Uni2NONE(title);
                    appArea.Summary = model.Summary;
                    appArea.Enabled = model.Enabled;
                    appAreaService.Update(appArea, transaction: transaction);
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
        public AppArea UpdateForm(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return null;
            string query = string.Empty;
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT TOP (1) * FROM View_App_Area WHERE ID = @Query";
            return _connection.Query<AppArea>(sqlQuery, new { Query = Id }).FirstOrDefault();
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(AppAreaIDModel model)
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
                    var appAreaService = new AppAreaService(_connection);
                    var appArea = appAreaService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(id), transaction: transaction).FirstOrDefault();
                    if (appArea == null)
                        return Notifization.NotFound();
                    appAreaService.Remove(appArea.ID, transaction: transaction);
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
        public ActionResult Details(AppAreaIDModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string id = model.ID;
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound();
            //
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT * FROM View_App_Area WHERE ID = @ID";
            var result = _connection.Query<RsAppArea>(sqlQuery, new { ID = id }).FirstOrDefault();
            if (result == null)
                return Notifization.NotFound(NotifizationText.NotFound);
            //
            return Notifization.DATALIST(NotifizationText.Success, data: result, role: null, paging: null);
        }
        //##############################################################################################################################################################################################################################################################
        public static string AreaDropdownList(string id)
        {
            try
            {
                string result = string.Empty;
                using (var AppAreaService = new AppAreaService())
                {
                    var dtList = AppAreaService.DataOption(id);
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
        public List<AppAreaOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_App_Area ORDER BY Title ASC";
                return _connection.Query<AppAreaOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<AppAreaOption>();
            }
        }
        //Static function
        // ##############################################################################################################################################################################################################################################################
        public static string GetAreaName(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return string.Empty;
                //
                AppAreaService appAreaService = new AppAreaService();
                var appArea = appAreaService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(id.ToLower())).FirstOrDefault();
                if (appArea == null)
                    return string.Empty;
                // 
                return appArea.Title;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
