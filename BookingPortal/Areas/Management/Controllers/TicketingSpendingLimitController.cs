using Helper;
using Helper.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebApplication.Management.Controllers
{
    [IsManage]
    [RouteArea("Management")]
    [RoutePrefix("TicketingSpendingLimit")]
    public class TicketingSpendingLimitController : CMSController
    {
        // GET: BackEnd/bank
        public ActionResult DataList()
        {
            return View();
        }
         
        public ActionResult Setting(string id)
        {
            TiketingSpendingLimitService service = new TiketingSpendingLimitService();
            TiketingSpendingLimitResult model = service.ViewgentSpendingLimit(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        public ActionResult Details(string id)
        {
            TiketingSpendingLimitService service = new TiketingSpendingLimitService();
            TiketingSpendingLimitResult model = service.ViewgentSpendingLimit(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                using (var service = new TiketingSpendingLimitService())
                {
                    return service.DataList(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        //##########################################################################################################################################################################################################################################################
        
         

        [HttpPost]
        [Route("Action/Setting")]
        public ActionResult Setting(TiketingSpendingLimitSettingModel model)
        {
            try
            {
                using (var service = new TiketingSpendingLimitService())
                    return service.Setting(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>:" + ex);
            }
        }
    }
}