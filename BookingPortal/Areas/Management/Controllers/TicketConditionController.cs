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

namespace WebApplication.Management.Controllers
{
    [IsManage]
    [RouteArea("Management")]
    [RoutePrefix("TicketCondition")]
    public class TicketConditionController : CMSController
    {
        public ActionResult FeeSetting()
        {
            return View();
        }

        // ******************************************************************************************************************************
        [HttpPost]
        [Route("Action/Condition04")]
        public ActionResult AirTicketCondition(AirTicketConditionFeeConfigModel model)
        {
            try
            {
                using (var service = new AirTicketConditionFeeService())
                    return service.ConditionFee04(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        } 
        [HttpPost]
        [Route("Action/EventEnd")]
        public ActionResult AirTicketConditionEventEnd(AirTicketConditionEventEndModel model)
        {
            try
            {
                using (var service = new AirTicketConditionFeeService())
                    return service.AirTicketConditionEventEnd(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }
    }
}