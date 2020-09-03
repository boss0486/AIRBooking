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
    [Table("App_Article")]
    public partial class Article : WEBModel
    {
        public Article()
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
        public string ViewTotal { get; set; }
        public string ViewDate { get; set; }
        [NotMapped]
        public int DocumentFile { get; set; }
    }
    // model
    public class ArticleCreateModel
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
        public string ViewTotal { get; set; }
        public string ViewDate { get; set; }
        public int Enabled { get; set; }
        [NotMapped]
        public HttpPostedFileBase DocumentFile { get; set; }
        [NotMapped]
        public IList<string> Photos { get; set; }
        [NotMapped]
        public IList<string> EditorFile { get; set; }
    }
    public class ArticleUpdateModel : ArticleCreateModel
    {
        public string ID { get; set; }
    }
    public class ArticleIDModel
    {
        public string ID { get; set; }
    }

    public class ViewArticle : WEBModel
    {
        public ViewArticle()
        {
            ImageFile = AttachmentFile.GetFile(ImageFile);

        }
        public ViewArticle(string id, string categoryID, string categoryName, string categoryAlias, string title, string alias, string textID, string imgFile, string summary, string htmlNote, string htmlText, string tag, int viewTotal, string viewDate, string languageID, int enabled, string siteID, string createdBy, DateTime createdDate)
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
        public string ImageFile
        {
            get; set;
        }
        public string ImagePath
        {
            get => AttachmentFile.GetFile(ImageFile);
            set
            {
                ImageFile = AttachmentFile.GetFile(ImageFile);
            }
        }
        public int ViewTotal { get; set; }
        public string ViewDate { get; set; }

        [NotMapped]
        public List<ViewAttachment> Photos { get; set; }

    }
    public class ArticleResult : WEBModelResult
    {

        public string ID { get; set; }
        public string CategoryID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string TextID { get; set; }
        public string ImageFile { get; set; }
        public string Summary { get; set; }
        public string HtmlNote { get; set; }
        public string HtmlText { get; set; }
        public string Tag { get; set; }
        public string ViewTotal { get; set; }
        public string ViewDate { get; set; }

         
    }
    public class ArticleOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }

    public class ArticleStatusModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public ArticleStatusModel(int Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }
    public class ArticleSearchModel
    {
        public string Query { get; set; }
        public string CategoryID { get; set; }
        public int State { get; set; }
        public int Status { get; set; }
        public int Page { get; set; }
    }
}