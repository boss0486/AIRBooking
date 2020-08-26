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
    public interface IMenuService : IEntityService<Menu> { }
    public class MenuService : EntityService<Menu>, IMenuService
    {
        public MenuService() : base() { }
        public MenuService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(SearchModel model)
        {
            int page = model.Page;
            string query = string.Empty;
            if (string.IsNullOrEmpty(model.Query))
                query = "";
            else
                query = model.Query;
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT * FROM View_App_Menu 
                                     WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                            
                                     ORDER BY [CreatedDate] ";
            var dtList = _connection.Query<ViewMenu>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);
            var result = dtList.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);

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
            return Notifization.DATALIST(NotifizationText.Success, data: result, role: roleAccountModel, paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################

        public ActionResult GetMenuByLevel(SearchModel model)
        {
            try
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
                string sqlQuery = @"SELECT * FROM View_App_Menu_Option WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' 
                                    AND ParentID ='' " + whereCondition + " ORDER BY OrderID ASC ";
                var menuService = new MenuService(_connection);
 
                var dtList = menuService.Query<ViewMenuLevelModel>(sqlQuery, new { Query = query, Enabled = status }).ToList();
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
            catch
            {
                return Notifization.NotService;
            }
        }
        public List<ViewMenuLevelModel> GetSubMenuByLevel(string parentId, int status, string query)
        {
            try
            {
                string whereCondition = string.Empty;
                if (status != (int)ModelEnum.Enabled.NONE)
                    whereCondition = "AND Enabled = @Enabled";
                string sqlQuery = @"SELECT * FROM View_App_Menu_Option WHERE Title LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' 
                AND ParentID = @ParentID  " + whereCondition + " ORDER BY OrderID ASC";
                var menuService = new MenuService(_connection);
                List<ViewMenuLevelModel> dtList = menuService.Query<ViewMenuLevelModel>(sqlQuery, new { Query = query, ParentID = parentId }).ToList();

                if (dtList.Count <= 0)
                    return new List<ViewMenuLevelModel>();
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
                return new List<ViewMenuLevelModel>();
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(MenuCreateFormModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    MenuService menuService = new MenuService(_connection);
                    string title = model.Title;
                    var menu = menuService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()), transaction: transaction);
                    if (menu.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    string alias = model.Alias;
                    if (string.IsNullOrWhiteSpace(alias))
                        alias = Helper.Library.Uni2NONE(title);
                    alias = alias.ToLower();
                    //
                    string parentId = model.ParentID;
                    if (parentId.Equals("0") || parentId == "")
                        parentId = " ";
                    //
                    int orderId = model.OrderID;
                    if (orderId != (int)MenuEnum.Sort.FIRST && orderId != (int)MenuEnum.Sort.LAST)
                        orderId = (int)MenuEnum.Sort.LAST;
                    //
                    var meta = new MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    var _controllerText = MetaSEO.ControllerText.ToLower();
                    //
                    string imgFile = model.ImageFile;
                    var id = menuService.Create<string>(new Menu()
                    {
                        ParentID = parentId,
                        Title = model.Title,
                        Alias = alias,
                        Path = "",
                        Summary = model.Summary,
                        IconFont = model.IconFont,
                        ImageFile = model.ImageFile,
                        PageTemlate = model.PageTemlate,
                        OrderID = orderId,
                        LocationID = model.LocationID,
                        MvcController = _controllerText,
                        Enabled = model.Enabled,
                    }, transaction: transaction);
                    id = id.ToLower();
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
                    var menuParent = menuService.GetAlls(m => m.ID.Equals(parentId), transaction: transaction).FirstOrDefault();
                    if (menuParent != null)
                        strPath = menuParent.Path + "/" + id;
                    else
                        strPath = "/" + id;

                    var menuPath = menuService.GetAlls(m => m.ID.Equals(id), transaction: transaction).FirstOrDefault();
                    menuPath.Path = strPath;
                    menuService.Update(menuPath, transaction: transaction);
                    //sort
                    var menus = menuService.GetAlls(m => m.ParentID.Equals(parentId) && (m.ID != (id)), transaction: transaction).OrderBy(m => m.OrderID).ToList();
                    if (model.OrderID == (int)MenuEnum.Sort.FIRST)
                    {
                        if (menus.Count > 0 && !string.IsNullOrEmpty(parentId))
                        {
                            int count = 2;
                            foreach (var item in menus)
                            {
                                if (!item.ID.Equals(id))
                                {
                                    var menuOther = menuService.GetAlls(m => m.ID.Equals(item.ID), transaction: transaction).FirstOrDefault();
                                    if (menuOther != null)
                                    {
                                        menuOther.OrderID = item.OrderID + 1;
                                        menuService.Update(menuOther, transaction: transaction);
                                    }
                                    count++;
                                }
                            }
                        }
                    }
                    else if (model.OrderID == (int)MenuEnum.Sort.LAST)
                    {
                        if (menus.Count > 0 && !string.IsNullOrEmpty(parentId))
                        {
                            int max = menus.Max(m => m.OrderID);
                            var menuOther = menuService.GetAlls(m => m.ID.Equals(id), transaction: transaction).FirstOrDefault();
                            if (menuOther != null)
                            {
                                menuOther.OrderID = max + 1;
                                menuService.Update(menuOther, transaction: transaction);
                            }
                        }
                    }

                    transaction.Commit();
                    return Notifization.Success(NotifizationText.CREATE_SUCCESS);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(MenuUpdateFormModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    var menuService = new MenuService(_connection);
                    string title = model.Title;
                    string id = model.ID;
                    if (string.IsNullOrEmpty(id))
                        return Notifization.NotFound();
                    id = id.ToLower();
                    //
                    var menu = menuService.GetAlls(m => m.ID.Equals(id), transaction: transaction).FirstOrDefault();
                    if (menu == null)
                        return Notifization.NotFound();
                    //
                    var mTitle = menuService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !m.ID.Equals(id), transaction: transaction).ToList();
                    if (mTitle.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");

                    string alias = model.Alias;
                    var mAlias = menuService.GetAlls(m => m.Alias.ToLower().Equals(alias.ToLower()) && !m.ID.Equals(id), transaction: transaction).ToList();
                    if (mAlias.Count > 0)
                        return Notifization.Invalid("Đường dẫn đã được sử dụng");
                    //
                    if (string.IsNullOrWhiteSpace(alias))
                        alias = Helper.Library.Uni2NONE(title);
                    alias = alias.ToLower();
                    string parentId = menu.ParentID;
                    string menuPath = menu.Path;
                    // check parentId
                    if (string.IsNullOrEmpty(model.ParentID) || model.ParentID.Equals("0"))
                    {
                        parentId = "";
                        menuPath = "/" + id;
                    }
                    else
                    {
                        if (!menu.ID.Equals(model.ParentID))
                            parentId = model.ParentID;

                        var mPath = menuService.GetAlls(m => m.ID.Equals(model.ParentID), transaction: transaction).FirstOrDefault();
                        if (mPath != null)
                            menuPath = mPath.Path + "/" + id;

                    }
                    var meta = new MetaSEO();
                    var attachmentIngredientService = new AttachmentIngredientService(_connection);
                    var _controllerText = MetaSEO.ControllerText.ToLower();
                    string imgFile = model.ImageFile;
                    // update content
                    menu.ParentID = parentId;
                    menu.Path = menuPath;
                    menu.IconFont = model.IconFont;
                    menu.ImageFile = imgFile;
                    menu.Title = title;
                    menu.Summary = model.Summary;
                    menu.Alias = alias;
                    menu.PageTemlate = model.PageTemlate;
                    menu.BackLink = model.BackLink;
                    menu.MvcController = _controllerText;
                    menu.LocationID = (int)MenuEnum.Location.MAIN;
                    menu.Enabled = model.Enabled;
                    menuService.Update(menu, transaction: transaction);
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
                    var mnChildren = menuService.GetAlls(m => m.ParentID.Equals(id), transaction: transaction).ToList();
                    if (mnChildren.Count > 0)
                    {
                        _sortdf = true;
                        foreach (var item in mnChildren)
                        {
                            var mn = menuService.GetAlls(m => m.ID.Equals(item.ID), transaction: transaction).FirstOrDefault();
                            if (mn != null)
                            {
                                mn.Path = menuPath + "/" + item.ID;
                                menuService.Update(mn, transaction: transaction);
                            }
                        }
                    }
                    //sort
                    parentId = parentId.ToLower();
                    var menus = menuService.GetAlls(m => m.ParentID.Equals(parentId) && m.ID != id, transaction: transaction).OrderBy(m => m.OrderID).ToList();
                    // set sort default if user not select sort
                    int sort = model.OrderID;
                    if (_sortdf && sort == (int)MenuEnum.Sort.NONE)
                        sort = (int)MenuEnum.Sort.LAST;
                    if (sort == (int)MenuEnum.Sort.FIRST)
                    {
                        if (menus.Count > 0 && !string.IsNullOrEmpty(parentId))
                        {
                            menu.OrderID = 1;
                            menuService.Update(menu, transaction: transaction);
                            int cnt = 2;
                            foreach (var item in menus)
                            {
                                var menuLast = menuService.GetAlls(m => m.ID.Equals(item.ID), transaction: transaction).FirstOrDefault();
                                if (menuLast != null)
                                {
                                    menuLast.OrderID = cnt;
                                    menuService.Update(menuLast, transaction: transaction);
                                    cnt++;
                                }
                            }
                        }
                    }
                    else if (sort == (int)MenuEnum.Sort.LAST)
                    {

                        if (menus.Count > 0 && !string.IsNullOrEmpty(parentId))
                        {
                            int cnt = 1;
                            foreach (var item in menus)
                            {
                                var menuLast = menuService.GetAlls(m => m.ID.Equals(item.ID), transaction: transaction).FirstOrDefault();
                                if (menuLast != null)
                                {
                                    menuLast.OrderID = cnt;
                                    menuService.Update(menuLast, transaction: transaction);
                                    cnt++;
                                }
                            }
                            menu.OrderID = menus.Count + 1;
                            menuService.Update(menu, transaction: transaction);
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
        public ViewMenu UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_Menu WHERE ID = @Query";
                var model = _connection.Query<ViewMenu>(sqlQuery, new { Query = Id }).FirstOrDefault();
                return model;
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(MenuIDModel model)
        {
            try
            {
                using (var _connectDb = DbConnect.Connection.CMS)
                {
                    _connectDb.Open();
                    using (var transaction = _connectDb.BeginTransaction())
                    {
                        try
                        {
                            MenuService MenuService = new MenuService(_connectDb);
                            var Menu = MenuService.GetAlls(m => m.ID.Equals(model.ID.ToLower()), transaction: transaction).FirstOrDefault();
                            if (Menu == null)
                                return Notifization.ERROR("Không tìm thấy dữ liệu");
                            // remote menu, children menu
                            string sqlQuery = @"
                                WITH MenuTemp AS
                                (select ID,ImageFile  
                                 from View_Menu
                                 where  ID  =  @ID
                                 union all
                                 select c.ID, c.ImageFile
                                 from View_Menu c
                                    inner join MenuTemp mn on c.ParentID = mn.ID
                                )SELECT ID,ImageFile FROM MenuTemp ";
                            var menu = _connectDb.Query<MenuDeleteAndAtactmentModel>(sqlQuery, new { ID = model.ID }, transaction: transaction).ToList();
                            // delete file
                            if (menu.Count > 0)
                            {
                                foreach (var item in menu)
                                    AttachmentFile.DeleteFile(item.ImageFile, transaction: transaction);
                            }
                            // delete menu
                            _connectDb.Query("sp_Menu_delete", new { ID = model.ID }, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);
                            transaction.Commit();
                            return Notifization.Success(NotifizationText.DELETE_SUCCESS);
                        }
                        catch
                        {
                            transaction.Rollback();
                            return Notifization.NotService;
                        }
                    }
                }
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLPageTemplate(string id)
        {
            try
            {
                List<MenuPageTypeOptionModel> menuTypeOptionModels = new List<MenuPageTypeOptionModel>{
                    new MenuPageTypeOptionModel("Introduction", "Giới thiệu"),
                    new MenuPageTypeOptionModel("Article", "Bài viết"),
                    new MenuPageTypeOptionModel("Product", "Sản phẩm"),
                    new MenuPageTypeOptionModel("Service", "Dịch vụ"),
                    new MenuPageTypeOptionModel("Recruitment", "Tuyển dụng"),
                    new MenuPageTypeOptionModel("Contact", "Liên hệ"),
                    new MenuPageTypeOptionModel("Faq", "Faq")
                };
                string result = string.Empty;
                foreach (var item in menuTypeOptionModels)
                {
                    string selected = string.Empty;
                    if (!string.IsNullOrEmpty(id) && item.ID.ToLower().Equals(id.ToLower()))
                        selected = "selected";

                    result += "<option value='" + item.ID + "' " + selected + ">" + item.Title + "</option>";
                }
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public string GetMenuOption(string id)
        {
            string result = @"<li><a><i class='far fa-square mn-ckeck' data-val='0'></i><span>Tạo mới</span></a></li>";
            string sqlQuery = "SELECT * FROM View_App_Menu_Option WHERE ParentID ='' ORDER BY OrderID ASC ";
            var menuService = new MenuService(_connection);
            var dtList = menuService.Query<MenuOptionModel>(sqlQuery).ToList();
            if (dtList.Count <= 0)
                return result;
            foreach (var item in dtList)
            {
                string menuList = GetSubMenuOption(item.ID, id);
                string iAngle = string.Empty;
                if (!string.IsNullOrEmpty(menuList))
                    iAngle = "<i class='fa fa-angle-left iplus'></i>";

                string icon = "fa-square";
                if (id != null && item.ID.ToLower().Equals(id.ToLower()))
                    icon = "fa-check-square actived ";
                result += "<li><a><i class='far " + icon + " mn-ckeck' data-id='" + item.ID + "'></i><span>" + item.Title + iAngle + "</span></a>" + menuList + "</li>";
            }
            return result;
        }
        public string GetSubMenuOption(string parentId, string id)
        {
            try
            {
                string result = string.Empty;
                string sqlQuery = "SELECT * FROM View_App_Menu_Option WHERE ParentID = @ParentID  ORDER BY OrderID ASC";
                var menuService = new MenuService(_connection);
                var dtList = menuService.Query<MenuOptionModel>(sqlQuery, new { ParentID = parentId }).ToList();
                if (dtList.Count <= 0)
                    return string.Empty;
                result += "<ul class='sidebar-submenu'>";
                foreach (var item in dtList)
                {
                    string menuList = GetSubMenuOption(item.ID, id);
                    string iAngle = string.Empty;
                    if (!string.IsNullOrEmpty(menuList))
                        iAngle = "<i class='fa fa-angle-left iplus'></i>";

                    string icon = "fa-square";
                    if (id != null && item.ID.ToLower().Equals(id.ToLower()))
                        icon = "fa-check-square actived";
                    result += "<li><a><i class='far " + icon + " mn-ckeck' data-id='" + item.ID + "'></i><span>" + item.Title + iAngle + "</span></a>" + menuList + "</li>";
                }
                result += "</ul>";
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string ShowMenuOption(string id)
        {
            using (var menuService = new MenuService())
            {
                return menuService.GetMenuOption(id);
            }
        }
        //##############################################################################################################################################################################################################################################################
        //#######################################################################################################################################################################################
        public ActionResult SortUp(MenuItemIDModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    MenuService menuService = new MenuService(_connection);
                    var menu = menuService.GetAlls(m => m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).FirstOrDefault();
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
                        var menus = menuService.GetAlls(m => m.ParentID.ToLower().Equals(_parentId.ToLower()) && !m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).OrderBy(m => m.OrderID).ToList();
                        if (menus.Count > 0)
                        {
                            foreach (var item in menus)
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
                                        menuService.Update(menu, transaction: _transaction);
                                        _cntFirst++;
                                    }
                                    var itemFirst = menuService.GetAlls(m => m.ID.ToLower().Equals(lstFirst[i].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemFirst.OrderID = _cntFirst;
                                    menuService.Update(itemFirst, transaction: _transaction);
                                    _cntFirst++;
                                }

                            }
                            else
                            {
                                menu.OrderID = 1;
                                menuService.Update(menu, transaction: _transaction);
                                _cntFirst++;
                            }
                            //last
                            int _cntLast = _cntFirst;
                            if (lstLast.Count > 0)
                            {
                                foreach (var item in lstLast)
                                {
                                    var itemLast = menuService.GetAlls(m => m.ID.ToLower().Equals(item.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemLast.OrderID = _cntLast;
                                    menuService.Update(itemLast, transaction: _transaction);
                                    _cntLast++;
                                }
                            }
                        }
                        else
                        {
                            menu.OrderID = 1;
                            menuService.Update(menu, transaction: _transaction);
                        }
                    }
                    else
                    {
                        // truong hop cha , _parentId = ""
                        var menus = menuService.GetAlls(m => m.ParentID.Equals(_parentId) && !m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).OrderBy(m => m.OrderID).ToList();
                        if (menus.Count > 0)
                        {
                            foreach (var item in menus)
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
                                        menuService.Update(menu, transaction: _transaction);
                                        _cntFirst++;
                                    }
                                    var itemFirst = menuService.GetAlls(m => m.ID.ToLower().Equals(lstFirst[i].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemFirst.OrderID = _cntFirst;
                                    menuService.Update(itemFirst, transaction: _transaction);
                                    _cntFirst++;
                                }

                            }
                            else
                            {
                                menu.OrderID = 1;
                                menuService.Update(menu, transaction: _transaction);
                                _cntFirst++;
                            }
                            //last
                            int _cntLast = _cntFirst;
                            if (lstLast.Count > 0)
                            {
                                foreach (var item in lstLast)
                                {
                                    var itemLast = menuService.GetAlls(m => m.ID.ToLower().Equals(item.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemLast.OrderID = _cntLast;
                                    menuService.Update(itemLast, transaction: _transaction);
                                    _cntLast++;
                                }
                            }
                        }
                        else
                        {
                            menu.OrderID = 1;
                            menuService.Update(menu, transaction: _transaction);
                        }
                    }
                    _transaction.Commit();
                    return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST("::" + ex);
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
                    MenuService menuService = new MenuService(_connection);
                    var menu = menuService.GetAlls(m => m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                    if (menu == null)
                        return Notifization.TEST("::");
                    int _orderId = menu.OrderID;
                    string _parentId = menu.ParentID;
                    // list first
                    IList<MenuSortModel> lstFirst = new List<MenuSortModel>();
                    // list last
                    IList<MenuSortModel> lstLast = new List<MenuSortModel>();
                    //
                    if (!string.IsNullOrEmpty(_parentId))
                    {
                        var menus = menuService.GetAlls(m => m.ParentID.ToLower().Equals(_parentId.ToLower()) && !m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).OrderBy(m => m.OrderID).ToList();
                        if (menus.Count > 0)
                        {
                            foreach (var item in menus)
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
                            // xu ly
                            int _cntFirst = 1;
                            if (lstFirst.Count > 0)
                            {
                                foreach (var item in lstFirst)
                                {
                                    var itemFirst = menuService.GetAlls(m => m.ID.ToLower().Equals(item.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemFirst.OrderID = _cntFirst;
                                    menuService.Update(itemFirst, transaction: _transaction);
                                    _cntFirst++;
                                }

                            }
                            //  last
                            int _cntLast = _cntFirst;
                            if (lstLast.Count == 1)
                            {
                                var itemLast = menuService.GetAlls(m => m.ID.ToLower().Equals(lstLast[0].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                itemLast.OrderID = _cntLast;
                                menuService.Update(itemLast, transaction: _transaction);
                                //
                                menu.OrderID = _cntLast + 1;
                                menuService.Update(menu, transaction: _transaction);
                                _cntLast++;
                            }
                            else if (lstLast.Count > 1)
                            {
                                for (int i = 0; i < lstLast.Count; i++)
                                {
                                    if (i == 1)
                                    {
                                        menu.OrderID = _cntLast;
                                        menuService.Update(menu, transaction: _transaction);
                                        _cntLast++;
                                    }
                                    var itemLast = menuService.GetAlls(m => m.ID.ToLower().Equals(lstLast[i].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemLast.OrderID = _cntLast;
                                    menuService.Update(itemLast, transaction: _transaction);
                                    _cntLast++;
                                }
                            }
                        }
                        else
                        {
                            menu.OrderID = 1;
                            menuService.Update(menu, transaction: _transaction);
                        }
                    }
                    else
                    {
                        // truong hop cha , _parentId = ""
                        var menus = menuService.GetAlls(m => m.ParentID.Equals(_parentId) && !m.ID.ToLower().Equals(model.ID.ToLower()), transaction: _transaction).OrderBy(m => m.OrderID).ToList();
                        if (menus.Count > 0)
                        {
                            foreach (var item in menus)
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
                            // xu ly
                            int _cntFirst = 1;
                            if (lstFirst.Count > 0)
                            {
                                foreach (var item in lstFirst)
                                {
                                    var itemFirst = menuService.GetAlls(m => m.ID.ToLower().Equals(item.ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemFirst.OrderID = _cntFirst;
                                    menuService.Update(itemFirst, transaction: _transaction);
                                    _cntFirst++;
                                }
                            }
                            //  last
                            int _cntLast = _cntFirst;
                            if (lstLast.Count == 1)
                            {
                                var itemLast = menuService.GetAlls(m => m.ID.ToLower().Equals(lstLast[0].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                itemLast.OrderID = _cntLast;
                                menuService.Update(itemLast, transaction: _transaction);
                                //
                                menu.OrderID = _cntLast + 1;
                                menuService.Update(menu, transaction: _transaction);
                                _cntLast++;
                            }
                            else if (lstLast.Count > 1)
                            {
                                for (int i = 0; i < lstLast.Count; i++)
                                {
                                    if (i == 1)
                                    {
                                        menu.OrderID = _cntLast;
                                        menuService.Update(menu, transaction: _transaction);
                                        _cntLast++;
                                    }
                                    var itemLast = menuService.GetAlls(m => m.ID.ToLower().Equals(lstLast[i].ID.ToLower()), transaction: _transaction).FirstOrDefault();
                                    itemLast.OrderID = _cntLast;
                                    menuService.Update(itemLast, transaction: _transaction);
                                    _cntLast++;
                                }
                            }
                        }
                        else
                        {
                            menu.OrderID = 1;
                            menuService.Update(menu, transaction: _transaction);
                        }
                    }
                    _transaction.Commit();
                    return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST("::" + ex);
                }
            }// end transaction
        }
        //#######################################################################################################################################################################################
        // Show home page
        public string GetMenuBar(string id)
        {
            string result = @"";
            string sqlQuery = "SELECT * FROM View_App_Menu WHERE ParentID ='' ORDER BY OrderID ASC ";
            var menuService = new MenuService(_connection);
            var dtList = menuService.Query<Menu>(sqlQuery).ToList();
            if (dtList.Count <= 0)
                return result;
            foreach (var item in dtList)
            {
                string menuList = GetSubMenuBar(item.ID, id);
                string iAngle = string.Empty;
                string _subclass = string.Empty;
                string _link = string.Empty;
                if (!string.IsNullOrWhiteSpace(menuList))
                {
                    iAngle = "<i class='fa fa-angle-down iplus'></i>";
                    _subclass = "menu-has-children";
                   
                }
                if (!string.IsNullOrWhiteSpace(item.BackLink))
                {
                    _link = item.BackLink;
                }
                else if (!string.IsNullOrWhiteSpace(item.PathAction))
                {
                    _link = item.PathAction;
                }
                else
                {
                    _link = "#";
                }                  
                result += "<li class='" + _subclass + "'><a href='"+ _link + "'>" + item.Title + "</a>" + menuList + "</li>";
            }
            return result;
        }
        public string GetSubMenuBar(string parentId, string id)
        {
            try
            {
                string result = string.Empty;
                string sqlQuery = "SELECT * FROM View_App_Menu WHERE ParentID = @ParentID  ORDER BY OrderID ASC";
                var menuService = new MenuService(_connection);
                var dtList = menuService.Query<Menu>(sqlQuery, new { ParentID = parentId }).ToList();
                if (dtList.Count == 0)
                    return string.Empty;
                result += "<ul>";
                foreach (var item in dtList)
                {
                    string menuList = GetSubMenuBar(item.ID, id);
                    string iAngle = string.Empty;
                    string _subclass = string.Empty;
                    string _link = string.Empty;
                    if (!string.IsNullOrWhiteSpace(menuList))
                    {
                        iAngle = "<i class='fa fa-angle-down iplus'></i>";
                        _subclass = "menu-has-children";
                    }
                    if (!string.IsNullOrWhiteSpace(item.BackLink))
                    {
                        _link = item.BackLink;
                    }
                    else if (!string.IsNullOrWhiteSpace(item.PathAction))
                    {
                        _link = item.PathAction;
                    }
                    else
                    {
                        _link = "#";
                    }
                    result += "<li class='" + _subclass + "'><a href='"+ _link + "'>" + item.Title + "</a>" + menuList + "</li>";
                }
                result += "</ul>";
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
        // 
    }
}