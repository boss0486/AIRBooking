using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("MenuItem")]
    public partial class MenuItem : WEBModel
    {
        public MenuItem()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string RouteArea { get; set; }

        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string IconFont { get; set; }
        public int OrderID { get; set; }
        public int LocationID { get; set; }
        public int IsPermission { get; set; }
        public string Path { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }
        public string PathAction { get; set; }

    }

    public class MenuItemResult
    {
        public string ID { get; set; }
        public string RouteArea { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string Path { get; set; }
        public string IconFont { get; set; }
        public int OrderID { get; set; }
        public int IsPermission { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }
        public string PathAction { get; set; }
        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
    public class MenuItemCreateFormModel
    {
        public string RouteArea { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string IconFont { get; set; }
        public int IsPermission { get; set; }
        public int OrderID { get; set; }
        public int LocationID { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }
        public string PathAction { get; set; }
        public int Enabled { get; set; }

    }

    public class MenuItemUpdateFormModel : MenuItemCreateFormModel
    {
        public string ID { get; set; }
    }
    public class MenuItemOptionModel
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string IconFont { get; set; }
        public int TotalItem { get; set; }
        public string PathAction { get; set; }
    }
    public class MenuItemIDModel
    {
        public string ID { get; set; }
    }
    public class MenuItemCategoryIDModel
    {
        public string CategoryID { get; set; }
    }
    public class MenuItemDeleteAndAtactmentModel
    {
        public string ID { get; set; }
        public string ImageFile { get; set; }
    }
    public class MenuItemEditModel
    {
        public string ID { get; set; }
        public string Val { get; set; }
        public string Field { get; set; }
    }

    public class ViewMenuItemLevelResult
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string IconFont { get; set; }
        public string OrderID { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }
        public string PathAction { get; set; }
        private string _areaId;
        public string AreaID
        {
            get
            {
                return AreaApplicationService.GetAreaKeyByID(_areaId);
            }
            set
            {
                _areaId = value;
            }
        }
        public string Path { get; set; }
        public int Enabled { get; set; }

        public List<ViewMenuItemLevelResult> SubMenuLevelModel { get; set; }

    }


    public class ViewMenuItemPermissionResult
    {
        public string ID { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public List<ViewMenuItemPermissionActionResult> Actions { get; set; }
    }
    public class ViewMenuItemPermissionActionResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
    }


    public partial class MenuActionModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public MenuActionModel(int Id, string title, string action)
        {
            ID = Id;
            Title = title;
        }
    }
    public partial class MenuSortModel
    {
        public string ID { get; set; }
        public int OrderID { get; set; }
    }














    public class MenuItemOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}