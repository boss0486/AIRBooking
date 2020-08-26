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
    [IsManage(act: false)]
    [RouteArea("Development")]
    [RoutePrefix("Account")]
    public class AccountController : CMSController
    {
        // GET: BackEnd/Account
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
            CMSUserService service = new CMSUserService();
            UserModel model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        [IsManage(act: false, text: "Profile")]
        public new ActionResult Profile()
        {
            string id = Helper.Current.UserLogin.IdentifierID;
            CMSUserService service = new CMSUserService();
            UserModel model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        [IsManage(act: false, text: "Details")]
        public ActionResult Details(string id)
        {
            CMSUserService service = new CMSUserService();
            UserModel model = service.GetUserModel(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        [IsManage(act: false, text: "Setting")]
        public ActionResult Setting()
        {
            return View();
        }

        [IsManage(act: false, text: "Change-Password")]
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
        [IsManage(act: true)]
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
        //[IsManage(api: true)]
        [Route("Action/ChangePinCode")]
        public ActionResult ChangePinCode(UserChangePinCodeModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.Invalid();
                //
                string pinCode = model.PinCode;
                string newPinCode = model.NewPinCode;
                string reNewPinCode = model.ReNewPinCode;
                //
                if (string.IsNullOrEmpty(newPinCode))
                    return Notifization.Invalid("Không được để trống mã pin");
                if (!Validate.TestPinCode(newPinCode))
                    return Notifization.Invalid("Mã pin giới hạn [0-9]");
                if (newPinCode.Length != 8)
                    return Notifization.Invalid("Mã pin giới hạn [8] ký tự");
                // call service
                using (var userLoginService = new UserLoginService())
                    return userLoginService.ChangePinCode(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        // API ********************************************************************************************************

        [HttpPost]
        [IsManage(act: true, text: "DataList")]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                CMSUserService service = new CMSUserService();
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
                var service = new CMSUserService();
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
                var service = new CMSUserService();
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
        [IsManage(act: true, text: "Block")]
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
        [IsManage(act: true, text: "Unlock")]
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
        [IsManage(act: true, text: "Active")]
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
        [IsManage(act: true, text: "UnActive")]
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

    }
}