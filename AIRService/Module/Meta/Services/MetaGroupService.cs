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
    public interface IMetaGroupSEOService : IEntityService<MetaGroup> { }
    public class MetaGroupService : EntityService<MetaGroup>, IMetaGroupSEOService
    {
        public MetaGroupService() : base() { }
        public MetaGroupService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(SearchModel model)
        {
            #region
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
            #endregion
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_MetaGroup WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY [CreatedDate]";
            var dtList = _connection.Query<MetaGroup>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query) }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(MetaGroupCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    MetaGroupService MetaGroupService = new MetaGroupService(_connection);
                    string title = model.Title;
                    var MetaGroups = MetaGroupService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.Title.ToLower() == title.ToLower(), transaction: transaction);
                    if (MetaGroups.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    var Id = MetaGroupService.Create<string>(new MetaGroup()
                    {
                        Title = title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(title),
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
        public ActionResult Update(MetaGroupUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    var metaGroupService = new MetaGroupService(_connection);
                    string id = model.ID.ToLower();
                    var metaGroup = metaGroupService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (metaGroup == null)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    string title = model.Title;
                    metaGroup = metaGroupService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && metaGroup.ID != id, transaction: _transaction).FirstOrDefault();
                    if (metaGroup != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    metaGroup.Title = title;
                    metaGroup.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    metaGroup.Summary = model.Summary;
                    metaGroup.Enabled = model.Enabled;
                    metaGroupService.Update(metaGroup, transaction: _transaction);
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
        public MetaGroup UpdateForm(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_MetaGroup WHERE ID = @Query";
                return _connection.Query<MetaGroup>(sqlQuery, new { Query = id }).FirstOrDefault();
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
                return Notifization.Invalid();
            //
            id = id.ToLower();
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    MetaGroupService metaGroupService = new MetaGroupService(_connection);
                    MetaGroup metaGroup = metaGroupService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (metaGroup == null)
                        return Notifization.NotFound();
                    //
                    metaGroupService.Remove(metaGroup.ID, transaction: _transaction);
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
                if (string.IsNullOrWhiteSpace(id))
                    return Notifization.Invalid();
                //
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM App_MetaGroup WHERE ID = @ID";
                var item = _connection.Query<MetaGroupResult>(sqlQuery, new { ID = id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                return Notifization.Data(MessageText.Success, data: item);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DropDownListMetaGroup(string id)
        {
            try
            {
                string result = string.Empty;
                using (var MetaGroupService = new MetaGroupService())
                {
                    var dtList = MetaGroupService.DataOption(id);
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
        public List<MetaGroupOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_MetaGroup ORDER BY Title ASC";
                return _connection.Query<MetaGroupOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<MetaGroupOption>();
            }
        }
    }
}