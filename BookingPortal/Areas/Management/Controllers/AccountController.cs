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
    [IsManage]
    [RouteArea("Management")]
    [RoutePrefix("Account")]
    public class AccountController : CMSController
    {
        public ActionResult LoginInfo()
        {
            string id = Helper.Current.UserLogin.IdentifierID;
            AccountService service = new AccountService();
            UserResult model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

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
            AccountService service = new AccountService();
            UserResult model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        public new ActionResult Profile(string id = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                id = Helper.Current.UserLogin.IdentifierID;
            }
            AccountService service = new AccountService();
            UserResult model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }        
        
        public ActionResult Details(string id)
        {
            AccountService service = new AccountService();
            UserResult model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        //
        public ActionResult ChangePassword()
        {
            return View();
        }


        // API ********************************************************************************************************
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                AccountService service = new AccountService();
                return service.DataList(model);
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
                AccountService service = new AccountService();
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
                AccountService service = new AccountService();
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
                AccountService service = new AccountService();
                return service.Delete(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>" + ex);
            }
        }
        [HttpPost]
        [Route("Action/ChangePassword")]
        public ActionResult ChangePassword(UserChangePasswordModel model)
        {
            try
            {
                // call service
                using (var service = new AccountService())
                    return service.ChangePassword(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }
    }
}