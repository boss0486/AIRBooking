using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Helper;
using WebCore.Entities;
using WebCore.Services;

namespace WebCore.Core
{
    public class CMSController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string _url = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            bool IsHasManageController = filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(IsManage), false);
            bool IsHasManageAction = filterContext.ActionDescriptor.IsDefined(typeof(IsManage), false);
            // API method
            string _controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string _actionName = filterContext.ActionDescriptor.ActionName;
            if (HttpContext.Request.IsAjaxRequest())
            {
                // da login
                if (Helper.User.Access.IsLogin())
                {
                    // kiem tra hien tai con session login hay ko
                    var sessionLogin = Helper.Current.UserLogin.IdentifierID;
                    if (string.IsNullOrWhiteSpace(sessionLogin))
                    {
                        filterContext.Result = Helper.Notifization.Error("Phiên làm việc đã hết hạn");
                    }
                    else if (IsHasManageController)
                    {
                        if (IsHasManageAction)
                        {
                            if (!CheckPermission(filterContext))
                                filterContext.Result = Helper.Notifization.Error(MessageText.AccessDenied);
                        }      
                    }
                }
                else // chua login
                {
                    filterContext.Result = Helper.Notifization.Error("Yêu cầu đăng nhập hệ thống");
                }

            }
            // Page
            else if (Helper.User.Access.IsLogin())
            {
                // kiem tra hien tai con session login hay ko
                var sessionLogin = Helper.Current.UserLogin.IdentifierID;
                if (string.IsNullOrWhiteSpace(sessionLogin))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Authen", Action = "Login", Area = "Authentication", r = _url }));
                }
                else if (IsHasManageController)
                {
                    if (IsHasManageAction)
                    {
                        if (!CheckPermission(filterContext))
                            filterContext.Result = new RedirectResult(Helper.Page.Navigate.PathForbidden);

                    }
                }
            }
            else // chua login
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Authen", Action = "Login", Area = "Authentication", r = _url }));
            }
            base.OnActionExecuting(filterContext);
        }



        // ###########################################################################################################################################################################################
        public bool CheckAPIAction(ActionExecutingContext filterContext)
        {
            try
            {
                bool manageFilter = filterContext.ActionDescriptor.IsDefined(typeof(IsManage), true);
                if (!manageFilter)
                    return false;
                //
                string actionName = filterContext.ActionDescriptor.ActionName;
                var type = filterContext.Controller.GetType();
                MemberInfo method = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where(m => m.IsDefined(typeof(IsManage), true) && m.Name.Equals(actionName)).FirstOrDefault();
                var manage = (IsManage)Attribute.GetCustomAttribute(method, typeof(IsManage));
                // return for api then -> return action result
                if (manage != null && manage.Action)
                    return true;
                //
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckPermission(ActionExecutingContext filterContext)
        {

            // RouteArea in controller is required
            var routeAreaCheck = filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(RouteAreaAttribute), true);
            if (!routeAreaCheck)
                return false;
            //
            var controller = filterContext.Controller.GetType();
            string actionName = filterContext.ActionDescriptor.ActionName;
            RouteAreaAttribute routeAreaAttribute = (RouteAreaAttribute)Attribute.GetCustomAttribute(controller, typeof(RouteAreaAttribute));
            var routerArea = routeAreaAttribute.AreaName;
            //
            if (PermissionService.CheckPermission(routerArea, controller.Name, actionName))
                return true;
            //
            return false;

        }
        // ###########################################################################################################################################################################################
    }
    // 
    public class IsManage : Attribute
    {
        public bool Action { get; set; }
        public bool Login { get; set; }
        public string Text { get; set; }
        public IsManage()
        {
            // something
            this.Action = false;
            this.Login = false;
            this.Text = string.Empty;
        }
        public IsManage(bool act, string text = null)
        {
            this.Action = act;
            this.Login = false;
            this.Text = text;
        }
        public IsManage(bool act, bool login, string text = null)
        {
            this.Action = act;
            this.Login = login;
            this.Text = text;
        }
    }
    //public class Authorized : Attribute
    //{
    //    public bool Allow { get; set; }
    //    public Authorized()
    //    {
    //        // something
    //        this.Allow = false;
    //    }
    //    public Authorized(bool allow)
    //    {
    //        this.Allow = allow;
    //    }
    //}


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
