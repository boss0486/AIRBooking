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
        public ActionResult DataList(SearchModel model)
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
            string sqlQuery = @"SELECT * FROM MenuItem WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' 
                                    AND ParentID IS NULl " + whereCondition + " ORDER BY RouteArea, OrderID ASC ";

            var dtList = _connection.Query<MenuItemModelResult>(sqlQuery, new { RouteArea = routeArea, Query = query, Enabled = status }).ToList();
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
            //
            return Notifization.Data(MessageText.Success, data: dtList, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        public List<MenuItemModelResult> GetSubMenuByLevel(string routeArea, string parentId, int status, string query)
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
                string sqlQuery = @"SELECT * FROM MenuItem WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' 
                AND ParentID = @ParentID " + whereCondition + " ORDER BY RouteArea, OrderID ASC";
                List<MenuItemModelResult> dtList = _connection.Query<MenuItemModelResult>(sqlQuery, new { RouteArea = routeArea, Query = query, ParentID = parentId }).ToList();
                if (dtList.Count == 0)
                    return new List<MenuItemModelResult>();
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
                return new List<MenuItemModelResult>();
            }
        }

        //##############################################################################################################################################################################################################################################################
        public List<MenuItemLayout> GetMenuItemData()
        {
            List<MenuItemLayout> menuItemModelResults = new List<MenuItemLayout>();
            string routeArea = string.Empty;
            if (Helper.Current.UserLogin.IsCMSUser)
                routeArea = AreaApplicationService.GetRouteAreaID((int)AreaApplicationEnum.AreaType.DEVELOPMENT);
            else
                routeArea = AreaApplicationService.GetRouteAreaID((int)AreaApplicationEnum.AreaType.MANAGEMENT);
            //

            if (string.IsNullOrWhiteSpace(routeArea))
                return menuItemModelResults;
            //
            string sqlQuery = @"SELECT * FROM MenuItem WHERE RouteArea = @RouteArea AND Enabled = 1 ORDER BY OrderID ASC ";
            var dataList = _connection.Query<MenuItemLayout>(sqlQuery, new { RouteArea = routeArea }).ToList();
            // 
            return dataList;
        }


        //##############################################################################################################################################################################################################################################################
        public ActionResult MenuItemManage()
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



            //string wherecondition = string.Empty;
            //if (!Helper.Current.UserLogin.IsCMSUser && !Helper.Current.UserLogin.IsAdminInApplication)
            //{
            //    // select role of user login
            //    RoleActionSettingService.RoleListForUser

            //    wherecondition += " AND ";
            //}



            string sqlQuery = @"SELECT * FROM MenuItem WHERE RouteArea = @RouteArea AND Enabled = 1 ORDER BY OrderID ASC ";
            var allData = _connection.Query<MenuItemModelResult>(sqlQuery, new { RouteArea = routeArea }).ToList();

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
        public List<MenuItemModelResult> SubMenuItemByLevel(string parentId, List<MenuItemModelResult> allData)
        {
            List<MenuItemModelResult> dtList = allData.Where(m => m.ParentID == parentId).ToList();
            if (dtList.Count == 0)
                return new List<MenuItemModelResult>();
            //
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
            string sqlQuery = @"SELECT * FROM MenuItem WHERE RouteArea = @RouteArea AND ParentID IS NULL AND Enabled = 1 ORDER BY OrderID ASC ";
            var dtList = _connection.Query<MenuItemModelResult>(sqlQuery, new { RouteArea = routeArea }).ToList();
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
        public List<MenuItemModelResult> SubMenuItemCategory(string parentId)
        {
            string sqlQuery = @"SELECT * FROM MenuItem WHERE ParentID = @ParentID AND Enabled = 1 ORDER BY OrderID ASC";
            List<MenuItemModelResult> dtList = _connection.Query<MenuItemModelResult>(sqlQuery, new { ParentID = parentId }).ToList();
            if (dtList.Count == 0)
                return new List<MenuItemModelResult>();
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
                    string title = model.Title;
                    string summary = model.Summary;
                    //
                    if (string.IsNullOrWhiteSpace(routeArea))
                        return Notifization.Invalid("Phân vùng không hợp lệ");
                    //
                    routeArea = routeArea.ToLower();
                    AreaApplicationService areaApplicationService = new AreaApplicationService();
                    AreaOption area = areaApplicationService.DataOption().Where(m => m.ID == routeArea).FirstOrDefault();
                    if (area == null)
                        return Notifization.Invalid("Phân vùng không hợp lệ");
                    //
                    string parentId = model.ParentID;
                    if (string.IsNullOrWhiteSpace(parentId) || parentId.Length != 36)
                        parentId = null;
                    else
                        parentId = parentId.ToLower();
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
                    if (string.IsNullOrWhiteSpace(parentId) || parentId.Length != 36)
                        parentId = null;
                    else
                        parentId = parentId.ToLower();
                    //
                    if (string.IsNullOrWhiteSpace(controllerId) || controllerId.Length != 36)
                        controllerId = null;
                    //
                    if (string.IsNullOrWhiteSpace(actionId) || actionId.Length != 36)
                        actionId = null;
                    // get path
                    if (!string.IsNullOrWhiteSpace(controllerId) && !string.IsNullOrWhiteSpace(actionId))
                    {
                        var sqlPath = @"SELECT Top (1) Concat('/',c.RouteArea,'/', c.KeyID,'/',a.KeyID) as ActionPath FROM MenuController as c
                                     INNER JOIN MenuAction as a ON a.CategoryID = c.ID AND c.RouteArea = a.RouteArea
                                     WHERE  c.RouteArea = @RouteArea AND c.ID = @ControllerID AND a.ID = @ActionID";
                        pathAction = _connection.Query<string>(sqlPath, new { RouteArea = routeArea, ControllerID = controllerId, ActionID = actionId }, transaction: _transaction).FirstOrDefault();
                    }
                    //
                    string _sqlTitle = @"SELECT ID FROM MenuItem WHERE RouteArea = @RouteArea AND ParentID = @ParentID AND  Title = @Title";
                    var modelTitle = _connection.Query<MenuItemIDModel>(_sqlTitle, new { RouteArea = routeArea, ParentID = parentId, Title = title }, transaction: _transaction).FirstOrDefault();
                    if (modelTitle != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng" + _sqlTitle);
                    //
                    int sortType = model.OrderID;
                    int orderId = 1;
                    if (sortType != (int)MenuItemEnum.Sort.FIRST)
                    {
                        MenuItem menuItem = menuItemService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ParentID) && m.ParentID == parentId && m.RouteArea.ToLower() == routeArea, transaction: _transaction).LastOrDefault();
                        if (menuItem != null)
                        {
                            orderId = menuItem.OrderID + 1;
                        }
                    }
                    //
                    var meta = new MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    // get last order
                    // page manager, navigate to page manager
                    var id = menuItemService.Create<string>(new MenuItem()
                    {
                        RouteArea = routeArea,
                        ParentID = parentId,
                        Title = title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(title),
                        Summary = model.Summary,
                        IconFont = model.IconFont,
                        OrderID = sortType,
                        LocationID = 0,
                        IsPermission = model.IsPermission,
                        MvcController = controllerId,
                        MvcAction = actionId,
                        PathAction = pathAction,
                        Enabled = model.Enabled,
                    }, transaction: _transaction);
                    // update part
                    //string strPath = string.Empty;
                    //var menuParent = menuItemService.GetAlls(m => m.ID = parentId , transaction: _transaction).FirstOrDefault();
                    //if (menuParent != null)
                    //    strPath = menuParent.Path + "/" + id;
                    //else
                    //    strPath = "/" + id;

                    //var menuPath = menuItemService.GetAlls(m => m.ID ==id, transaction: _transaction).FirstOrDefault();
                    //menuPath.Path = strPath;
                    //menuItemService.Update(menuPath, transaction: _transaction);
                    ////sort
                    //var menus = menuItemService.GetAlls(m => m.ParentID == parentId && (m.ID != (id)), transaction: _transaction).OrderBy(m => m.OrderID).ToList();

                    //if (model.OrderID == (int)MenuItemEnum.Sort.FIRST)
                    //{
                    //    if (menus.Count > 0)
                    //    {
                    //        int count = 2;
                    //        foreach (var item in menus)
                    //        {
                    //            if (item.ID != id)
                    //            {
                    //                var menuOther = menuItemService.GetAlls(m => m.ID == item.ID, transaction: _transaction).FirstOrDefault();
                    //                if (menuOther != null)
                    //                {
                    //                    menuOther.OrderID = item.OrderID + 1;
                    //                    menuItemService.Update(menuOther, transaction: _transaction);
                    //                }
                    //                count++;
                    //            }
                    //        }
                    //    }
                    //}
                    //else
                    //{

                    //    if (menus.Count > 0)
                    //    {
                    //        int max = menus.Max(m => m.OrderID);
                    //        var menuOther = menuItemService.GetAlls(m => m.ID =id, transaction: _transaction).FirstOrDefault();
                    //        if (menuOther != null)
                    //        {
                    //            menuOther.OrderID = max + 1;
                    //            menuItemService.Update(menuOther, transaction: _transaction);
                    //        }
                    //    }
                    //}
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
                    string id = model.ID.ToLower();
                    string title = model.Title;
                    string controllerId = model.MvcController;
                    string actionId = model.MvcAction;
                    string parentId = model.ParentID;
                    if (string.IsNullOrWhiteSpace(parentId) || parentId.Length != 36)
                        parentId = null;
                    else
                        parentId = parentId.ToLower();
                    //
                    if (string.IsNullOrWhiteSpace(controllerId) || controllerId.Length != 36)
                        controllerId = null;
                    //
                    if (string.IsNullOrWhiteSpace(actionId) || actionId.Length != 36)
                        actionId = null;
                    //
                    MenuItem menuItem = menuItemService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (menuItem == null)
                        return Notifization.NotFound();
                    string menuId = menuItem.ID;
                    string routeArea = menuItem.RouteArea;
                    string pathAction = string.Empty;
                    if (!string.IsNullOrWhiteSpace(controllerId) && !string.IsNullOrWhiteSpace(actionId))
                    {
                        string sqlPath = @"SELECT Top (1) Concat('/',c.RouteArea,'/', c.KeyID,'/',a.KeyID) as ActionPath FROM MenuController as c
                                     INNER JOIN MenuAction as a ON a.CategoryID = c.ID AND c.RouteArea = a.RouteArea
                                     WHERE  c.RouteArea = @RouteArea AND c.ID = @ControllerID AND a.ID = @ActionID";
                        pathAction = _connection.Query<string>(sqlPath, new { RouteArea = routeArea, ControllerID = controllerId, ActionID = actionId }, transaction: _transaction).FirstOrDefault();
                    }
                    //
                    string _sqlTitle = @"SELECT ID FROM MenuItem WHERE RouteArea = @RouteArea AND ParentID = @ParentID AND  Title = @Title AND ID != @ID ";
                    MenuItemIDModel modelTitle = menuItemService.Query<MenuItemIDModel>(_sqlTitle, new { RouteArea = routeArea, ParentID = parentId, Title = title, ID = id }, transaction: _transaction).FirstOrDefault();
                    if (modelTitle != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    string alias = Helper.Page.Library.FormatToUni2NONE(title).ToLower();
                    string menuPath = menuItem.Path;
                    // check parentId
                    if (string.IsNullOrWhiteSpace(model.ParentID) || model.ParentID == "0")
                    {
                        parentId = "";
                        menuPath = "/" + id;
                    }
                    else
                    {
                        if (menuItem.ID != model.ParentID)
                            parentId = model.ParentID;

                        MenuItem mPath = menuItemService.GetAlls(m => m.ID == model.ParentID.ToLower(), transaction: _transaction).FirstOrDefault();
                        if (mPath != null)
                            menuPath = mPath.Path + "/" + id;

                    }
                    MetaSEO meta = new MetaSEO();
                    AttachmentIngredientService attachmentIngredientService = new AttachmentIngredientService(_connection);
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
                    List<MenuItem> mnChildren = menuItemService.GetAlls(m => m.ParentID == id, transaction: _transaction).ToList();
                    if (mnChildren.Count > 0)
                    {
                        _sortdf = true;
                        foreach (var item in mnChildren)
                        {
                            var mn = menuItemService.GetAlls(m => m.ID == item.ID, transaction: _transaction).FirstOrDefault();
                            if (mn != null)
                            {
                                mn.Path = menuPath + "/" + item.ID;
                                menuItemService.Update(mn, transaction: _transaction);
                            }
                        }
                    }
                    //sort
                    parentId = parentId.ToLower();
                    List<MenuItem> menus = menuItemService.GetAlls(m => m.ParentID == parentId && m.ID != id, transaction: _transaction).OrderBy(m => m.OrderID).ToList();
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
                                var menuLast = menuItemService.GetAlls(m => m.ID == item.ID, transaction: _transaction).FirstOrDefault();
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
                                var menuLast = menuItemService.GetAlls(m => m.ID == item.ID, transaction: _transaction).FirstOrDefault();
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
        public MenuItemModel GetMenuItemByID(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                //
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM MenuItem WHERE ID = @Query";
                var model = _connection.Query<MenuItemModel>(sqlQuery, new { Query = id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.Invalid(MessageText.Invalid);
            //
            id = id.ToLower();

            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    MenuItemService menuItemService = new MenuItemService(_connection);
                    var menuItem = menuItemService.GetAlls(m => m.ID == id, transaction: transaction).FirstOrDefault();
                    if (menuItem == null)
                        return Notifization.Error("Không tìm thấy dữ liệu");
                    // update OrderID
                    int orderId = menuItem.OrderID;
                    string parentId = menuItem.ParentID;
                    // tru order > hon

                    var menuItems = menuItemService.GetAlls(m => m.OrderID > orderId && m.RouteArea == menuItem.RouteArea && m.ParentID == parentId, transaction: transaction).ToList();
                    if (menuItems.Count > 0)
                    {
                        string sqlQuery = @"UPDATE MenuItem SET OrderID = (OrderID -1) WHERE ID IN ('" + String.Join("','", menuItems.Select(m => m.ID)) + "')";
                        _connection.Execute(sqlQuery, new { OrderID = orderId, ParentID = parentId, RouteArea = menuItem.RouteArea }, transaction: transaction);
                    }
                    // delete menu
                    _connection.Query("sp_menuitem_delete", new { ID = id }, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);
                    transaction.Commit();
                    return Notifization.Success(MessageText.DeleteSuccess);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Notifization.TEST(">>" + ex);
                }
            }
        }
        //##############################################################################################################################################################################################################################################################       
        // show category
        public ActionResult LeftMenuItemOption(MenuItemIDModel model)
        {
            string sqlQuery = @"SELECT * FROM MenuItem WHERE ParentID ='' AND Enabled = 1 ORDER BY OrderID ASC ";
            var dtList = _connection.Query<MenuItemModelResult>(sqlQuery, new { }).ToList();
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
        public List<MenuItemModelResult> LeftSubMenuItemOption(string parentId)
        {
            string sqlQuery = @"SELECT * FROM MenuItem WHERE ParentID = @ParentID AND Enabled = 1 ORDER BY OrderID ASC";
            List<MenuItemModelResult> dtList = _connection.Query<MenuItemModelResult>(sqlQuery, new { ParentID = parentId }).ToList();
            if (dtList.Count == 0)
                return new List<MenuItemModelResult>();
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
        //        string sqlQuery = "SELECT * FROM MenuItem WHERE ParentID ='' AND [Enabled] = 1 ORDER BY OrderID ASC ";

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
        //    string sqlQuery = "SELECT * FROM MenuItem WHERE ParentID = @ParentID AND [Enabled] = 1  ORDER BY OrderID ASC";
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
                    string id = model.ID.ToLower();
                    MenuItemService menuItemService = new MenuItemService(_connection);
                    var menu = menuItemService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (menu == null)
                        return Notifization.Invalid(MessageText.Invalid);
                    int _orderId = menu.OrderID;
                    string _parentId = menu.ParentID;
                    string roundArea = menu.RouteArea.ToLower();
                    // list first
                    MenuItem lastMenu = menuItemService.GetAlls(m => !string.IsNullOrWhiteSpace(m.RouteArea) && m.RouteArea == roundArea && m.ParentID == _parentId && m.OrderID < _orderId, transaction: _transaction).OrderBy(m => m.OrderID).LastOrDefault();
                    if (lastMenu != null)
                    {
                        int lastOrderID = lastMenu.OrderID;
                        lastMenu.OrderID = _orderId;
                        // update last menu
                        menuItemService.Update(lastMenu, transaction: _transaction);
                        // update current menu
                        menu.OrderID = lastOrderID;
                        menuItemService.Update(menu, transaction: _transaction);
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
        public ActionResult SortDown(MenuItemIDModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    string id = model.ID.ToLower();
                    MenuItemService menuItemService = new MenuItemService(_connection);
                    var menu = menuItemService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (menu == null)
                        return Notifization.Invalid(MessageText.Invalid);
                    int _orderId = menu.OrderID;
                    string _parentId = menu.ParentID;
                    string roundArea = menu.RouteArea.ToLower();
                    // list first
                    MenuItem firstMenu = menuItemService.GetAlls(m => !string.IsNullOrWhiteSpace(m.RouteArea) && m.RouteArea == roundArea && m.ParentID == _parentId && m.OrderID > _orderId, transaction: _transaction).OrderBy(m => m.OrderID).FirstOrDefault();
                    if (firstMenu != null)
                    {
                        int firstOrderID = firstMenu.OrderID;
                        firstMenu.OrderID = _orderId;
                        // update last menu
                        menuItemService.Update(firstMenu, transaction: _transaction);
                        // update current menu
                        menu.OrderID = firstOrderID;
                        menuItemService.Update(menu, transaction: _transaction);
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
        //

        public ActionResult MenuItemManage_Sort()
        {
            AreaApplicationService areaApplicationService = new AreaApplicationService();
            var areaOptions = areaApplicationService.DataOption();
            if (areaOptions.Count > 0)
            {
                foreach (var areaItem in areaOptions)
                {
                    string routeArea = areaItem.ID;
                    string sqlQuery = @"SELECT * FROM MenuItem WHERE RouteArea = @RouteArea ORDER BY OrderID ASC ";
                    var allData = _connection.Query<MenuItemModelResult>(sqlQuery, new { RouteArea = routeArea }).ToList();
                    if (allData.Count == 0)
                        return Notifization.NotFound(MessageText.NotFound);
                    // 
                    var dtList = allData.Where(m => string.IsNullOrWhiteSpace(m.ParentID)).ToList();
                    if (dtList.Count > 0)
                    {
                        int _cnt = 1;
                        foreach (var item in dtList)
                        {
                            var subMenus = SubMenuItemByLevel_Sort(item.ID, allData);
                            //
                            sqlQuery = @"UPDATE MenuItem SET OrderID =" + _cnt + " WHERE RouteArea = @RouteArea AND ID = @ID";
                            _connection.Execute(sqlQuery, new { RouteArea = routeArea, ID = item.ID });
                            _cnt++;
                        }
                    }
                }
            }
            return Notifization.Success(MessageText.Success);
        }
        public List<MenuItemModelResult> SubMenuItemByLevel_Sort(string parentId, List<MenuItemModelResult> allData)
        {
            if (allData.Count == 0)
                return new List<MenuItemModelResult>();
            //
            List<MenuItemModelResult> dtList = allData.Where(m => m.ParentID == parentId).ToList();
            if (dtList.Count == 0)
                return new List<MenuItemModelResult>();
            //
            int _cnt = 1;
            foreach (var item in dtList)
            {
                var menuLists = SubMenuItemByLevel_Sort(item.ID, allData);
                string sqlQuery = @"UPDATE MenuItem SET OrderID =" + _cnt + " WHERE RouteArea = @RouteArea AND ID = @ID";
                _connection.Execute(sqlQuery, new { RouteArea = item.RouteArea, item.ID });
                _cnt++;
            }
            return dtList;
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
                            if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
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
                string sqlQuery = @"SELECT * FROM MenuItem ORDER BY Title ASC";
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
                var menuController = menuControllerService.GetAlls(m => m.ID == _controllerId.ToLower()).FirstOrDefault();
                if (menuController != null)
                    pathAction += "/" + menuController.RouteArea + "/" + menuController.KeyID;
                //
                MenuActionService menuActionService = new MenuActionService();
                var menuAction = menuActionService.GetAlls(m => m.ID != _actionId.ToLower()).FirstOrDefault();
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
        public string GetSiteMap(string _controllerId, string _actionId)
        {
            string result = string.Empty;
            string pathAction = string.Empty;
            if (string.IsNullOrWhiteSpace(_controllerId))
                return string.Empty;
            // check controller
            MenuControllerService menuControllerService = new MenuControllerService();
            MenuController menuController = menuControllerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.KeyID) && m.KeyID.ToLower() == _controllerId.ToLower()).FirstOrDefault();
            if (menuController == null)
                return string.Empty;
            // 
            var menuActionService = new MenuActionService();
            MenuAction menuAction = menuActionService.GetAlls(m => !string.IsNullOrWhiteSpace(m.KeyID) && m.CategoryID == menuController.ID && m.KeyID.ToLower() == _actionId.ToLower()).FirstOrDefault();
            if (menuAction == null)
                return result;
            // 
            if (string.IsNullOrWhiteSpace(menuController.ID) || string.IsNullOrWhiteSpace(menuAction.ID))
                return result;
            //
            MenuItemService menuItemService = new MenuItemService();
            var menuItem = menuItemService.GetAlls(m => m.MvcController == menuController.ID && m.MvcAction == menuAction.ID).FirstOrDefault();
            if (menuItem == null)
                return result;
            //
            result = menuItem.Title;
            string parentId = menuItem.ParentID;
            if (!string.IsNullOrWhiteSpace(parentId))
            {
                menuItem = menuItemService.GetAlls(m => m.ID == parentId).FirstOrDefault();
                if (menuItem != null)
                    return menuItem.Title + " / " + result;
            }
            return result;
        }
        //##############################################################################################################################################################################################################################################################
    }
}
