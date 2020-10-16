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
    [Table("RoleActionSetting")]
    public partial class RoleActionSetting : WEBModel
    {
        public RoleActionSetting()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string RouteArea { get; set; }
        public string RoleID { get; set; }
        public string ControllerID { get; set; }
        public string ActionID { get; set; }
    }
    public partial class RoleForUser
    {
        public string KeyID { get; set; }
        public string Title { get; set; }
    }

    public partial class RoleActionKey
    {
        public static string DataList = "DataList";
        public static string Create = "Create";
        public static string Update = "Update";
        public static string Delete = "Delete";
        public static string Setting = "Setting";
        public static string Details = "Details";
    }
}