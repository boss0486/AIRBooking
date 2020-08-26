using Helper;
using System.Web.Mvc;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using WebCore.Core;
using WebCore.Services;
using WebCore.Entities;
using Helper.User;

namespace WebApplication.Authentication.Controllers
{
    [RouteArea("Authentication")]
    //[RoutePrefix("Authen")]
    public class AuthenController : Controller
    {
        // GET: Authentication/Login
        [Route("")]
        [Route("Login")]
        [IsManage(false, "Login")]
        public ActionResult Login()
        {
            ViewBag.UId = string.Empty;
            ViewBag.PId = string.Empty;
            ViewBag.Rmb = string.Empty;
            try
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    //do something
                    HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    var _login = JsonConvert.DeserializeObject<CookiModel>(authTicket.UserData);
                    if (_login != null && _login.IsRemember)
                    {
                        ViewBag.UId = _login.LoginID;
                        ViewBag.PId = _login.Password;
                        ViewBag.Rmb = "checked";
                    }
                }
            }
            catch
            {
                //
            }

            return View();
        }

        [Route("QrLogin")]
        [IsManage(false, "QrLogin")]
        public ActionResult QrLogin()
        {
            return View();
        }


        [Route("OTPAuthen")]
        [IsManage(false, "OTPAuthen")]
        public ActionResult OTPAuthen()
        {
            return View();
        }

        [Route("Forgot")]
        [IsManage(false, "Forgot")]
        public ActionResult Forgot()
        {
            ViewBag.Token = Request.QueryString["token"];
            return View();
        }

        [Route("SendOtp")]
        [IsManage(false, "SendOtp")]
        public ActionResult SendOtp()
        {
            return View();
        }

        // API ********************************************************************************************************
        [HttpPost]
        [IsManage(true, "Login")]
        [Route("Action/Login")]
        public ActionResult Login(LoginReqestModel model)
        {
            try
            {
                var service = new UserService();
                return service.Login(model);
            }
            catch (System.Exception ex)
            {
                return Notifization.Error("::" + ex.ToString());
            }
            // call service
        }

        [HttpPost]
        [IsManage(true, "PinCode")]
        [Route("Action/PinCode")]
        public ActionResult PinCode(LoginQRReqestModel model)
        {
            var service = new UserService();
            return service.LoginQR(model);
        }


        [HttpPost]
        [Route("Action/Logout")]
        public ActionResult Logout()
        {
            try
            {
                HttpContext.Session.Abandon();
                return Notifization.Success("Xin chờ!");
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //[HttpPost]
        //[IsManage]
        //[Route("Action/SendOtp")]
        //public ActionResult SendOtp(EmailModel model)
        //{
        //    // call service
        //    using (var service = new UserLoginService())
        //        return service.SendOtp(new
        //        {

        //        });
        //}


        //[HttpPost]
        //[IsManage]
        //[Route("Action/Password")]
        //public ActionResult Password(UserResetPasswordModel model)
        //{
        //    if (model == null)
        //        return Notifization.Invalid();
        //    if (string.IsNullOrEmpty(model.OTPCode))
        //        return Notifization.Error("Không được để trống mã OTP");
        //    //
        //    if (string.IsNullOrEmpty(model.Password))
        //        return Notifization.Error("Không được để trống mật khẩu");
        //    //
        //    if (string.IsNullOrEmpty(model.RePassword))
        //        return Notifization.Error("Vui lòng nhập lại mật khẩu");
        //    //
        //    if (!Helper.Page.Validate.TestPassword(model.Password))
        //        return Notifization.Invalid("Yêu cầu mật khẩu bảo mật hơn");
        //    if (model.Password.Length < 4 || model.Password.Length > 16)
        //        return Notifization.Invalid("Mật khẩu giới hạn [4-16] ký tự");
        //    //
        //    if (!model.Password.Equals(model.RePassword))
        //        return Notifization.Error("Xác nhận mật khẩu chưa đúng");
        //    // call service
        //    using (var userSettingService = new UserSettingService())
        //        return userSettingService.ResetPassword(model);
        //}
    }
}