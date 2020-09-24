﻿using AL.NetFrame.Attributes;
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
        private string _createdDate;
        public string ReportDate
        {
            get
            {
                if (_createdDate == null)
                    return "../" + "../" + "..";
                return Helper.Time.TimeHelper.FormatToDate(Convert.ToDateTime(_createdDate), Helper.Language.LanguageCode.Vietnamese.ID);
            }
            set
            {
                _createdDate = value;
            }
        }

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
    }
}