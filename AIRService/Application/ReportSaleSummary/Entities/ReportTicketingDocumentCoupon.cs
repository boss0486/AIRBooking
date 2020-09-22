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
    [Table("App_ReportTicketingDocument_Coupon")]
    public partial class ReportTicketingDocumentCoupon : WEBModel
    {
        public ReportTicketingDocumentCoupon()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string MarketingFlightNumber { get; set; }
        public string ClassOfService { get; set; }
        public string FareBasis { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string BookingStatus { get; set; }
        public string CurrentStatus { get; set; }
        public string SystemDateTime { get; set; }
        public string FlownCoupon_DepartureDateTime { get; set; }
    }
}