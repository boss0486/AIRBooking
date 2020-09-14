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
    [Table("App_ServicePack")]
    public partial class ServicePack : WEBModel
    {
        public ServicePack()
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
    public class ServicePackCreateModel
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
    public class ServicePackUpdateModel : ServicePackCreateModel
    {
        public string ID { get; set; }
    }
    public class ServicePackIDModel
    {
        public string ID { get; set; }
    }

    public class ServicePackResult : WEBModelResult
    {
        public ServicePackResult()
        {
            ImageFile = AttachmentFile.GetFile(ImageFile);
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
    public class ServicePackOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public double Price { get; set; }
        public double PriceListed { get; set; }
        public string PriceText { get; set; }
    }
    public class ServicePackStateModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public ServicePackStateModel(int Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }
    public class ServicePackSearchModel: SearchModel
    {
        public string CategoryID { get; set; }
        public int Status { get; set; }
    }
}