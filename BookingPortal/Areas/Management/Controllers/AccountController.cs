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
    [RoutePrefix("Account")]
    public class AccountController : CMSController
    {
        [IsManage(act: true, text: "Create")]
        public ActionResult LoginInfo()
        {
            string id = Helper.Current.UserLogin.IdentifierID;
            AccountService service = new AccountService();
            UserModel model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

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
            AccountService service = new AccountService();
            UserModel model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        [IsManage(act: true, text: "Profile")]
        public new ActionResult Profile()
        {
            string id = Helper.Current.UserLogin.IdentifierID;
            AccountService service = new AccountService();
            UserModel model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        //
        [IsManage(act: false, text: "Change-Password")]
        public ActionResult Password()
        {
            return View();
        }


        // API ********************************************************************************************************
        [HttpPost]
        [IsManage(act: true, text: "DataList")]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                AccountService service = new AccountService();
                return service.Datalist(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Create")]
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
        [IsManage(act: true, text: "Update")]
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
        [IsManage(act: true, text: "Delete")]
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
        [IsManage(act: true)]
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