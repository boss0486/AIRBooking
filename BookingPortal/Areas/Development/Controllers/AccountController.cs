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
    [RoutePrefix("Account")]
    public class AccountController : CMSController
    {
        // GET: BackEnd/Account
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
            CMSUserService service = new CMSUserService();
            UserResult model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        public new ActionResult Profile()
        {
            string id = Helper.Current.UserLogin.IdentifierID;
            CMSUserService service = new CMSUserService();
            UserResult model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        public ActionResult Details(string id)
        {
            CMSUserService service = new CMSUserService();
            UserResult model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        public ActionResult Password()
        {
            return View();
        }
        //
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
                using (var service = new CMSUserService())
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
                CMSUserService service = new CMSUserService();
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
                var service = new CMSUserService();
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
                var service = new CMSUserService();
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
                // call service
                using (var service = new CMSUserService())
                    return service.Delete(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(">>" + ex);
            }
        }


        //[HttpPost]
        //[IsManage(api: true, text: "Details")]
        //public ActionResult Details(string id)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(id))
        //            return Notifization.Invalid(MessageText.Invalid);
        //        //
        //        id = id.Trim();
        //        // call service
        //        var service = new CMSUserService();
        //        return service.Details(id);
        //    }
        //    catch (Exception)
        //    {
        //        return Notifization.NotService;
        //    }
        //}

        [HttpPost]
        [Route("Action/Block")]
        public ActionResult Block(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Notifization.Invalid(MessageText.Invalid);
                //
                id = id.Trim();
                UserSettingService userSettingService = new UserSettingService();
                return userSettingService.UserBlock(id);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/Unlock")]
        public ActionResult Unlock(string id)
        {
            try
            {

                if (string.IsNullOrEmpty(id))
                    return Notifization.Invalid(MessageText.Invalid);
                //
                id = id.Trim();
                UserSettingService userSettingService = new UserSettingService();
                return userSettingService.UserUnlock(id);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/Active")]
        public ActionResult Active(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Notifization.Invalid(MessageText.Invalid);
                //
                id = id.Trim();
                UserSettingService userSettingService = new UserSettingService();
                return userSettingService.UserActive(id);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/UnActive")]
        public ActionResult UnActive(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Notifization.Invalid(MessageText.Invalid);
                //
                id = id.Trim();
                UserSettingService userSettingService = new UserSettingService();
                return userSettingService.UserUnActive(id);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }


        //public ActionResult GENID(SearchModel model)
        //{
        //    try
        //    {
        //        string strQuery = string.Empty;
        //        int page = 1;
        //        using (var userLoginService = new UserLoginService())
        //        {
        //            if (model == null)
        //                return Notifization.Invalid();
        //            strQuery = model.Query;
        //            page = model.Page;
        //            if (!string.IsNullOrWhiteSpace(strQuery))
        //                strQuery = strQuery.Trim();

        //            return userLoginService.GenEMPID();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Notifization.NotService;
        //    }
        //}
        // For permission ##################################################################################################################################################################
    }
}