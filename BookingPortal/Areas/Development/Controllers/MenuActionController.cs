using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helper;
using Helper.Page;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebApplication.Development.Controllers
{
    [IsManage]
    [RouteArea("Development")]
    [RoutePrefix("Menu-Action")]
    public class MenuActionController : CMSController
    {
        // GET: BackEnd/MenuActions
        public ActionResult DataList()
        {
            return View();
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DropdownList")]
        public ActionResult DropdownList(string id)
        {
            try
            {
                using (var service = new MenuActionService())
                {
                    var data = service.DataOption(id);
                    if (data.Count == 0)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    return Notifization.Option("OK", data);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Get-ActionBy-CategoryID")]
        public ActionResult GetActionByCategoryID(ActionByCategoryModel model)
        {
            try
            {
                using (var service = new MenuActionService())
                {
                    var data = service.GetActionPageByCategoryID(model);
                    if (data.Count == 0)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    return Notifization.Option("OK", data);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

    }
}