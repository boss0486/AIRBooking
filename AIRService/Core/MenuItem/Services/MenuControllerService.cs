using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using Helper;
using System.Web;
using WebCore.Core;
using System.Reflection;
using WebCore.Model.Enum;
using WebCore.ENM;
using WebCore.Model;
using WebCore.Entities;
using WebCore.Model.Entities;
using System.IO;
using Newtonsoft.Json;

namespace WebCore.Services
{
    public interface IMenuControllerService : IEntityService<MenuController> { }
    public class MenuControllerService : EntityService<MenuController>, IMenuControllerService
    {
        public MenuControllerService() : base() { }
        public MenuControllerService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
            string query = model.Query;
            int page = model.Page;
            //
            if (!string.IsNullOrWhiteSpace(query))
                query = model.Query;
            else
                query = "";
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM MenuController WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' ORDER BY [CreatedDate]";
            var dtList = _connection.Query<MenuControllerResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query) }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(MessageText.NotFound);

            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            Helper.Model.RoleAccountModel roleAccountModel = new Helper.Model.RoleAccountModel
            {

            };
            return Notifization.Data(MessageText.Success + "::" + sqlQuery, data: result, role: roleAccountModel, paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public List<MvcControllerOption> MenuControllerOnActionList(MvcControllerSetting model)
        {
            // 
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM MenuController WHERE RouteArea = @RouteArea AND Enabled = 1 ORDER BY Title";
            List<MvcControllerOption> dtList = _connection.Query<MvcControllerOption>(sqlQuery, new { RouteArea = model.RouteArea }).ToList();
            if (dtList.Count == 0)
                return new List<MvcControllerOption>();
            // 
            foreach (var item in dtList)
            {
                sqlQuery = @" SELECT * FROM MenuAction WHERE CategoryID = @CategoryID AND Enabled = 1 ORDER BY ApiAction, Title";
                List<MvcActionOption> actionList = _connection.Query<MvcActionOption>(sqlQuery, new { RouteArea = model.RouteArea, CategoryID = item.ID }).ToList();
                item.Actions = actionList;
                item.ActionCount = actionList.Count();
            }
            // return
            return dtList;
        }

        public List<MvcControllerForPermision> PermisionController(MvcControllerRoleIDModel model)
        {
            // 
            string langID = Helper.Current.UserLogin.LanguageID;
            string roleId = model.RoleID;
            string sqlQuery = @" SELECT c.ID,c.KeyID,c.Title,Status = CASE WHEN (select count(s.ID) from RoleControllerSetting as s where s.RouteArea = c.RouteArea AND s.ControllerID = c.ID AND s.RoleID = @RoleID ) > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END
                FROM MenuController as c WHERE c.RouteArea =  @RouteArea ORDER BY Title  ";
            List<MvcControllerForPermision> dtList = _connection.Query<MvcControllerForPermision>(sqlQuery, new { RouteArea = model.RouteArea, RoleID = roleId }).ToList();
            if (dtList.Count == 0)
                return new List<MvcControllerForPermision>();
            // 

            foreach (var item in dtList)
            {
                sqlQuery = @" SELECT a.ID,a.KeyID,a.Title, Status = CASE WHEN (select count(s.ID) from RoleActionSetting as s where s.RouteArea = a.RouteArea AND s.ControllerID = a.CategoryID AND s.RoleID = @RoleID ) > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END 
                              FROM MenuAction as a WHERE a.RouteArea = @RouteArea AND a.CategoryID = @CategoryID ORDER BY a.OrderID";
                List<MvcActionForPermision> actionList = _connection.Query<MvcActionForPermision>(sqlQuery, new { RouteArea = model.RouteArea, CategoryID = item.ID, RoleID = roleId }).ToList();
                item.Actions = actionList;
            }
            // return
            return dtList;
        }


        public ActionResult MenuControlSync()
        {
            AreaApplicationService areaApplicationService = new AreaApplicationService();
            var areaOptions = areaApplicationService.DataOption();
            if (areaOptions.Count == 0)
                return Notifization.Invalid(MessageText.Invalid);
            //
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    foreach (var area in areaOptions)
                    {
                        string routeArea = area.ID;
                        // read json
                        List<MvcControllerModel> listController = new List<MvcControllerModel>();
                        string file = string.Empty;
                        if (AreaApplicationService.GetAreaTypeByID(routeArea) == (int)AreaApplicationEnum.AreaType.DEVELOPMENT)
                            file = HttpContext.Current.Server.MapPath(@"/Areas/Development/Data/dev-contruct-permission.json");
                        //
                        if (AreaApplicationService.GetAreaTypeByID(routeArea) == (int)AreaApplicationEnum.AreaType.MANAGEMENT)
                            file = HttpContext.Current.Server.MapPath(@"/Areas/Development/Data/mnr-contruct-permission.json");
                        //
                        if (string.IsNullOrWhiteSpace(file))
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        using (StreamReader r = new StreamReader(file))
                        {
                            string json = r.ReadToEnd();
                            List<PerissionControllerModel> perissionControllerModel = JsonConvert.DeserializeObject<List<PerissionControllerModel>>(json);
                            if (perissionControllerModel != null && perissionControllerModel.Count > 0)
                            {
                                foreach (var ctrlItem in perissionControllerModel)
                                {
                                    List<MvcActionModel> actions = new List<MvcActionModel>();
                                    List<PerissionActionModel> perissionActionModels = new List<PerissionActionModel>();

                                    if (ctrlItem.Action != null || ctrlItem.Action.Count > 0)
                                    {
                                        foreach (var actItem in ctrlItem.Action)
                                        {
                                            actions.Add(new MvcActionModel
                                            {
                                                ControllerID = ctrlItem.KeyID,
                                                RouteArea = routeArea,
                                                OrderID = actItem.OrderID,
                                                KeyID = actItem.KeyID,
                                                Title = actItem.Title,
                                                Method = actItem.Method,
                                                APIRouter = actItem.APIRouter
                                            });
                                        }
                                    }
                                    //
                                    listController.Add(new MvcControllerModel
                                    {
                                        RouteArea = routeArea,
                                        RoutePrefix = "--",
                                        KeyID = ctrlItem.KeyID,
                                        Title = ctrlItem.Title,
                                        OrderID = ctrlItem.OrderID,
                                        Actions = actions,
                                    });
                                }
                            }
                        }
                        if (listController.Count == 0)
                        {
                            // delete all action and all controller
                            _connection.Execute("Truncate Table MenuAction", transaction: _transaction);
                            _connection.Execute("Truncate Table MenuController", transaction: _transaction);
                            _transaction.Commit();
                            return Notifization.Error("Deleted all controller and action");
                        };
                        //////// #1. find all controller need delete
                        string sqlController = @"SELECT * FROM MenuController WHERE RouteArea = @RouteArea AND ID NOT IN ('" + String.Join("','", listController.Select(m => Helper.Security.Library.FakeGuidID(m.RouteArea + m.KeyID))) + "')";
                        List<string> lstControllerId = _connection.Query<MenuController>(sqlController, new { RouteArea = routeArea }, transaction: _transaction).Select(m => m.ID).ToList();
                        // #2. delete all action of controller need delete
                        if (lstControllerId.Count > 0)
                        {
                            //
                            string sqlAction = @"DELETE MenuAction WHERE RouteArea = @RouteArea AND CategoryID IN ('" + String.Join("','", lstControllerId) + "')";
                            _connection.Execute(sqlAction, new { RouteArea = routeArea }, transaction: _transaction);
                            //
                            sqlController = @"DELETE MenuController WHERE RouteArea = @RouteArea AND ID IN ('" + String.Join("','", lstControllerId) + "')";
                            _connection.Execute(sqlController, new { RouteArea = routeArea }, transaction: _transaction);
                        }
                        MenuControllerService menuControllerService = new MenuControllerService(_connection);
                        //////// delete controller and action not in list ************************************************************************************************************
                        // delete all action of the controller in db but not in action list of model
                        // #1 get action of controller in db
                        var lstControllerIdInDb = menuControllerService.GetAlls(m => m.RouteArea == routeArea, transaction: _transaction).Select(m => m.ID).ToList();
                        if (lstControllerIdInDb.Count > 0)
                        {
                            string sqlAction = @"DELETE MenuAction WHERE RouteArea = @RouteArea AND CategoryID IN ('" + String.Join("','", lstControllerIdInDb) + "')";
                            _connection.Execute(sqlAction, new { CategoryID = lstControllerIdInDb, RouteArea = routeArea }, transaction: _transaction);
                        }
                        // area
                        MenuActionService menuActionService = new MenuActionService(_connection);
                        foreach (var controller in listController)
                        {
                            string prefix = controller.RoutePrefix;//GetRoutePrefixTemplate(controller.GetType());
                            string ctrlKeyId = controller.KeyID;
                            string ctrlTitle = controller.Title;
                            //
                            string controllerId = Helper.Security.Library.FakeGuidID(routeArea + ctrlKeyId);
                            //
                            var actionList = controller.Actions;
                            var menuController = menuControllerService.GetAlls(m => m.ID == controllerId && m.RouteArea == routeArea, transaction: _transaction).FirstOrDefault();
                            if (menuController == null)
                            {
                                // check csdl
                                menuControllerService.Create<string>(new MenuController()
                                {
                                    ID = controllerId,
                                    RouteArea = routeArea,
                                    RoutePrefix = prefix,
                                    KeyID = ctrlKeyId,
                                    Title = ctrlTitle,
                                    OrderID = controller.OrderID,
                                    //Enabled = (int)ModelEnum.Enabled.ENABLED,
                                }, transaction: _transaction);
                                // create action
                                if (actionList.Count == 0)
                                {
                                    // delete all action in database
                                    string sqlAction = @"DELETE MenuAction WHERE RouteArea = @RouteArea AND CategoryID = @CategoryID";
                                    _connection.Execute(sqlAction, new { RouteArea = routeArea, CategoryID = controllerId }, transaction: _transaction);
                                    continue;
                                }
                                //
                                foreach (var action in actionList)
                                {
                                    var actKeyId = action.KeyID;
                                    var actTitle = action.Title;
                                    string actionId = Helper.Security.Library.FakeGuidID(controllerId + actKeyId);
                                    // Because controller new should be not need check action
                                    var menuAction = menuActionService.GetAlls(m => m.CategoryID == controllerId && m.KeyID == actKeyId && m.RouteArea == routeArea, transaction: _transaction).FirstOrDefault();
                                    if (menuAction == null)
                                    {
                                        menuActionService.Create<string>(new MenuAction()
                                        {
                                            ID = actionId,
                                            RouteArea = routeArea,
                                            CategoryID = controllerId,
                                            KeyID = actKeyId,
                                            Method = action.Method,
                                            Title = actTitle,
                                            APIRouter = action.APIRouter,
                                            OrderID = action.OrderID,
                                            Enabled = (int)ModelEnum.Enabled.ENABLED,
                                        }, transaction: _transaction);
                                    }
                                    else
                                    {
                                        menuAction.Method = action.Method;
                                        menuAction.Title = actTitle;
                                        menuAction.APIRouter = action.APIRouter;
                                        menuAction.OrderID = action.OrderID;
                                        menuActionService.Update(menuAction, transaction: _transaction);
                                    }
                                }
                            }
                            else // update action 
                            {
                                // update controller for: area and prefix
                                menuController.RouteArea = controller.RouteArea;
                                menuController.RoutePrefix = controller.RoutePrefix;
                                menuController.Title = ctrlTitle;
                                menuController.OrderID = controller.OrderID;
                                menuControllerService.Update(menuController, transaction: _transaction);
                                // update action 
                                if (actionList.Count == 0)
                                {
                                    // delete all action in database
                                    string sqlAction = @"DELETE MenuAction WHERE RouteArea = @RouteArea AND CategoryID = @CategoryID";
                                    _connection.Execute(sqlAction, new { RouteArea = routeArea, CategoryID = menuController.ID }, transaction: _transaction);
                                    continue;
                                }
                                //
                                controllerId = menuController.ID;
                                foreach (var action in actionList)
                                {
                                    var actKeyId = action.KeyID;
                                    var actTitle = action.Title;
                                    string actionId = Helper.Security.Library.FakeGuidID(controllerId + actKeyId);
                                    var menuAction = menuActionService.GetAlls(m => m.CategoryID == controllerId && m.KeyID == actKeyId && m.RouteArea == routeArea, transaction: _transaction).FirstOrDefault();
                                    if (menuAction == null)
                                    {
                                        menuActionService.Create<string>(new MenuAction()
                                        {
                                            ID = actionId,
                                            RouteArea = routeArea,
                                            CategoryID = controllerId,
                                            KeyID = actKeyId,
                                            Method = action.Method,
                                            Title = actTitle,
                                            APIRouter = action.APIRouter,
                                            OrderID = action.OrderID,
                                            Enabled = (int)ModelEnum.Enabled.ENABLED,
                                        }, transaction: _transaction);
                                    }
                                    else
                                    {
                                        menuAction.Method = action.Method;
                                        menuAction.Title = actTitle;
                                        menuAction.APIRouter = action.APIRouter;
                                        menuAction.OrderID = action.OrderID;
                                        menuActionService.Update(menuAction, transaction: _transaction);
                                    }
                                }
                            }
                        }
                    }
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST(">>:" + ex);
                }
            }
        }

        public ActionResult MenuControlSyncByRouteArea(string routeArea)
        {
            // read json
            List<MvcControllerModel> listController = new List<MvcControllerModel>();
            string file = string.Empty;
            if (AreaApplicationService.GetAreaTypeByID(routeArea) == (int)AreaApplicationEnum.AreaType.DEVELOPMENT)
                file = HttpContext.Current.Server.MapPath(@"/Areas/Development/Data/dev-contruct-permission.json");
            //
            if (AreaApplicationService.GetAreaTypeByID(routeArea) == (int)AreaApplicationEnum.AreaType.MANAGEMENT)
                file = HttpContext.Current.Server.MapPath(@"/Areas/Development/Data/mnr-contruct-permission.json");
            //
            if (string.IsNullOrWhiteSpace(file))
                return Notifization.Invalid(MessageText.Invalid);
            //
            using (StreamReader r = new StreamReader(file))
            {
                string json = r.ReadToEnd();
                List<PerissionControllerModel> perissionControllerModel = JsonConvert.DeserializeObject<List<PerissionControllerModel>>(json);
                if (perissionControllerModel != null && perissionControllerModel.Count > 0)
                {
                    foreach (var ctrlItem in perissionControllerModel)
                    {
                        List<MvcActionModel> actions = new List<MvcActionModel>();
                        List<PerissionActionModel> perissionActionModels = new List<PerissionActionModel>();

                        if (ctrlItem.Action != null || ctrlItem.Action.Count > 0)
                        {
                            foreach (var actItem in ctrlItem.Action)
                            {
                                actions.Add(new MvcActionModel
                                {
                                    ControllerID = ctrlItem.KeyID,
                                    RouteArea = routeArea,
                                    OrderID = actItem.OrderID,
                                    KeyID = actItem.KeyID,
                                    Title = actItem.Title,
                                    Method = actItem.Method,
                                    APIRouter = actItem.APIRouter
                                });
                            }
                        }
                        //
                        listController.Add(new MvcControllerModel
                        {
                            RouteArea = routeArea,
                            RoutePrefix = "--",
                            KeyID = ctrlItem.KeyID,
                            Title = ctrlItem.Title,
                            OrderID = ctrlItem.OrderID,
                            Actions = actions,
                        });
                    }
                }
            }

            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (listController.Count == 0)
                    {
                        // delete all action and all controller
                        _connection.Execute("Truncate Table MenuAction", transaction: _transaction);
                        _connection.Execute("Truncate Table MenuController", transaction: _transaction);
                        _transaction.Commit();
                        return Notifization.Error("Deleted all controller and action");
                    };
                    //////// #1. find all controller need delete
                    string sqlController = @"SELECT * FROM MenuController WHERE ID NOT IN ('" + String.Join("','", listController.Select(m => Helper.Security.Library.FakeGuidID(m.RouteArea + m.KeyID))) + "')";
                    List<string> lstControllerId = _connection.Query<MenuController>(sqlController, transaction: _transaction).Select(m => m.ID).ToList();
                    // #2. delete all action of controller need delete
                    if (lstControllerId.Count > 0)
                    {
                        //
                        string sqlAction = @"DELETE MenuAction WHERE CategoryID IN ('" + String.Join("','", lstControllerId) + "')";
                        _connection.Execute(sqlAction, transaction: _transaction);
                        //
                        sqlController = @"DELETE MenuController WHERE ID IN ('" + String.Join("','", lstControllerId) + "')";
                        _connection.Execute(sqlController, transaction: _transaction);
                    }
                    MenuControllerService menuControllerService = new MenuControllerService(_connection);
                    //////// delete controller and action not in list ************************************************************************************************************
                    // delete all action of the controller in db but not in action list of model
                    // #1 get action of controller in db
                    var lstControllerIdInDb = menuControllerService.GetAlls(m => m.RouteArea == routeArea, transaction: _transaction).Select(m => m.ID).ToList();
                    if (lstControllerIdInDb.Count > 0)
                    {
                        string sqlAction = @"DELETE MenuAction WHERE CategoryID IN ('" + String.Join("','", lstControllerIdInDb) + "')";
                        _connection.Execute(sqlAction, new { CategoryID = lstControllerIdInDb }, transaction: _transaction);
                    }
                    // area
                    MenuActionService menuActionService = new MenuActionService(_connection);
                    foreach (var controller in listController)
                    {
                        string prefix = controller.RoutePrefix;//GetRoutePrefixTemplate(controller.GetType());
                        string ctrlKeyId = controller.KeyID;
                        string ctrlTitle = controller.Title;
                        //
                        string controllerId = Helper.Security.Library.FakeGuidID(routeArea + ctrlKeyId);
                        //
                        var actionList = controller.Actions;
                        var menuController = menuControllerService.GetAlls(m => m.ID == controllerId, transaction: _transaction).FirstOrDefault();
                        if (menuController == null)
                        {
                            // check csdl
                            menuControllerService.Create<string>(new MenuController()
                            {
                                ID = controllerId,
                                RouteArea = routeArea,
                                RoutePrefix = prefix,
                                KeyID = ctrlKeyId,
                                Title = ctrlTitle,
                                OrderID = controller.OrderID,
                                //Enabled = (int)ModelEnum.Enabled.ENABLED,
                            }, transaction: _transaction);
                            // create action
                            if (actionList.Count == 0)
                            {
                                // delete all action in database
                                string sqlAction = @"DELETE MenuAction WHERE CategoryID = @CategoryID";
                                _connection.Execute(sqlAction, new { CategoryID = controllerId }, transaction: _transaction);
                                continue;
                            }
                            //
                            foreach (var action in actionList)
                            {
                                var actKeyId = action.KeyID;
                                var actTitle = action.Title;
                                string actionId = Helper.Security.Library.FakeGuidID(controllerId + actKeyId);
                                // Because controller new should be not need check action
                                var menuAction = menuActionService.GetAlls(m => m.CategoryID == controllerId && m.KeyID == actKeyId, transaction: _transaction).FirstOrDefault();
                                if (menuAction == null)
                                {
                                    menuActionService.Create<string>(new MenuAction()
                                    {
                                        ID = actionId,
                                        RouteArea = routeArea,
                                        CategoryID = controllerId,
                                        KeyID = actKeyId,
                                        Method = action.Method,
                                        Title = actTitle,
                                        APIRouter = action.APIRouter,
                                        OrderID = action.OrderID,
                                        Enabled = (int)ModelEnum.Enabled.ENABLED,
                                    }, transaction: _transaction);
                                }
                                else
                                {
                                    menuAction.Method = action.Method;
                                    menuAction.Title = actTitle;
                                    menuAction.APIRouter = action.APIRouter;
                                    menuAction.OrderID = action.OrderID;
                                    menuActionService.Update(menuAction, transaction: _transaction);
                                }
                            }
                        }
                        else // update action 
                        {
                            // update controller for: area and prefix
                            menuController.RouteArea = controller.RouteArea;
                            menuController.RoutePrefix = controller.RoutePrefix;
                            menuController.Title = ctrlTitle;
                            menuController.OrderID = controller.OrderID;
                            menuControllerService.Update(menuController, transaction: _transaction);
                            // update action 
                            if (actionList.Count == 0)
                            {
                                // delete all action in database
                                string sqlAction = @"DELETE MenuAction WHERE CategoryID = @CategoryID";
                                _connection.Execute(sqlAction, new { CategoryID = menuController.ID }, transaction: _transaction);
                                continue;
                            }
                            //
                            controllerId = menuController.ID;
                            foreach (var action in actionList)
                            {
                                var actKeyId = action.KeyID;
                                var actTitle = action.Title;
                                string actionId = Helper.Security.Library.FakeGuidID(controllerId + actKeyId);
                                var menuAction = menuActionService.GetAlls(m => m.CategoryID == controllerId && m.KeyID == actKeyId, transaction: _transaction).FirstOrDefault();
                                if (menuAction == null)
                                {
                                    menuActionService.Create<string>(new MenuAction()
                                    {
                                        ID = actionId,
                                        RouteArea = routeArea,
                                        CategoryID = controllerId,
                                        KeyID = actKeyId,
                                        Method = action.Method,
                                        Title = actTitle,
                                        APIRouter = action.APIRouter,
                                        OrderID = action.OrderID,
                                        Enabled = (int)ModelEnum.Enabled.ENABLED,
                                    }, transaction: _transaction);
                                }
                                else
                                {
                                    menuAction.Method = action.Method;
                                    menuAction.Title = actTitle;
                                    menuAction.APIRouter = action.APIRouter;
                                    menuAction.OrderID = action.OrderID;
                                    menuActionService.Update(menuAction, transaction: _transaction);
                                }
                            }
                        }
                    }
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST(">>:" + ex);
                }
            }
        }

        public string GetRouteAreaTemplate(Type t)
        {
            RouteAreaAttribute routeAreaAttribute = (RouteAreaAttribute)Attribute.GetCustomAttribute(t, typeof(RouteAreaAttribute));
            if (routeAreaAttribute == null)
                return string.Empty;
            //
            return routeAreaAttribute.AreaName.Replace("Controller", "");
        }
        public string GetRoutePrefixTemplate(Type t)
        {
            RoutePrefixAttribute routeAreaAttribute = (RoutePrefixAttribute)Attribute.GetCustomAttribute(t, typeof(RoutePrefixAttribute));
            if (routeAreaAttribute == null)
                return string.Empty;
            //
            return routeAreaAttribute.Prefix.Replace("Controller", "");
        }
        //
        public string GetRouteTemplate(MethodBase method)
        {
            var attr = (RouteAttribute[])method.GetCustomAttributes(typeof(RouteAttribute), true);
            List<string> lstAttr = new List<string>();
            if (attr.Count() > 0)
            {
                foreach (var item in attr)
                {
                    lstAttr.Add(item.Template);
                }
            }
            string value = String.Join(",", lstAttr);
            return value;
        }
        //
        //#######################################################################################################################################################################################
        public static string DropdownList(string id, string cateId)
        {
            try
            {
                string result = string.Empty;
                using (var service = new MenuControllerService())
                {
                    var dtList = service.DataOption(cateId);
                    if (dtList.Count > 0)
                    {
                        int cnt = 0;
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && item.ID == id)
                                select = "selected";
                            //
                            result += "<option value='" + item.ID + "'" + select + ">" + item.Title + "</option>";
                            cnt++;
                        }
                    }
                    return result;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public List<MvcControllerOption> DataOption(string cateId)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM MenuController WHERE RouteArea = @RouteArea ORDER BY Title ASC";
                return _connection.Query<MvcControllerOption>(sqlQuery, new { RouteArea = cateId }).ToList();
            }
            catch
            {
                return new List<MvcControllerOption>();
            }
        }
    }
}