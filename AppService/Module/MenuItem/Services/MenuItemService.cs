using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using WebCore.Entities;
using Helper;
using System.Web;
using WebCore.Core;
using WebCore.Model.Entities;
using WebCore.Model.Enum;
using WebCore.ENM;

namespace WebCore.Services
{
    public interface IMenuItemService : IEntityService<MenuItem> { }
    public class MenuItemService : EntityService<MenuItem>, IMenuItemService
    {
        public MenuItemService() : base() { }
        public MenuItemService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult UpdateInline(MenuItemEditModel model)
        {
            MenuItemService menuItemService = new MenuItemService(_connection);
            var menuItem = menuItemService.GetAlls(m => m.ID.Equals(model.ID.ToLower())).FirstOrDefault();
            if (menuItem == null)
                return Notifization.NotFound();
            if (model.Field == "Title")
            {
                menuItem.Title = model.Val;
                menuItem.Summary = model.Val;
                menuItem.Alias = Helper.Library.Uni2NONE(menuItem.Title);
                menuItemService.Update(menuItem);
                return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
            }

            if (model.Field == "PathAction")
            {
                menuItem.PathAction = model.Val;
                menuItemService.Update(menuItem);
                return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
            }
            if (model.Field == "Controller")
            {
                menuItem.MvcController = model.Val;
                menuItemService.Update(menuItem);
                return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
            }
            if (model.Field == "Action")
            {
                menuItem.MvcAction = model.Val;
                menuItemService.Update(menuItem);
                return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
            }

            if (model.Field == "IsPermission")
            {
                menuItem.Enabled = Convert.ToInt32(model.Val);
                menuItemService.Update(menuItem);
                return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
            }
            if (model.Field == "Enabled")
            {
                menuItem.IsPermission = Convert.ToInt32(model.Val);
                menuItemService.Update(menuItem);
                return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
            }
            if (model.Field == "OrderID")
            {
                menuItem.OrderID = Convert.ToInt32(model.Val);
                menuItemService.Update(menuItem);
                return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
            }
            return Notifization.TEST("::-");
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(SearchModel model)
        {
            int page = model.Page;
            string query = string.Empty;
            if (string.IsNullOrEmpty(model.Query))
                query = "";
            else
                query = model.Query;
            int status = model.Status;
            string whereCondition = string.Empty;
            if (status != (int)ModelEnum.Enabled.NONE)
                whereCondition = "AND Enabled = @Enabled";
            string sqlQuery = @"SELECT * FROM View_MenuItem_Option WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' 
                                    AND ParentID ='' " + whereCondition + " ORDER BY OrderID ASC ";
            var menuService = new MenuService(_connection);
            var dtList = menuService.Query<ViewMenuItemLevelModel>(sqlQuery, new { Query = query, Enabled = status }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound();

            foreach (var item in dtList)
            {
                var subMenus = GetSubMenuByLevel(item.ID, status, query);
                if (subMenus.Count > 0)
                    item.SubMenuLevelModel = subMenus;
            }
            Library.PagingModel pagingModel = new Library.PagingModel
            {
                PageSize = Library.Paging.PAGESIZE,
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
            return Notifization.DATALIST(NotifizationText.Success, data: dtList, role: roleAccountModel, paging: pagingModel);
        }
        public List<ViewMenuItemLevelModel> GetSubMenuByLevel(string parentId, int status, string query)
        {
            try
            {
                string whereCondition = string.Empty;
                if (status != (int)ModelEnum.Enabled.NONE)
                    whereCondition = "AND Enabled = @Enabled";
                string sqlQuery = @"SELECT * FROM View_MenuItem_Option WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' 
                AND ParentID = @ParentID  " + whereCondition + " ORDER BY OrderID ASC";
                var menuService = new MenuService(_connection);
                List<ViewMenuItemLevelModel> dtList = menuService.Query<ViewMenuItemLevelModel>(sqlQuery, new { Query = query, ParentID = parentId }).ToList();

                if (dtList.Count <= 0)
                    return new List<ViewMenuItemLevelModel>();
                foreach (var item in dtList)
                {
                    var menuLists = GetSubMenuByLevel(item.ID, status, query);
                    if (menuLists.Count > 0)
                        item.SubMenuLevelModel = menuLists;
                }
                return dtList;
            }
            catch
            {
                return new List<ViewMenuItemLevelModel>();
            }
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult LeftMenuByLevel(MenuCategoryModel model)
        {
            int groupId = model.GroupID;
            string whereCondition = string.Empty;
            if (groupId != (int)MenuItemEnum.GroupID.NONE)
                whereCondition += " AND LocationID =" + groupId;

            string sqlQuery = @"SELECT * FROM View_MenuItem_Option WHERE ParentID ='' AND Enabled = 1 " + whereCondition + " ORDER BY OrderID ASC ";
            var menuService = new MenuService(_connection);
            var dtList = menuService.Query<ViewMenuItemLevelModel>(sqlQuery, new { }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound();

            foreach (var item in dtList)
            {
                var subMenus = LeftSubMenuByLevel(item.ID);
                if (subMenus.Count > 0)
                    item.SubMenuLevelModel = subMenus;
            }
            return Notifization.OPTION(NotifizationText.Success, data: dtList);
        }
        public List<ViewMenuItemLevelModel> LeftSubMenuByLevel(string parentId)
        {
            string sqlQuery = @"SELECT * FROM View_MenuItem_Option WHERE ParentID = @ParentID AND Enabled = 1 ORDER BY OrderID ASC";
            var menuService = new MenuService(_connection);
            List<ViewMenuItemLevelModel> dtList = menuService.Query<ViewMenuItemLevelModel>(sqlQuery, new { ParentID = parentId }).ToList();
            if (dtList.Count <= 0)
                return new List<ViewMenuItemLevelModel>();
            foreach (var item in dtList)
            {
                var menuLists = LeftSubMenuByLevel(item.ID);
                if (menuLists.Count > 0)
                    item.SubMenuLevelModel = menuLists;
            }
            return dtList;
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(MenuItemCreateFormModel model)
        {
            if (model == null)
                return Notifization.Invalid("Dữ liệu không hợp lệ");
            string title = model.Title;
            string summary = model.Summary;
            if (string.IsNullOrEmpty(title))
                return Notifization.Invalid("Không được để trống tiêu đề");
            title = title.Trim();
            if (!Validate.FormatText(title))
                return Notifization.Invalid("Tiêu đề không hợp lệ");
            if (title.Length < 2 || title.Length > 80)
                return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
            // summary valid               
            if (!string.IsNullOrEmpty(summary))
            {
                summary = summary.Trim();
                if (!Validate.FormatText(summary))
                    return Notifization.Invalid("Mô tả không hợp lệ");
                if (summary.Length < 1 || summary.Length > 120)
                    return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
            }
            //
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    MenuItemService menuItemService = new MenuItemService(_connection);
                    var menuItem = menuItemService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()), transaction: transaction);
                    if (menuItem.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    string parentId = model.ParentID;
                    if (parentId.Equals("0") || parentId == "")
                        parentId = "";
                    //
                    int orderId = model.OrderID;
                    if (orderId != (int)MenuItemEnum.Sort.FIRST && orderId != (int)MenuItemEnum.Sort.LAST)
                        orderId = (int)MenuItemEnum.Sort.LAST;
                    //
                    var meta = new MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    var _controllerText = MetaSEO.ControllerText.ToLower();
                    var _actionText = MetaSEO.ActionText.ToLower();
                    string imgFile = model.ImageFile;
                    // page manager, navigate to page manager
                    string _pathAction = model.PathAction;
                    // create
                    var id = menuItemService.Create<string>(new MenuItem()
                    {
                        ParentID = parentId,
                        Title = title,
                        Alias = Helper.Library.Uni2NONE(title),
                        Path = "",
                        Summary = model.Summary,
                        IconFont = model.IconFont,
                        ImageFile = imgFile,
                        ImageHover = string.Empty,
                        OrderID = orderId,
                        IsPermission = model.IsPermission,
                        LocationID = model.LocationID,
                        PathAction = _pathAction,
                        MvcController = _controllerText,
                        MvcAction = _actionText,
                        Enabled = model.Enabled,
                    }, transaction: transaction);
                    // update part

                    // file
                    if (!string.IsNullOrEmpty(imgFile))
                    {
                        string guid = attachmentIngredientService.Create<string>(new Entities.AttachmentIngredient()
                        {
                            ForID = id,
                            FileID = imgFile,
                            CategoryID = _controllerText,
                            TypeID = (int)ModelEnum.FileType.ALONE
                        }, transaction: transaction);
                    }
                    // update part
                    string strPath = string.Empty;
                    var menuParent = menuItemService.GetAlls(m => m.ID.Equals(parentId), transaction: transaction).FirstOrDefault();
                    if (menuParent != null)
                        strPath = menuParent.Path + "/" + id;
                    else
                        strPath = "/" + id;

                    var menuPath = menuItemService.GetAlls(m => m.ID.Equals(id), transaction: transaction).FirstOrDefault();
                    menuPath.Path = strPath;
                    menuItemService.Update(menuPath, transaction: transaction);
                    //sort
                    var menus = menuItemService.GetAlls(m => m.ParentID.Equals(parentId) && (m.ID != (id)), transaction: transaction).OrderBy(m => m.OrderID).ToList();

                    if (model.OrderID == (int)MenuItemEnum.Sort.FIRST)
                    {
                        if (menus.Count > 0)
                        {
                            int count = 2;
                            foreach (var item in menus)
                            {
                                if (!item.ID.Equals(id))
                                {
                                    var menuOther = menuItemService.GetAlls(m => m.ID.Equals(item.ID), transaction: transaction).FirstOrDefault();
                                    if (menuOther != null)
                                    {
                                        menuOther.OrderID = item.OrderID + 1;
                                        menuItemService.Update(menuOther, transaction: transaction);
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
                            var menuOther = menuItemService.GetAlls(m => m.ID.Equals(id), transaction: transaction).FirstOrDefault();
                            if (menuOther != null)
                            {
                                menuOther.OrderID = max + 1;
                                menuItemService.Update(menuOther, transaction: transaction);
                            }
                        }
                    }
                    transaction.Commit();
                    return Notifization.Success(NotifizationText.CREATE_SUCCESS);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Notifization.TEST("::" + ex);
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(MenuItemUpdateFormModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    MenuItemService menuItemService = new MenuItemService(_connection);
                    string id = model.ID;
                    string title = model.Title;
                    var menuItem = menuItemService.GetAlls(m => m.ID.Equals(model.ID.ToLower()), transaction: transaction).FirstOrDefault();
                    if (menuItem == null)
                        return Notifization.NotFound();
                    string menuId = menuItem.ID;
                    //
                    var modelTitle = menuItemService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !m.ID.Equals(model.ID.ToLower()), transaction: transaction).ToList();
                    if (modelTitle.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    string alias = Helper.Library.Uni2NONE(title).ToLower();
                    string parentId = menuItem.ParentID;
                    string menuPath = menuItem.Path;
                    // check parentId
                    if (string.IsNullOrEmpty(model.ParentID) || model.ParentID.Equals("0"))
                    {
                        parentId = "";
                        menuPath = "/" + id;
                    }
                    else
                    {
                        if (!menuItem.ID.Equals(model.ParentID))
                            parentId = model.ParentID;

                        var mPath = menuItemService.GetAlls(m => m.ID.Equals(model.ParentID), transaction: transaction).FirstOrDefault();
                        if (mPath != null)
                            menuPath = mPath.Path + "/" + id;

                    }
                    var meta = new MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    var _controllerText = MetaSEO.ControllerText.ToLower();
                    string imgFile = model.ImageFile;
                    // page manager, navigate to page manager
                    string _pathAction = model.PathAction;
                    // update content
                    menuItem.ParentID = parentId;
                    menuItem.IconFont = model.IconFont;
                    menuItem.ImageFile = imgFile;
                    menuItem.Title = title;
                    menuItem.Summary = model.Summary;
                    menuItem.IsPermission = model.IsPermission;
                    menuItem.PathAction = _pathAction;
                    menuItem.LocationID = (int)MenuItemEnum.Location.LEFT;
                    menuItem.Enabled = model.Enabled;
                    menuItemService.Update(menuItem, transaction: transaction);
                    // update part
                    // file
                    if (string.IsNullOrWhiteSpace(imgFile))
                    {
                        var imagerFile = attachmentIngredientService.GetAlls(m => m.CategoryID.Equals(_controllerText) && m.ForID.Equals(id) && m.TypeID == (int)ModelEnum.FileType.ALONE).FirstOrDefault();
                        if (imagerFile != null)
                            attachmentIngredientService.Remove(imagerFile, transaction: transaction);
                    }
                    else
                    {
                        string guid = attachmentIngredientService.Create<string>(new Entities.AttachmentIngredient()
                        {
                            ForID = id,
                            FileID = imgFile,
                            CategoryID = _controllerText,
                            TypeID = (int)ModelEnum.FileType.ALONE
                        }, transaction: transaction);
                    }
                    bool _sortdf = false;
                    // update part
                    string parentPath = string.Empty;
                    var mnChildren = menuItemService.GetAlls(m => m.ParentID.Equals(id), transaction: transaction).ToList();
                    if (mnChildren.Count > 0)
                    {
                        _sortdf = true;
                        foreach (var item in mnChildren)
                        {
                            var mn = menuItemService.GetAlls(m => m.ID.Equals(item.ID), transaction: transaction).FirstOrDefault();
                            if (mn != null)
                            {
                                mn.Path = menuPath + "/" + item.ID;
                                menuItemService.Update(mn, transaction: transaction);
                            }
                        }
                    }
                    //sort
                    parentId = parentId.ToLower();
                    var menus = menuItemService.GetAlls(m => m.ParentID.Equals(parentId) && m.ID != id, transaction: transaction).OrderBy(m => m.OrderID).ToList();
                    // set sort default if user not select sort
                    int sort = model.OrderID;
                    if (_sortdf)
                        sort = (int)MenuItemEnum.Sort.LAST;
                    if (sort == (int)MenuItemEnum.Sort.FIRST)
                    {
                        if (menus.Count > 0 && !string.IsNullOrEmpty(parentId))
                        {
                            menuItem.OrderID = 1;
                            menuItemService.Update(menuItem, transaction: transaction);
                            int cnt = 2;
                            foreach (var item in menus)
                            {
                                var menuLast = menuItemService.GetAlls(m => m.ID.Equals(item.ID), transaction: transaction).FirstOrDefault();
                                if (menuLast != null)
                                {
                                    menuLast.OrderID = cnt;
                                    menuItemService.Update(menuLast, transaction: transaction);
                                    cnt++;
                                }
                            }
                        }
                    }
                    else if (sort == (int)MenuItemEnum.Sort.LAST)
                    {

                        if (menus.Count > 0 && !string.IsNullOrEmpty(parentId))
                        {
                            int cnt = 1;
                            foreach (var item in menus)
                            {
                                var menuLast = menuItemService.GetAlls(m => m.ID.Equals(item.ID), transaction: transaction).FirstOrDefault();
                                if (menuLast != null)
                                {
                                    menuLast.OrderID = cnt;
                                    menuItemService.Update(menuLast, transaction: transaction);
                                    cnt++;
                                }
                            }
                            menuItem.OrderID = menus.Count + 1;
                            menuItemService.Update(menuItem, transaction: transaction);
                        }
                    }
                    transaction.Commit();
                    return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        public MenuItemModel UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_MenuItem                                 
                                     WHERE ID = @Query";
                var model = _connection.Query<MenuItemModel>(sqlQuery, new { Query = Id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        //public ActionResult Delete(MenuItemIDModel model)
        //{
        //    _connection.Open();
        //    using (var transaction = _connection.BeginTransaction())
        //    {
        //        try
        //        {
        //            MenuItemService menuItemService = new MenuItemService(_connection);
        //            var menuItem = menuItemService.GetAlls(m => m.ID.Equals(model.ID.ToLower()), transaction: transaction).FirstOrDefault();
        //            if (menuItem == null)
        //                return Notifization.ERROR("Không tìm thấy dữ liệu");
        //            // delete menu
        //            _connection.Query("sp_menuitem_delete", new { ID = model.ID }, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);
        //            transaction.Commit();
        //            return Notifization.SUCCESS(NotifizationText.DELETE_SUCCESS);
        //        }
        //        catch
        //        {
        //            transaction.Rollback();
        //            return Notifization.NotService;
        //        }
        //    }
        //}
        public ActionResult Delete(MenuItemIDModel model)
        {
            List<MenuItemIDModel> menuItemIDModels = new List<MenuItemIDModel>();
            string sqlQuery = @"SELECT * FROM View_MenuItem_Option WHERE ID = @ID";
            var menuService = new MenuService(_connection);
            var menuItem = menuService.Query<ViewMenuItemLevelModel>(sqlQuery, new { ID = model.ID }).FirstOrDefault();
            if (menuItem ==  null)
                return Notifization.NotFound();
            menuItemIDModels.Add(new MenuItemIDModel { ID = menuItem.ID });
            var menuLists = DeleteSubMenuByLevel(menuItem.ID);
            if (menuLists.Count > 0)
                menuItemIDModels.Add(new MenuItemIDModel { ID = menuItem.ID });
            return Notifization.OPTION(NotifizationText.Success, data: menuItemIDModels);
        }
        public List<MenuItemIDModel> DeleteSubMenuByLevel(string parentId)
        {
            List<MenuItemIDModel> menuItemIDModels = new List<MenuItemIDModel>();
            string sqlQuery = @"SELECT ID FROM View_MenuItem_Option WHERE ParentID = @ParentID";
            var menuService = new MenuService(_connection);
            List<ViewMenuItemLevelModel> dtList = menuService.Query<ViewMenuItemLevelModel>(sqlQuery, new { ParentID = parentId }).ToList();
            if (dtList.Count > 0)
            {
                foreach (var item in dtList)
                {
                    var menuLists = DeleteSubMenuByLevel(item.ID);
                    if (menuLists.Count > 0)
                        menuItemIDModels.Add(new MenuItemIDModel { ID = item.ID });
                }
            }
            return menuItemIDModels;
        }
        //##############################################################################################################################################################################################################################################################       
        // show category
        public ActionResult LeftMenuItemOption(MenuItemIDModel model)
        {
            string sqlQuery = @"SELECT * FROM View_MenuItem_Option WHERE ParentID ='' AND Enabled = 1 ORDER BY OrderID ASC ";
            var menuService = new MenuService(_connection);
            var dtList = menuService.Query<ViewMenuItemLevelModel>(sqlQuery, new { }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound();

            foreach (var item in dtList)
            {
                var subMenus = LeftSubMenuItemOption(item.ID);
                if (subMenus.Count > 0)
                    item.SubMenuLevelModel = subMenus;
            }
            return Notifization.OPTION(NotifizationText.Success, data: dtList);
        }
        public List<ViewMenuItemLevelModel> LeftSubMenuItemOption(string parentId)
        {
            string sqlQuery = @"SELECT * FROM View_MenuItem_Option WHERE ParentID = @ParentID AND Enabled = 1 ORDER BY OrderID ASC";
            var menuService = new MenuService(_connection);
            List<ViewMenuItemLevelModel> dtList = menuService.Query<ViewMenuItemLevelModel>(sqlQuery, new { ParentID = parentId }).ToList();
            if (dtList.Count <= 0)
                return new List<ViewMenuItemLevelModel>();
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
        //        string sqlQuery = "SELECT * FROM View_MenuItem_Option WHERE ParentID ='' AND [Enabled] = 1 ORDER BY OrderID ASC ";

        //        var dtList = menuItemService.Query<MenuItemOptionModel>(sqlQuery, new { LocationID = locationId }).ToList();
        //        if (dtList.Count <= 0)
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
        //    string sqlQuery = "SELECT * FROM View_MenuItem_Option WHERE ParentID = @ParentID AND [Enabled] = 1  ORDER BY OrderID ASC";
        //    MenuItemService menuItemService = new MenuItemService();
        //    var dtList = menuItemService.Query<MenuItemOptionModel>(sqlQuery, new { ParentID = parentId }).ToList();
        //    if (dtList.Count <= 0)
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
        public string MenuPermission(int locationId, MenuItemIDModel model)
        {
            try
            {
                string result = string.Empty;
                string sqlQuery = "SELECT * FROM View_MenuItem_Option WHERE ParentID ='' AND IsPermission =  1 AND Enabled = 1 AND LocationID =  @LocationID   ORDER BY OrderID ASC ";
                MenuItemService menuItemService = new MenuItemService();
                var dtList = menuItemService.Query<MenuItemOptionModel>(sqlQuery, new { locationId }).ToList();
                if (dtList.Count <= 0)
                    return result;
                int _lv = 0;
                foreach (var item in dtList)
                {
                    string _id = item.ID;
                    string subMenuText = SubMenuPermission(_id, _lv, model);

                    string iAngle = string.Empty;
                    if (!string.IsNullOrEmpty(subMenuText))
                        iAngle = "<span class='fa fa-angle-left iplus'></span>";

                    result += "<li id='Group" + _id + "'  class='list-group-item'><i  data-option='" + _id + "' class='_menu-i far " + GetSelectedFunc(model.ID, item.ID) + "' aria-hidden='true'></i> <span class='_menu-lbl' data-option='" + _id + "'> " + item.Title + " </span>" + subMenuText + "</li>";
                    _lv++;
                }
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
        public string SubMenuPermission(string parentId, int _lv, MenuItemIDModel model)
        {
            try
            {
                string result = string.Empty;
                string sqlQuery = "SELECT * FROM View_MenuItem_Option WHERE ParentID = @ParentID AND IsPermission =  1 AND Enabled = 1  ORDER BY OrderID ASC";
                MenuItemService menuItemService = new MenuItemService();
                var dtList = menuItemService.Query<MenuItemOptionModel>(sqlQuery, new { ParentID = parentId }).ToList();
                if (dtList.Count <= 0)
                    return string.Empty;
                result += "<ul class='list-group'>";

                foreach (var item in dtList)
                {
                    string _id = item.ID;
                    string subMenuText = SubMenuPermission(item.ID, _lv, model);
                    string iAngle = string.Empty;
                    if (!string.IsNullOrEmpty(subMenuText))
                        iAngle = "<span class='fa fa-angle-left iplus'></span>";
                    result += "<li id='Group" + _id + "'  class='list-group-item'><i  data-option='" + _id + "' class='_menu-i far " + GetSelectedFunc(model.ID, item.ID) + "' aria-hidden='true'></i> <span class='_menu-lbl' data-option='" + _id + "'> " + item.Title + " </span>" + subMenuText + "</li>";
                }
                result += "</ul>";
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetSelectedFunc(string roleId, string funcId)
        {
            try
            {
                RoleSettingService roleSettingService = new RoleSettingService(_connection);
                var roleSetting = roleSettingService.GetAlls(m => m.RoleID.ToLower().Equals(roleId.ToLower()) && m.FuncID.ToLower().Equals(funcId.ToLower())).FirstOrDefault();
                if (roleSetting != null)
                    return "fa-check-square actived";
                else
                    return "fa-square";

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
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
                        return Notifization.ERROR(NotifizationText.NOTSERVICE);
                    int _orderId = menu.OrderID;
                    string _parentId = menu.ParentID;
                    // list first
                    IList<MenuSortModel> lstFirst = new List<MenuSortModel>();
                    // list last
                    IList<MenuSortModel> lstLast = new List<MenuSortModel>();
                    //
                    if (!string.IsNullOrEmpty(_parentId))
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
                    return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                }
                catch (Exception)
                {
                    _transaction.Rollback();
                    return Notifization.ERROR(NotifizationText.NOTSERVICE);
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
                        return Notifization.ERROR(NotifizationText.NOTSERVICE);
                    int _orderId = menuItem.OrderID;
                    string _parentId = menuItem.ParentID;
                    // list first
                    IList<MenuSortModel> lstFirst = new List<MenuSortModel>();
                    // list last
                    IList<MenuSortModel> lstLast = new List<MenuSortModel>();
                    //
                    if (!string.IsNullOrEmpty(_parentId))
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
                    return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                }
                catch (Exception)
                {
                    _transaction.Rollback();
                    return Notifization.ERROR(NotifizationText.NOTSERVICE);
                }
            }// end transaction
        }
        //#######################################################################################################################################################################################
        public string GetMenuName(string _controller, string _action)
        {
            string _result = string.Empty;
            if (!string.IsNullOrWhiteSpace(_controller) && !string.IsNullOrWhiteSpace(_action))
            {
                MenuItemService menuItemService = new MenuItemService(_connection);
                var menuItem = menuItemService.GetAlls(m => !string.IsNullOrWhiteSpace(m.MvcController) && !string.IsNullOrWhiteSpace(m.MvcAction) && m.MvcController.Equals(_controller.ToLower()) && m.MvcAction.Equals(_action.ToLower())).FirstOrDefault();
                if (menuItem == null)
                    return string.Empty;
                return menuItem.Title;
            }
            return string.Empty;
        }

        //#######################################################################################################################################################################################

    }
}