using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    }
}