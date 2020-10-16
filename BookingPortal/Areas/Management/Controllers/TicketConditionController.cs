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
        public ActionResult Update()
        {
            return View();
        }
        public ActionResult Details()
        {
            return View();
        }

        // ******************************************************************************************************************************
        [HttpPost]
        [Route("Action/Condition04")]
        public ActionResult AirTicketCondition04(AirTicketCondition04ConfigModel model)
        {
            try
            {
                using (var service = new AirTicketCondition04Service())
                    return service.ConditionFee04(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }
        [HttpPost]
        [Route("Action/EventEnd04")]
        public ActionResult AirTicketCondition04EventEnd(AirTicketCondition04EventEndModel model)
        {
            try
            {
                using (var service = new AirTicketCondition04Service())
                    return service.AirTicketCondition04EventEnd(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }
        // ******************************************************************************************************************************
        [HttpPost]
        [Route("Action/Condition05")]
        public ActionResult AirTicketCondition05(AirTicketCondition05ConfigModel model)
        {
            try
            {
                using (var service = new AirTicketCondition05Service())
                    return service.ConditionFee05Setting(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }
        [HttpPost]
        [Route("Action/EventEnd05")]
        public ActionResult AirTicketCondition05EventEnd(AirTicketConditionFlightLocationID05Model model)
        {
            try
            {
                using (var service = new AirTicketCondition05Service())
                    return service.AirTicketCondition05EventEnd(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }

        [HttpPost]
        [Route("Action/GetCondition05")]
        public ActionResult GetCondition05(AirTicketConditionFlightLocationID05Model model)
        {
            try
            {
                using (var service = new AirTicketCondition05Service())
                    return Notifization.Data("Ok", service.GetAirTicketConditionByID(model));
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }
    }
}