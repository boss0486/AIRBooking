using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;
using WebCore.Model.Entities;
using Helper;
using WebCore.Entities;
using WebCore.Services;
using Helper.Page;

namespace WebApplication.Development.Controllers
{
    [IsManage(act: false)]
    [RouteArea("Development")]
    [RoutePrefix("Menu-Category")]
    public class MenuCategoryController : CMSController
    {
        // GET: BackEnd/MenuCategory
        [IsManage(act: false, text: "List")]
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
        public ActionResult Update()
        {
            return View();
        }

        //
        [IsManage(act: false, text: "Details")]
        public ActionResult Details()
        {
            return View();
        }

        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [IsManage(act: true, text: "List")]
        [Route("Action/Data-List")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                string strQuery = string.Empty;
                int page = 1;
                using (var service = new MenuCategoryService())
                    return service.Datalist(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Create")]
        [Route("Action/Create")]
        public ActionResult Create(MenuCategoryCreateModel model)
        {
            try
            {
                using (var service = new MenuCategoryService())
                {
                    if (model == null)
                        return Notifization.Invalid();

                    string title = model.Title;
                    string summary = model.Summary;
                    if (string.IsNullOrEmpty(title))
                        return Notifization.Invalid("Không được để trống tiêu đề");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tiêu đề không hợp lệ");
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
                    // summary valid               
                    if (!string.IsNullOrEmpty(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }

                    return service.Create(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Update")]
        [Route("Action/Update")]
        public ActionResult Update(MenuCategoryUpdateModel model)
        {
            try
            {
                using (var service = new MenuCategoryService())
                {
                    if (model == null)
                        return Notifization.Invalid();

                    string title = model.Title;
                    string summary = model.Summary;
                    if (string.IsNullOrEmpty(title))
                        return Notifization.Invalid("Không được để trống tiêu đề");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tiêu đề không hợp lệ");
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
                    // summary valid               
                    if (!string.IsNullOrEmpty(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }
                    return service.Update(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [IsManage(act: true, text: "Delete")]
        [Route("Action/Delete")]
        public ActionResult Delete(MenuCategoryIDModel model)
        {
            try
            {
                using (var service = new MenuCategoryService())
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
        [IsManage(act: true, text: "Details")]
        [Route("Action/Details")]
        public ActionResult Details(MenuCategoryIDModel model)
        {
            try
            {
                using (var service = new MenuCategoryService())
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
    }
}