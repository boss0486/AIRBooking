using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebCore.Model.Entities;
using Dapper;
using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;

namespace WebCore.Entities
{

    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_Banner")]
    public partial class Banner : WEBModel
    {
        public Banner()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public int LocationID { get; set; }
        public string ImageFile { get; set; }
        public string BackLink { get; set; }
    }

    // model
    public class BannerCreateModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }
        public int LocationID { get; set; }
        public string ImageFile { get; set; }
        public string BackLink { get; set; }
        public IList<string> Photos { get; set; }

    }
    public class BannerUpdateModel : BannerCreateModel
    {
        public string ID { get; set; }
    }
    public class BannerIDModel
    {
        public string ID { get; set; }
    }

    public class ViewBanner : RsModel
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
        public int LocationID { get; set; }
        public string ImageFile { get; set; }
        public string BackLink { get; set; }

        [NotMapped]
        public List<ViewAttachment> Photos { get; set; }
        public ViewBanner(string id, string title, string summary, string alias, int locactionId, string imageFile, string backLink, string languageId, int enabled, string siteId, string createdBy, DateTime createdDate)
        {
            ID = id;
            Title = title;
            Alias = alias;
            Summary = summary;
            LocationID = locactionId;
            ImageFile = imageFile;
            BackLink = backLink;
            LanguageID = languageId;
            Enabled = enabled;
            SiteID = siteId;
            CreatedBy = createdBy;
            CreatedDate = Helper.Library.FormatDate(createdDate);
        }

        public ViewBanner()
        {
        }
    }
    public class BannerOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string LocaionID { get; set; }
    }

    public class BannerLocationModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public BannerLocationModel(int Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }
}