using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;
using WebCore.Model.Entities;
using Helper;
using WebCore.Entities;
using WebCore.Services;
using Helper.Page;

namespace WebApplication.Development.Controllers
{
    [IsManage]
    [RouteArea("Development")]
    [RoutePrefix("Area")]
    public class AreaController : CMSController
    {
        //OPTION ##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DropdownList")]
        public ActionResult DropdownList()
        {
            try
            {
                var service = new AreaApplicationService();
                var data = service.DataOption();
                if (data.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                return Notifization.Option("OK", data);

            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

    }
}