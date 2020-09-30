using System.Web.Mvc;

namespace WebApplication.Areas.Authentication
{
    public class AuthenticationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Authentication";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            //context.MapRoute(
            //     "Authentication_default",
            //     "Authentication/{action}/{id}",
            //     new { controller = "Login", action = "Index", id = UrlParameter.Optional },
            //    new string[] { "WebApplication.Authentication.Controllers" } // namespace of file HomeController
            // );

            // for page html
            context.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            context.Routes.MapMvcAttributeRoutes();
            context.MapRoute(
                 "Authentication_default",
                 "Authentication/{action}/{id}",
                 new { controller = "Authen", action = "Login", id = UrlParameter.Optional },
                 new string[] { "WebApplication.Authentication.Controllers" } // namespace of Controller
             );

            //context.MapRoute(
            //    "Authentication_2",
            //    "Api/{controller}/{action}/{id}",
            //    new { controller = "Authen", action = "Login", id = UrlParameter.Optional },
            //   new string[] { "WebApplication.Authentication.Controllers" } // namespace of file HomeController
            //);
            //context.MapRoute(
            //    "Authentication_2",
            //    "Api/{controller}/{action}",
            //     new { controller = "Authen", action = UrlParameter.Optional, id = UrlParameter.Optional },
            //   new string[] { "WebApplication.Authentication.Controllers" } // namespace of file HomeController
            //);
        }
    }
   
}