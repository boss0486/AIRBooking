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
    [RoutePrefix("Wallet-Customer-Deposit-History")]
    public class WalletCustomerDepositHistoryController : CMSController
    {
        // GET: Management/WalletHistory
        public ActionResult DataList()
        {
            ViewBag.StarDate = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy");
            return View();
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                using (var service = new WalletCustomerDepositHistoryService())
                {
                    return service.DataList(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        } 
    }
}