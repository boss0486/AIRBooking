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
        public string Method { get; set; }
        public string KeyID { get; set; }
        public string Title { get; set; }
        public string APIRouter { get; set; }
        public int OrderID { get; set; }
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
        public bool Status { get; set; }
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
        public int OrderID { get; set; }
        public string RouteArea { get; set; }
        public string ControllerID { get; set; }
        public string KeyID { get; set; }
        public string Method { get; set; }
        public string APIRouter { get; set; }
        public string Title { get; set; }
    }

}