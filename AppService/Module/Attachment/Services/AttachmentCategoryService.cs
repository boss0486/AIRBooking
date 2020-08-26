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
    public interface IAttachmentCategoryService : IEntityService<AttachmentCategory> { }
    public class AttachmentCategoryService : EntityService<AttachmentCategory>, IAttachmentCategoryService
    {
        public AttachmentCategoryService() : base() { }
        public AttachmentCategoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            try
            {
                string query = string.Empty;
                if (string.IsNullOrEmpty(strQuery))
                    query = "";
                else
                    query = strQuery;
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT * FROM View_AttachmentCategory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
                var dtList = _connection.Query<AttachmentCategory>(sqlQuery, new { Query = query }).ToList();
                if (dtList.Count <= 0)
                    return Notifization.NotFound(NotifizationText.NotFound);

                var resultData = new List<RsAttachmentCategory>();
                foreach (var item in dtList)
                {
                    resultData.Add(new RsAttachmentCategory(item.ID, item.Title, item.Summary, item.Alias,item.ControllerID, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate));
                }
                var result = resultData.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
                if (result.Count <= 0 && page > 1)
                {
                    page -= 1;
                    result = resultData.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
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
                    Alias = Helper.Library.Uni2NONE(model.Title),
                    Summary = model.Summary,
                    ControllerID = model.ControllerID,
                    LanguageID = Current.LanguageID,
                    Enabled = model.Enabled,
                });
                string temp = string.Empty;
                return Notifization.Success(NotifizationText.CREATE_SUCCESS);
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
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        string Id = model.ID;
                        if (string.IsNullOrEmpty(Id))
                            return Notifization.NotFound(NotifizationText.NotFound);
                        Id = Id.ToLower();
                        var attachmentCategoryService = new AttachmentCategoryService(_connection);
                        var attachmentCategory = attachmentCategoryService.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                        if (attachmentCategory == null)
                            return Notifization.NotFound(NotifizationText.NotFound);

                        string title = model.Title;
                        var dpm = attachmentCategoryService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !attachmentCategory.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                        if (dpm.Count > 0)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");
                        // update user information
                        attachmentCategory.Title = title;
                        attachmentCategory.Alias = Helper.Library.Uni2NONE(title);
                        attachmentCategory.Summary = model.Summary;
                        attachmentCategory.ControllerID = model.ControllerID;
                        attachmentCategory.Enabled = model.Enabled;
                        attachmentCategoryService.Update(attachmentCategory, transaction: transaction);
                        transaction.Commit();
                        return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
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
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_AttachmentCategory WHERE ID = @Query";
                return _connection.Query<AttachmentCategory>(sqlQuery, new { Query = Id }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string Id)
        {
            try
            {
                if (Id == null)
                    return Notifization.NotFound();
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        var AttachmentCategoryService = new AttachmentCategoryService(_connection);
                        var AttachmentCategory = AttachmentCategoryService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                        if (AttachmentCategory == null)
                            return Notifization.NotFound();
                        AttachmentCategoryService.Remove(AttachmentCategory.ID, transaction: transaction);
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
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(NotifizationText.Invalid);
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT * FROM View_AttachmentCategory WHERE ID = @ID";
                var item = _connection.Query<AttachmentCategory>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(NotifizationText.NotFound);
                var result = new RsAttachmentCategory(item.ID, item.Title, item.Summary, item.Alias,item.ControllerID, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate);
                return Notifization.DATALIST(NotifizationText.Success, data: result, role: null, paging: null);
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
        public List<AttachmentCategoryOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_AttachmentCategory_Option ORDER BY Title ASC";
                return _connection.Query<AttachmentCategoryOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<AttachmentCategoryOption>();
            }
        }
    }
}
