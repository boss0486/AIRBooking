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
    [Table("UserSetting")]
    public partial class UserSetting : WEBModel
    {
        public UserSetting()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }

        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string UserID { get; set; }
        public string AuthenType { get; set; }
        public string SecurityPassword { get; set; }
        public bool IsBlock { get; set; }
    }
}
