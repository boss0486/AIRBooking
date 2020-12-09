using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using ApiPortalBooking.Models.VNA_WS_Model;
using Dapper;
using Helper.File;
using Helper.Language;
using Helper.TimeData;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebCore.Model.Entities;
namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_ReportSaleSummary")]
    public partial class ReportSaleSummary : WEBModel
    {
        public ReportSaleSummary()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string EmployeeNumber { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string PassengerName { get; set; }
        public string PnrLocator { get; set; }
        public string TicketPrinterLniata { get; set; }
        public string TransactionTime { get; set; }
        public bool ExceptionItem { get; set; }
        public bool DecoupleItem { get; set; }
        public string TicketStatusCode { get; set; }
        public bool IsElectronicTicket { get; set; }
        public DateTime ReportDate { get; set; }
    }

    public partial class ReportSaleSummaryResult : ReportSaleSummaryFeeResult
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string EmployeeNumber { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string PassengerName { get; set; }
        public string PnrLocator { get; set; }
        public string TicketPrinterLniata { get; set; }
        public string TransactionTime { get; set; }
        public bool ExceptionItem { get; set; }
        public bool DecoupleItem { get; set; }
        public string TicketStatusCode { get; set; }
        public bool IsElectronicTicket { get; set; } 
        public DateTime ReportDate { get; set; }
        [NotMapped]
        public string ReportDateText => TimeFormat.FormatToViewDate(Convert.ToDateTime(ReportDate), Helper.Language.LanguageCode.Vietnamese.ID);

    }

    public partial class ReportSaleSummaryFeeResult : WEBModelResult
    {
        public double FareAmount { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }

    }
    public class ReportSaleSummaryModel
    {
        public ReportSaleSummary ReportSaleSummary { get; set; }
        public List<ReportTicketingDocumentCoupon> TicketingDocumentCoupons { get; set; }
        public ReportTicketingDocumentAmount TicketingDocumentAmount { get; set; }
        public List<ReportTicketingDocumentTaxes> TicketingDocumentTaxes { get; set; }
        public List<VNA_ReportSaleSummaryTicketingDocument> TicketingDocumentStatus { get; set; }
    }

    //
    public class ReportEprSearchModel : SearchModel
    {
        public string ReportDate { get; set; }
        public string CurrentStatus { get; set; }
    }
    // search report


    public partial class AirPassengerReult : WEBModelResult
    {
        public string ID { get; set; }
        public string EmployeeNumber { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public string PassengerName { get; set; }
        public string PnrLocator { get; set; }
        public string TicketPrinterLniata { get; set; }
        public string TransactionTime { get; set; }
        public bool ExceptionItem { get; set; }
        public bool DecoupleItem { get; set; }
        public string TicketStatusCode { get; set; }
        public bool IsElectronicTicket { get; set; }
        public DateTime ReportDate { get; set; }
        public string ReportDateText => TimeFormat.FormatToViewDate(Convert.ToDateTime(ReportDate), Helper.Language.LanguageCode.Vietnamese.ID);
        public string ReportSaleSummaryID { get; set; }
        public string MarketingFlightNumber { get; set; }
        public string ClassOfService { get; set; }
        public string FareBasis { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime StartDateTime { get; set; }
        public string EndDateTime { get; set; }
        [NotMapped]
        public string StartDateTimeText
        {
            get
            {
                return TimeFormat.FormatToViewDateTime(StartDateTime, Helper.Language.LanguageCode.Vietnamese.ID);
            }
        }
        [NotMapped]
        public string EndDateTimeText
        {
            get
            {
                return TimeFormat.FormatToViewDateTime(EndDateTime, Helper.Language.LanguageCode.Vietnamese.ID);
            }
        }
        public string BookingStatus { get; set; }
        public string CurrentStatus { get; set; }
    }
}