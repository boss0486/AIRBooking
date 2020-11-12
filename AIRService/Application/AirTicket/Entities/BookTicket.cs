using AIRService.Models;
using AIRService.WS.Entities;
using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using ApiPortalBooking.Models.VNA_WS_Model;
using Dapper;
using System;
using System.Collections.Generic;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Table("App_BookTicket")]
    public partial class BookTicket : WEBModel
    {
        public BookTicket()
        {
            ID = Guid.NewGuid().ToString();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string BookOrderID { get; set; }
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
    public class BookTicketCreateModel
    {
        public string CategoryID { get; set; }
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
    public class BookTicketUpdateModel : BookTicketCreateModel
    {
        public string ID { get; set; }
    }
    public class BookTicketIDModel
    {
        public string ID { get; set; }
    }

    public class RequestBookTicketModel
    {

        public List<RequestBookPassengerModel> RequestAppBookPassengers { get; set; }
        public List<RequestBookTaxModel> RequestAppBookFares { get; set; }
    }

    public class BookTicketOrder
    {
        public string PNR { get; set; }
        public string Summary { get; set; }
        public int PassengerGroup { get; set; }
        public int ItineraryType { get; set; }
        public DateTime OrderDate { get; set; }
        public BookTicketingInfo TiketingInfo { get; set; }
        public BookTicketOrderContact Contacts { get; set; }
        public List<BookSegmentModel> Flights { get; set; }
        public List<BookTicketPassenger> Passengers { get; set; }
        public List<FareTax> FareTaxs { get; set; }
        public List<FareFlight> FareFlights { get; set; } 
    }
    public class BookTicketingInfo
    {
        public int PassengerGroup { get; set; }
        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public string TiketingID { get; set; }
        public string TiketingName { get; set; }
    }
    public class BookTicketOrderContact
    {
        public BookKhachLeRqContact BookKhachLeContact { get; set; }
        public BookCompanyContactModel BookCompanyContact { get; set; }
    }
    public class BookCompanyContactModel
    {
        public string CompanyID { get; set; }
        public string CompanyCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class BookTicketPassenger
    {
        public string PassengerType { get; set; }
        public string FullName { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
    public class BookTicketingRq
    {
        public string ProviderID { get; set; }
        public string TiketingID { get; set; }
    }



    public class BookContactRqModel
    {
        public BookKhachLeRqContact BookKhachLeContact { get; set; }
        public BookCompanyRqContact BookCompanyContact { get; set; }
    }
    public class BookKhachLeRqContact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class BookCompanyRqContact
    {
        public string CompanyID { get; set; }
    }


    public class BookTicketDetails
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
        public List<BookFareDetails> BookFares { get; set; }
    }

    public class BookPassengerDetails
    {
        public string PassengerType { get; set; }
        public string FullName { get; set; }
        public int Gender { get; set; }
        public string DateOfBirth { get; set; }
    }
    public class BookFareDetails
    {
        public string PassengerType { get; set; }
        public int PassengerQty { get; set; }
        public double PriceTotal { get; set; }
        public double TaxTotal { get; set; }
        public List<BookTax> FareTaxs { get; set; }
    }
    public class BookContacDetails
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class BookTicketOrderDetails
    {

        public List<BookTicketDetails> BookTickets { get; set; }
        public List<BookPassengerDetails> BookPassengers { get; set; }
        public List<BookFareDetails> BookFares { get; set; }
        public List<BookContacDetails> Contacts { get; set; }
    }

    public class GenderModel
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
    //##
    public class Response_PassengerDetailsModel
    {
        public string PNR { get; set; }
    }
    public class PNRModel
    {
        public string PNR { get; set; }
    }

    public class ExTitketPassengerFareModel
    {
        public string FullName { get; set; }
        public string DateOfBirth { get; set; }
        public string PassengerType { get; set; }
        public double PriceTotal { get; set; }
        public double TaxTotal { get; set; }
    }

    // 
    public class Request_BookModel
    {
        public string Summary { get; set; }
        public int ItineraryType { get; set; }
        public int PassengerGroup { get; set; }
        public BookTicketingRq TicketingInfo { get; set; }
        public List<BookTicketPassenger> Passengers { get; set; }
        public List<BookSegmentModel> Flights { get; set; }
        public BookContactRqModel Contacts { get; set; }
        public string TimeZoneLocal { get; set; } 
    }

}