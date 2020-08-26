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
    [RoutePrefix("Bank")]
    [IsManage(act: false)]
    public class BankController : CMSController
    {
        // GET: BackEnd/bank
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
            BankService service = new BankService();
            var model = service.UpdateForm(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        [IsManage(act: false)]
        public ActionResult Details(string id)
        {
            BankService service = new BankService();
            var model = service.UpdateForm(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [IsManage(act: true, text: "DataList")]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                using (var service = new BankService())
                { 
                    return service.DataList(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Create")]
        [Route("Action/Create")]
        public ActionResult Create(BankCreateModel model)
        {
            try
            {
                using (var service = new BankService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Update")]
        [Route("Action/Update")]
        public ActionResult Update(BankUpdateModel model)
        {
            try
            {
                using (var service = new BankService())
                    return service.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Delete")]
        [Route("Action/Delete")]
        public ActionResult Delete(BankIDModel model)
        {
            try
            {
                using (var service = new BankService())
                {
                    if (model == null)
                        return Notifization.Invalid();
                    return service.Delete(model.ID);
                }
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Details")]
        [Route("Action/Details")]
        public ActionResult Details(BankIDModel model)
        {
            try
            {
                using (var service = new BankService())
                {
                    if (model == null)
                        return Notifization.Invalid();
                    return service.Detail(model.ID);
                }
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }

    }
}