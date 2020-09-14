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
                using (var userGroupService = new RoleService())
                    return userGroupService.DataList(model);
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
                using (var roleService = new RoleService())
                {
                    if (model == null)
                        return Notifization.Invalid(MessageText.Invalid);

                    string title = model.Title;
                    string summary = model.Summary;
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tên nhóm quyền ");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tên nhóm quyền không hợp lệ");
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tên nhóm quyền giới hạn 2-80 ký tự");
                    // summary valid
                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        if (!Validate.TestAlphabet(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                        summary = summary.Trim();
                    };

                    return roleService.Create(model);
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
                using (var roleService = new RoleService())
                {
                    return roleService.Update(model);
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
                using (var roleService = new RoleService())
                {
                    if (model == null)
                        return Notifization.Invalid();
                    return roleService.Delete(model.ID);
                }
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DropDownList")]
        [IsManage(skip: true)]
        public ActionResult DropDownList()
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

    }
}