﻿using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using System.Linq;
using System.Text;
using Dapper;
using System.Collections.Generic;
using WebCore.Model.Entities;

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

    public class ViewBanner : WEBModelResult
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