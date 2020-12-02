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
    [RoutePrefix("AgentFee")]
    public class AgentFeeController : CMSController
    {
        // GET: BackEnd/AgentFee
        public ActionResult DataList()
        {
            return View();
        }
        public ActionResult Setting()
        {
            if (Helper.Current.UserLogin.IsCustomerLogged())
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                string customerId = CustomerService.GetCustomerIDByUserID(userId);
                AirAgentFeeService airFeeAgentService = new AirAgentFeeService();
                AirAgentFee airFeeAgentResult = airFeeAgentService.GetAgentFee(customerId);
                if (airFeeAgentResult != null)
                {
                    return View(airFeeAgentResult);
                }
            }
            return View();
        }

        public ActionResult Details(string id)
        {
            AirAgentFeeService airFeeAgentService = new AirAgentFeeService();
            var airFeeAgentResult = airFeeAgentService.ViewAgentFee(id);
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
                using (var service = new AirAgentFeeService())
                    return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }
        [HttpPost]
        [Route("Action/GetAgentFee")]
        public ActionResult GetAgentFee(AirAgentFee_AgentModel model)
        {
            try
            {
                using (var service = new AirAgentFeeService())
                    return service.GetAgentFee(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/ConfigFee")]
        public ActionResult Update(AirAgentFeeConfigModel model)
        {
            try
            {
                using (var service = new AirAgentFeeService())
                    return service.ConfigFee(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }
        [HttpPost]
        [Route("Action/GetFeeConfig")]
        public ActionResult GetFeeConfigByNational(AirAgentFee_NationalIDModel model)
        {
            try
            {
                using (var service = new AirAgentFeeService())
                    return service.GetFeeConfigByNational(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }
    }
}