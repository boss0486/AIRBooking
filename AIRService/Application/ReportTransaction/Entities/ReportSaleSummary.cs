using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper.File;
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

    public partial class ReportSaleSummaryResult : WEBModelResult
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
        public ReportSaleSummaryFeeResult SaleSummaryFee { get; set; }

    }

    public partial class ReportSaleSummaryFeeResult
    {
        public string FareAmount { get; set; }
        public string TaxAmount { get; set; }
        public string TotalAmount { get; set; }

    }

}