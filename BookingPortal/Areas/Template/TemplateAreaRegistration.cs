using System.Web.Mvc;

namespace WebApplication.Areas.Template
{
    public class TemplateAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Template";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapMvcAttributeRoutes();
            context.MapRoute(
                "Template_default",
                "Template/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "WebApplication.Template.Controllers" } // namespace of Controller
            );
        }
    }
}