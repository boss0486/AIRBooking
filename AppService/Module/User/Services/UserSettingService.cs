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
using System.IdentityModel.Tokens.Jwt;
using WebCore.Model.Enum;
using CMSCore.Services;
using CMSCore.Entities;

namespace WebCore.Services
{
    public interface IUserSettingService : IEntityService<UserSetting> { }
    public class UserSettingService : EntityService<UserSetting>, IUserSettingService
    {
        public UserSettingService() : base() { }
        public UserSettingService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult UserBlock(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            Id = Id.ToLower();
            UserSettingService userSettingService = new UserSettingService(_connection);
            var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
            if (userSetting == null)
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            userSetting.IsBlock = true;
            userSettingService.Update(userSetting);
            return Notifization.Success("Update successfuly");
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult UserUnlock(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            Id = Id.ToLower();
            UserSettingService userSettingService = new UserSettingService(_connection);
            var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
            if (userSetting == null)
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            userSetting.IsBlock = false;
            userSettingService.Update(userSetting);
            return Notifization.Success("Update successfuly");
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult UserActive(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            Id = Id.ToLower();
            UserSettingService userSettingService = new UserSettingService(_connection);
            var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
            if (userSetting == null)
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            userSetting.Enabled = (int)ModelEnum.Enabled.ENABLED;
            userSettingService.Update(userSetting);
            return Notifization.Success("Update successfuly");
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult UserUnActive(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            Id = Id.ToLower();
            UserSettingService userSettingService = new UserSettingService(_connection);
            var userSetting = userSettingService.GetAlls(m => m.UserID.ToLower().Equals(Id)).FirstOrDefault();
            if (userSetting == null)
                return Notifization.NotFound("Dữ liệu không hợp lệ");
            userSetting.Enabled = (int)ModelEnum.Enabled.DISABLE;
            userSettingService.Update(userSetting);
            return Notifization.Success("Update successfuly");
        }
        //##############################################################################################################################################################################################################################################################

        public ActionResult SendOtp(UserEmailModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string mail = model.Email.Trim();
            if (string.IsNullOrEmpty(mail))
                return Notifization.Invalid("Không được để trống địa chỉ email");
            //
            if (!Validate.FormatEmail(mail))
                Notifization.Invalid("Địa chỉ email không hợp lệ");
            string strEmail = model.Email;
            string sqlQuerry = @"SELECT TOP 1 * FROM View_Login WHERE Email = @Email";
            var login = _connection.Query<LoginModel>(sqlQuerry, new
            {
                Email = strEmail
            }).FirstOrDefault();
            if (login == null)
                return Notifization.NotFound();
            //  send otp 
            string strOtp = Helper.Security.OTPCode;
            string subject = "XÁC THỰC OTP";
            int status = Mail.SendOTP_ForGotPassword(strEmail, subject, strOtp);
            if (status != 1)
                return Notifization.ERROR("Không thể gửi mã OTP");
            //
            string strGuid = new Guid().ToString();
            string strToken = Helper.Security.Token.Create(login.LoginID, strGuid);
            if (login.IsCMSUser)
            {
                CMSUserLoginService cmsUserLoginService = new CMSUserLoginService(_connection);
                var userLogin = cmsUserLoginService.GetAlls(m => m.ID.ToLower().Equals(login.ID.ToLower())).FirstOrDefault();
                if (userLogin == null)
                    return Notifization.Invalid(NotifizationText.Invalid);
                userLogin.OTPCode = strOtp;
                userLogin.TokenID = strToken;
                cmsUserLoginService.Update(userLogin);
            }
            else
            {
                UserLoginService userLoginService = new UserLoginService(_connection);
                var userLogin = userLoginService.GetAlls(m => m.ID.ToLower().Equals(login.ID.ToLower())).FirstOrDefault();
                if (userLogin == null)
                    return Notifization.Invalid(NotifizationText.Invalid);
                userLogin.OTPCode = strOtp;
                userLogin.TokenID = strToken;
                userLoginService.Update(userLogin);
            }
            return Notifization.Success("Mã OTP đã được gửi tới email của bạn", "/Authentication/forgot?token=" + strToken);
        }
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TokenID))
                return Notifization.Invalid("Dữ liệu không hợp lệ");
            // a sample jwt encoded token string which is supposed to be extracted from 'Authorization' HTTP header in your Web Api controller
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(model.TokenID);
            var token = handler.ReadToken(model.TokenID) as JwtSecurityToken;
            string loginId = token.Claims.First(c => c.Type == "TokenID").Value;
            string tokenKey = token.Claims.First(c => c.Type == "TokenKey").Value;
            string tokenTime = token.Claims.First(c => c.Type == "TokenTime").Value;
            if (string.IsNullOrWhiteSpace(loginId))
                return Notifization.UnAuthorized;
            //
            string sqlQuerry = @"SELECT TOP 1 * FROM View_Login WHERE LoginID = @LoginID";
            var login = _connection.Query<LoginModel>(sqlQuerry, new
            {
                LoginID = loginId
            }).FirstOrDefault();
            if (login == null)
                return Notifization.NotFound();
            if (login.IsCMSUser)
            {
                CMSUserLoginService cMSUserLoginService = new CMSUserLoginService(_connection);
                var user = cMSUserLoginService.GetAlls(m => m.LoginID.ToLower().Equals(loginId.ToLower())).FirstOrDefault();
                if (user == null)
                    return Notifization.Invalid("Dữ liệu không hợp lệ");
                string otp = user.OTPCode;
                if (string.IsNullOrWhiteSpace(otp))
                    return Notifization.Invalid("Dữ liệu không hợp lệ");
                if (!otp.ToLower().Equals(model.OTPCode.ToLower()))
                    return Notifization.Invalid("Sai mã OTP");
                user.OTPCode = string.Empty;
                user.TokenID = string.Empty;
                user.PassID = Helper.Security.Encryption256Hashed(model.Password);
                cMSUserLoginService.Update(user);
            }
            else
            {
                UserLoginService userLoginService = new UserLoginService(_connection);
                var user = userLoginService.GetAlls(m => m.LoginID.ToLower().Equals(loginId.ToLower())).FirstOrDefault();
                if (user == null)
                    return Notifization.Invalid("Dữ liệu không hợp lệ");
                string otp = user.OTPCode;
                if (string.IsNullOrWhiteSpace(otp))
                    return Notifization.Invalid("Dữ liệu không hợp lệ");
                if (!otp.ToLower().Equals(model.OTPCode.ToLower()))
                    return Notifization.Invalid("Sai mã OTP");
                user.OTPCode = string.Empty;
                user.TokenID = string.Empty;
                user.PassID = Helper.Security.Encryption256Hashed(model.Password);
                userLoginService.Update(user);
            }
            return Notifization.Success(NotifizationText.UPDATE_SUCCESS, "/Authentication");
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLUser(string _user)
        {
            return string.Empty;
        }
        //##############################################################################################################################################################################################################################################################
    }
}