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
    [RoutePrefix("Transaction-Deposit")]
    [IsManage(act: false)]
    public class TransactionDepositController : CMSController
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

        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [IsManage(act: true, text: "DataList")]
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
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Create")]
        [Route("Action/Create")]
        public ActionResult Create(TransactionDepositCreateModel model)
        {
            try
            {
                using (var service = new TransactionDepositService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>:" + ex);
            }
        }
    }
}