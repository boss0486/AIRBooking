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
    [RoutePrefix("AgentSpendingLimit")]
    public class AgentSpendingLimitController : CMSController
    {
        // GET: BackEnd/bank
        public ActionResult DataList()
        {
            return View();
        }
        public ActionResult Payment()
        {
            return View();
        }

        public ActionResult Setting(string id)
        {
            AgentSpendingLimitService service = new AgentSpendingLimitService();
            AgentSpendingLimitResult model = service.ViewgentSpendingLimit(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        public ActionResult Details(string id)
        {
            AgentSpendingLimitService service = new AgentSpendingLimitService();
            AgentSpendingLimitResult model = service.ViewgentSpendingLimit(id);
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
                using (var service = new AgentSpendingLimitService())
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
        [Route("Action/Payment")]
        public ActionResult PayList(SearchModel model)
        {
            try
            {
                using (var service = new AgentSpendingLimitPaymentService())
                {
                    return service.ShowPay(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/ExPayment")]
        public ActionResult ExPayment(AgentSpendingLimitPaymentSettingModel model)
        {
            try
            {
                using (var service = new AgentSpendingLimitPaymentService())
                {
                    return service.ExPayment(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Setting")]
        public ActionResult Setting(AgentSpendingLimitSettingModel model)
        {
            try
            {
                using (var service = new AgentSpendingLimitService())
                    return service.Setting(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>:" + ex);
            }
        }
    }
}