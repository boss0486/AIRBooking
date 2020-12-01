using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_National")]
    public partial class National : WEBModel
    {
        public National()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CodeID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
    }

    // model
    public class NationalCreateModel
    {
        public string CodeID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }
    }
    public class NationalUpdateModel : NationalCreateModel
    {
        public string ID { get; set; }
    }
    public class NationalIDModel
    {
        public string ID { get; set; }
    }
    public class NationalResult : WEBModelResult
    {
        public string ID { get; set; }
        public string CodeID { get; set; }  
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Alias { get; set; }
    }
    public class NationalOptionModel
    {
        public string ID { get; set; }
        public string CodeID { get; set; } 
        public string Title { get; set; }
        public string Alias { get; set; }
    }
}