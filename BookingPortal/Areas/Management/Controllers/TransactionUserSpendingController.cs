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
    [RoutePrefix("TransactionUserSpending")]
    [IsManage(act: false)]
    public class TransactionUserSpendingController : CMSController
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
        public ActionResult Details(string id)
        {
            TransactionUserSpendingService service = new TransactionUserSpendingService();
            var model = service.GetTransactionUserSpendingModel(id);
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
                using (var service = new TransactionUserSpendingService())
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
        public ActionResult Create(TransactionUserSpendingCreateModel model)
        {
            try
            {
                using (var service = new TransactionUserSpendingService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>>:" + ex);
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Delete")]
        [Route("Action/Delete")]
        public ActionResult Delete(TransactionUserSpendingIDModel model)
        {
            try
            {
                using (var service = new TransactionUserSpendingService())
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
        public ActionResult Details(TransactionUserSpendingIDModel model)
        {
            try
            {
                using (var service = new TransactionUserSpendingService())
                {
                    if (model == null)
                        return Notifization.Invalid();
                    return service.Details(model.ID);
                }
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }

    }
}