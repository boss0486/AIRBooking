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
    [Table("SiteInfo")]
    public partial class SiteInfo : WEBModel
    {
        public SiteInfo()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string IconFile { get; set; }
        public string ImageFile { get; set; }
        public string Summary { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string Gmaps { get; set; }
        public string GoogleAnalytic { get; set; }
        // site
        [NotMapped]
        public string SiteName { get; set; }
        [NotMapped]
        public string Domain { get; set; }        
    }
    public class SiteInfoModel
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string IconFile { get; set; }
        public string ImageFile { get; set; }
        public string Summary { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string Gmaps { get; set; }
        public string GoogleAnalytic { get; set; }
        public int Enabled { get; set; }

    }
    public class RsSiteInfo : RsExtendCMSSite
    {
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string IconFile { get; set; }
        public string ImageFile { get; set; }
        public string Summary { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string Gmaps { get; set; }
        public string GoogleAnalytic { get; set; }
        public int Enabled { get; set; }

        public string SiteID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }

        public RsSiteInfo(string Id, string siteName, string domain, string parentId, string title, string alias, string iconFile, string imageFile, string summary, string email, string fax, string phone, string tel, string address, string gmap, string googleAnalytic, string languageId, int enabled, string siteId, string createdBy, string createdDate)
        {
            ID = Id;
            SiteName = siteName;
            Domain = domain;
            ParentID = parentId;
            Title = title;
            Alias = alias;
            IconFile = iconFile;
            ImageFile = imageFile;
            Summary = summary;
            Email = email;
            Fax = fax;
            Phone = phone;
            Tel = tel;
            Address = address;
            Gmaps = gmap;
            GoogleAnalytic = googleAnalytic;
            SiteID = siteId;
            Enabled = enabled;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }
    }

    public class RsExtendCMSSite
    {
        public string SiteName { get; set; }
        public string Domain { get; set; }

    }
    public class SiteInfoCreateFormModel
    {
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string IconFile { get; set; }
        public string ImageFile { get; set; }
        public string Summary { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string Gmaps { get; set; }
        public string GoogleAnalytic { get; set; }
        public string SiteID { get; set; }
        public int Enabled { get; set; }
        [Dapper.NotMapped]
        public HttpPostedFileBase DocumentFile { get; set; }
        [Dapper.NotMapped]
        public HttpPostedFileBase DocumentIconFile { get; set; }
    }

    public class SiteInfoUpdateFormModel : SiteInfoCreateFormModel
    {
        public string ID { get; set; }
    }
    public class SiteInfoOptionModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
    public class SiteInfoIDModel
    {
        public string ID { get; set; }
    }
    public class SiteInfoDeleteAndAtactmentModel
    {
        public string ID { get; set; }
        public string ImageFile { get; set; }
        public string IconFile { get; set; }
    }



    public partial class SiteInfoOptionListModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public SiteInfoOptionListModel(int Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }
}