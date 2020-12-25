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
    [RoutePrefix("TransactionUserSpending")]
    public class TransactionUserSpendingController : CMSController
    {
        // GET: BackEnd/bank
        public ActionResult DataList()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            //TransactionUserSpendingService service = new TransactionUserSpendingService();
            //TransactionUserSpendingResult model = service.GetTransactionUserSpendingModel(id);
            //if (model != null)
            //    return View(model);
            ////
            return View();
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            return Notifization.NotService;
            //try
            //{
            //    using (var service = new TransactionUserSpendingService())
            //    {
            //        return service.DataList(model);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return Notifization.NotService;
            //}
        }

        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(TransactionUserSpendingCreateModel model)
        {
            return Notifization.NotService;
            //try
            //{
            //    using (var service = new TransactionUserSpendingService())
            //        return service.Create(model);
            //}
            //catch (Exception ex)
            //{
            //    return Notifization.TEST(">>>:" + ex);
            //}
        }

        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(TransactionUserSpendingIDModel model)
        {
            return Notifization.NotService;
            //try
            //{
            //    using (var service = new TransactionUserSpendingService())
            //    {
            //        if (model == null)
            //            return Notifization.Invalid();
            //        return service.Delete(model.ID);
            //    }
            //}
            //catch (Exception)
            //{
            //    return Notifization.NotService;
            //}
        }
    }
}