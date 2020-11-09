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
    public class CustomerController : CMSController
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
            CustomerService service = new CustomerService();
            Customer model = service.GetCustomerByID(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        public ActionResult Details(string id)
        {
            CustomerService service = new CustomerService();
            CustomerResult model = service.ViewCustomerByID(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(CustomerSearchModel model)
        {
            try
            {
                using (var service = new CustomerService())
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
        public ActionResult Create(CustomerCreateModel model)
        {
            try
            {
                using (var service = new CustomerService())
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
        public ActionResult Update(CustomerUpdateModel model)
        {
            try
            {
                using (var service = new CustomerService())
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
        public ActionResult Delete(CustomerIDModel model)
        {
            try
            {
                using (var service = new CustomerService())
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
                var service = new CustomerService();
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
        public ActionResult AgentData()
        {
            try
            {
                var service = new CustomerService(); 
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
        [Route("Action/GetCustomer-By-SuplierID")]
        [IsManage(skip: true)]
        public ActionResult GetCustomerBySuplierID(SupplierIDModel model)
        {
            try
            {
                var service = new CustomerService();
                var data = service.GetCustomerBySupplierIDOption(model.ID, (int)CustomerEnum.CustomerType.AGENT);
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