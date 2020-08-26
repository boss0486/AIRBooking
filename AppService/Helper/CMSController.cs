using System;
using System.Linq;
using System.Web.Mvc;
using CMSCore.Entities;
using WebCore.Services;
using System.Web.Routing;
using Helper;

namespace WebCore.Core
{
    public class CMSController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //try
            //{
                bool skipImportantTaskFilter = filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(Authorized), true) || filterContext.ActionDescriptor.IsDefined(typeof(Authorized), true);
                if (!skipImportantTaskFilter)
                {


                    //// Retrieves the cookie that contains your custom FormsAuthenticationTicket.
                    //System.Web.HttpCookie authCookie = HttpContext.CurrentHandler.ProcessRequest.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];
                    //// Decrypts the FormsAuthenticationTicket that is held in the cookie's .Value property.
                    //System.Web.Security.FormsAuthenticationTicket authTicket = System.Web.Security.FormsAuthentication.Decrypt(authCookie.Value);

                    if (Current.LoginState())
                    {
                        var sessionLogin = Helper.Current.IdentifierID;
                        if (string.IsNullOrWhiteSpace(sessionLogin))
                        {
                            string _url = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                            bool actionFilter = filterContext.ActionDescriptor.IsDefined(typeof(Authorized), true);
                            if (actionFilter)
                            {
                                filterContext.Result = Helper.Notifization.ERROR("Phiên làm việc đã hết hạn, nhấn F5 để đăng nhập lại.");
                                return;
                            }
                            else
                            {
                                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new {  Action = "Login", Area = "Authentication", r = _url }));
                                return;
                            }
                        }
                    }
                    else
                    {
                        var sessionLogin = Helper.Current.IdentifierID;
                        if (string.IsNullOrWhiteSpace(sessionLogin))
                    {
                        string _url = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Action = "Login", Area = "Authentication", r= _url }));
                            return;

                        }
                    }
                }
                base.OnActionExecuting(filterContext);
           // }
            //catch (Exception ex)
            //{
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Login", Action = "Index", Area = "Authentication", r = ex.ToString() }));
            //}
        }

    }
    // 

    public class Authorized : Attribute { }



    //[AttributeUsage(AttributeTargets.All)]
    //public class Authorized : Attribute
    //{
    //    public bool Allow { get; set; }
    //    public Authorized(bool _val)
    //    {
    //        this.Allow = _val;
    //    }
    //}
    //public class APIAuthorization : Attribute
    //{
    //    public bool Allow { get; set; }
    //    public APIAuthorization(bool _val)
    //    {
    //        this.Allow = _val;
    //    }
    //}
}
