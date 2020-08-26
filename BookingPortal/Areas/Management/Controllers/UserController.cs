using System;
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

namespace WebApplication.Management.Controllers
{
    [IsManage(act: false)]
    [RouteArea("Management")]
    [RoutePrefix("User")]
    public class UserController : CMSController
    {
        // GET: BackEnd/User
        [IsManage(act: false, text: "DataList")]
        public ActionResult DataList()
        {
            return View();
        }

        [IsManage(act: false, text: "Create")]
        public ActionResult Create()
        {
            return View();
        }

        [IsManage(act: false, text: "Update")]
        public ActionResult Update(string id)
        {
            UserClientService service = new UserClientService();
            UserClientModel model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        [IsManage(act: false, text: "Details")]
        public ActionResult Details(string id)
        {
            UserClientService service = new UserClientService();
            UserClientModel model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        // #######################################################################################################################################
        [HttpPost]
        [IsManage(act: true, text: "DataList")]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                UserClientService service = new UserClientService();
                return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Create")]
        [Route("Action/Create")]
        public ActionResult Create(UserClientCreateModel model)
        {
            try
            {
                var service = new UserClientService();
                return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Update")]
        [Route("Action/Update")]
        public ActionResult Update(UserClientUpdateModel model)
        {
            try
            {
                var service = new UserClientService();
                return service.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Delete")]
        [Route("Action/Delete")]
        public ActionResult Delete(UserIDModel model)
        {
            try
            {
                using (var service = new UserClientService())
                    return service.Delete(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>" + ex);
            }
        }

        // #######################################################################################################################################

        [HttpPost]
        [Route("Action/GetUserIsHasRoleBooker")]
        public ActionResult GetUserIsHasRoleBooker(UserIDModel model)
        {
            try
            {
                using (var service = new UserClientService())
                    return service.GetUserIsHasRoleBooker(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>" + ex);
            }
        }

    }
}