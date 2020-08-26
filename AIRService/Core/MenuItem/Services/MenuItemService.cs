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
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.ENM;
using Helper.Page;
using WebCore.Model.Enum;
using System.IO;

namespace WebCore.Services
{
    public interface IMenuItemService : IEntityService<MenuItem> { }
    public class MenuItemService : EntityService<MenuItem>, IMenuItemService
    {
        public MenuItemService() : base() { }
        public MenuItemService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        //public ActionResult UpdateInline(MenuItemEditModel model)
        //{
        //    MenuItemService menuItemService = new MenuItemService(_connection);
        //    var menuItem = menuItemService.GetAlls(m => m.ID.Equals(model.ID.ToLower())).FirstOrDefault();
        //    if (menuItem == null)
        //        return Notifization.NotFound();
        //    if (model.Field == "Title")
        //    {
        //        menuItem.Title = model.Val;
        //        menuItem.Summary = model.Val;
        //        menuItem.Alias = Helper.Page.Library.FormatToUni2NONE(menuItem.Title);
        //        menuItemService.Update(menuItem);
        //        return Notifization.Success(MessageText.UpdateSuccess);
        //    }

        //    if (model.Field == "PathAction")
        //    {
        //        menuItem.PathAction = model.Val;
        //        menuItemService.Update(menuItem);
        //        return Notifization.Success(MessageText.UpdateSuccess);
        //    }
        //    if (model.Field == "Controller")
        //    {
        //        menuItem.MvcController = model.Val;
        //        menuItemService.Update(menuItem);
        //        return Notifization.Success(MessageText.UpdateSuccess);
        //    }
        //    if (model.Field == "Action")
        //    {
        //        menuItem.MvcAction = model.Val;
        //        menuItemService.Update(menuItem);
        //        return Notifization.Success(MessageText.UpdateSuccess);
        //    }

        //    if (model.Field == "IsPermission")
        //    {
        //        menuItem.Enabled = Convert.ToInt32(model.Val);
        //        menuItemService.Update(menuItem);
        //        return Notifization.Success(MessageText.UpdateSuccess);
        //    }
        //    if (model.Field == "Enabled")
        //    {
        //        menuItem.IsPermission = Convert.ToInt32(model.Val);
        //        menuItemService.Update(menuItem);
        //        return Notifization.Success(MessageText.UpdateSuccess);
        //    }
        //    if (model.Field == "OrderID")
        //    {
        //        menuItem.OrderID = Convert.ToInt32(model.Val);
        //        menuItemService.Update(menuItem);
        //        return Notifization.Success(MessageText.UpdateSuccess);
        //    }
        //    return Notifization.TEST("::-");
        //}
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(SearchModel model)
        {
            //
            int page = model.Page;
            string query = string.Empty;
            if (string.IsNullOrWhiteSpace(model.Query))
                query = "";
            else
                query = model.Query;
            //
            int status = model.Status;
            string whereCondition = string.Empty;
            if (status != (int)ModelEnum.Enabled.NONE)
                whereCondition = "AND Enabled = @Enabled";
            //
            string routeArea = model.AreaID;
            if (!string.IsNullOrWhiteSpace(routeArea))
                whereCondition = "AND RouteArea = @RouteArea";
            //
            string sqlQuery = @"SELECT * FROM View_MenuItem WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' 
                                    AND ParentID ='' " + whereCondition + " ORDER BY RouteArea, OrderID ASC ";
            var menuService = new MenuService(_connection);
            var dtList = menuService.Query<ViewMenuItemLevelResult>(sqlQuery, new { RouteArea = routeArea, Query = query, Enabled = status }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound();

            foreach (var item in dtList)
            {
                var subMenus = GetSubMenuByLevel(routeArea, item.ID, status, query);
                if (subMenus.Count > 0)
                    item.SubMenuLevelModel = subMenus;
            }
            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            Helper.Model.RoleAccountModel roleAccountModel = new Helper.Model.RoleAccountModel
            {
                Create = true,
                Update = true,
                Details = true,
                Delete = true,
                Block = true,
                Active = true,
            };
            return Notifization.Data(MessageText.Success, data: dtList, role: roleAccountModel, paging: pagingModel);
        }
        public List<ViewMenuItemLevelResult> GetSubMenuByLevel(string routeArea, string parentId, int status, string query)
        {
            try
            {
                string whereCondition = string.Empty;
                if (status != (int)ModelEnum.Enabled.NONE)
                    whereCondition = "AND Enabled = @Enabled";
                //
                if (!string.IsNullOrWhiteSpace(routeArea))
                    whereCondition = "AND RouteArea = @RouteArea";
                //
                string sqlQuery = @"SELECT * FROM View_MenuItem WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' 
                AND ParentID = @ParentID " + whereCondition + " ORDER BY RouteArea, OrderID ASC";
                var menuService = new MenuService(_connection);
                List<ViewMenuItemLevelResult> dtList = menuService.Query<ViewMenuItemLevelResult>(sqlQuery, new { RouteArea = routeArea, Query = query, ParentID = parentId }).ToList();
                if (dtList.Count == 0)
                    return new List<ViewMenuItemLevelResult>();
                foreach (var item in dtList)
                {
                    var menuLists = GetSubMenuByLevel(routeArea, item.ID, status, query);
                    if (menuLists.Count > 0)
                        item.SubMenuLevelModel = menuLists;
                }
                return dtList;
            }
            catch
            {
                return new List<ViewMenuItemLevelResult>();
            }
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult MenuItemManage(AreaIDRequestModel model)
        {
            string routeArea = string.Empty;
            if (Helper.Current.UserLogin.IsCMSUser)
                routeArea = AreaApplicationService.GetRouteAreaID((int)AreaApplicationEnum.AreaType.DEVELOPMENT);
            else
                routeArea = AreaApplicationService.GetRouteAreaID((int)AreaApplicationEnum.AreaType.MANAGEMENT);
            //


            if (string.IsNullOrWhiteSpace(routeArea))
                return Notifization.NotFound();
            //
            string sqlQuery = @"SELECT * FROM View_MenuItem WHERE RouteArea = @RouteArea AND Enabled = 1 ORDER BY OrderID ASC ";
            var menuService = new MenuService(_connection);
            var allData = menuService.Query<ViewMenuItemLevelResult>(sqlQuery, new { RouteArea = routeArea }).ToList();

            var dtList = allData.Where(m => string.IsNullOrWhiteSpace(m.ParentID)).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound();

            foreach (var item in dtList)
            {
                var subMenus = SubMenuItemByLevel(item.ID, allData);
                if (subMenus.Count > 0)
                    item.SubMenuLevelModel = subMenus;
            }
            return Notifization.Option(MessageText.Success + sqlQuery + routeArea, data: dtList);
        }
        public List<ViewMenuItemLevelResult> SubMenuItemByLevel(string parentId, List<ViewMenuItemLevelResult> allData)
        {
            //string sqlQuery = @"SELECT * FROM View_MenuItem WHERE ParentID = @ParentID AND Enabled = 1 ORDER BY OrderID ASC";
            //var menuService = new MenuService(_connection);
            //List<ViewMenuItemLevelResult> dtList = menuService.Query<ViewMenuItemLevelResult>(sqlQuery, new { ParentID = parentId }).ToList();


            List<ViewMenuItemLevelResult> dtList = allData.Where(m => m.ParentID.Equals(parentId)).ToList();
            if (dtList.Count == 0)
                return new List<ViewMenuItemLevelResult>();
            foreach (var item in dtList)
            {
                var menuLists = SubMenuItemByLevel(item.ID, allData);
                if (menuLists.Count > 0)
                    item.SubMenuLevelModel = menuLists;
            }
            return dtList;
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult MenuItemCategory(AreaIDRequestModel model)
        {
            string routeArea = model.RouteArea;
            if (string.IsNullOrWhiteSpace(routeArea))
                return Notifization.NotFound();
            //
            string sqlQuery = @"SELECT * FROM View_MenuItem WHERE RouteArea = @RouteArea AND ParentID ='' AND Enabled = 1 ORDER BY OrderID ASC ";
            var menuService = new MenuService(_connection);
            var dtList = menuService.Query<ViewMenuItemLevelResult>(sqlQuery, new { RouteArea = routeArea }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound();

            foreach (var item in dtList)
            {
                var subMenus = SubMenuItemCategory(item.ID);
                item.PathAction = GetPathActionPage(item.MvcController, item.MvcAction);
                if (subMenus.Count > 0)
                    item.SubMenuLevelModel = subMenus;
            }
            return Notifization.Option(MessageText.Success + sqlQuery + routeArea, data: dtList);
        }
        public List<ViewMenuItemLevelResult> SubMenuItemCategory(string parentId)
        {
            string sqlQuery = @"SELECT * FROM View_MenuItem WHERE ParentID = @ParentID AND Enabled = 1 ORDER BY OrderID ASC";
            var menuService = new MenuService(_connection);
            List<ViewMenuItemLevelResult> dtList = menuService.Query<ViewMenuItemLevelResult>(sqlQuery, new { ParentID = parentId }).ToList();
            if (dtList.Count == 0)
                return new List<ViewMenuItemLevelResult>();
            foreach (var item in dtList)
            {
                var menuLists = SubMenuItemCategory(item.ID);
                item.PathAction = GetPathActionPage(item.MvcController, item.MvcAction);
                if (menuLists.Count > 0)
                    item.SubMenuLevelModel = menuLists;
            }
            return dtList;
        }
        //##############################################################################################################################################################################################################################################################
        //public ActionResult MenuItemPremission(AreaIDRequestModel model)
        //{
        //    string routeArea = model.RouteArea;
        //    if (string.IsNullOrWhiteSpace(routeArea))
        //        return Notifization.NotFound();
        //    //
        //    string sqlQuery = @"SELECT * FROM  WHERE RouteArea = @RouteArea AND Enabled = 1 ORDER BY OrderID ASC ";
        //    var menuService = new MenuService(_connection);
        //    var allData = menuService.Query<ViewMenuItemLevelResult>(sqlQuery, new { RouteArea = routeArea }).ToList();
        //    if (allData.Count == 0)
        //        return Notifization.NotFound();
        //    //

        //    var dtList = allData.Where(m => string.IsNullOrWhiteSpace(m.ParentID)).ToList();

        //    List<ViewMenuItemLevelResult> viewMenuItemLevelResults = new List<ViewMenuItemLevelResult>();
        //    foreach (var item in dtList)
        //    {

        //        var subMenus = SubMenuItemPremission(item.ID, viewMenuItemLevelResults, allData);
        //        //item.PathAction = GetPathActionPage(item.MvcController, item.MvcAction);
        //        if (subMenus.Count == 0)
        //        {
        //            viewMenuItemLevelResults.Add(item);
        //        }
        //    }
        //    //
        //    List<ViewMenuItemPermissionResult> viewMenuItemPermissionResults = new List<ViewMenuItemPermissionResult>();
        //    if (viewMenuItemLevelResults.Count > 0)
        //    {
        //        foreach (var item in viewMenuItemLevelResults)
        //        {
        //            string label = "";
        //            string path = item.Path;
        //            if (path.Contains("/"))
        //            {
        //                string[] arr = path.Split('/');

        //                if (arr.Length > 0)
        //                {

        //                    foreach (var itemPath in arr)
        //                    {
        //                        var menu = allData.Where(m => m.ID.Equals(itemPath)).FirstOrDefault();
        //                        if (menu != null && !string.IsNullOrWhiteSpace(menu.Title))
        //                        {
        //                            if (itemPath == arr[arr.Length - 1])
        //                                label += menu.Title;
        //                            else
        //                                label += menu.Title + " <i class='far fa-arrow-alt-circle-right'></i> ";
        //                        }
        //                    }
        //                }

        //            }
        //            // action
        //            List<ViewMenuItemPermissionActionResult> actionList = new List<ViewMenuItemPermissionActionResult>();
        //            sqlQuery = @"SELECT ID, Title FROM MenuAction WHERE RouteArea = @RouteArea AND CategoryID = @CategoryID ORDER BY Title ASC ";
        //            var dtActions = menuService.Query<ViewMenuItemPermissionActionResult>(sqlQuery, new { RouteArea = routeArea, CategoryID = item.MvcController }).ToList();
        //            // return
        //            //if (!string.IsNullOrWhiteSpace(item.MvcAction))
        //            //{
        //            //    actionList = new List<ViewMenuItemPermissionActionResult>();
        //            //    var actTitle = "-";
        //            //    string actId = item.MvcAction;
        //            //    if (dtActions.Count > 0)
        //            //    {
        //            //        var action = dtActions.Where(m => m.ID.Equals(actId)).FirstOrDefault();
        //            //        if (action != null)
        //            //            actTitle = action.Title;

        //            //    }
        //            //    actionList.Add(new ViewMenuItemPermissionActionResult
        //            //    {
        //            //        ID = actId,
        //            //        Title = actTitle
        //            //    });
        //            //}
        //            //else
        //            //{
        //            //    actionList = dtActions;
        //            //}
        //            viewMenuItemPermissionResults.Add(new ViewMenuItemPermissionResult
        //            {
        //                ID = item.ID,
        //                Title = label,
        //                Path = item.Path,
        //                Actions = dtActions
        //            });
        //        }
        //    }
        //    return Notifization.Option(MessageText.Success + sqlQuery + routeArea, data: viewMenuItemPermissionResults);
        //}
        //public List<ViewMenuItemLevelResult> SubMenuItemPremission(string parentId, List<ViewMenuItemLevelResult> viewMenuItemLevelResults, List<ViewMenuItemLevelResult> allData)
        //{
        //    List<ViewMenuItemLevelResult> dtList = allData.Where(m => m.ParentID.Equals(parentId)).ToList();
        //    if (dtList.Count == 0)
        //        return new List<ViewMenuItemLevelResult>();
        //    foreach (var item in dtList)
        //    {
        //        var menuLists = SubMenuItemPremission(item.ID, viewMenuItemLevelResults, allData);
        //        //item.PathAction = GetPathActionPage(item.MvcController, item.MvcAction);
        //        if (menuLists.Count == 0)
        //        {
        //            viewMenuItemLevelResults.Add(item);
        //        }
        //    }
        //    return dtList;
        //}


        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(MenuItemCreateFormModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid("Dữ liệu không hợp lệ");
                    //
                    string routeArea = model.RouteArea;
                    if (string.IsNullOrWhiteSpace(routeArea))
                        return Notifization.Invalid("Area không hợp lệ");
                    //
                    AreaApplicationService areaApplicationService = new AreaApplicationService();
                    var area = areaApplicationService.DataOption().Where(m => m.ID.ToLower().Equals(routeArea.ToLower())).FirstOrDefault();
                    if (area == null)
                        return Notifization.Invalid("Area không hợp lệ");
                    //
                    string title = model.Title;
                    string summary = model.Summary;
                    //
                    string parentId = model.ParentID;
                    if (parentId.Equals("0") || parentId == "")
                        parentId = "";
                    //
                    if (string.IsNullOrWhiteSpace(title))
                        return Notifization.Invalid("Không được để trống tiêu đề");
                    title = title.Trim();
                    if (!Validate.TestText(title))
                        return Notifization.Invalid("Tiêu đề không hợp lệ");
                    if (title.Length < 2 || title.Length > 80)
                        return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
                    // summary valid               
                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }
                    MenuItemService menuItemService = new MenuItemService(_connection);
                    string controllerId = model.MvcController;
                    string actionId = model.MvcAction;
                    string pathAction = string.Empty;
                    //
                    if (!string.IsNullOrWhiteSpace(controllerId) && !string.IsNullOrWhiteSpace(actionId))
                    {
                        var sqlPath = @"SELECT Top (1) Concat('/',c.RouteArea,'/', c.KeyID,'/',a.KeyID) as ActionPath FROM MenuController as c
                                     INNER JOIN MenuAction as a ON a.CategoryID = c.ID AND c.RouteArea = a.RouteArea
                                     WHERE  c.RouteArea = @RouteArea AND c.ID = @ControllerID AND a.ID = @ActionID";
                        pathAction =  _connection.Query<string>(sqlPath, new { RouteArea = routeArea, ControllerID = controllerId, ActionID = actionId }, transaction: _transaction).FirstOrDefault();
                    }

                    string _sqlTitle = @"SELECT ID FROM View_MenuItem WHERE RouteArea = @RouteArea AND ParentID = @ParentID AND  Title = @Title";
                    var modelTitle = _connection.Query<MenuItemIDModel>(_sqlTitle, new { RouteArea = routeArea, ParentID = parentId, Title = title }, transaction: _transaction).FirstOrDefault();
                    if (modelTitle != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng" + _sqlTitle);
                    //
                    int orderId = model.OrderID;
                    if (orderId != (int)MenuItemEnum.Sort.FIRST && orderId != (int)MenuItemEnum.Sort.LAST)
                        orderId = (int)MenuItemEnum.Sort.LAST;
                    //
                    var meta = new MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);

                    // page manager, navigate to page manager
                    var id = menuItemService.Create<string>(new MenuItem()
                    {
                        RouteArea = routeArea,
                        ParentID = parentId,
                        Title = title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(title),
                        Summary = model.Summary,
                        IconFont = model.IconFont,
                        OrderID = orderId,
                        LocationID = 0,
                        IsPermission = model.IsPermission,
                        MvcController = controllerId,
                        MvcAction = actionId,
                        PathAction = pathAction,
                        Enabled = model.Enabled,
                    }, transaction: _transaction);
                    // update part
                    string strPath = string.Empty;
                    var menuParent = menuItemService.GetAlls(m => m.ID.Equals(parentId), transaction: _transaction).FirstOrDefault();
                    if (menuParent != null)
                        strPath = menuParent.Path + "/" + id;
                    else
                        strPath = "/" + id;

                    var menuPath = menuItemService.GetAlls(m => m.ID.Equals(id), transaction: _transaction).FirstOrDefault();
                    menuPath.Path = strPath;
                    menuItemService.Update(menuPath, transaction: _transaction);
                    //sort
                    var menus = menuItemService.GetAlls(m => m.ParentID.Equals(parentId) && (m.ID != (id)), transaction: _transaction).OrderBy(m => m.OrderID).ToList();

                    if (model.OrderID == (int)MenuItemEnum.Sort.FIRST)
                    {
                        if (menus.Count > 0)
                        {
                            int count = 2;
                            foreach (var item in menus)
                            {
                                if (!item.ID.Equals(id))
                                {
                                    var menuOther = menuItemService.GetAlls(m => m.ID.Equals(item.ID), transaction: _transaction).FirstOrDefault();
                                    if (menuOther != null)
                                    {
                                        menuOther.OrderID = item.OrderID + 1;
                                        menuItemService.Update(menuOther, transaction: _transaction);
                                    }
                                    count++;
                                }
                            }
                        }
                    }
                    else
                    {

                        if (menus.Count > 0)
                        {
                            int max = menus.Max(m => m.OrderID);
                            var menuOther = menuItemService.GetAlls(m => m.ID.Equals(id), transaction: _transaction).FirstOrDefault();
                            if (menuOther != null)
                            {
                                menuOther.OrderID = max + 1;
                                menuItemService.Update(menuOther, transaction: _transaction);
                            }
                        }
                    }
                    _transaction.Commit();
                    return Notifization.Success(MessageText.CreateSuccess);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST("::" + ex);
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(MenuItemUpdateFormModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    MenuItemService menuItemService = new MenuItemService(_connection);
                    string id = model.ID;
                    string title = model.Title;
                    var controllerId = model.MvcController;
                    var actionId = model.MvcAction;
                    string parentId = model.ParentID;

                    if (parentId.Equals("0") || parentId == "")
                        parentId = "";
                    //
                    var menuItem = menuItemService.GetAlls(m => m.ID.Equals(model.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (menuItem == null)
                        return Notifization.NotFound();
                    string menuId = menuItem.ID;
                    string routeArea = menuItem.RouteArea;
                    var pathAction = string.Empty;
                    if (!string.IsNullOrWhiteSpace(controllerId) && !string.IsNullOrWhiteSpace(actionId))
                    {
                        var sqlPath = @"SELECT Top (1) Concat('/',c.RouteArea,'/', c.KeyID,'/',a.KeyID) as ActionPath FROM MenuController as c
                                     INNER JOIN MenuAction as a ON a.CategoryID = c.ID AND c.RouteArea = a.RouteArea
                                     WHERE  c.RouteArea = @RouteArea AND c.ID = @ControllerID AND a.ID = @ActionID";
                        pathAction = _connection.Query<string>(sqlPath, new { RouteArea = routeArea, ControllerID = controllerId, ActionID = actionId }, transaction: _transaction).FirstOrDefault();
                    }
                    //
                    string _sqlTitle = @"SELECT ID FROM View_MenuItem WHERE RouteArea = @RouteArea AND ParentID = @ParentID AND  Title = @Title AND ID != @ID ";
                    var modelTitle = menuItemService.Query<MenuItemIDModel>(_sqlTitle, new { RouteArea = routeArea, ParentID = parentId, Title = title, ID = id }, transaction: _transaction).FirstOrDefault();
                    if (modelTitle != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    string alias = Helper.Page.Library.FormatToUni2NONE(title).ToLower();
                    string menuPath = menuItem.Path;
                    // check parentId
                    if (string.IsNullOrWhiteSpace(model.ParentID) || model.ParentID.Equals("0"))
                    {
                        parentId = "";
                        menuPath = "/" + id;
                    }
                    else
                    {
                        if (!menuItem.ID.Equals(model.ParentID))
                            parentId = model.ParentID;

                        var mPath = menuItemService.GetAlls(m => m.ID.Equals(model.ParentID), transaction: _transaction).FirstOrDefault();
                        if (mPath != null)
                            menuPath = mPath.Path + "/" + id;

                    }
                    var meta = new MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    // update content
                    menuItem.ParentID = parentId;
                    menuItem.IconFont = model.IconFont;
                    menuItem.Title = title;
                    menuItem.Summary = model.Summary;
                    menuItem.IsPermission = model.IsPermission;
                    menuItem.LocationID = (int)MenuItemEnum.Location.LEFT;
                    menuItem.MvcController = controllerId;
                    menuItem.MvcAction = actionId;
                    menuItem.PathAction = pathAction;
                    menuItem.Path = menuPath;

                    menuItem.Enabled = model.Enabled;
                    menuItemService.Update(menuItem, transaction: _transaction);
                    // update part 
                    bool _sortdf = false;
                    // update part
                    string parentPath = string.Empty;
                    var mnChildren = menuItemService.GetAlls(m => m.ParentID.Equals(id), transaction: _transaction).ToList();
                    if (mnChildren.Count > 0)
                    {
                        _sortdf = true;
                        foreach (var item in mnChildren)
                        {
                            var mn = menuItemService.GetAlls(m => m.ID.Equals(item.ID), transaction: _transaction).FirstOrDefault();
                            if (mn != null)
                            {
                                mn.Path = menuPath + "/" + item.ID;
                                menuItemService.Update(mn, transaction: _transaction);
                            }
                        }
                    }
                    //sort
                    parentId = parentId.ToLower();
                    var menus = menuItemService.GetAlls(m => m.ParentID.Equals(parentId) && m.ID != id, transaction: _transaction).OrderBy(m => m.OrderID).ToList();
                    // set sort default if user not select sort
                    int sort = model.OrderID;
                    if (_sortdf)
                        sort = (int)MenuItemEnum.Sort.LAST;
                    if (sort == (int)MenuItemEnum.Sort.FIRST)
                    {
                        if (menus.Count > 0 && !string.IsNullOrWhiteSpace(parentId))
                        {
                            menuItem.OrderID = 1;
                            menuItemService.Update(menuItem, transaction: _transaction);
                            int cnt = 2;
                            foreach (var item in menus)
                            {
                                var menuLast = menuItemService.GetAlls(m => m.ID.Equals(item.ID), transaction: _transaction).FirstOrDefault();
                                if (menuLast != null)
                                {
                                    menuLast.OrderID = cnt;
                                    menuItemService.Update(menuLast, transaction: _transaction);
                                    cnt++;
                                }
                            }
                        }
                    }
                    else if (sort == (int)MenuItemEnum.Sort.LAST)
                    {
                        if (menus.Count > 0)
                        {
                            int cnt = 1;
                            foreach (var item in menus)
                            {
                                var menuLast = menuItemService.GetAlls(m => m.ID.Equals(item.ID), transaction: _transaction).FirstOrDefault();
                                if (menuLast != null)
                                {
                                    menuLast.OrderID = cnt;
                                    menuItemService.Update(menuLast, transaction: _transaction);
                                    cnt++;
                                }
                            }
                            menuItem.OrderID = menus.Count + 1;
                            menuItemService.Update(menuItem, transaction: _transaction);
                        }
                    }
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch
                {
                    _transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        public MenuItemResult UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_MenuItem WHERE ID = @Query";
                var model = _connection.Query<MenuItemResult>(sqlQuery, new { Query = Id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(MenuItemIDModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    MenuItemService menuItemService = new MenuItemService(_connection);
                    var menuItem = menuItemService.GetAlls(m => m.ID.Equals(model.ID.ToLower()), transaction: transaction).FirstOrDefault();
                    if (menuItem == null)
                        return Notifization.Error("Không tìm thấy dữ liệu");
                    // delete menu
                    _connection.Query("sp_menuitem_delete", new { ID = model.ID }, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);
                    transaction.Commit();
                    return Notifization.Success(MessageText.DeleteSuccess);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################       
        // show category
        public ActionResult LeftMenuItemOption(MenuItemIDModel model)
        {
            string sqlQuery = @"SELECT * FROM View_MenuItem WHERE ParentID ='' AND Enabled = 1 ORDER BY OrderID ASC ";
            var menuService = new MenuService(_connection);
            var dtList = menuService.Query<ViewMenuItemLevelResult>(sqlQuery, new { }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound();

            foreach (var item in dtList)
            {
                var subMenus = LeftSubMenuItemOption(item.ID);
                if (subMenus.Count > 0)
                    item.SubMenuLevelModel = subMenus;
            }
            return Notifization.Option(MessageText.Success, data: dtList);
        }
        public List<ViewMenuItemLevelResult> LeftSubMenuItemOption(string parentId)
        {
            string sqlQuery = @"SELECT * FROM View_MenuItem WHERE ParentID = @ParentID AND Enabled = 1 ORDER BY OrderID ASC";
            var menuService = new MenuService(_connection);
            List<ViewMenuItemLevelResult> dtList = menuService.Query<ViewMenuItemLevelResult>(sqlQuery, new { ParentID = parentId }).ToList();
            if (dtList.Count == 0)
                return new List<ViewMenuItemLevelResult>();
            foreach (var item in dtList)
            {
                var menuLists = LeftSubMenuItemOption(item.ID);
                if (menuLists.Count > 0)
                    item.SubMenuLevelModel = menuLists;
            }
            return dtList;
        }
        //##############################################################################################################################################################################################################################################################
        // show menu manager
        //public static string GetCMSMenu(int locationId, string Id = null)
        //{
        //    string result = string.Empty;
        //    // action home
        //    using (var menuItemService = new MenuItemService())
        //    {
        //        string pathAction = menuItemService.GetMenuAction(locationId, null);
        //        result += @"<li></li>";
        //        //
        //        string sqlQuery = "SELECT * FROM View_MenuItem WHERE ParentID ='' AND [Enabled] = 1 ORDER BY OrderID ASC ";

        //        var dtList = menuItemService.Query<MenuItemOptionModel>(sqlQuery, new { LocationID = locationId }).ToList();
        //        if (dtList.Count == 0)
        //            return result;
        //        foreach (var item in dtList)
        //        {
        //            pathAction = item.PathAction;
        //            if (UserRoleService.AuthenzationAccess(Current.LoginUser.ID, item.ID, string.Empty))
        //            {
        //                string cls = string.Empty;
        //                if (item.TotalItem > 0)
        //                {
        //                    cls = "menu-toggle";
        //                    pathAction = "javascript:void(0);";
        //                }
        //                result += "<li><a href='" + pathAction + "' class='" + cls + "'><span><i class='" + item.IconFont + "' aria-hidden='true'></i>&nbsp;" + item.Title + "</span></a>" + GetCMSSubMenu(item.ID, Id) + "</li>";
        //            }
        //        }
        //        return result;
        //    }
        //}
        //public static string GetCMSSubMenu(string parentId, string Id = null)
        //{
        //    string result = string.Empty;
        //    string sqlQuery = "SELECT * FROM View_MenuItem WHERE ParentID = @ParentID AND [Enabled] = 1  ORDER BY OrderID ASC";
        //    MenuItemService menuItemService = new MenuItemService();
        //    var dtList = menuItemService.Query<MenuItemOptionModel>(sqlQuery, new { ParentID = parentId }).ToList();
        //    if (dtList.Count == 0)
        //        return string.Empty;
        //    result += "<ul class='ml-menu'>";
        //    foreach (var item in dtList)
        //    {
        //        if (UserRoleService.AuthenzationAccess(Current.LoginUser.ID, item.ID, string.Empty))
        //        {
        //            string cls = string.Empty;
        //            string pathAction = item.PathAction;
        //            if (item.TotalItem > 0)
        //            {
        //                cls = "menu-toggle";
        //                pathAction = string.Empty;
        //            }
        //            result += "<li><a href='" + pathAction + "' class='" + cls + "'><i class='" + item.IconFont + "'></i><span>" + item.Title + "</span></a>" + GetCMSSubMenu(item.ID) + "</li>";
        //        }
        //    }
        //    result += "</ul>";
        //    return result;
        //}
        //##############################################################################################################################################################################################################################################################
        //public string GetMenuAction(int Id, string pathAction)
        //{
        //    try
        //    {
        //        string result = string.Empty;
        //        var lst = MenuAction().Where(m => m.ID == Id).FirstOrDefault();
        //        return lst.PathAction;
        //    }
        //    catch (Exception)
        //    {
        //        return string.Empty;
        //    }
        //}
        // 
        //public static List<MenuActionModel> MenuAction()
        //{
        //    //  get from controler...
        //    var optionListModels = new List<MenuActionModel>
        //    {
        //        new MenuActionModel(0, "ADMIN", "/cms"),
        //        new MenuActionModel(1, "HOME",""),
        //        new MenuActionModel(2, "OFFICE","/backend")
        //    };
        //    return optionListModels;
        //}

        //##############################################################################################################################################################################################################################################################
        // Phan quyen for menu



        //#######################################################################################################################################################################################
        public ActionResult SortUp(MenuItemIDModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    MenuItemService menuItemService = new MenuItemService(_connection);
                    var menu = menuItemService.GetAlls(m => m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (menu == null)
                        return Notifization.Error(MessageText.NotService);
                    int _orderId = menu.OrderID;
                    string _parentId = menu.ParentID;
                    // list first
                    IList<MenuSortModel> lstFirst = new List<MenuSortModel>();
                    // list last
                    IList<MenuSortModel> lstLast = new List<MenuSortModel>();
                    //
                    if (!string.IsNullOrWhiteSpace(_parentId))
                    {
                        var menuItems = menuItemService.GetAlls(m => m.ParentID.ToLower().Equals(_parentId.ToLower()) && !m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).OrderBy(m => m.OrderID).ToList();
                        if (menuItems.Count > 0)
                        {
                            foreach (var item in menuItems)
                            {
                                // set list first
                                if (item.OrderID < menu.OrderID)
                                {
                                    lstFirst.Add(new MenuSortModel
                                    {
                                        ID = item.ID,
                                        OrderID = item.OrderID
                                    });
                                }
                                // set list last
                                if (item.OrderID > menu.OrderID)
                                {
                                    lstLast.Add(new MenuSortModel
                                    {
                                        ID = item.ID,
                                        OrderID = item.OrderID
                                    });
                                }
                            }
                            //  first
                            int _cntFirst = 1;
                            if (lstFirst.Count > 0)
                            {
                                for (int i = 0; i < lstFirst.Count; i++)
                                {
                                    if (i == lstFirst.Count - 1)
                                    {
                                        menu.OrderID = _cntFirst;
                                        menuItemService.Update(menu, transaction: _transaction);
                                        _cntFirst++;
                                    }
                                    var itemFirst = menuItemService.GetAlls(m => m.ID.ToLower().Equals(lstFirst[i].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemFirst.OrderID = _cntFirst;
                                    menuItemService.Update(itemFirst, transaction: _transaction);
                                    _cntFirst++;
                                }

                            }
                            else
                            {
                                menu.OrderID = 1;
                                menuItemService.Update(menu, transaction: _transaction);
                                _cntFirst++;
                            }
                            //last
                            int _cntLast = _cntFirst;
                            if (lstLast.Count > 0)
                            {
                                foreach (var item in lstLast)
                                {
                                    var itemLast = menuItemService.GetAlls(m => m.ID.ToLower().Equals(item.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemLast.OrderID = _cntLast;
                                    menuItemService.Update(itemLast, transaction: _transaction);
                                    _cntLast++;
                                }
                            }
                        }
                        else
                        {
                            menu.OrderID = 1;
                            menuItemService.Update(menu, transaction: _transaction);
                        }
                    }
                    else
                    {
                        // truong hop cha , _parentId = ""
                        var menuItems = menuItemService.GetAlls(m => m.ParentID.Equals(_parentId) && !m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).OrderBy(m => m.OrderID).ToList();
                        if (menuItems.Count > 0)
                        {
                            foreach (var item in menuItems)
                            {
                                // set list first
                                if (item.OrderID < menu.OrderID)
                                {
                                    lstFirst.Add(new MenuSortModel
                                    {
                                        ID = item.ID,
                                        OrderID = item.OrderID
                                    });
                                }
                                // set list last
                                if (item.OrderID > menu.OrderID)
                                {
                                    lstLast.Add(new MenuSortModel
                                    {
                                        ID = item.ID,
                                        OrderID = item.OrderID
                                    });
                                }
                            }
                            //  first
                            int _cntFirst = 1;
                            if (lstFirst.Count > 0)
                            {
                                for (int i = 0; i < lstFirst.Count; i++)
                                {
                                    if (i == lstFirst.Count - 1)
                                    {
                                        menu.OrderID = _cntFirst;
                                        menuItemService.Update(menu, transaction: _transaction);
                                        _cntFirst++;
                                    }
                                    var itemFirst = menuItemService.GetAlls(m => m.ID.ToLower().Equals(lstFirst[i].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemFirst.OrderID = _cntFirst;
                                    menuItemService.Update(itemFirst, transaction: _transaction);
                                    _cntFirst++;
                                }

                            }
                            else
                            {
                                menu.OrderID = 1;
                                menuItemService.Update(menu, transaction: _transaction);
                                _cntFirst++;
                            }
                            //last
                            int _cntLast = _cntFirst;
                            if (lstLast.Count > 0)
                            {
                                foreach (var item in lstLast)
                                {
                                    var itemLast = menuItemService.GetAlls(m => m.ID.ToLower().Equals(item.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemLast.OrderID = _cntLast;
                                    menuItemService.Update(itemLast, transaction: _transaction);
                                    _cntLast++;
                                }
                            }
                        }
                        else
                        {
                            menu.OrderID = 1;
                            menuItemService.Update(menu, transaction: _transaction);
                        }
                    }
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch (Exception)
                {
                    _transaction.Rollback();
                    return Notifization.Error(MessageText.NotService);
                }
            }// end transaction
        }
        //#######################################################################################################################################################################################
        public ActionResult SortDown(MenuItemIDModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    MenuItemService menuItemService = new MenuItemService(_connection);
                    var menuItem = menuItemService.GetAlls(m => m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (menuItem == null)
                        return Notifization.Error(MessageText.NotService);
                    int _orderId = menuItem.OrderID;
                    string _parentId = menuItem.ParentID;
                    // list first
                    IList<MenuSortModel> lstFirst = new List<MenuSortModel>();
                    // list last
                    IList<MenuSortModel> lstLast = new List<MenuSortModel>();
                    //
                    if (!string.IsNullOrWhiteSpace(_parentId))
                    {
                        var menuItems = menuItemService.GetAlls(m => m.ParentID.ToLower().Equals(_parentId.ToLower()) && !m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).OrderBy(m => m.OrderID).ToList();
                        if (menuItems.Count > 0)
                        {
                            foreach (var item in menuItems)
                            {
                                // set list first
                                if (item.OrderID < menuItem.OrderID)
                                {
                                    lstFirst.Add(new MenuSortModel
                                    {
                                        ID = item.ID,
                                        OrderID = item.OrderID
                                    });
                                }
                                // set list last
                                if (item.OrderID > menuItem.OrderID)
                                {
                                    lstLast.Add(new MenuSortModel
                                    {
                                        ID = item.ID,
                                        OrderID = item.OrderID
                                    });
                                }
                            }
                            // xu ly
                            int _cntFirst = 1;
                            if (lstFirst.Count > 0)
                            {
                                foreach (var item in lstFirst)
                                {
                                    var itemFirst = menuItemService.GetAlls(m => m.ID.ToLower().Equals(item.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemFirst.OrderID = _cntFirst;
                                    menuItemService.Update(itemFirst, transaction: _transaction);
                                    _cntFirst++;
                                }

                            }
                            //  last
                            int _cntLast = _cntFirst;
                            if (lstLast.Count == 1)
                            {
                                var itemLast = menuItemService.GetAlls(m => m.ID.ToLower().Equals(lstLast[0].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                itemLast.OrderID = _cntLast;
                                menuItemService.Update(itemLast, transaction: _transaction);
                                //
                                menuItem.OrderID = _cntLast + 1;
                                menuItemService.Update(menuItem, transaction: _transaction);
                                _cntLast++;
                            }
                            else if (lstLast.Count > 1)
                            {
                                for (int i = 0; i < lstLast.Count; i++)
                                {
                                    if (i == 1)
                                    {
                                        menuItem.OrderID = _cntLast;
                                        menuItemService.Update(menuItem, transaction: _transaction);
                                        _cntLast++;
                                    }
                                    var itemLast = menuItemService.GetAlls(m => m.ID.ToLower().Equals(lstLast[i].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemLast.OrderID = _cntLast;
                                    menuItemService.Update(itemLast, transaction: _transaction);
                                    _cntLast++;
                                }
                            }
                        }
                        else
                        {
                            menuItem.OrderID = 1;
                            menuItemService.Update(menuItem, transaction: _transaction);
                        }
                    }
                    else
                    {
                        // truong hop cha , _parentId = ""
                        var menuItems = menuItemService.GetAlls(m => m.ParentID.Equals(_parentId) && !m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).OrderBy(m => m.OrderID).ToList();
                        if (menuItems.Count > 0)
                        {
                            foreach (var item in menuItems)
                            {
                                // set list first
                                if (item.OrderID < menuItem.OrderID)
                                {
                                    lstFirst.Add(new MenuSortModel
                                    {
                                        ID = item.ID,
                                        OrderID = item.OrderID
                                    });
                                }
                                // set list last
                                if (item.OrderID > menuItem.OrderID)
                                {
                                    lstLast.Add(new MenuSortModel
                                    {
                                        ID = item.ID,
                                        OrderID = item.OrderID
                                    });
                                }
                            }
                            // xu ly
                            int _cntFirst = 1;
                            if (lstFirst.Count > 0)
                            {
                                foreach (var item in lstFirst)
                                {
                                    var itemFirst = menuItemService.GetAlls(m => m.ID.ToLower().Equals(item.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemFirst.OrderID = _cntFirst;
                                    menuItemService.Update(itemFirst, transaction: _transaction);
                                    _cntFirst++;
                                }

                            }
                            //  last
                            int _cntLast = _cntFirst;
                            if (lstLast.Count == 1)
                            {
                                var itemLast = menuItemService.GetAlls(m => m.ID.ToLower().Equals(lstLast[0].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                itemLast.OrderID = _cntLast;
                                menuItemService.Update(itemLast, transaction: _transaction);
                                //
                                menuItem.OrderID = _cntLast + 1;
                                menuItemService.Update(menuItem, transaction: _transaction);
                                _cntLast++;
                            }
                            else if (lstLast.Count > 1)
                            {
                                for (int i = 0; i < lstLast.Count; i++)
                                {
                                    if (i == 1)
                                    {
                                        menuItem.OrderID = _cntLast;
                                        menuItemService.Update(menuItem, transaction: _transaction);
                                        _cntLast++;
                                    }
                                    var itemLast = menuItemService.GetAlls(m => m.ID.ToLower().Equals(lstLast[i].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemLast.OrderID = _cntLast;
                                    menuItemService.Update(itemLast, transaction: _transaction);
                                    _cntLast++;
                                }
                            }
                        }
                        else
                        {
                            menuItem.OrderID = 1;
                            menuItemService.Update(menuItem, transaction: _transaction);
                        }
                    }
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch (Exception)
                {
                    _transaction.Rollback();
                    return Notifization.Error(MessageText.NotService);
                }
            }// end transaction
        }
        //#######################################################################################################################################################################################
        public static string DropdownListMenuItem(string id)
        {
            try
            {
                string result = string.Empty;
                using (var MenuItemService = new MenuItemService())
                {
                    var dtList = MenuItemService.DataOption(id);
                    if (dtList.Count > 0)
                    {
                        int cnt = 0;
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && item.ID.Equals(id.ToLower()))
                                select = "selected";
                            else if (cnt == 0)
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
        public List<MenuItemOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_MenuItem ORDER BY Title ASC";
                return _connection.Query<MenuItemOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<MenuItemOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string GetPathActionPage(string _controllerId, string _actionId)
        {
            try
            {

                string pathAction = string.Empty;
                if (string.IsNullOrWhiteSpace(_controllerId) || string.IsNullOrWhiteSpace(_actionId))
                    return string.Empty;
                MenuControllerService menuControllerService = new MenuControllerService();
                var menuController = menuControllerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(_controllerId.ToLower())).FirstOrDefault();
                if (menuController != null)
                    pathAction += "/" + menuController.RouteArea + "/" + menuController.KeyID;
                //
                MenuActionService menuActionService = new MenuActionService();
                var menuAction = menuActionService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(_actionId.ToLower())).FirstOrDefault();
                if (menuAction != null)
                    pathAction += "/" + menuAction.KeyID;
                //
                return pathAction;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //##############################################################################################################################################################################################################################################################
    }
}
