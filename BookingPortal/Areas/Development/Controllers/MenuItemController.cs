using Helper;
using Helper.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebApplication.Development.Controllers
{
    [IsManage(act: false)]
    [RouteArea("Development")]
    [RoutePrefix("Menu-Item")]
    public class MenuItemController : CMSController
    {
        // GET: BackEnd/MenuItem
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
        public ActionResult Update()
        {
            return View();
        }
        //
        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(MenuItemCreateFormModel model)
        {
            try
            {
                // call service            
                using (var menuItemService = new MenuItemService())
                    return menuItemService.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        //
        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(MenuItemUpdateFormModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.Invalid("Dữ liệu không hợp lệ");
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
                // call service        
                using (var menuItemService = new MenuItemService())
                    return menuItemService.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }
        //
        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(MenuItemIDModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.Invalid("Dữ liệu không hợp lệ");
                string Id = model.ID;
                if (string.IsNullOrEmpty(Id))
                    return Notifization.Invalid(MessageText.Invalid);
                // call service
                using (var menuItemService = new MenuItemService())
                    return menuItemService.Delete(model);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }
        //
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                using (var menuItemService = new MenuItemService())
                    return menuItemService.Datalist(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        // menu category
        [HttpPost]
        [Route("Action/MenuItem-ByLevel")]
        public ActionResult MenuItemByLevel(AreaIDRequestModel model)
        {
            try
            {
                using (var menuItemService = new MenuItemService())
                    return menuItemService.MenuItemCategory(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        // 
        [HttpPost]
        public ActionResult MenuCategoryItemOption(MenuItemIDModel model)
        {
            try
            {
                using (var menuItemService = new MenuItemService())
                {
                    return menuItemService.LeftMenuItemOption(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }
        //
        //[HttpPost]
        //public ActionResult UpdateInLine(MenuItemEditModel model)
        //{
        //    try
        //    {
        //        using (var menuItemService = new MenuItemService())
        //            return menuItemService.UpdateInline(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Notifization.TEST("::" + ex);
        //    }
        //}
        //
        [HttpPost]
        [Route("Action/SortUp")]
        public ActionResult SortUp(MenuItemIDModel model)
        {
            try
            {
                using (var menuItemService = new MenuItemService())
                    return menuItemService.SortUp(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        //
        [HttpPost]
        [Route("Action/SortDown")]
        public ActionResult SortDown(MenuItemIDModel model)
        {
            try
            {
                using (var menuItemService = new MenuItemService())
                    return menuItemService.SortDown(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }


        // ###########################################################################################################################################33






        [IsManage(act: false, text: "Setting")]
        public ActionResult Setting()
        {
            return View();
        }

        [IsManage(act: false, text: "Keyword-List")]
        public ActionResult Keyword()
        {
            return View();
        }

        [IsManage(act: false, text: "Permission")]
        public ActionResult Permission()
        {
            return View();
        }


        // ********************************************************************************************************
        [HttpPost]
        [IsManage(act: true, text: "Sync")]
        [Route("Action/Menu-Sync")]
        public ActionResult MenuControlSync()
        {
            try
            {
                //Assembly asm = Assembly.GetExecutingAssembly();
                ////var types = AppDomain.CurrentDomain.GetAssemblies().Get);
                ////var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
                //
                //var types = asm.GetType();
                MenuControllerService menuControllerService = new MenuControllerService();
                //
                Assembly asm = Assembly.GetExecutingAssembly();
                var model = asm.GetTypes();
                return menuControllerService.MenuControlSync(model);

            }
            catch (Exception ex)

            {
                return Notifization.TEST("::" + ex);
            }
        }


        // ********************************************************************************************************
        [HttpPost]
        [IsManage(act: true)]
        [Route("Action/Menu-Sync-List")]
        public ActionResult MenuSyncControl(MvcControllerSetting model)
        {
            try
            {
                MenuControllerService menuControllerService = new MenuControllerService();
                var dataList = menuControllerService.MenuControllerOnActionList(model);
                if (dataList.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                return Notifization.Data(MessageText.Success, dataList);
            }
            catch (Exception ex)

            {
                return Notifization.TEST("::" + ex);
            }
        }


    }
}
