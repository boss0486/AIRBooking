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
    public interface IMetaSEOService : IEntityService<Meta> { }
    public class MetaService : EntityService<Meta>, IMetaSEOService
    {
        public MetaService() : base() { }
        public MetaService(System.Data.IDbConnection db) : base(db) { }
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
            string sqlQuery = @"SELECT * FROM App_Meta WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' ORDER BY [CreatedDate]";
            var dtList = _connection.Query<Meta>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
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
        public ActionResult Create(MetaCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();

                    string title = model.MetaTitle;
                    string summary = model.MetaDescription;
                    string metaKeyword = model.MetaKeyword;
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
                    MetaService MetaService = new MetaService(_connection);
                    var Metas = MetaService.GetAlls(m => !string.IsNullOrWhiteSpace(m.MetaTitle) && m.MetaTitle.ToLower() == model.MetaTitle.ToLower(), transaction: transaction);
                    if (Metas.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    var Id = MetaService.Create<string>(new Meta()
                    {
                        MetaTitle = title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(title),
                        MetaDescription = summary,
                        MetaKeyword = metaKeyword,
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
        public ActionResult Update(MetaUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string id = model.ID.ToLower();
                    string title = model.MetaTitle;
                    string summary = model.MetaDescription;
                    string metaKeyword = model.MetaKeyword;
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
                    var MetaService = new MetaService(_connection);
                  
                    var meta = MetaService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (meta == null)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    meta = MetaService.GetAlls(m => !string.IsNullOrWhiteSpace(m.MetaTitle) && m.MetaTitle.ToLower() == title.ToLower() && meta.ID != id, transaction: _transaction).FirstOrDefault();
                    if (meta != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    meta.MetaTitle = title;
                    meta.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    meta.MetaDescription = summary;
                    meta.MetaKeyword = metaKeyword;
                    meta.Enabled = model.Enabled;
                    MetaService.Update(meta, transaction: _transaction);
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
        public Meta UpdateForm(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            string query = string.Empty;
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT TOP (1) * FROM App_Meta WHERE ID = @Query";
            return _connection.Query<Meta>(sqlQuery, new { Query = id }).FirstOrDefault();
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound();
            //
            id = id.ToLower();
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    MetaService metaService = new MetaService(_connection);
                    var meta = metaService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (meta == null)
                        return Notifization.NotFound();
                    //
                    metaService.Remove(meta.ID, transaction: _transaction);
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
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound(MessageText.Invalid);
            //
            id = id.ToLower();
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_Meta WHERE ID = @ID";
            var item = _connection.Query<MetaResult>(sqlQuery, new { ID = id }).FirstOrDefault();
            if (item == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            return Notifization.Data(MessageText.Success, data: item, role: null, paging: null);
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLMeta(string id)
        {
            try
            {
                string result = string.Empty;
                using (var MetaService = new MetaService())
                {
                    var dtList = MetaService.DataOption(id);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                                select = "selected";
                            result += "<option value='" + item.ID + "'" + select + ">" + item.MetaTitle + "</option>";
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
        public List<MetaOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_Meta ORDER BY Title ASC";
                return _connection.Query<MetaOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<MetaOption>();
            }
        }
    }
}