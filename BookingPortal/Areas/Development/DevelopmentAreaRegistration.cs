using System.Web.Mvc;

namespace WebApplication.Areas.Development
{
    public class DevelopmentAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Development";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // attribute
            context.Routes.MapMvcAttributeRoutes();
            // url route
            context.MapRoute(
                "Development_default",
                "Development/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "WebApplication.Development.Controllers" }
            );
        }
    }
}