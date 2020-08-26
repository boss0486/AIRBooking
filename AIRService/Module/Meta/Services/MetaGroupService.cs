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
    public interface IMetaGroupSEOService : IEntityService<MetaGroup> { }
    public class MetaGroupService : EntityService<MetaGroup>, IMetaGroupSEOService
    {
        public MetaGroupService() : base() { }
        public MetaGroupService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM View_App_MetaGroup WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
            var dtList = _connection.Query<MetaGroup>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);

            var resultData = new List<RsMetaGroup>();
            foreach (var item in dtList)
            {
                resultData.Add(new RsMetaGroup(item.ID, item.Alias, item.Title, item.Summary, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate));
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
        public ActionResult Create(MetaGroupCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    MetaGroupService MetaGroupService = new MetaGroupService(_connection);
                    string title = model.Title;
                    var MetaGroups = MetaGroupService.GetAlls(m => m.Title.ToLower() == title.ToLower(), transaction: transaction);
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
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var MetaGroupService = new MetaGroupService(_connection);
                    string Id = model.ID.ToLower();
                    var MetaGroup = MetaGroupService.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                    if (MetaGroup == null)
                        return Notifization.NotFound(MessageText.NotFound);

                    string title = model.Title;
                    var dpm = MetaGroupService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !MetaGroup.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (dpm.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    MetaGroup.Title = title;
                    MetaGroup.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    MetaGroup.Summary = model.Summary;
                    MetaGroup.Enabled = model.Enabled;
                    MetaGroupService.Update(MetaGroup, transaction: transaction);
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
        public MetaGroup UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_MetaGroup WHERE ID = @Query";
                return _connection.Query<MetaGroup>(sqlQuery, new { Query = Id }).FirstOrDefault();
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
                    var MetaGroupService = new MetaGroupService(_connection);
                    var MetaGroup = MetaGroupService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (MetaGroup == null)
                        return Notifization.NotFound();
                    MetaGroupService.Remove(MetaGroup.ID, transaction: transaction);
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
        public ActionResult Detail(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM View_App_MetaGroup WHERE ID = @ID";
                var item = _connection.Query<MetaGroup>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(MessageText.NotFound);
                var result = new RsMetaGroup(item.ID, item.Alias, item.Title, item.Summary, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate);
                return Notifization.Data(MessageText.Success, data: result, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLMetaGroup(string id)
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
        public List<MetaGroupOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_App_MetaGroup ORDER BY Title ASC";
                return _connection.Query<MetaGroupOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<MetaGroupOption>();
            }
        }
    }
}