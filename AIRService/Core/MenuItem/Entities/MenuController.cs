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
    [Table("MenuController")]
    public class MenuController 
    {
        //public MenuController()
        //{
        //    ID = Guid.NewGuid().ToString().ToLower();
        //}
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string KeyID { get; set; }
        public string RouteArea { get; set; }
        public string RoutePrefix { get; set; }
        public string Title { get; set; }
        public int OrderID { get; set; }
    }

    public class MenuControllerCreateModel
    {
        public string KeyID { get; set; }
        public string RouteArea { get; set; }
        public string RoutePrefix { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }

    }
    public class MenuControllerUpdateModel : MenuControllerCreateModel
    {
        public string ID { get; set; }
    }
    public class MenuControllerResult
    {
        public string ID { get; set; }
        public string KeyID { get; set; }
        public string RouteArea { get; set; }
        public string RoutePrefix { get; set; }
        public string Title { get; set; }
        public int OrderID { get; set; }
        public string ActionCount { get; set; }

    }
    public class MvcControllerForPermision
    {
        public string ID { get; set; }
        public string KeyID { get; set; }
        public string Title { get; set; }
        public bool Status { get; set; }
        public List<MvcActionForPermision> Actions { get; set; }
    }
    public class MvcControllerRoleIDModel
    {
        public string RoleID { get; set; }
        public string RouteArea { get; set; }
    }
    //
    public class MvcControllerOption
    {
        public string ID { get; set; }
        public string KeyID { get; set; }
        public string RouteArea { get; set; }
        public string RoutePrefix { get; set; }
        public string Title { get; set; }
        public string IconFont { get; set; }
        public int ActionCount { get; set; }
        public List<MvcActionOption> Actions { get; set; }
    }

    // for auto
    public class MvcControllerModel
    {
        public string RouteArea { get; set; }
        public string RoutePrefix { get; set; }
        public string KeyID { get; set; }
        public string Title { get; set; }
        public int OrderID { get; set; }
        public List<MvcActionModel> Actions { get; set; }
    }
    public class MvcControllerSetting
    {
        public string RouteArea { get; set; }
    }

}