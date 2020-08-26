using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.Entities;
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface ICMSUserInfoService : IEntityService<CMSUserInfo> { }
    public class CMSUserInfoService : EntityService<CMSUserInfo>, ICMSUserInfoService
    {
        public CMSUserInfoService() : base() { }
        public CMSUserInfoService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(NotifizationText.Invalid);
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP 1 * FROM View_CMSUser as u WHERE ID = @ID ";
                var item = _connection.Query<CMSUserViewModal>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(NotifizationText.NotFound);
                string _workshitName = string.Empty;
                var data = new RsCMSUserModel(item.ID, item.LoginID, item.ImageFile, item.FirstName, item.LastName, item.Nickname, item.Birthday, item.Email, item.Phone, item.Address, item.IdentifierID, item.TimekepingID, _workshitName, item.DepartmentID, item.DepartmentPartID, item.DepartmentName, item.DepartmentPartName, item.LanguageID, item.SiteID, item.CreatedBy, item.IsBlock, item.Enabled, item.CreatedDate);
                return Notifization.DATALIST(NotifizationText.Success, data: data, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public RsCMSUserModel CMSUserViewModel(string Id)
        {
            RsCMSUserModel rsCMSUserModel = new RsCMSUserModel();
            try
            {
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP 1 * FROM View_CMSUser  as u WHERE ID = @ID ";
                var item = _connection.Query<CMSUserViewModal>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return rsCMSUserModel;
                string _workshitName = string.Empty;
                var data = new RsCMSUserModel(item.ID, item.LoginID, item.ImageFile, item.FirstName, item.LastName, item.Nickname, item.Birthday, item.Email, item.Phone, item.Address, item.IdentifierID, item.TimekepingID, _workshitName, item.DepartmentID, item.DepartmentPartID, item.DepartmentName, item.DepartmentPartName, item.LanguageID, item.SiteID, item.CreatedBy, item.IsBlock, item.Enabled, item.CreatedDate);
                return data;
            }
            catch
            {
                return rsCMSUserModel;
            }
        }

        //Static function
        //##############################################################################################################################################################################################################################################################
        public static string GetLoginName(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return string.Empty;
                //
                CMSUserInfoService cMSUserInfoService = new CMSUserInfoService();
                var cMSUserInfo = cMSUserInfoService.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID.ToLower().Equals(id.ToLower())).FirstOrDefault();
                if (cMSUserInfo == null)
                    return string.Empty;
                // 
                return cMSUserInfo.FirstName + " " + cMSUserInfo.LastName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}