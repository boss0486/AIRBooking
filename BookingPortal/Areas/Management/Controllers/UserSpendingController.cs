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
    [RoutePrefix("UserSpending")]
    public class UserSpendingController : CMSController
    {
        // GET: BackEnd/bank
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
        [Route("Action/Setting")]
        public ActionResult Setting(UserSpendingCreateModel model)
        {
            try
            {
                using (var service = new UserSpendingService())
                    return service.Setting(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>>:" + ex);
            }
        } 
    }
}