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
    [Table("Language")]
    public partial class Language
    {
        [Key]
        [IgnoreInsert]
        [IgnoreUpdate]
        public string ID { get; set; }
        [IgnoreInsert]
        [IgnoreUpdate]
        public string Title { get; set; }
        public string LangID { get; set; }
        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public partial class LanguageModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string LanguageID { get; set; }
        public string LangID { get; set; }
        public int Enabled { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public partial class RsLanguage
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string LanguageID { get; set; }
        public string LangID { get; set; }
        public string Enabled { get; set; }
        public string CreatedDate { get; set; }
        public RsLanguage(string id, string title, string languageID, string langID, string enabled, string createddate)
        {
            this.ID = id;
            this.Title = title;
            this.LanguageID = languageID;
            this.LangID = langID;
            this.Enabled = enabled;
            this.CreatedDate = createddate;

        }
    }

    public class LanguageOption
    {
        public  string LanguageID { get; set; }
        public  string Title { get; set; }
    }
}