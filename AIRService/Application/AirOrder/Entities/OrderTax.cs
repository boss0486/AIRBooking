using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_OrderTax")]
    public partial class OrderTax
    {
        public OrderTax()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string PassengerType { get; set; }
        public string Title { get; set; }
        public string TaxCode { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }
    // model
    public class OrderTaxCreateModel
    {
        public string PassengerType { get; set; }
        public string Title { get; set; }
        public string TaxCode { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }
    public class OrderTaxUpdateModel : OrderTaxCreateModel
    {
        public string ID { get; set; }
    }
    public class OrderTaxIDModel
    {
        public string ID { get; set; }
    }
}