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
    [Table("App_OrderPrice")]
    public partial class OrderPrice  
    {
        public OrderPrice()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }

        public string TicketID { get; set; }
        public int FlightType { get; set; }
        public string PassengerType { get; set; }      
        public string Title { get; set; }
        public  double Amount { get; set; }
        public string Unit { get; set; }
    }
    // model
    public class OrderPriceCreateModel
    {
        public string TicketID { get; set; }
        public int FlightType { get; set; }
        public string PassengerType { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }
    public class OrderPriceUpdateModel : OrderPriceCreateModel
    {
        public string ID { get; set; }
    }
    public class OrderPriceIDModel
    {
        public string ID { get; set; }
    }
}