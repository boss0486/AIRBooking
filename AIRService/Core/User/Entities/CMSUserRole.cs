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
    [Table("CMSUserRole")]
    public partial class CMSUserRole 
    {
        public CMSUserRole()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }

        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string UserID { get; set; }
        public string RoleID { get; set; }
    }

    public class CMSUserRoleModel
    {
        public string UserID { get; set; }
        public List<string> ArrRole { get; set; }
    }
}
