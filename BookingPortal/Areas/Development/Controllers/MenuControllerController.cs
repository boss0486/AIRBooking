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
    [IsManage(act: false)]
    [RouteArea("Development")]
    [RoutePrefix("Menu-Controller")]
    public class MenuControllerController : Controller
    {
        // GET: BackEnd/MenuControllers
        [IsManage(act: false, text: "List")]
        public ActionResult DataList()
        {
            return View();
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DropdownList")]
        public ActionResult DropdownList(string cateId)
        {
            try
            {
                using (var service = new MenuControllerService())
                {
                    var data = service.DataOption(cateId);
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