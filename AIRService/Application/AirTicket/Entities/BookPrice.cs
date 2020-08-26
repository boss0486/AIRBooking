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
    [Table("App_BookPrice")]
    public partial class BookPrice  
    {
        public BookPrice()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string PNR { get; set; }
        public string TicketID { get; set; }
        public int FlightType { get; set; }
        public string PassengerType { get; set; }      
        public string Title { get; set; }
        public  double Amount { get; set; }
        public string Unit { get; set; }
    }
    // model
    public class BookPriceCreateModel
    {
        public string PNR { get; set; }
        public string TicketID { get; set; }
        public string PassengerType { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }
    public class BookPriceUpdateModel : BookPriceCreateModel
    {
        public string ID { get; set; }
    }
    public class BookPriceIDModel
    {
        public string ID { get; set; }
    }
}