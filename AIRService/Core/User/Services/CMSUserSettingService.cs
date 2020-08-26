using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using Helper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using WebCore.Model.Enum;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Services;

namespace WebCore.Services
{
    public interface ICMSUserSettingService : IEntityService<CMSUserSetting> { }
    public class CMSUserSettingService : EntityService<CMSUserSetting>, ICMSUserSettingService
    {
        public CMSUserSettingService() : base() { }
        public CMSUserSettingService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult CMSUserBlock(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            Id = Id.ToLower();
            CMSUserSettingService cmsUserSettingService = new CMSUserSettingService(_connection);
            var cmsUserSetting = cmsUserSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
            if (cmsUserSetting == null)
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            cmsUserSetting.IsBlock = true;
            cmsUserSettingService.Update(cmsUserSetting);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult CMSUserUnlock(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            Id = Id.ToLower();
            var cmsUserSettingService = new CMSUserSettingService(_connection);
            var cmsUserSetting = cmsUserSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
            if (cmsUserSetting == null)
            {
                var userSettingService = new UserSettingService(_connection);
                var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
                if (userSetting != null)
                {
                    userSetting.IsBlock = false;
                    userSettingService.Update(userSetting);
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                else
                    return Notifization.NotFound("Dữ liệu không hợp lệ");
            }
            cmsUserSetting.IsBlock = false;
            cmsUserSettingService.Update(cmsUserSetting);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult CMSUserActive(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            Id = Id.ToLower();
            var cmsUserSettingService = new CMSUserSettingService(_connection);
            var cmsUserSetting = cmsUserSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
            if (cmsUserSetting == null)
            {
                var userSettingService = new UserSettingService(_connection);
                var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
                if (userSetting != null)
                {
                    userSetting.Enabled = (int)ModelEnum.Enabled.ENABLED;
                    userSettingService.Update(userSetting);
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                else
                    return Notifization.NotFound("Dữ liệu không hợp lệ");
            }
            cmsUserSetting.Enabled = (int)ModelEnum.Enabled.ENABLED;
            cmsUserSettingService.Update(cmsUserSetting);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult CMSUserUnActive(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            Id = Id.ToLower();
            var cmsUserSettingService = new CMSUserSettingService(_connection);
            var cmsUserSetting = cmsUserSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
            if (cmsUserSetting == null)
            {
                var userSettingService = new UserSettingService(_connection);
                var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
                if (userSetting != null)
                {
                    userSetting.Enabled = (int)ModelEnum.Enabled.DISABLE;
                    userSettingService.Update(userSetting);
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                else
                    return Notifization.NotFound("Dữ liệu không hợp lệ");
            }
            cmsUserSetting.Enabled = (int)ModelEnum.Enabled.DISABLE;
            cmsUserSettingService.Update(cmsUserSetting);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################

        public ActionResult SendOtp(UserEmailModel model)
        {
            string strEmail = model.Email.ToLower();
            CMSUserLoginService cmsUserLoginService = new CMSUserLoginService(_connection);
            CMSUserInfoService cmsUserInfoService = new CMSUserInfoService(_connection);
            var cmsUserInfo = cmsUserInfoService.GetAlls(m => m.Email.ToLower().Equals(model.Email.ToLower())).FirstOrDefault();
            if (cmsUserInfo == null)
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            // send mail
            string strOTP = Helper.Security.Library.OTPCode;
            string strGuid = new Guid().ToString();
            string strToken = Helper.Security.Token.Create(cmsUserInfo.UserID);
            //  send otp for reset password 
            string subject = "HRM-XÁC THỰC OTP";
            int status = Helper.Email.EMailService.SendOTP_ForGotPassword(strEmail, subject, strOTP);
            if (status != 1)
                return Notifization.Error("Không thể gửi mã OTP tới email của bạn");
            //
            var cmsUserLogin = cmsUserLoginService.GetAlls(m => m.ID.Equals(cmsUserInfo.UserID.ToLower())).FirstOrDefault();
            cmsUserLogin.OTPCode = strOTP;
            cmsUserLogin.TokenID = strToken;
            cmsUserLoginService.Update(cmsUserLogin);
            return Notifization.Success("Mã OTP đã được gửi tới email của bạn", "/Authentication/login/forgot?token=" + strToken);
        }
        public ActionResult ResetPassword(UserResetPasswordModel model)
        {
            CMSUserLoginService cmsUserLoginService = new CMSUserLoginService(_connection);
            // a sample jwt encoded token string which is supposed to be extracted from 'Authorization' HTTP header in your Web Api controller
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(model.TokenID);
            var token = handler.ReadToken(model.TokenID) as JwtSecurityToken;
            string UserID = token.Claims.First(c => c.Type == "Identities").Value;
            string tokenKey = token.Claims.First(c => c.Type == "TokenKey").Value;
            string tokenTime = token.Claims.First(c => c.Type == "TokenTime").Value;
            //
            if (string.IsNullOrEmpty(UserID) && string.IsNullOrEmpty(tokenKey) && string.IsNullOrEmpty(tokenTime))
                return Notifization.UnAuthorized;
            var cmsUser = cmsUserLoginService.GetAlls(m => m.ID.Equals(UserID.ToLower())).FirstOrDefault();
            if (cmsUser == null)
                return Notifization.Invalid("Dữ liệu không hợp lệ");
            string otp = cmsUser.OTPCode;
            if (string.IsNullOrEmpty(otp))
                return Notifization.Invalid("Dữ liệu không hợp lệ");
            if (!otp.ToLower().Equals(model.OTPCode.ToLower()))
                return Notifization.Invalid("Mã OTP chưa đúng");
            cmsUser.OTPCode = string.Empty;
            cmsUser.TokenID = string.Empty;
            cmsUser.Password = Helper.Security.Library.Encryption256(model.Password);
            cmsUserLoginService.Update(cmsUser);
            return Notifization.Success(MessageText.UpdateSuccess, "/authorization");
        }


        //##############################################################################################################################################################################################################################################################
        public static string DDLCMSUser(string id)
        {
            try
            {
                string result = string.Empty;
                using (var CMSUserSettingService = new CMSUserSettingService())
                {
                    var dtList = CMSUserSettingService.DataOption(id);
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
        public List<UserOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT UserID AS ID, CONCAT(FirstName,LastName) as Title  
                                    FROM View_CMSUser WHERE  IsBlock = 0 AND Enabled = 1 ORDER BY Title ASC";
                return _connection.Query<UserOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<UserOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################

    }
}