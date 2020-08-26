using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using WebCore.Entities;
using System;
using Dapper;
using System.Linq;
using PagedList;
using Helper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;

namespace WebCore.Services
{
    public interface IUserInfoService : IEntityService<UserInfo> { }
    public class UserInfoService : EntityService<UserInfo>, IUserInfoService
    {
        public UserInfoService() : base() { }
        public UserInfoService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(NotifizationText.Invalid);
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP 1 * FROM View_User  as u WHERE ID = @ID ";
                var item = _connection.Query<UserViewModal>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(NotifizationText.NotFound);
                string _workshitName = string.Empty;
                var data = new RsUserModel(item.ID, item.LoginID, item.ImageFile, item.FirstName, item.LastName, item.Nickname, item.Birthday, item.Email, item.Phone, item.Address, item.IdentifierID, item.TimekepingID, _workshitName, item.DepartmentID, item.DepartmentPartID, item.DepartmentName, item.DepartmentPartName, item.LanguageID, item.SiteID, item.CreatedBy, item.IsBlock, item.Enabled, item.CreatedDate);
                return Notifization.DATALIST(NotifizationText.Success, data: data, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public RsUserModel UserViewModel(string Id)
        {
            RsUserModel rsUserModel = new RsUserModel();
            try
            {
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP 1 * FROM View_User  as u WHERE ID = @ID ";
                var item = _connection.Query<UserViewModal>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return rsUserModel;

                string _workshitName = string.Empty;
                var data = new RsUserModel(item.ID, item.LoginID, item.ImageFile, item.FirstName, item.LastName, item.Nickname, item.Birthday, item.Email, item.Phone, item.Address, item.IdentifierID, item.TimekepingID, _workshitName, item.DepartmentID, item.DepartmentPartID, item.DepartmentName, item.DepartmentPartName, item.LanguageID, item.SiteID, item.CreatedBy, item.IsBlock, item.Enabled, item.CreatedDate);
                return data;
            }
            catch
            {
                return rsUserModel;
            }
        }


    }
}