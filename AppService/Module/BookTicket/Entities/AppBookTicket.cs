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
    [Table("App_BookTicket")]
    public partial class AppBookTicket : WEBModel
    {
        public AppBookTicket()
        {
            ID = Guid.NewGuid().ToString();

        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string PNR { get; set; }
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
        public double FareBase { get; set; }
        public double Amount { get; set; }
        public string ReturnID { get; set; }
    }
    // model
    public class AppBookTicketCreateModel
    {
        public string PNR { get; set; }
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
    public class AppBookTicketUpdateModel : AppBookTicketCreateModel
    {
        public string ID { get; set; }
    }
    public class AppBookTicketIDModel
    {
        public string ID { get; set; }
    }

    public class RequestAppBookTicketModel
    {

        public List<RequestAppBookPassengerModel> RequestAppBookPassengers { get; set; }
        public List<RequestAppBookFareModel> RequestAppBookFares { get; set; }
    }





    public class BookOrder
    {
        public BookOrderFlight BookOrderFlight { get; set; }

        public List<BookOrderPassenger> BookOrderPassengers { get; set; }
        public List<BookOrderFare> BookOrderFares { get; set; }
    }
    public class BookOrderFlight
    {
        public string PNR { get; set; }
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
        public double FareBase { get; set; }
        public string ReturnID { get; set; }
    }
    public class BookOrderPassenger
    {
        public string PassengerType { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    public class BookOrderFare
    {
        public string PassengerType { get; set; }
        public string Title { get; set; }
        public string TaxCode { get; set; }
        public double Amount { get; set; }
        public string Unit { get; set; }

    }














    public class Response_AppBookTicketDetailsModel
    {
        public string PNR { get; set; }
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
        public List<AppBookFare> BookFares { get; set; }
    }

    public class Response_AppBookPassengerDetailsModel
    {
        public string PassengerType { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    //public class Response_AppBookFareDetailsModel
    //{
    //    public string PassengerType { get; set; }
    //    public string Title { get; set; }
    //    public string TaxCode { get; set; }
    //    public double Amount { get; set; }
    //    public string Unit { get; set; }
    //}

    public class Response_AppBookDetailsModel
    {
        public List<Response_AppBookTicketDetailsModel> AppBookTicketDetails { get; set; }
        public List<Response_AppBookPassengerDetailsModel> AppBookPassengerDetails { get; set; }
    }
    public class GenderModel
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}