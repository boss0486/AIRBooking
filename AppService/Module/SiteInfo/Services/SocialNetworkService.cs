using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using WebCore.Entities;
using Helper;
using System.Web;
using System.Web.Configuration;
using System.Data;

namespace WebCore.Services
{
    public interface ISocialNetworkService : IEntityService<SocialNetwork> { }
    public class SocialNetworkService : EntityService<SocialNetwork>, ISocialNetworkService
    {
        public SocialNetworkService() : base() { }
        public SocialNetworkService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT * FROM View_SocialNetwork
                                     WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                     ORDER BY [CreatedDate]";
            var dtList = _connection.Query<SocialNetwork>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);
            var resultData = new List<RsSocialNetwork>();
            foreach (var item in dtList)
            {
                resultData.Add(new RsSocialNetwork(item.ID, item.Title, item.Alias, item.BackLink, item.IconFile, item.SiteID, item.Enabled, item.CreatedBy, Helper.Library.FormatDate(item.CreatedDate)));
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
        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(SocialNetworkCreateFormModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    SocialNetworkService SocialNetworkService = new SocialNetworkService(_connection);
                    string title = model.Title;
                    var SocialNetwork = SocialNetworkService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()), transaction: transaction).ToList();
                    if (SocialNetwork.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // create
                    var ID = SocialNetworkService.Create<string>(new SocialNetwork()
                    {
                        Title = model.Title,
                        Alias = Helper.Library.Uni2NONE(model.Title),
                        BackLink = model.BackLink,
                        SiteID = "",
                        IconFile = model.IconFile,
                        Enabled = model.Enabled
                    }, transaction: transaction);
                    // site id
                    string strPath = string.Empty;
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
        public ActionResult Update(SocialNetworkUpdateFormModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    SocialNetworkService SocialNetworkService = new SocialNetworkService(_connection);
                    string title = model.Title;
                    var SocialNetwork = SocialNetworkService.GetAlls(m => m.ID.Equals(model.ID.ToLower()), transaction: transaction).FirstOrDefault();
                    if (SocialNetwork == null)
                        return Notifization.NotFound();
                    string menuId = SocialNetwork.ID;
                    //
                    var modelTitle = SocialNetworkService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !m.ID.Equals(model.ID.ToLower()), transaction: transaction).ToList();
                    if (modelTitle.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update content
                    SocialNetwork.Title = title;
                    SocialNetwork.Alias = Helper.Library.Uni2NONE(model.Title);
                    SocialNetwork.BackLink = model.BackLink;
                    SocialNetwork.IconFile = model.IconFile;
                    SocialNetwork.Enabled = model.Enabled;
                    SocialNetworkService.Update(SocialNetwork, transaction: transaction);
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
        public RsSocialNetwork UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_SocialNetwork                                 
                                     WHERE ID = @Query";
                var model = _connection.Query<RsSocialNetwork>(sqlQuery, new { Query = Id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(SocialNetworkIDModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    SocialNetworkService SocialNetworkService = new SocialNetworkService(_connection);
                    var SocialNetwork = SocialNetworkService.GetAlls(m => m.ID.Equals(model.ID.ToLower()), transaction: transaction).FirstOrDefault();
                    if (SocialNetwork == null)
                        return Notifization.NotFound();
                    // remote menu, children menu
                    SocialNetworkService.Remove(SocialNetwork.ID, transaction: transaction);
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

    }
}