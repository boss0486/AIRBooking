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

namespace WebApplication.Development.Controllers
{
    [IsManage]
    [RouteArea("Development")]
    [RoutePrefix("User")]
    public class UserController : CMSController
    {
        // GET: BackEnd/User
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
            UserService service = new UserService();
            UserModel model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        public new ActionResult Profile()
        {
            return View();
        }

        public ActionResult Details(string id)
        {
            UserService service = new UserService();
            UserModel model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        public ActionResult Setting()
        {
            return View();
        }

        public ActionResult Password()
        {
            return View();
        }

        public ActionResult Pincode()
        {
            return View();
        }

        // API ********************************************************************************************************
        [HttpPost]
        [Route("Action/ChangePassword")]
        public ActionResult ChangePassword(UserChangePasswordModel model)
        {
            try
            {
                // call service
                using (var service = new UserService())
                    return service.ChangePassword(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                UserService service = new UserService();
                return service.Datalist(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(UserCreateModel model)
        {
            try
            {
                var service = new UserService();
                return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(UserUpdateModel model)
        {
            try
            {
                var service = new UserService();
                return service.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(UserIDModel model)
        {
            try
            {
                using (var service = new UserService())
                    return service.Delete(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>" + ex);
            }
        }
    }
}