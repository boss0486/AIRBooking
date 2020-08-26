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
}