using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;

namespace WebCore.Entities
{ 
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("MenuCategory")]
    public partial class MenuCategory : WEBModel
    {
        public MenuCategory()
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
    public class MenuCategoryCreateModel
    {
        public string Title { get; set; }   
        public string Summary { get; set; }
        public int Enabled { get; set; }
    }
    public class MenuCategoryUpdateModel : MenuCategoryCreateModel
    {
        public string ID { get; set; }
    }
    public class MenuCategoryIDModel
    {
        public string ID { get; set; }
    }
    public class MenuCategoryResult : WEBModelResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; } 
    }
    public class MenuCategoryOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}