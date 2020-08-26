using Helper;
using Helper.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebApplication.Template.Controllers
{
    [RouteArea("Template")]
    [RoutePrefix("Menu-Item")]
    public class MenuItemController : Controller
    {
        // GET: BackEnd/MenuItem
        [HttpPost]
        [Route("Action/MenuItem-Manage")]
        public ActionResult MenuItemManage()
        {
            try
            {
                RouteAreaAttribute areaAttribute = (RouteAreaAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(RouteAreaAttribute));
                string routeArea = areaAttribute.AreaName;
                using (var menuItemService = new MenuItemService())
                    return menuItemService.MenuItemManage(new AreaIDRequestModel
                    {
                        RouteArea = routeArea
                    });
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }
    }
}
