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

    public class MenuItemIDModel
    {
        public string ID { get; set; }
    }

    public class MenuItemModel
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string IconFont { get; set; }
        public string OrderID { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }
        public string PathAction { get; set; }
        public string RouteArea { get; set; }
        public string Path { get; set; }
        public int Enabled { get; set; }
        public List<MenuItemModel> SubMenuModel { get; set; }
    }

    public class MenuItemLayout
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string IconFont { get; set; }
        public string OrderID { get; set; }
       
        public string PathAction { get; set; }
        private string _mvcController;
        public string MvcController
        {
            get
            {
                return MenuControllerService.GetKeyByID(_mvcController);
            }
            set
            {
                _mvcController = value;
            }
        }
        private string _mvcAction;
        public string MvcAction
        {
            get
            {
                return MenuActionService.GetKeyByID(_mvcAction);
            }
            set
            {
                _mvcAction = value;
            }
        }
    }

    public class MenuItemModelResult
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
        public string RouteArea
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

        public List<MenuItemModelResult> SubMenuLevelModel { get; set; }

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