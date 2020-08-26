using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper.File;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebCore.Model.Entities;
namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_Service")]
    public partial class AppService : WEBModel
    {
        public AppService()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CategoryID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string TextID { get; set; }
        public string Summary { get; set; }
        public string HtmlNote { get; set; }
        public string HtmlText { get; set; }
        public string Tag { get; set; }
        public string ImageFile { get; set; }
        public double Price { get; set; }
        public double PriceListed { get; set; }
        public string PriceText { get; set; }
        public int ViewTotal { get; set; }
        public string ViewDate { get; set; }
    }
    // model
    public class AppServiceCreateModel
    {
        public string CategoryID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string TextID { get; set; }
        public string Summary { get; set; }
        [AllowHtml]
        public string HtmlNote { get; set; }
        [AllowHtml]
        public string HtmlText { get; set; }
        public string Tag { get; set; }
        public string ImageFile { get; set; }
        public double Price { get; set; }
        public double PriceListed { get; set; }
        public string PriceText { get; set; }
        public int ViewTotal { get; set; }
        public string ViewDate { get; set; }
        public int Enabled { get; set; }
        [NotMapped]
        public IList<string> Photos { get; set; }
    }
    public class AppServiceUpdateModel : AppServiceCreateModel
    {
        public string ID { get; set; }
    }
    public class AppServiceIDModel
    {
        public string ID { get; set; }
    }

    public class ViewAppService : WEBModel
    {
        public ViewAppService()
        {
            ImageFile = AttachmentFile.GetFile(ImageFile);
        }
        public ViewAppService(string id, string categoryID, string categoryName, string categoryAlias, string title, string alias, string textID, string imgFile, string summary, string htmlNote, string htmlText, double price, double priceListed, string priceText, string tag, int viewTotal, string viewDate, string languageID, int enabled, string createdBy, DateTime createdDate)
        {
            ID = id;
            CategoryID = categoryID;
            this.CategoryName = categoryName;
            this.CategoryAlias = categoryAlias;
            this.Title = title;
            this.Alias = alias;
            this.TextID = textID;
            this.ImageFile = AttachmentFile.GetFile(imgFile);
            this.Summary = summary;
            this.HtmlNote = htmlNote;
            this.HtmlText = htmlText;
            this.Price = price;
            this.PriceListed = priceListed;
            this.PriceText = priceText;
            this.Tag = tag;
            this.ViewTotal = viewTotal;
            this.ViewDate = viewDate;
            this.LanguageID = languageID;
            this.Enabled = enabled;
            this.CreatedBy = createdBy;
            this.CreatedDate = createdDate;
        }
        public string ID { get; set; }
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryAlias { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string TextID { get; set; }
        public string Summary { get; set; }
        public string HtmlNote { get; set; }
        public string HtmlText { get; set; }
        public string Tag { get; set; }
        public string ImageFile { get; set; }
        public string ImagePath
        {
            get => AttachmentFile.GetFile(ImageFile);
            set
            {
                ImageFile = AttachmentFile.GetFile(ImageFile);
            }
        }
        public double Price { get; set; }
        public double PriceListed { get; set; }
        public string PriceText { get; set; }
        public int ViewTotal { get; set; }
        public string ViewDate { get; set; }
        [NotMapped]
        public List<ViewAttachment> Photos { get; set; }
    }
    public class RsAppService
    {
        public string ID { get; set; }
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string TextID { get; set; }
        public string ImageFile { get; set; }
        public string Summary { get; set; }
        public string HtmlNote { get; set; }
        public string HtmlText { get; set; }
        public double Price { get; set; }
        public double PriceListed { get; set; }
        public string PriceText { get; set; }
        public string Tag { get; set; }
        public int ViewTotal { get; set; }
        public string ViewDate { get; set; }
        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        public string SiteID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public RsAppService(string Id, string categoryId, string categoryName, string title, string alias, string textId, string imageFile, string summary, string htmlNote, string htmlText, double price, double priceListed, string priceText, string tag, int viewTotal, string viewDate, string languageId, int enabled, string siteId, string createdBy, DateTime createdDate)
        {
            ID = Id;
            CategoryID = categoryId;
            CategoryName = categoryName;
            Title = title;
            Alias = alias;
            TextID = textId;
            ImageFile = imageFile;
            Summary = summary;
            HtmlNote = "";
            HtmlText = "";
            Price = price;
            PriceListed = priceListed;
            PriceText = priceText;
            Tag = tag;
            ViewTotal = viewTotal;
            ViewDate = viewDate;
            LanguageID = languageId;
            Enabled = enabled;
            SiteID = siteId;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }
    }
    public class AppServiceOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public double Price { get; set; }
        public double PriceListed { get; set; }
        public string PriceText { get; set; }
    }
    public class AppServiceStateModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public AppServiceStateModel(int Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }
    public class AppServiceSearchModel
    {
        public string Query { get; set; }
        public string CategoryID { get; set; }
        public int Status { get; set; }
        public int Page { get; set; }
    }
}