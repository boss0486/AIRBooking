using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebCore.Model.Entities;
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
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string Path { get; set; }
        public string IconFont { get; set; }
        public string ImageFile { get; set; }
        public string ImageHover { get; set; }
        public int OrderID { get; set; }
        public string Permission { get; set; }
        public int IsPermission { get; set; }
        public int LocationID { get; set; }
        public string PathAction { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }

    }

    public class MenuItemModel
    {
        public string ID { get; set; }
        public string IntID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string Path { get; set; }
        public string IconFont { get; set; }
        public string ImageFile { get; set; }
        public string ImageHover { get; set; }
        public int OrderID { get; set; }
        public string Permission { get; set; }
        public int IsPermission { get; set; }
        public int LocationID { get; set; }
        public string PathAction { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }
        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }
    public class RsMenuItem
    {
        public string ID { get; set; }
        public string IntID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string Path { get; set; }
        public string IconFont { get; set; }
        public string IconHover { get; set; }
        public string ImageFile { get; set; }
        public string ImageHover { get; set; }
        public int OrderID { get; set; }
        public string Permission { get; set; }
        public int IsPermission { get; set; }
        public string PathAction { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }

        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public RsMenuItem(string Id, string parentID, string title, string summary, string alias, string path, string image, string imageHover, int orderID, string permission, int isPermission, string pathAction, string controller, string action, string languageID, int enabled, string createdBy, string createdDate)
        {
            ID = Id;
            ParentID = parentID;
            Title = title;
            Summary = summary;
            Alias = alias;
            Path = path;
            ImageFile = image;
            ImageHover = imageHover;
            OrderID = orderID;
            Permission = permission;
            IsPermission = isPermission;
            PathAction = pathAction;
            MvcController = controller;
            MvcAction = action;
            LanguageID = languageID;
            Enabled = enabled;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }
    }
    public class MenuItemCreateFormModel
    {
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string IconFont { get; set; }
        public string ImageFile { get; set; }
        public string ImageHover { get; set; }
        public int IsPermission { get; set; }
        public int OrderID { get; set; }
        public int LocationID { get; set; }
        public string PathAction { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }
        public int Enabled { get; set; }

        [Dapper.NotMapped]
        public HttpPostedFileBase DocumentFile { get; set; }
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
        public string IconHover { get; set; }
        public int TotalItem { get; set; }
        public string PathAction { get; set; }
    }
    public class MenuItemIDModel
    {
        public string ID { get; set; }
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

    public class ViewMenuItemLevelModel
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string IconFont { get; set; }
        public string IconHover { get; set; }       
        public string PathAction { get; set; }
        public string OrderID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Enabled { get; set; }
        public int TotalItem { get; set; }
        public List<ViewMenuItemLevelModel> SubMenuLevelModel { get; set; }

    }
    public partial class MenuActionModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string PathAction { get; set; }
        public MenuActionModel(int Id, string title, string action)
        {
            ID = Id;
            Title = title;
            PathAction = action;
        }
    }
    public partial class MenuSortModel
    {
        public string ID { get; set; }
        public int OrderID { get; set; }         
    }
    public class MenuCategoryModel
    {
        public string Query { get; set; }
        public int Page { get; set; }
        public int Status { get; set; }
        public int GroupID { get; set; }
    }
}