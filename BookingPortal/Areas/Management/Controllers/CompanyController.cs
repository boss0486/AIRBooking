﻿using Helper;
using Helper.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;
using WebCore.ENM;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebApplication.Management.Controllers
{
    [IsManage]
    [RouteArea("Management")]
    [RoutePrefix("Company")]
    public class CompanyController : CMSController
    {
        // GET: BackEnd/Company
        public ActionResult DataList()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Update(string id)
        {
            CompanyService service = new CompanyService();
            Company model = service.GetCompanyByID(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        public ActionResult Details(string id)
        {
            CompanyService service = new CompanyService();
            CompanyResult model = service.ViewCompanyByID(id);
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
                using (var service = new CompanyService())
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
        [Route("Action/Create")]
        public ActionResult Create(CompanyCreateModel model)
        {
            try
            {
                using (var service = new CompanyService())
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
        [Route("Action/Update")]
        public ActionResult Update(CompanyUpdateModel model)
        {
            try
            {
                using (var service = new CompanyService())
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
        [Route("Action/Delete")]
        public ActionResult Delete(CompanyIDModel model)
        {
            try
            {
                using (var service = new CompanyService())
                    return service.Delete(model);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }
        //OPTION ##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DropdownList")]
        [IsManage(skip: true)]
        public ActionResult DropdownList()
        {
            try
            {
                var service = new CompanyService();
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