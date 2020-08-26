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
    [Table("SocialNetwork")]
    public partial class SocialNetwork : WEBModel
    {
        public SocialNetwork()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string BackLink { get; set; }
        public string IconFile { get; set; }   
    }
    public class RsSocialNetwork
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string BackLink { get; set; }
        public string IconFile { get; set; }
        public int Enabled { get; set; }
        public string SiteID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }

        public RsSocialNetwork(string Id, string title, string alias,string backLink, string iconFile,  string siteId,int enabled, string createdBy, string createdDate)
        {
            ID = Id;
            Title = title;
            Alias = alias;
            BackLink = backLink;
            IconFile = iconFile;
            SiteID = siteId;
            Enabled = enabled;
            CreatedBy = createdBy;
            CreatedDate = createdDate;
        }
    }

    public class SocialNetworkCreateFormModel
    {
        public string Title { get; set; }
        public string BackLink { get; set; }
        public string IconFile { get; set; }
        public int Enabled { get; set; }
    }

    public class SocialNetworkUpdateFormModel : SocialNetworkCreateFormModel
    {
        public string ID { get; set; }
    }
     
    public class SocialNetworkIDModel
    {
        public string ID { get; set; }
    }   
    //public partial class SocialNetworkOptionListModel
    //{
    //    public int ID { get; set; }
    //    public string Title { get; set; }
    //    public SocialNetworkOptionListModel(int Id, string title)
    //    {
    //        ID = Id;
    //        Title = title;
    //    }
    //}
}