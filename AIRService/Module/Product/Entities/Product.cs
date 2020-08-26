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
    [Table("App_Product")]
    public partial class Product : WEBModel
    {
        public Product()
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
        public string Originate { get; set; }
        public string MadeIn { get; set; }
        public string Warranty { get; set; }
        public int State { get; set; }
        public int ViewTotal { get; set; }
        public string ViewDate { get; set; }
    }
    // model
    public class ProductCreateModel
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
        public string Originate { get; set; }
        public string MadeIn { get; set; }
        public string Warranty { get; set; }
        public int State { get; set; }
        public int ViewTotal { get; set; }
        public string ViewDate { get; set; }
        public int Enabled { get; set; }
        [NotMapped]
        public IList<string> Photos { get; set; }
    }
    public class ProductUpdateModel : ProductCreateModel
    {
        public string ID { get; set; }
    }
    public class ProductIDModel
    {
        public string ID { get; set; }
    }

    public class ViewProduct : WEBModel
    {
        public ViewProduct()
        {
            ImageFile = AttachmentFile.GetFile(ImageFile);

        }
        public ViewProduct(string id, string categoryID, string categoryName, string categoryAlias, string title, string alias, string textID, string imgFile, string summary, string htmlNote, string htmlText, double price, double priceListed, string priceText, string originate, string madeIn, string warranty, int state, string tag, int viewTotal, string viewDate, string languageID, int enabled, string siteID, string createdBy, DateTime createdDate)
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
            this.Originate = originate;
            this.MadeIn = madeIn;
            this.Warranty = warranty;
            this.State = state;
            this.Tag = tag;
            this.ViewTotal = viewTotal;
            this.ViewDate = viewDate;
            this.LanguageID = languageID;
            this.Enabled = enabled;
            this.SiteID = siteID;
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
        public string ImageFile{get; set;}
        public string ImagePath {
            get => AttachmentFile.GetFile(ImageFile);
            set
            {
                ImageFile = AttachmentFile.GetFile(ImageFile);
            }
        }
        public double Price { get; set; }
        public double PriceListed { get; set; }
        public string PriceText { get; set; }
        public string Originate { get; set; }
        public string MadeIn { get; set; }
        public string Warranty { get; set; }
        public int State { get; set; }
        public int ViewTotal { get; set; }
        public string ViewDate { get; set; }
        [NotMapped]
        public List<ViewAttachment> Photos { get; set; }

    }
    public class RsProduct
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
        public string Originate { get; set; }
        public string MadeIn { get; set; }
        public string Warranty { get; set; }
        public int State { get; set; }
        public string Tag { get; set; }
        public int ViewTotal { get; set; }
        public string ViewDate { get; set; }
        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        public string SiteID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public RsProduct(string Id, string categoryId, string categoryName, string title, string alias, string textId, string imageFile, string summary, string htmlNote, string htmlText, double price, double priceListed, string priceText, string originate, string madeIn, string warranty, int state, string tag, int viewTotal, string viewDate, string languageId, int enabled, string siteId, string createdBy, DateTime createdDate)
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
            Originate = originate;
            MadeIn = madeIn;
            Warranty = warranty;
            State = state;
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
    public class ProductOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public double Price { get; set; }
        public double PriceListed { get; set; }
        public string PriceText { get; set; }
        public string Originate { get; set; }
        public string MadeIn { get; set; }
        public string Warranty { get; set; }
        public int State { get; set; }
    }

    public class ProductStateModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public ProductStateModel(int Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }
    
    public class ProductSearchModel
    {
        public string Query { get; set; }
        public string CategoryID { get; set; }
        public int State { get; set; }
        public int Status { get; set; }
        public int Page { get; set; }
    }
}