﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helper;
using Helper.Page;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebApplication.Development.Controllers
{
    [IsManage(act: false)]
    [RouteArea("Development")]
    [RoutePrefix("CMSRole")]
    public class CMSRoleController : CMSController
    {
        // GET: Adm/UserGroup
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
            CMSRoleService service = new CMSRoleService();
            var model = service.CMSRoleModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        [IsManage(act: false)]
        public ActionResult Details(string id)
        {
            CMSRoleService service = new CMSRoleService();
            var model = service.CMSRoleModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [IsManage(act: true)]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                using (var service = new CMSRoleService())
                    return service.Datalist(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true)]
        [Route("Action/Create")]
        public ActionResult Create(CMSRoleCreateModel model)
        {
            try
            {
                string areaId = model.AreaID;
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                areaId = areaId.Trim();
                string areaKeyId = AreaApplicationService.GetAreaKeyByID(areaId);
                if (areaKeyId == "Development")
                {
                    using (var service = new CMSRoleService())
                        return service.Create(model);
                }
                if (areaKeyId == "Management")
                {
                    return Notifization.TEST("ok");
                }
                return Notifization.Invalid(MessageText.Invalid);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [IsManage(act: true)]
        [Route("Action/Update")]
        public ActionResult Update(CMSRoleUpdateModel model)
        {
            try
            {
                string areaId = model.AreaID;
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                areaId = areaId.Trim();
                string areaKeyId = AreaApplicationService.GetAreaKeyByID(areaId);
                if (areaKeyId == "Development")
                {
                    using (var service = new CMSRoleService())
                        return service.Update(model);
                }
                if (areaKeyId == "Management")
                {
                    return Notifization.TEST("ok");
                }
                return Notifization.Invalid(MessageText.Invalid);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true)]
        [Route("Action/Delete")]
        public ActionResult Delete(RoleIDModel model)
        {
            try
            {
                using (var service = new CMSRoleService())
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
        [IsManage(act: true)]
        [Route("Action/Details")]
        public ActionResult Details(CMSRoleIDModel model)
        {
            try
            {
                using (var service = new CMSRoleService())
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

        //##############################################################################################################################################################################################################################################################

        [HttpPost]
        [IsManage(act: true)]
        [Route("Action/DropdownList")]
        public ActionResult DropdownList(CMSRoleAreaIDModel model)
        {
            try
            {
                string areaId = model.AreaID;
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                areaId = areaId.Trim();
                string areaKeyId = AreaApplicationService.GetAreaKeyByID(areaId);
                if (areaKeyId == "Development")
                {
                    using (var service = new CMSRoleService())
                        return Notifization.Data(MessageText.Success, service.DataOption(""));
                }
                if (areaKeyId == "Management")
                {
                    return Notifization.TEST("ok");
                }
                return Notifization.Invalid(MessageText.Invalid);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }

        //##############################################################################################################################################################################################################################################################
    }
}