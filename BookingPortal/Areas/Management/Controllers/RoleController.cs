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
    [RoutePrefix("Role")]
    public class RoleController : CMSController
    {
        // GET: Adm/UserGroup
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
            RoleService service = new RoleService();
            var model = service.GetRoleModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        public ActionResult Details(string id)
        {
            RoleService service = new RoleService();
            var model = service.GetRoleModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        //API ##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                using (var service = new RoleService())
                    return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(RoleCreateModel model)
        {
            try
            {
                using (var service = new RoleService())
                { 
                    return service.Create(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(RoleUpdateModel model)
        {
            try
            {
                using (var service = new RoleService())
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
        public ActionResult Delete(RoleIDModel model)
        {
            try
            {
                using (var service = new RoleService())
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
        [Route("Action/SortUp")]
        public ActionResult SortUp(RoleIDModel model)
        {
            try
            {
                using (var service = new RoleService())
                    return service.SortUp(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        //
        [HttpPost]
        [Route("Action/SortDown")]
        public ActionResult SortDown(RoleIDModel model)
        {
            try
            {
                using (var service = new RoleService())
                    return service.SortDown(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        //##############################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/GetDataOption")]
        [IsManage(skip: true)]
        public ActionResult GetDataOption()
        {
            try
            {
                using (var service = new RoleService())
                {
                    var dtList = service.DataOption();
                    if (dtList.Count == 0)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    return Notifization.Data(MessageText.Success, dtList);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>" + ex);
            }
        }
        
        [HttpPost]
        [Route("Action/GetRoleForUser")]
        [IsManage(skip: true)]
        public ActionResult GetRoleForUser(UserIDModel userIDModel)
        {
            try
            {
                using (var service = new RoleService())
                {
                    var dtList = service.GetRoleForUser(userIDModel.ID);
                    if (dtList.Count == 0)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    return Notifization.Data(MessageText.Success, dtList);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>" + ex);
            }
        }

    }
}