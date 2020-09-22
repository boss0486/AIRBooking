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
    [Table("App_ReportSaleSummaryDetails")]
    public partial class ReportSaleSummaryDetails : WEBModel
    {
        public ReportSaleSummaryDetails()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string DocumentNumber { get; set; }
        public string AssociatedDocument { get; set; }
        public string ReasonForIssuanceCode { get; set; }
        public string ReasonForIssuanceDesc { get; set; }
        public int CouponNumber { get; set; }
        public string TicketingProvider { get; set; }
        public int FlightNumber { get; set; }
        public string ClassOfService { get; set; }
        public DateTime DepartureDtm { get; set; }
        public string DecoupleItem { get; set; }
        public DateTime ArrivalDtm { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public string CouponStatus { get; set; }
        public string FareBasis { get; set; }
        public string BaggageAllowance { get; set; }
    }
    public class ReportSaleSummaryDetailsCreate
    {
        public string DocumentNumber { get; set; }
        public string AssociatedDocument { get; set; }
        public string ReasonForIssuanceCode { get; set; }
        public string ReasonForIssuanceDesc { get; set; }
        public int CouponNumber { get; set; }
        public string TicketingProvider { get; set; }
        public int FlightNumber { get; set; }
        public string ClassOfService { get; set; }
        public DateTime DepartureDtm { get; set; }
        public string DecoupleItem { get; set; }
        public DateTime ArrivalDtm { get; set; }
        public string DepartureCity { get; set; }
        public string ArrivalCity { get; set; }
        public string CouponStatus { get; set; }
        public string FareBasis { get; set; }
        public string BaggageAllowance { get; set; }
    }
}