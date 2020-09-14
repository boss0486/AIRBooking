using Helper;
using System.Web.Mvc;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using WebCore.Core;
using WebCore.Services;
using WebCore.Entities;
using Helper.User;
using System;

namespace WebApplication.Authentication.Controllers
{
    [IsManage]
    [RouteArea("Authentication")]
    //[RoutePrefix("Authen")]
    public class AuthenController : Controller
    {
        // GET: Authentication/Login
        [Route("")]
        [Route("Login")]
        [Route("Authen/Login")]
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

        [Route("OTPAuthen")]
        public ActionResult OTPAuthen()
        {
            return View();
        }

        [Route("Forgot")]
        public ActionResult Forgot()
        {
            ViewBag.Token = Request.QueryString["token"];
            return View();
        }

        [Route("ResetPassword")]
        public ActionResult ResetPassword()
        {
            ViewBag.Token = Request.QueryString["token"];
            return View();
        }

        // API ********************************************************************************************************
        [HttpPost]
        [Route("Action/Login")]
        [IsManage(skip: true)]
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
        [Route("Action/Logout")]
        [IsManage(skip: true)]
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

        [HttpPost]
        [Route("Action/Forgot")]
        [IsManage(skip: true)]
        public ActionResult Forgot(UserEmailModel model)
        {
            try
            {
                var service = new UserService();
                return service.ForgotPassword(model);
            }
            catch(Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        //

        [HttpPost]
        [Route("Action/ResetPassword")]
        [IsManage(skip: true)]
        public ActionResult ResetPassword(UserResetPasswordModel model)
        {
            try
            {
                var service = new UserService();
                return service.ResetPassword(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        //
    }
}