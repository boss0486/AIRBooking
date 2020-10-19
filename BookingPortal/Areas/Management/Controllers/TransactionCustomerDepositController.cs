﻿using Helper;
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
    [RoutePrefix("TransactionCustomerDeposit")]
    public class TransactionCustomerDepositController : CMSController
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
            TransactionCustomerDepositService service = new TransactionCustomerDepositService();
            TransactionCustomerDepositResult model = service.TransactionDepositModel(id);
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
                using (var service = new TransactionCustomerDepositService())
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
        [Route("Action/Create")]
        public ActionResult Create(TransactionCustomerDepositCreateModel model)
        {
            try
            {
                using (var service = new TransactionCustomerDepositService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>:" + ex);
            }
        }
    }
}