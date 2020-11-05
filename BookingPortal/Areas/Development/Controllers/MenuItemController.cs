using Helper;
using Helper.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;
using WebCore.ENM;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebApplication.Development.Controllers
{
    [IsManage]
    [RouteArea("Development")]
    [RoutePrefix("Menu-Item")]
    public class MenuItemController : CMSController
    {
        // GET: BackEnd/MenuItem
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
            MenuItemService menuItemService = new MenuItemService();
            var model = menuItemService.GetMenuItemByID(id);
            if (model != null)
                return View(model);
            // 
            return View();
        }

        public ActionResult Setting()
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
                using (var menuItemService = new MenuItemService())
                    return menuItemService.Update(model);
            }
            catch  
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
                using (var menuItemService = new MenuItemService())
                    return menuItemService.Delete(model.ID);
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
                    return menuItemService.DataList(model);
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

        [HttpPost]
        [Route("Action/SortAuto")]
        public ActionResult SortAuto()
        {
            try
            {
                using (var menuItemService = new MenuItemService())
                    return menuItemService.MenuItemManage_Sort();
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        // ###########################################################################################################################################33


        [HttpPost]
        [Route("Action/Menu-Sync")]
        public ActionResult MenuControlSync()
        {
            try
            {
                MenuControllerService menuControllerService = new MenuControllerService();
                return menuControllerService.MenuControlSync();

            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Menu-Sync-Area")]
        public ActionResult MenuControlSyncByAreaID(MenuItemIDModel model)
        {
            try
            {
                MenuControllerService menuControllerService = new MenuControllerService();
                string routeArea = model.ID;
                return menuControllerService.MenuControlSyncByRouteArea(routeArea);

            }
            catch (Exception ex)

            {
                return Notifization.TEST("::" + ex);
            }
        }



        [HttpPost]
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
