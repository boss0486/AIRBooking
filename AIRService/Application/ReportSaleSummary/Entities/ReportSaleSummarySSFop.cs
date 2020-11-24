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
    [Table("App_ReportSaleSummarySSFop")]
    public partial class ReportSaleSummarySSFop 
    {
        public ReportSaleSummarySSFop()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string ReportTransactionID { get; set; }
        public string FopCode { get; set; }
        public string CurrencyCode { get; set; }
        public double FareAmount { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }

    }
} 