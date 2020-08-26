using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Web;
using WebCore.Model.Entities;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("CMSUserSetting")]
    public partial class CMSUserSetting : WEBModel
    {
        public CMSUserSetting()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string AreaID { get; set; }
        public string UserID { get; set; }
        public string AuthenType { get; set; }
        public string SecurityPassword { get; set; }
        public string RoleID { get; set; }
        public bool IsBlock { get; set; }
    }
}
