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
                    else if (!CheckPermission(filterContext))
                    {
                        filterContext.Result = Helper.Notifization.Error(MessageText.AccessDenied);
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
                else if (!CheckPermission(filterContext))
                {
                    filterContext.Result = new RedirectResult(Helper.Page.Navigate.PathForbidden);
                }
            }
            else // chua login
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { Controller = "Authen", Action = "Login", Area = "Authentication", r = _url }));
            }
            base.OnActionExecuting(filterContext);
        }



        // ###########################################################################################################################################################################################
        public bool CheckSkipAction(ActionExecutingContext filterContext)
        {
            try
            {
                bool manageFilter = filterContext.ActionDescriptor.IsDefined(typeof(IsManage), true);
                if (!manageFilter)
                    return false;
                //
                string actionName = filterContext.ActionDescriptor.ActionName;
                var type = filterContext.Controller.GetType();
                MemberInfo method = type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where(m => m.IsDefined(typeof(IsManage), true) && m.Name == actionName).FirstOrDefault();
                var manage = (IsManage)Attribute.GetCustomAttribute(method, typeof(IsManage));
                // return for api then -> return action result
                if (manage != null && manage.Skip)
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
            if (Helper.Current.UserLogin.IsCMSUser)
                return true;
            // IsManage is not found in controller and method
            if (CheckSkipAction(filterContext))
                return true;
            //
            // RouteArea in controller is required
            var routeAreaCheck = filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(RouteAreaAttribute), true);
            if (!routeAreaCheck)
                return false;
            //
            var controller = filterContext.Controller.GetType();
            RouteAreaAttribute routeAreaAttribute = (RouteAreaAttribute)Attribute.GetCustomAttribute(controller, typeof(RouteAreaAttribute));
            string routerArea = routeAreaAttribute.AreaName;
            string controllerText = controller.Name;
            string actionText = filterContext.ActionDescriptor.ActionName;

            //
            if (Helper.Current.UserLogin.IsAdminInApplication)
                return true;
            //
            if (string.IsNullOrWhiteSpace(controllerText) || string.IsNullOrWhiteSpace(actionText))
                return false;
            //
            controllerText = controllerText.Replace("Controller", "");
            actionText = Helper.Page.Library.FirstCharToUpper(actionText);

            //
            string userId = Helper.Current.UserLogin.IdentifierID;
            //#1. Get role of user
            UserRoleService userRoleService = new UserRoleService();
            var userRole = userRoleService.GetAlls(m => m.UserID == userId).FirstOrDefault();
            if (userRole == null)
                return false;
            //
            string roleId = userRole.RoleID;
            string controllerId = Helper.Security.Library.FakeGuidID(routerArea + controllerText);
            string actionId = Helper.Security.Library.FakeGuidID(controllerId + actionText);
            //#2. check  
            using (PermissionService service = new PermissionService())
            {
                string sqlQuery = @" SELECT c.ID FROM RoleControllerSetting as c INNER JOIN RoleActionSetting as a ON a.ControllerID = c.ControllerID AND a.RoleID = c.RoleID
                                     WHERE c.RoleID = @RoleID AND c.ControllerID = @ControllerID AND a.ActionID = @ActionID ";
                var role = service.Query<PermissionIDModel>(sqlQuery, new { RoleID = roleId, ControllerID = controllerId, ActionID = actionId }).FirstOrDefault();
                if (role != null)
                    return true;
                //
                return false;
            }
        }
        // ###########################################################################################################################################################################################
    }
    // 
    public class IsManage : Attribute
    {
        public bool Skip { get; set; }
        public IsManage()
        {

            this.Skip = false;
        }
        public IsManage(bool skip = false)
        {
            this.Skip = skip;
        }
    }
}
