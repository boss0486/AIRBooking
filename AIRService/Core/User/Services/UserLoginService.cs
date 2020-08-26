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
using Helper.Page;
using Helper.File;
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface IUserLoginService : IEntityService<UserLogin> { }
    public class UserLoginService : EntityService<UserLogin>, IUserLoginService
    {
        public UserLoginService() : base() { }
        public UserLoginService(System.Data.IDbConnection db) : base(db) { }

        //##############################################################################################################################################################################################################################################################
        public ActionResult ChangePassword(UserChangePasswordModel model)
        {
            string loginId = Helper.Current.UserLogin.IdentifierID;
            if (string.IsNullOrWhiteSpace(loginId))
                return Notifization.Error("Bạn cần đăng nhập trước");
            //
            string passId = Helper.Security.Library.Encryption256(model.Password);
            // check account system
            UserLoginService UserLoginService = new UserLoginService(_connection);
            var UserLogin = UserLoginService.GetAlls(m => m.ID.Equals(loginId)).FirstOrDefault();
            if (UserLogin == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            if (!UserLogin.Password.Equals(passId))
                return Notifization.NotFound("Mật khẩu cũ chưa đúng");
            // update
            UserLogin.Password = Helper.Security.Library.Encryption256(model.NewPassword);
            UserLoginService.Update(UserLogin);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult ChangePinCode(UserChangePinCodeModel model)
        {
            string loginId = Helper.Current.UserLogin.IdentifierID;
            if (string.IsNullOrWhiteSpace(loginId))
                return Notifization.Error("Bạn cần đăng nhập trước");
            //
            string pincode = Helper.Security.Library.Encryption256(model.PinCode);
            var UserLoginService = new UserLoginService(_connection);
            var UserLogin = UserLoginService.GetAlls(m => m.ID.Equals(loginId)).FirstOrDefault();
            if (UserLogin == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            if (!UserLogin.PinCode.ToLower().Equals(pincode))
                return Notifization.NotFound("Mã pin chưa đúng");
            // update
            UserLogin.PinCode = Helper.Security.Library.Encryption256(model.PinCode);
            UserLoginService.Update(UserLogin);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Logout()
        {
            try
            {
                HttpContext.Current.Session.Abandon();
                return Notifization.Success("Xin chờ!");
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public string CreateID(System.Data.IDbConnection _connectDb, System.Data.IDbTransaction transaction = null)
        {
            UserLoginService userLoginService = new UserLoginService(_connectDb);
            string result = string.Empty;
            int counter = 1;
            do
            {
                result = new Guid().ToString();
                var accountID = userLoginService.Get(result, transaction: transaction);
                if (accountID == null)
                    return result;
                else
                    result = string.Empty;
                counter++;
            } while (result == string.Empty && counter < 20);
            return result;
        }
        public string CreateID()
        {
            UserLoginService userLoginService = new UserLoginService(_connection);
            string result = string.Empty;
            int counter = 1;
            do
            {
                result = new Guid().ToString();
                var accountID = userLoginService.Get(result);
                if (accountID == null)
                    return result;
                else
                    result = string.Empty;
                counter++;
            } while (result == string.Empty && counter < 20);
            return result;
        }
        //##############################################################################################################################################################################################################################################################

    }
}