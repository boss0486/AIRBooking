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
    [Table("RoleAction")]
    public partial class RoleActionAction : WEBModel
    {
        public RoleActionAction()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string GroupID { get; set; }
        public int Val { get; set; }
    }

    // model
    public class RoleActionCreateModel : WEBModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string GroupID { get; set; }
        public int Val { get; set; }
    }
    public class RoleActionUpdateModel : RoleActionCreateModel
    {
        public string ID { get; set; }
    }
    public class RoleActionIDModel
    {
        public string ID { get; set; }
    }
    public class RsRoleAction : RsModel
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string GroupID { get; set; }
        public int Val { get; set; }

        public RsRoleAction(string id, string title, string summary, string groupId, int val, string languageId, int enabled, string siteId, string createdBy, DateTime createdDate)
        {
            ID = id;
            Title = title;
            GroupID = groupId;
            Val = val;
            Summary = summary;
            LanguageID = languageId;
            Enabled = enabled;
            SiteID = siteId;
            CreatedBy = createdBy;
            CreatedDate = Helper.Library.FormatDate(createdDate);
        }
    }
    public class RoleActionOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string GroupID { get; set; }
        public int Val { get; set; }
    }
 
 
}