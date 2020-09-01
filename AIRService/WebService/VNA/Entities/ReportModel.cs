﻿using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models.VNA_WS_Model
{
    public class ReportModel : TokenModel
    {
        public DateTime ReportDate { get; set; }
        public string EmpNumber { get; set; }
    }
    public class EmpReportModel : TokenModel
    {
        public DateTime ReportDate { get; set; }
    }

    public class ReportDetailsModel : TokenModel
    {
        public DateTime ReportDate { get; set; }
        public string DocumentNumber { get; set; }
    }

    public class ReportSaleSummaryTransaction
    {
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
        public ReportSaleSummaryTransactionSSFop SaleSummaryTransactionSSFop { get; set; }
    }

    public class ReportSaleSummaryTransactionSSFop
    {
        public string FopCode { get; set; }
        public string CurrencyCode { get; set; }
        public double FareAmount { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }
    }
    public class ReportSummaryDetails
    {
        public string DocumentNumber { get; set; }
        public string AssociatedDocument { get; set; }
        public string ReasonForIssuanceCode { get; set; }
        public string ReasonForIssuanceDesc { get; set; }
        public int CouponNumber { get; set; }
        public string TicketingProvider { get; set; }
        public int FlightNumber { get; set; }
        public string ClassOfService { get; set; }
        public string DepartureDtm { get; set; }
        public string ArrivalDtm { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public string CouponStatus { get; set; }
        public string FareBasis { get; set; }
        public string BaggageAllowance { get; set; }
    }
    //
    public class ReportSaleSummaryResult
    {
        public string EmpNumber { get; set; }
        public bool Status { get; set; }
        public List<ReportSaleSummaryTransaction> SaleSummaryTransaction { get; set; }
    }
    public class ReportSaleSummaryDetailsResult
    {
        public string DocumentNumber { get; set; }
        public bool Status { get; set; }
        public ReportSummaryDetails SaleSummaryDetails { get; set; }
    }
    public class ReportEprResult
    {
        public ReportSaleSummaryResult SaleSummary { get; set; }
        public List<ReportSaleSummaryDetailsResult> SaleSummaryDetails { get; set; }
    }
}