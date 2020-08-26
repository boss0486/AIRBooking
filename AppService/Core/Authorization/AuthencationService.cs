using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using Helper;
using System.Web.Mvc;
using CMSCore.Entities;
using System.Web;
using WebCore.Model.Enum;
using System.Web.Security;

namespace CMSCore.Services
{
    public interface ILoginService : IEntityService<CMSUserLogin> { }
    public class LoginService : EntityService<CMSUserLogin>, ILoginService
    {
        public LoginService() : base() { }
        public LoginService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Login(LoginReqestModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string loginId = model.LoginID;
            string password = model.PassID;
            if (string.IsNullOrWhiteSpace(loginId) && string.IsNullOrWhiteSpace(password))
                return Notifization.Invalid();
            //                          
            string sqlQuerry = @"SELECT TOP 1 * FROM View_Login WHERE LoginID = @LoginID AND PassID = @PassID";
            var login = _connection.Query<LoginModel>(sqlQuerry, new
            {
                model.LoginID,
                PassID = Security.Encryption256Hashed(password)
            }).FirstOrDefault();
            //
            if (login == null)
                return Notifization.ERROR("Sai thông tin tài khoản hoặc mật khẩu");
            //
            if (login.Enabled != (int)ModelEnum.Enabled.ENABLED)
                return Notifization.ERROR("Tài khoản chưa được kích hoạt");
            //
            if (login.IsBlock)
                return Notifization.ERROR("Tài khoản của bạn bị khóa");
            //
            HttpContext.Current.Session["IdentifyID"] = login.ID;
            HttpContext.Current.Session["LoginID"] = login.LoginID;
            // delete cookies
            HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(-1);
            // set cooki
            string _uData = "{'LoginID': '', 'PassID': '', 'IsRemember': 0} ";
            if (model.IsRemember)
                _uData = "{'LoginID': '" + model.LoginID + "', 'PassID': '" + model.PassID + "', 'IsRemember': 1} ";

            bool isPersistent = false;
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                model.LoginID,
                DateTime.Now,
                DateTime.Now.AddDays(10),
                isPersistent,
                _uData,
                FormsAuthentication.FormsCookiePath
            );
            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);
            // Create the cookie.
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

            //navigate by role | navigate param
            string _url = Helper.Web.NavigateByParam(model.Url);
            if (string.IsNullOrWhiteSpace(_url))
                _url = "/backend/home/index";
            //
            if (login.IsCMSUser)
                return Notifization.Success("Đăng nhập thành công", _url);
            else
                return Notifization.Success("Đăng nhập thành công", _url);
        }

        public ActionResult LoginQR(LoginQRModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string loginId = model.LoginID;
            string pincode = model.PinCode;
            if (string.IsNullOrWhiteSpace(loginId) && string.IsNullOrWhiteSpace(pincode))
                return Notifization.Invalid();
            //              
            string sqlQuerry = @"SELECT TOP 1 * FROM View_Login WHERE LoginID = @LoginID AND PinCode = @PinCode";
            var login = _connection.Query<LoginModel>(sqlQuerry, new
            {
                LoginID = loginId,
                PinCode = Security.Encryption256Hashed(pincode)
            }).FirstOrDefault();
            //
            if (login == null)
                return Notifization.ERROR("Sai thông tin tài khoản hoặc mật khẩu");
            //
            if (login.Enabled != (int)ModelEnum.Enabled.ENABLED)
                return Notifization.ERROR("Tài khoản chưa được kích hoạt");
            //
            if (login.IsBlock)
                return Notifization.ERROR("Tài khoản của bạn bị khóa");
            //
            HttpContext.Current.Session["IdentifyID"] = login.ID;
            HttpContext.Current.Session["LoginID"] = login.LoginID;
            // delete cookies
            HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddDays(-1);
            // set cooki
            bool isPersistent = false;
            string _uData = "{'LoginID': '" + model.LoginID + "', 'PassID': '', 'IsRemember': 0} ";
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                model.LoginID,
                DateTime.Now,
                DateTime.Now.AddDays(10),
                isPersistent,
                _uData,
                FormsAuthentication.FormsCookiePath
            );
            // Encrypt the ticket.
            string encTicket = FormsAuthentication.Encrypt(ticket);
            // Create the cookie.
            HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            //navigate by account type
            if (login.IsCMSUser)
                return Notifization.Success("Đăng nhập thành công", "/Authentication/index");
            else
                return Notifization.Success("Đăng nhập thành công", "/Authentication/index");
        }

        public ActionResult Logout(LoginReqestModel model)
        {
            return Notifization.Success("Đăng xuất thành công", "");
        }
    }
}
