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
    [Table("App_ReportTicketingDocument_Taxes")]
    public partial class ReportTicketingDocumentTaxes
    {
        public ReportTicketingDocumentTaxes()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public DateTime ReportDate { get; set; }
        public string DocumentNumber { get; set; }
        public string TaxCode { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }
}