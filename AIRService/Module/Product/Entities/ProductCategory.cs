using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;

namespace WebCore.Entities
{ 
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_ProductCategory")]
    public partial class ProductCategory : WEBModel
    {
        public ProductCategory()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
    }

    // model
    public class ProductCategoryCreateModel
    {
        public string Title { get; set; }   
        public string Summary { get; set; }
        public int Enabled { get; set; }
    }
    public class ProductCategoryUpdateModel : ProductCategoryCreateModel
    {
        public string ID { get; set; }
    }
    public class ProductCategoryIDModel
    {
        public string ID { get; set; }
    }
    public class RsProductCategory : WEBModelResult
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }

        public RsProductCategory(string id, string title, string summary, string alias, string languageId, int enabled, string siteId, string createdBy, DateTime createdDate)
        {
            ID = id;
            Title = title;
            Alias = alias;
            Summary = summary;
            LanguageID = languageId;
            Enabled = enabled;
            SiteID = siteId;
            CreatedBy = createdBy;
            CreatedDate = Helper.Page.Library.FormatToDate(createdDate);
        }
    }
    public class ProductCategoryOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}