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
    [Table("MenuAction")]
    public partial class MenuAction : WEBModel
    {
        //public MenuAction()
        //{
        //    ID = Guid.NewGuid().ToString().ToLower();
        //}
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string RouteArea { get; set; }
        public string CategoryID { get; set; }
        public string KeyID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public string IconFont { get; set; }
        public string Path { get; set; }
        public bool APIAction { get; set; }
    }



    public partial class ActionByCategoryModel
    {
        public string ID { get; set; }
        public string CategoryID { get; set; }
    }

    //
    public partial class MvcActionForPermision
    {
        public string ID { get; set; }
        public string KeyID { get; set; }
        public string Title { get; set; }
        public int Total { get; set; }
        public bool APIAction { get; set; }
    }
    // for auto
    public partial class MvcActionOption
    {
        public string ID { get; set; }
        public string CategoryID { get; set; }
        public string KeyID { get; set; }
        public string Title { get; set; }
        public bool APIAction { get; set; }

    }
    public partial class MvcActionModel
    {
        public string RouteArea { get; set; }
        public string ControllerID { get; set; }
        public string Text { get; set; }
        public string Router { get; set; }
        public string Attributes { get; set; }
        public bool APIAction { get; set; }

    }

}