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
    [Table("RoleSetting")]
    public partial class RoleSetting
    {
        public RoleSetting()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string RoleID { get; set; }
        public string FuncID { get; set; }
        public string ActionID { get; set; }
        public Boolean IsAction { get; set; }
        public string SiteID { get; set; }
    }

    public class RoleSettingModel
    {
        public string RoleID { get; set; }
        public List<string> ArrFuncID { get; set; }
        public string FuncID { get; set; }
        public List<string> ArrActionID { get; set; }
    }

    public class RemoveSubRole
    {
        public int ID { get; set; }
    }
}