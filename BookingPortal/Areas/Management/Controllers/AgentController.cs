using Helper;
using Helper.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;
using WebCore.ENM;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebApplication.Management.Controllers
{
    [IsManage]
    [RouteArea("Management")]
    [RoutePrefix("Customer")]
    public class AgentController : CMSController
    {
        // GET: BackEnd/Customer
        public ActionResult DataList()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Update(string id)
        {
            AirAgentService service = new AirAgentService();
            AirAgent model = service.GetAgentByID(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        public ActionResult Details(string id)
        {
            AirAgentService service = new AirAgentService();
            AirAgentResult model = service.ViewCustomerByID(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(AirAgentSearchModel model)
        {
            try
            {
                using (var service = new AirAgentService())
                {
                    return service.DataList(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(AirAgentCreateModel model)
        {
            try
            {
                using (var service = new AirAgentService())
                {
                    return service.Create(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(AirAgentUpdateModel model)
        {
            try
            {
                using (var service = new AirAgentService())
                {
                    return service.Update(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(AirAgentIDModel model)
        {
            try
            {
                using (var service = new AirAgentService())
                    return service.Delete(model);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }
        //OPTION ##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DropdownList")]
        [IsManage(skip: true)]
        public ActionResult DropdownList()
        {
            try
            {
                var service = new AirAgentService();
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
        [HttpPost]
        [Route("Action/AgentData")]
        [IsManage(skip: true)]
        public ActionResult GetAgentAll()
        {
            try
            {
                var service = new AirAgentService();
                var data = service.GetAgentData();
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

        [HttpPost]
        [Route("Action/GetAgentForCustomerType")]
        [IsManage(skip: true)]
        public ActionResult GetAgentForCustomerType(AirAgentType model)
        {
            try
            {
                var service = new AirAgentService();
                return service.GetAgentForCustomerType(model.CustomerType);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        //OPTION ##########################################################################################################################################################################################################################################################

    }
}