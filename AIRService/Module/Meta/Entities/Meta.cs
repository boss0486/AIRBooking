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
    [Table("App_Meta")]
    public partial class Meta : WEBModel
    {
        public Meta()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public int GroupID { get; set; }
        public string Alias { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
    }
    // model
    public class MetaCreateModel
    {
        public int GroupID { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public int Enabled { get; set; }
    }
    public class MetaUpdateModel : MetaCreateModel
    {
        public string ID { get; set; }
    }
    public class MetaIDModel
    {
        public string ID { get; set; }
    }
    public class RsMeta : WEBModelResult
    {
        public string ID { get; set; }
        public string Alias { get; set; }
        public int GroupID { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeyword { get; set; }
        public RsMeta(string id, string alias, int groupId, string metaTitle, string metaDescription, string metaKeyword, string languageId, int enabled, string siteId, string createdBy, DateTime createdDate)
        {
            ID = id;
            Alias = alias;
            GroupID = groupId;
            MetaTitle = metaTitle;
            MetaDescription = metaDescription;
            MetaKeyword = metaKeyword;
            LanguageID = languageId;
            Enabled = enabled;
            SiteID = siteId;
            CreatedBy = createdBy;
            CreatedDate = Helper.Page.Library.FormatToDate(createdDate);
        }
    }
    public class MetaOption
    {
        public string ID { get; set; }
        public int GroupID { get; set; }
        public string MetaTitle { get; set; }
        public string Alias { get; set; }
    }
}