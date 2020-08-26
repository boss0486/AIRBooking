using AIRService.Models;
using AIRService.WS.Entities;
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
    [Table("App_OrderTicket")]
    public partial class OrderTicket
    {
        public OrderTicket()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CategoryID { get; set; }
        public string Summary { get; set; }
        public int ADT { get; set; }
        public int CNN { get; set; }
        public int INF { get; set; }
        public int Direction { get; set; }
        public int NumberInParty { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string ResBookDesigCode { get; set; }
        public int FlightNumber { get; set; }
        public int AirEquipType { get; set; }
        public string ReturnID { get; set; }
    }
    // model
    public class OrderTicketCreateModel
    {
        public string CategoryID { get; set; }
        public string Summary { get; set; }
        public int ADT { get; set; }
        public int CNN { get; set; }
        public int INF { get; set; }
        public int Direction { get; set; }
        public int NumberInParty { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string ResBookDesigCode { get; set; }
        public int FlightNumber { get; set; }
        public int AirEquipType { get; set; }
        public double Amount { get; set; }
        public string ReturnID { get; set; }
        public int Enabled { get; set; }
    }
    public class OrderTicketUpdateModel : OrderTicketCreateModel
    {
        public string ID { get; set; }
    }
    public class OrderTicketIDModel
    {
        public string ID { get; set; }
    }

    public class Request_OrderTicketModel
    {
        public List<RequestOrderFlightModel> Flights { get; set; }
        public List<RequestOrderTaxModel> TaxFare { get; set; }
        public List<RequestOrderPriceModel> PriceFare { get; set; }
    }

    public class RequestOrderFlightModel
    {
        public int ADT { get; set; }
        public int CNN { get; set; }
        public int INF { get; set; }
        public int NumberInParty { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string ResBookDesigCode { get; set; }
        public int FlightNumber { get; set; }
        public int AirEquipType { get; set; }
    }

    public class RequestOrderPriceModel
    {
        public int FlightType { get; set; }
        public string PassengerType { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }

    public class RequestOrderTaxModel
    {
        public string PassengerType { get; set; }
        public string Title { get; set; }
        public string TaxCode { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }
    }
}