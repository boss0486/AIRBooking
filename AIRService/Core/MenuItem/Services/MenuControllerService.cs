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

namespace WebCore.Services
{
    public interface IMenuControllerService : IEntityService<MenuController> { }
    public class MenuControllerService : EntityService<MenuController>, IMenuControllerService
    {
        public MenuControllerService() : base() { }
        public MenuControllerService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(SearchModel model)
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
            string sqlQuery = @"SELECT * FROM View_MenuController WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE('" + query + "') +'%' ORDER BY [CreatedDate]";
            var dtList = _connection.Query<MenuControllerResult>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
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
        public ActionResult Update(MenuControllerUpdateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var service = new MenuControllerService(_connection);
                    string Id = model.ID.ToLower();
                    var menuController = service.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                    if (menuController == null)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    string title = model.Title;
                    var menuControllers = service.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !menuController.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (menuControllers.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    menuController.Title = title;
                    menuController.Summary = model.Summary;
                    menuController.Enabled = model.Enabled;
                    service.Update(menuController, transaction: transaction);
                    transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM View_MenuController WHERE ID = @ID";
                var item = _connection.Query<AreaResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                return Notifization.Data(MessageText.Success, data: item, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        //public List<MvcControllerOption> MenuControllerList(MvcControllerSetting model)
        //{
        //    // 
        //    string langID = Helper.Current.UserLogin.LanguageID;
        //    string sqlQuery = @"SELECT * FROM View_MenuController WHERE AreaID = @AreaID ORDER BY Title";
        //    List<MvcControllerOption> dtList = _connection.Query<MvcControllerOption>(sqlQuery, new { AreaID = model.RouteArea }).ToList();
        //    if (dtList.Count == 0)
        //        return new List<MvcControllerOption>();
        //    // return
        //    return dtList;
        //}
        public List<MvcControllerOption> MenuControllerOnActionList(MvcControllerSetting model)
        {
            // 
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM View_MenuController WHERE RouteArea = @RouteArea AND Enabled = 1 ORDER BY Title";
            List<MvcControllerOption> dtList = _connection.Query<MvcControllerOption>(sqlQuery, new { RouteArea = model.RouteArea }).ToList();
            if (dtList.Count == 0)
                return new List<MvcControllerOption>();
            // 
            foreach (var item in dtList)
            {
                sqlQuery = @" SELECT * FROM View_MenuAction WHERE CategoryID = @CategoryID AND Enabled = 1 ORDER BY ApiAction, Title";
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
            string sqlQuery = @" SELECT c.ID,c.KeyID,c.Title,(select count(s.ID) from RoleControllerSetting as s where s.RouteArea = c.RouteArea AND s.RoleID = @RoleID AND s.ControllerID = c.ID ) as Total 
                                 FROM MenuController as c WHERE c.RouteArea = @RouteArea AND Enabled = 1 ORDER BY Title ";
            List<MvcControllerForPermision> dtList = _connection.Query<MvcControllerForPermision>(sqlQuery, new { RouteArea = model.RouteArea , RoleID = roleId }).ToList();
            if (dtList.Count == 0)
                return new List<MvcControllerForPermision>();
            // 
            foreach (var item in dtList)
            {
                sqlQuery = @" SELECT a.ID,a.KeyID,a.Title,(select count(s.ID) from RoleActionSetting as s where s.RouteArea = a.RouteArea AND s.RoleID = @RoleID AND s.ControllerID =  a.CategoryID AND s.ActionID = a.ID) as Total
                              FROM View_MenuAction as a WHERE a.RouteArea = @RouteArea AND a.CategoryID = @CategoryID AND a.Enabled = 1 AND a.APIAction = 1 ORDER BY a.ApiAction, a.Title";
                List<MvcActionForPermision> actionList = _connection.Query<MvcActionForPermision>(sqlQuery, new { RouteArea = model.RouteArea, CategoryID = item.ID, RoleID = roleId }).ToList();
                item.Actions = actionList;
            }
            // return
            return dtList;
        }
        //

        //#######################################################################################################################################################################################
        //        public ActionResult MenuControlSync(Type[] model)
        //        {

        //            var controllerList = model.Where(type => typeof(Controller).IsAssignableFrom(type) && type.IsDefined(typeof(IsManage), true)).ToList();
        //            List<MvcControllerModel> listController = new List<MvcControllerModel>();
        //            foreach (var item in controllerList)
        //            {
        //                List<MvcActionModel> listAction = new List<MvcActionModel>();
        //                var actions = item.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
        //                    .Where(m => m.IsDefined(typeof(IsManage), true)).ToList();

        //                if (actions.Count > 0)
        //                {
        //                    foreach (var action in actions)
        //                    {

        //                        listAction.Add(new MvcActionModel
        //                        {
        //                            Text = action.Name,
        //                            Router = GetRouteTemplate(action),
        //                            Attributes = String.Join(",", action.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))),
        //                            APIAction = CheckAPIAction(action)
        //                        });
        //                    }
        //                }

        //                listController.Add(new MvcControllerModel
        //                {
        //                    RouteArea = GetRouteAreaTemplate(item),
        //                    RoutePrefix = GetRoutePrefixTemplate(item),
        //                    Text = item.Name.Replace("Controller", ""),
        //                    Actions = listAction
        //                }); ;
        //            }

        //            _connection.Open();
        //            using (var transaction = _connection.BeginTransaction())
        //            {
        //                try
        //                {
        //                    if (listController.Count == 0)
        //                    {
        //                        delete all action and all controller
        //                        _connection.Execute("Truncate Table MenuArea", transaction: transaction);
        //                        _connection.Execute("Truncate Table MenuAction", transaction: transaction);
        //                        _connection.Execute("Truncate Table MenuController", transaction: transaction);
        //                        transaction.Commit();
        //                        return Notifization.Error("Deleted all controller and action");
        //                    };
        //                    area
        //                   AreaApplicationService areaApplicationService = new AreaApplicationService();
        //                    var listArea = areaApplicationService.DataOption();
        //                    MenuControllerService menuControllerService = new MenuControllerService(_connection);
        //                    MenuActionService menuActionService = new MenuActionService(_connection);
        //                    foreach (var area in listArea)
        //                    {
        //                        string routeArea = area.KeyID;
        //                        string areaId = area.ID;

        //                        delete controller and action not in list * ***********************************************************************************************************
        //#1. find all controller need delete
        //                        string sqlController = @"SELECT * FROM MenuController WHERE KeyID NOT IN ('" + String.Join("','", listController.Select(m => m.Text)) + "')";
        //                        List<string> lstController = _connection.Query<MenuController>(sqlController, transaction: transaction).Select(m => m.ID).ToList();
        //#2. delete all action of controller need delete
        //                        string sqlAction = @"DELETE MenuAction WHERE CategoryID IN ('" + String.Join("','", lstController) + "')";
        //                        _connection.Execute(sqlAction, transaction: transaction);
        //#3. delete all controller need delete
        //                        sqlController = @"DELETE MenuController WHERE ID IN ('" + String.Join("','", lstController) + "')";
        //                        _connection.Execute(sqlController, transaction: transaction);
        //                        get list contrller is exsit in db
        //                       sqlController = @"SELECT * FROM MenuController WHERE KeyID IN ('" + String.Join("','", listController.Select(m => m.Text)) + "')";
        //                        lstController = _connection.Query<MenuController>(sqlController, transaction: transaction).Select(m => m.ID).ToList();
        //                        fill controller
        //                        sqlAction = @"DELETE MenuAction WHERE CategoryID NOT IN ('" + String.Join("','", lstController) + "')";
        //                        _connection.Execute(sqlAction, transaction: transaction);

        //                        var listController2 = listController.Where(m => m.RouteArea.ToLower().Equals(routeArea.ToLower())).ToList();
        //                        foreach (var controller in listController2)
        //                        {
        //                            string prefix = controller.RoutePrefix;//GetRoutePrefixTemplate(controller.GetType());
        //                            string ctrlText = controller.Text;
        //                            delete all action of the controller in db but not in action list of model
        //                             #1 get action of controller in db
        //                            var categoryId = menuControllerService.GetAlls(m => m.KeyID.ToLower().Equals(controller.Text.ToLower()), transaction: transaction).Select(m => m.ID).FirstOrDefault();
        //                            sqlAction = @"SELECT * FROM MenuAction WHERE CategoryID = '" + categoryId + "'";
        //                            List<string> actionListDb = _connection.Query<MenuAction>(sqlAction, transaction: transaction).Select(m => m.KeyID).ToList();
        //                            List<string> actionListModel = controller.Actions.Select(m => m.Text).ToList();
        //                            List<string> actionNotInModel = actionListDb.Except(actionListModel).ToList();
        //                            action in db and not in model
        //                             #2 delete action  
        //                            if (actionNotInModel.Count > 0)
        //                            {
        //                                get controller id
        //                               sqlAction = @"DELETE MenuAction WHERE CategoryID ='" + categoryId + "' AND KeyID IN ('" + String.Join("','", actionNotInModel) + "')";
        //                                _connection.Execute(sqlAction, transaction: transaction);
        //                            }

        //                            var actionList = controller.Actions;
        //                            var menuController = menuControllerService.GetAlls(m => m.KeyID.ToLower().Equals(ctrlText.ToLower()), transaction: transaction).FirstOrDefault();
        //                            if (menuController == null)
        //                            {

        //                                check csdl
        //                                string controllerId = routeArea + ctrlText;
        //                                var controllerIdinDb = menuControllerService.Create<string>(new MenuController()
        //                                {
        //                                    ID = controllerId,
        //                                    RouteArea = routeArea,
        //                                    RoutePrefix = prefix,
        //                                    KeyID = ctrlText,
        //                                    Title = ctrlText,
        //                                    Path = null,
        //                                    Summary = null,
        //                                    Enabled = (int)ModelEnum.Enabled.ENABLED,
        //                                }, transaction: transaction);
        //                                create action
        //                                if (actionList.Count == 0)
        //                                    continue;

        //                                foreach (var action in actionList)
        //                                {
        //                                    var actText = action.Text;
        //                                    var actPart = action.Router;
        //                                    int actOrderId = 0;
        //                                    Because controller new should be not need check action
        //                                    string actionId = controllerId + actText;
        //                                    var actionIdInDb = menuActionService.Create<string>(new MenuAction()
        //                                    {
        //                                        ID = actionId,
        //                                        CategoryID = controllerId,
        //                                        KeyID = actText,
        //                                        Title = actText,
        //                                        Alias = null,
        //                                        Path = actPart,
        //                                        Summary = null,
        //                                        IconFont = null,
        //                                        OrderID = actOrderId,
        //                                        APIAction = action.APIAction,
        //                                        Enabled = (int)ModelEnum.Enabled.ENABLED,
        //                                    }, transaction: transaction);
        //                                }
        //                            }
        //                            else // update action 
        //                            {
        //                                string controllerId = menuController.ID;
        //                                update controller for: area and prefix

        //                               menuController.RouteArea = controller.RouteArea;
        //                               menuController.RoutePrefix = controller.RoutePrefix;
        //                               menuControllerService.Update(menuController, transaction: transaction);
        //                                update action
        //                                if (actionList.Count == 0)
        //                                    continue;

        //                                foreach (var action in actionList)
        //                                {
        //                                    var actText = action.Text;
        //                                    var actionPart = action.Router;
        //                                    int actOrderId = 0;
        //                                    var menuAction = menuActionService.GetAlls(m => m.CategoryID.ToLower().Equals(controllerId.ToLower()) && m.KeyID.ToLower().Equals(actText.ToLower()), transaction: transaction).FirstOrDefault();
        //                                    if (menuAction == null)
        //                                    {
        //                                        var actionId = menuActionService.Create<string>(new MenuAction()
        //                                        {
        //                                            CategoryID = controllerId,
        //                                            Title = actText,
        //                                            KeyID = actText,
        //                                            Alias = Helper.Page.Library.FormatToUni2NONE(actText),
        //                                            Path = actionPart,
        //                                            OrderID = actOrderId,
        //                                            Enabled = (int)ModelEnum.Enabled.ENABLED,
        //                                        }, transaction: transaction);
        //                                    }
        //                                    else
        //                                    {
        //                                        menuAction.Path = actionPart;
        //                                        menuActionService.Update(menuAction, transaction: transaction);
        //                                    }
        //                                }
        //                            }
        //                        }



        //                    }// for area

        //                    transaction.Commit();
        //                    return Notifization.Success(MessageText.UpdateSuccess);
        //                }
        //                catch (Exception ex)
        //                {
        //                    transaction.Rollback();
        //                    return Notifization.TEST(">>:" + ex);
        //                }
        //            }
        //        }


        //#######################################################################################################################################################################################
        //#######################################################################################################################################################################################
        //#######################################################################################################################################################################################
        //#######################################################################################################################################################################################
        //#######################################################################################################################################################################################
        //#######################################################################################################################################################################################
        public ActionResult MenuControlSync(Type[] model)
        {
            var controllerList = model.Where(type => typeof(Controller).IsAssignableFrom(type) && type.IsDefined(typeof(IsManage), true)).ToList();
            List<MvcControllerModel> listController = new List<MvcControllerModel>();
            foreach (var item in controllerList)
            {
                List<MvcActionModel> listAction = new List<MvcActionModel>();
                var actions = item.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                    .Where(m => m.IsDefined(typeof(IsManage), true)).ToList();
                //
                string routeArea = GetRouteAreaTemplate(item);
                if (actions.Count > 0)
                {
                    foreach (var action in actions)
                    {
                        //  
                        listAction.Add(new MvcActionModel
                        {
                            RouteArea = routeArea,
                            ControllerID = item.Name.Replace("Controller", ""),
                            Text = action.Name,
                            Router = GetRouteTemplate(action),
                            Attributes = String.Join(",", action.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))),
                            APIAction = CheckAPIAction(action)
                        });
                    }
                }
                // 
                listController.Add(new MvcControllerModel
                {
                    RouteArea = routeArea,
                    RoutePrefix = GetRoutePrefixTemplate(item),
                    Text = item.Name.Replace("Controller", ""),
                    Actions = listAction
                }); ;
            }
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (listController.Count == 0)
                    {
                        // delete all action and all controller
                        _connection.Execute("Truncate Table MenuArea", transaction: _transaction);
                        _connection.Execute("Truncate Table MenuAction", transaction: _transaction);
                        _connection.Execute("Truncate Table MenuController", transaction: _transaction);
                        _transaction.Commit();
                        return Notifization.Error("Deleted all controller and action");
                    };
                    // area
                    AreaApplicationService areaApplicationService = new AreaApplicationService();
                    var listArea = areaApplicationService.DataOption();
                    MenuControllerService menuControllerService = new MenuControllerService(_connection);
                    MenuActionService menuActionService = new MenuActionService(_connection);
                    foreach (var controller in listController)
                    {
                        string routeArea = controller.RouteArea;
                        string prefix = controller.RoutePrefix;//GetRoutePrefixTemplate(controller.GetType());
                        string ctrlText = controller.Text;

                        //////// delete controller and action not in list ************************************************************************************************************
                        //////// #1. find all controller need delete
                        string sqlController = @"SELECT * FROM MenuController WHERE ID NOT IN ('" + String.Join("','", listController.Select(m => Helper.Security.Library.FakeGuidID(m.RouteArea + m.Text))) + "')";
                        List<string> lstController = _connection.Query<MenuController>(sqlController, transaction: _transaction).Select(m => m.ID).ToList();
                        // #2. delete all action of controller need delete
                        string sqlAction = @"DELETE MenuAction WHERE CategoryID IN ('" + String.Join("','", lstController) + "')";
                        _connection.Execute(sqlAction, transaction: _transaction);
                        // #3. delete all controller need delete
                        sqlController = @"DELETE MenuController WHERE ID IN ('" + String.Join("','", lstController) + "')";
                        _connection.Execute(sqlController, new { RouteArea = routeArea }, transaction: _transaction);
                        // get list contrller is exsit in db
                        sqlController = @"SELECT * FROM MenuController WHERE ID IN ('" + String.Join("','", listController.Select(m => Helper.Security.Library.FakeGuidID(m.RouteArea + m.Text))) + "')";
                        lstController = _connection.Query<MenuController>(sqlController, new { RouteArea = routeArea }, transaction: _transaction).Select(m => m.ID).ToList();
                        // fill controller
                        sqlAction = @"DELETE MenuAction WHERE CategoryID NOT IN ('" + String.Join("','", lstController) + "')";
                        _connection.Execute(sqlAction, transaction: _transaction);

                        // delete all action of the controller in db but not in action list of model
                        // #1 get action of controller in db
                        var categoryId = menuControllerService.GetAlls(m => m.ID.ToLower().Equals(Helper.Security.Library.FakeGuidID(m.RouteArea + controller.Text).ToLower()), transaction: _transaction).Select(m => m.ID).FirstOrDefault();
                        sqlAction = @"SELECT * FROM MenuAction WHERE CategoryID = @CategoryID";
                        List<string> actionListDb = _connection.Query<MenuAction>(sqlAction, new { CategoryID = categoryId }, transaction: _transaction).Select(m => m.KeyID).ToList();
                        List<string> actionListModel = controller.Actions.Select(m => m.Text).ToList();
                        List<string> actionNotInModel = actionListDb.Except(actionListModel).ToList();
                        // action in db and not in model
                        // #2 delete action  
                        if (actionNotInModel.Count > 0)
                        {
                            // get controller id
                            sqlAction = @"DELETE MenuAction WHERE CategoryID = @CategoryID AND KeyID IN ('" + String.Join("','", actionNotInModel) + "')";
                            _connection.Execute(sqlAction, new { CategoryID = categoryId }, transaction: _transaction);
                        }
                        //
                        string controllerId = Helper.Security.Library.FakeGuidID(routeArea + ctrlText);
                        //
                        var actionList = controller.Actions;
                        var menuController = menuControllerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(controllerId.ToLower()), transaction: _transaction).FirstOrDefault();
                        if (menuController == null)
                        {
                            // check csdl
                            categoryId = controllerId;
                            menuControllerService.Create<string>(new MenuController()
                            {
                                ID = controllerId,
                                RouteArea = routeArea,
                                RoutePrefix = prefix,
                                KeyID = ctrlText,
                                Title = ctrlText,
                                Path = null,
                                Summary = null,
                                Enabled = (int)ModelEnum.Enabled.ENABLED,
                            }, transaction: _transaction);
                            // create action
                            if (actionList.Count == 0)
                                continue;
                            //
                            foreach (var action in actionList)
                            {
                                var actText = action.Text;
                                var actPart = action.Router;
                                // Because controller new should be not need check action
                                var menuAction = menuActionService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CategoryID) &&
                                m.CategoryID.ToLower().Equals(categoryId.ToLower()) &&
                                m.KeyID.ToLower().Equals(actText.ToLower()), transaction: _transaction).FirstOrDefault();
                                if (menuAction == null)
                                {
                                    string actionId = controllerId + actText;
                                    if (action.APIAction)
                                        actionId = controllerId + "-Api-" + actText;
                                    //
                                    actionId = Helper.Security.Library.FakeGuidID(actionId);
                                    menuActionService.Create<string>(new MenuAction()
                                    {
                                        ID = actionId,
                                        RouteArea = routeArea,
                                        CategoryID = categoryId,
                                        Title = actText,
                                        KeyID = actText,
                                        Alias = Helper.Page.Library.FormatToUni2NONE(actText),
                                        Path = actPart,
                                        APIAction = action.APIAction,
                                        Enabled = (int)ModelEnum.Enabled.ENABLED,
                                    }, transaction: _transaction);
                                }
                                else
                                {
                                    menuAction.APIAction = action.APIAction;
                                    menuAction.Path = actPart;
                                    menuActionService.Update(menuAction, transaction: _transaction);
                                }
                            }
                        }
                        else // update action 
                        {
                            // update controller for: area and prefix
                            menuController.RouteArea = controller.RouteArea;
                            menuController.RoutePrefix = controller.RoutePrefix;
                            menuControllerService.Update(menuController, transaction: _transaction);
                            // update action 
                            if (actionList.Count == 0)
                                continue;
                            //
                            categoryId = menuController.ID;
                            foreach (var action in actionList)
                            {
                                var actText = action.Text;
                                var actPart = action.Router;
                                string actionId = controllerId + actText;
                                if (action.APIAction)
                                    actionId = controllerId + "-Api-" + actText;
                                //
                                actionId = Helper.Security.Library.FakeGuidID(actionId);
                                var menuAction = menuActionService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.CategoryID.ToLower().Equals(categoryId) && m.KeyID.ToLower().Equals(actText.ToLower()), transaction: _transaction).FirstOrDefault();
                                if (menuAction == null)
                                {
                                    menuActionService.Create<string>(new MenuAction()
                                    {
                                        ID = actionId,
                                        RouteArea = routeArea,
                                        CategoryID = categoryId,
                                        Title = actText,
                                        KeyID = actText,
                                        Alias = Helper.Page.Library.FormatToUni2NONE(actText),
                                        Path = actPart,
                                        APIAction = action.APIAction,
                                        Enabled = (int)ModelEnum.Enabled.ENABLED,
                                    }, transaction: _transaction);
                                }
                                else
                                {
                                    menuAction.APIAction = action.APIAction;
                                    menuAction.Path = actPart;
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
        public bool CheckAPIAction(MethodBase method)
        {
            try
            {
                var manage = (IsManage)Attribute.GetCustomAttribute(method, typeof(IsManage));
                // return for api then -> return action result
                if (manage != null && manage.Action)
                    return true;
                //
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
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
                            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(item.ID) && item.ID.ToLower().Equals(id.ToLower()))
                                select = "selected";
                            //
                            result += "<option value='" + item.ID + "'" + select + ">" + item.KeyID + "</option>";
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
                string sqlQuery = @"SELECT * FROM View_MenuController WHERE RouteArea = @RouteArea ORDER BY Title ASC";
                return _connection.Query<MvcControllerOption>(sqlQuery, new { RouteArea = cateId }).ToList();
            }
            catch
            {
                return new List<MvcControllerOption>();
            }
        }
    }
}