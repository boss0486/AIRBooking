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
    [RoutePrefix("Transaction-History")]
    [IsManage(act: false)]
    public class TransactionHistoryController : CMSController
    {
        // GET: Management/TransactionHistory
        public ActionResult Deposit()
        {
            return View();
        }

        public ActionResult Payment()
        {
            return View();
        }

        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [IsManage(act: true, text: "Deposit")]
        [Route("Action/Deposit")]
        public ActionResult Deposit(SearchModel model)
        {
            try
            {
                using (var service = new TransactionDepositHistoryService())
                {
                    return service.DataList(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [IsManage(act: true, text: "Payment")]
        [Route("Action/Payment")]
        public ActionResult Payment(SearchModel model)
        {
            try
            {
                using (var service = new TransactionPaymentHistoryService())
                {
                    return service.DataList(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }
    }
}