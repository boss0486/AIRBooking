using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using Helper.File;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebCore.Entities;

namespace WebCore.Services
{
    public interface IAttachmentService : IEntityService<Attachment> { }
    public partial class AttachmentService : EntityService<Attachment>, IAttachmentService
    {
        public AttachmentService() : base() { }
        public AttachmentService(System.Data.IDbConnection db) : base(db) { }


        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(AttachmentFolderModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.NotService;
                string query = model.Query;
                if (string.IsNullOrEmpty(query))
                    query = " ";
                int page = model.Page;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT  a.*, c.Title as 'CategoryName'  FROM Attachment as a LEFT JOIN  AttachmentCategory as c ON a.CategoryID  =  c.ID 
                                    WHERE a.Title LIKE N'%'+ dbo.Uni2NONE('') +'%'
                                    ORDER BY CreatedDate DESC";
                var dtList = _connection.Query<ViewAttachment>(sqlQuery, new { Query = query }).ToList();
                if (dtList.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);

                var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
                if (result.Count <= 0 && page > 1)
                {
                    page--;
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
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        public ActionResult Delete(AttachmentIDModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.NotFound();
                _connection.Open();
                var _Id = model.ID;
                if (string.IsNullOrEmpty(_Id))
                    return Notifization.NotFound();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        var attachmentService = new AttachmentService(_connection);
                        var attachment = attachmentService.GetAlls(m => m.ID.Equals(_Id.ToLower()), transaction: transaction).FirstOrDefault();
                        if (attachment == null)
                            return Notifization.NotFound();

                        attachmentService.Remove(_Id, transaction: transaction);
                        AttachmentFile.DeleteFile(_Id, transaction: transaction);
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
            catch
            {
                return Notifization.NotService;
            }
        }

        public List<ViewAttachment> AttachmentrListByForID(string _forId)
        {
            try
            {
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT  att.* FROM AttachmentIngredient atti LEFT JOIN  Attachment as att ON  att. ID = atti.FileID WHERE atti.ForID = @ForID";
                var dtList = _connection.Query<ViewAttachment>(sqlQuery, new { ForID = _forId }).ToList();
                if (dtList.Count == 0)
                    return new List<ViewAttachment>();
                return dtList;
            }
            catch 
            {
                return new List<ViewAttachment>();
            }
        }
    }
}