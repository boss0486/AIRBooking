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
    [RoutePrefix("AirportBookConfig")]
    public class AirportBookConfigController : CMSController
    {
        // GET: BackEnd/AirportBookConfig
        public ActionResult DataList()
        {
            return View();
        }
        public ActionResult Setting()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            AirportBookConfigService airFeeAgentService = new AirportBookConfigService();
            var airFeeAgentResult = airFeeAgentService.ViewAirportBookConfig(id);
            if (airFeeAgentResult != null)
                return View(airFeeAgentResult);
            //
            return View();
        }

        // ******************************************************************************************************************************
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                using (var service = new AirportBookConfigService())
                    return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }

        [HttpPost]
        [Route("Action/Setting")]
        public ActionResult Update(AirportBookConfig_SettingModel model)
        {
            try
            {
                using (var service = new AirportBookConfigService())
                    return service.AirportBookConfig_Setting(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        } 
    }
}