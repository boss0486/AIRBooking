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
    [RoutePrefix("TransactionDeposit")]
    public class TransactionDepositController : CMSController
    {
        // GET: BackEnd/bank
        public ActionResult DataList()
        {
            return View();
        }

        public ActionResult Deposit()
        {
            return View();
        }
        public ActionResult Details(string id)
        {
            TransactionDepositService service = new TransactionDepositService();
            TransactionDepositResult model = service.ViewTransactionDeposit(id);
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
                using (var service = new TransactionDepositService())
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
        [Route("Action/Deposit")]
        public ActionResult Deposit(TransactionDepositCreateModel model)
        {
            try
            {
                using (var service = new TransactionDepositService())
                    return service.Deposit(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>:" + ex);
            }
        }
    }
}