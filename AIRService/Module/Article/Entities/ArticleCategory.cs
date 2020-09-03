﻿using AL.NetFrame.Attributes;
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
    [Table("App_ArticleCategory")]
    public partial class ArticleCategory : WEBModel
    {
        public ArticleCategory()
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
    public class ArticleCategoryCreateModel
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }
    }
    public class ArticleCategoryUpdateModel : ArticleCategoryCreateModel
    {
        public string ID { get; set; }
    }
    public class ArticleCategoryIDModel
    {
        public string ID { get; set; }
    }

    public class ArticleCategoryResult : WEBModelResult
    {

        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
    }
    public class ArticleCategoryOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}