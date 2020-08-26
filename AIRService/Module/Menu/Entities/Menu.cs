using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper.File;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebCore.Model.Entities;
namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_Menu")]
    public partial class Menu : WEBModel
    {
        public Menu()
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
        public string PathAction { get; set; }
        public string IconFont { get; set; }
        public string ImageFile { get; set; }
        public int OrderID { get; set; }
        public int LocationID { get; set; }
        public string PageTemlate { get; set; }
        public string BackLink { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }
    }

    public class ViewMenu
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string Path { get; set; }
        public string PathAction { get; set; }
        public string IconFont { get; set; }
        public string ImageFile { get; set; }
        public string ImagePath
        {
            get => AttachmentFile.GetFile(ImageFile);
            set
            {
                ImageFile = AttachmentFile.GetFile(ImageFile);
            }
        }
        public int OrderID { get; set; }
        public string PageTemlate { get; set; }
        public string BackLink { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }
        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public ViewMenu(string Id, string parentID, string title, string summary, string alias, string path, string pathAction, string imageFile, int orderID, string pageType, string backLink, string controller, string action, string languageID, int enabled, string createdBy, DateTime createdDate)
        {
            ID = Id;
            ParentID = parentID;
            Title = title;
            Summary = summary;
            Alias = alias;
            Path = path;
            PathAction = pathAction;
            ImageFile = imageFile;
            OrderID = orderID;
            PageTemlate = pageType;
            BackLink = backLink;
            MvcController = controller;
            MvcAction = action;
            LanguageID = languageID;
            Enabled = enabled;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }

        public ViewMenu()
        {
        }
    }
    public class MenuCreateFormModel
    {
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string PathAction { get; set; }
        public string IconFont { get; set; }
        public string ImageFile { get; set; }
        public int OrderID { get; set; }
        public int LocationID { get; set; }
        public string PageTemlate { get; set; }
        public string BackLink { get; set; }
        public string MvcController { get; set; }
        public string MvcAction { get; set; }
        public int Enabled { get; set; }
    }

    public class MenuUpdateFormModel : MenuCreateFormModel
    {
        public string ID { get; set; }
    }

    public class MenuIDModel
    {
        public string ID { get; set; }
    }
    public class MenuDeleteAndAtactmentModel
    {
        public string ID { get; set; }
        public string ImageFile { get; set; }
    }
    public class MenuEditModel
    {
        public string ID { get; set; }
        public string Val { get; set; }
        public string Field { get; set; }
    }


    public partial class MenuPageTypeOptionModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public MenuPageTypeOptionModel(string Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }

    // menu list
    public class ViewMenuLevelModel
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public string IconFont { get; set; }
        public string IconHover { get; set; }
        public int TotalItem { get; set; }
        public string PathAction { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Enabled { get; set; }

        public List<ViewMenuLevelModel> SubMenuLevelModel { get; set; }

    }
    //
    public class MenuOptionModel
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
    //
}