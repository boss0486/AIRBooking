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
    public interface IAttachmentCategoryService : IEntityService<AttachmentCategory> { }
    public class AttachmentCategoryService : EntityService<AttachmentCategory>, IAttachmentCategoryService
    {
        public AttachmentCategoryService() : base() { }
        public AttachmentCategoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
            try
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
                    Status = model.Status,
                    TimeExpress = model.TimeExpress,
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
                string sqlQuery = @"SELECT * FROM AttachmentCategory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%'  ORDER BY [CreatedDate]";
                var dtList = _connection.Query<AttachmentCategoryResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
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
            catch
            {
                return Notifization.NotService;
            }
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(AttachmentCategoryCreateModel model)
        {
            try
            {
                var AttachmentCategoryService = new AttachmentCategoryService(_connection);
                var AttachmentCategorys = AttachmentCategoryService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower());
                if (AttachmentCategorys.Count > 0)
                    return Notifization.Invalid("Tiêu đề đã được sử dụng");

                var Id = AttachmentCategoryService.Create<string>(new AttachmentCategory()
                {
                    Title = model.Title,
                    Alias = Helper.Page.Library.FormatToUni2NONE(model.Title),
                    Summary = model.Summary,
                    ControllerID = model.ControllerID,
                    LanguageID = Helper.Current.UserLogin.LanguageID,
                    Enabled = model.Enabled,
                });
                string temp = string.Empty;
                return Notifization.Success(MessageText.CreateSuccess);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(AttachmentCategoryUpdateModel model)
        {
            try
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
                        var attachmentCategoryService = new AttachmentCategoryService(_connection);
                        var attachmentCategory = attachmentCategoryService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                        if (attachmentCategory == null)
                            return Notifization.NotFound(MessageText.NotFound);

                        string title = model.Title;
                        attachmentCategory = attachmentCategoryService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && attachmentCategory.ID != id, transaction: _transaction).FirstOrDefault();
                        if (attachmentCategory != null)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");
                        // update user information
                        attachmentCategory.Title = title;
                        attachmentCategory.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                        attachmentCategory.Summary = model.Summary;
                        attachmentCategory.ControllerID = model.ControllerID;
                        attachmentCategory.Enabled = model.Enabled;
                        attachmentCategoryService.Update(attachmentCategory, transaction: _transaction);
                        _transaction.Commit();
                        return Notifization.Success(MessageText.UpdateSuccess);
                    }
                    catch (Exception ex)
                    {
                        _transaction.Rollback();
                        return Notifization.NotService;
                    }
                }
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }
        public AttachmentCategory UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM AttachmentCategory WHERE ID = @Query";
                return _connection.Query<AttachmentCategory>(sqlQuery, new { Query = Id }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Notifization.NotFound(MessageText.Invalid);
                //
                id = id.ToLower();
                _connection.Open();
                using (var _transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        var AttachmentCategoryService = new AttachmentCategoryService(_connection);
                        var AttachmentCategory = AttachmentCategoryService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                        if (AttachmentCategory == null)
                            return Notifization.NotFound();
                        AttachmentCategoryService.Remove(AttachmentCategory.ID, transaction: _transaction);
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
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLAttachmentCategory(string id)
        {
            try
            {
                string result = string.Empty;
                using (var AttachmentCategoryService = new AttachmentCategoryService())
                {
                    var dtList = AttachmentCategoryService.DataOption(id);
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
        public List<AttachmentCategoryOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM AttachmentCategory ORDER BY Title ASC";
                return _connection.Query<AttachmentCategoryOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<AttachmentCategoryOption>();
            }
        }
    }
}
