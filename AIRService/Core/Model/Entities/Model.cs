using Dapper;
using Helper;
using Helper.Language;
using Helper.TimeData;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using WebCore.Model.Services;

namespace WebCore.Model.Entities
{
    public class WEBModel
    {
        public WEBModel()
        {
            LanguageID = Helper.Page.Default.LanguageID;
            SiteID = "";
            Enabled = 0;
            CreatedBy = Helper.Current.UserLogin.IdentifierID;
            CreatedDate = DateTime.Now;
        }
        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        public string SiteID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class WEBModelResult
    {
        public string LanguageID { get; set; }
        public int Enabled { get; set; }
        [NotMapped]
        public string EnabledText => ModelService.ViewEnableState(Enabled);
        public string SiteID { get; set; }
        private string _createdById = string.Empty;
        public string CreatedBy
        {
            get
            {
                return Helper.User.InFormation.GetInfCreateBy(_createdById);
            }
            set
            {
                _createdById = value;
            }
        }

        private string _createdDate;
        public string CreatedDate
        {
            get
            {
                if (_createdDate == null)
                    return "../" + "../" + "..";
                return TimeFormat.FormatToDate(Convert.ToDateTime(_createdDate), LanguagePage.GetLanguageCode);
            }
            set
            {
                _createdDate = value;
            }
        }
        [NotMapped]
        public string CreatedFullDate => TimeFormat.FormatToDateTime(Convert.ToDateTime(_createdDate), LanguagePage.GetLanguageCode);


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
        public int TimeExpress { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string TimeZoneLocal { get; set; }
        public string AreaID { get; set; }
    }
    public class SearchExpressOption
    {

        public int ID { get; set; }
        public string Title { get; set; }
        public SearchExpressOption(int id, string title)
        {
            ID = id;
            Title = title;
        }
    }
    public class SearchResult
    {
        public int Status { get; set; }
        public string Message { get; set; }

    }
    public class StatusModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public StatusModel(int id, string title)
        {
            ID = id;
            Title = title;
        }
    }
    // 
    public partial class OptionListModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public OptionListModel()
        {
            //
        }
        public OptionListModel(int Id, string title)
        {
            ID = Id;
            Title = title;
        }
    }
}
