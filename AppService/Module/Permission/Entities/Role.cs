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
    [Table("Role")]
    public partial class Role : WEBModel
    {
        public Role()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public int Level { get; set; }
    }

    // model
    public class RoleCreateModel : WEBModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Level { get; set; }
    }
    public class RoleUpdateModel : RoleCreateModel
    {
        public string ID { get; set; }
    }
    public class RoleIDModel
    {
        public string ID { get; set; }
    }
    public class RsRole : RsModel
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public int Level { get; set; }

        public RsRole(string id, string title, string summary, string alias, int level, string languageId, int enabled, string siteId, string createdBy, DateTime createdDate)
        {
            ID = id;
            Title = title;
            Alias = alias;
            Level = level;
            Summary = summary;
            LanguageID = languageId;
            Enabled = enabled;
            SiteID = siteId;
            CreatedBy = createdBy;
            CreatedDate = Helper.Library.FormatDate(createdDate);
        }
    }
    public class RoleOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public int Level { get; set; }

    }
}