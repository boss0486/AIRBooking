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
        public ActionResult Detail(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                //
                string langID = Helper.Current.UserLogin.LanguageID;
                var data = CMSUserViewModel(Id);
                if (data == null)
                    return Notifization.NotFound(MessageText.NotFound);
                return Notifization.Data(MessageText.Success, data: data, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public CMSUserResult CMSUserViewModel(string Id)
        {
            CMSUserResult  cMSUserResult = new CMSUserResult();
            try
            {
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP 1 * FROM View_CMSUser  as u WHERE ID = @ID ";
                var data = _connection.Query<CMSUserResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (data == null)
                    return cMSUserResult;
                //
                return data;
            }
            catch
            {
                return cMSUserResult;
            }
        }

        // function ###########################################################################################################################################################################################
        public string GetFullName(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return string.Empty;
                //
                using (var service = new CMSUserInfoService(_connection))
                {
                    var data = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.UserID.ToLower().Equals(Id.ToLower())).FirstOrDefault();
                    //
                    if (data == null)
                        return string.Empty;
                    //
                    return data.FullName;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}