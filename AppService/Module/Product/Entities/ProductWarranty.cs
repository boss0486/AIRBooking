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
    [Table("App_ProductWarranty")]
    public partial class ProductWarranty : WEBModel
    {
        public ProductWarranty()
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
    public class ProductWarrantyCreateModel
    {
        public string Title { get; set; }   
        public string Summary { get; set; }
        public int Enabled { get; set; }
    }
    public class ProductWarrantyUpdateModel : ProductWarrantyCreateModel
    {
        public string ID { get; set; }
    }
    public class ProductWarrantyIDModel
    {
        public string ID { get; set; }
    }
    public class RsProductWarranty : RsModel
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }

        public RsProductWarranty(string id, string title, string summary, string alias, string languageId, int enabled, string siteId, string createdBy, DateTime createdDate)
        {
            ID = id;
            Title = title;
            Alias = alias;
            Summary = summary;
            LanguageID = languageId;
            Enabled = enabled;
            SiteID = siteId;
            CreatedBy = createdBy;
            CreatedDate = Helper.Library.FormatDate(createdDate);
        }
    }
    public class ProductWarrantyOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}