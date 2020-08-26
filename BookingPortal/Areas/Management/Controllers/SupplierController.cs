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
    [RouteArea("Management")]
    [RoutePrefix("Supplier")]
    [IsManage(act: false)]
    public class SupplierController : CMSController
    {
        // GET: BackEnd/Supplier
        [IsManage(act: false)]
        public ActionResult DataList()
        {
            return View();
        }

        [IsManage(act: false)] 
        public ActionResult Create()
        {
            return View();
        }

        [IsManage(act: false)]
        public ActionResult Update(string id)
        {
            SupplierService service = new SupplierService();
            var model = service.GetSupplierModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        [IsManage(act: false)]
        public ActionResult Details(string id)
        {
            SupplierService service = new SupplierService();
            var model = service.GetSupplierModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [IsManage(act: true, text: "DataList")]
        [Route("Action/DataList")]
        public ActionResult DataList(SupplierSearchModel model)
        {
            try
            {
                using (var service = new SupplierService())
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
        [IsManage(act: true, text: "Create")]
        [Route("Action/Create")]
        public ActionResult Create(SupplierCreateModel model)
        {
            try
            {
                using (var service = new SupplierService())
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
        [IsManage(act: true, text: "Update")]
        [Route("Action/Update")]
        public ActionResult Update(SupplierUpdateModel model)
        {
            try
            {
                using (var service = new SupplierService())
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
        [IsManage(act: true, text: "Delete")]
        [Route("Action/Delete")]
        public ActionResult Delete(SupplierIDModel model)
        {
            try
            {
                using (var service = new SupplierService())
                    return service.Delete(model);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Details")]
        [Route("Action/Details")]
        public ActionResult Details(SupplierIDModel model)
        {
            try
            {
                using (var service = new SupplierService())
                    return Notifization.NotService;
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }

        //OPTION ##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DropdownList")]
        public ActionResult DropdownList()
        {
            try
            {
                var service = new SupplierService();
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
    }
}