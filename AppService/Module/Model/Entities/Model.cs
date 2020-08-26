using Dapper;
using Helper;
using System;
namespace WebCore.Model.Entities
{
    public class WEBModel
    {
        public WEBModel()
        {
            LanguageID = Current.LanguageID;
            SiteID = "";
            Enabled = 0;
            CreatedBy = Current.IdentifierID;
            CreatedDate = DateTime.Now;
        }
        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        public string SiteID { get; set; }
        public string CreatedBy { get; set; }
        [IgnoreUpdate]
        public DateTime CreatedDate { get; set; }
    }

    public class RsModel
    {
        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        public string SiteID { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
    }


    public class SiteIDModel
    {
        public string ID { get; set; }
    }

    public class SearchModel
    {
        public string Query { get; set; }
        public int Page { get; set; }
        public int Status { get; set; }
    }
    public class SearchExpressOption
    {

        public int ID { get; set; }
        public string Title { get; set; }
        public SearchExpressOption(int Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }

    public class StatusModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public StatusModel(int Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }
    // 
    public partial class OptionListModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public OptionListModel(int Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }
    // 
}
