using AIR.Helper.Session;
using AIRService.Service;
using AIRService.WebService.VNA.Authen;
using AIRService.WS.Entities;
using ApiPortalBooking.Models;
using ApiPortalBooking.Models.VNA_WS_Model;
using Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Services;

namespace WebApplication.Management.Controllers
{
    [RouteArea("Management")]
    [RoutePrefix("Home-Page")]
    public class HomeController : CMSController
    {
        // GET: Management/Home
        public ActionResult Index()
        {
            AirAgentService agentService = new AirAgentService();
            string agentId = AirAgentService.GetAgentIDByUserID(Helper.Current.UserLogin.IdentifierID);
            AirAgentResult airAgent = agentService.ViewAgentByID(agentId);
            if (airAgent != null)
                return View(airAgent);
            //
            return View();
        }
        public ActionResult Test()
        {
            return View();
        }
        // api ******************************************************************************************************************************

        [HttpPost]
        [Route("Action/BookingChart")]
        public ActionResult BookingChart(BookChart model)
        {
            try
            {
                using (var service = new BookOrderService())
                    return service.BookingChartData(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
    }
}