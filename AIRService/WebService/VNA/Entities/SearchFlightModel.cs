using AIRService.Models;
using AIRService.WebService.WSOTA_AirTaxRQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class FlightSearchModel
    {
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ReturnDateTime { get; set; }
        public int ADT { get; set; }
        public int CNN { get; set; }
        public int INF { get; set; }
        public bool IsRoundTrip { get; set; }
    }

    public class FlightFareModel
    {
        public string RPH { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime? ReturnDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string AirEquipType { get; set; }
        public string PassengerType { get; set; }
        public string FareBasisCode { get; set; }
        public string ResBookDesigCode { get; set; }
        public Resquet_WsTax_BaseFareModel BaseFare { get; set; }
    }

    public class Resquet_WsTaxModel : TokenModel
    {

        public string RPH { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime? ReturnDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string AirEquipType { get; set; }
        public string PassengerType { get; set; }
        public string ResBookDesigCode { get; set; }
        public short FlightNumber { get; set; }
        public Double TotalTravelTime { get; set; }

        public string FareBasisCode { get; set; }

        public Resquet_WsTax_BaseFareModel BaseFare { get; set; }
    }

    public class Resquet_WsTax_BaseFareModel
    {
        public float Amount { get; set; }
        public string CurrencyCode { get; set; }
    }



    public class Response_FlightFee
    {
        public string RPH { get; set; }
        public PassengerType PassengerType { get; set; }
        public Boolean RPHSpecified { get; set; }
        public Double Total { get; set; }
        public List<Taxes> Taxes { get; set; }
        public List<InteralFee> IFee { get; set; }
    }

    public class Response_FlightTaxInfo
    {
        public string Text { get; set; }
        public string TaxCode { get; set; }
        public Double Amount { get; set; }
    }
    public class InteralFee
    {
        public string Text { get; set; }
        public string TaxCode { get; set; }
        public Double Amount { get; set; }
    }

    /// ##############################################################################################################
    public class ItineraryInfo
    {
        public PTC_FareBreakdown PTC_FareBreakdown { get; set; }
        public TaxInfo TaxInfo { get; set; }
        public string RPH { get; set; }     
        public bool RPHSpecified { get; set; }
    }

    public class FeeDetailsModel
    {
        public List<ItineraryInfo> ItineraryInfo { get; set; }
    }

    public class TaxInfo
    {
        public List<Taxes> Taxes { get; set; }
        public object TaxDetails { get; set; }
        public string RPH { get; set; }
        public Double Total { get; set; }

    }
    public class Taxes
    {
        public string Text { get; set; }
        public string TaxCode { get; set; }
        public Double Amount { get; set; }
    }

    public class PTC_FareBreakdown
    {
        public PassengerType PassengerType { get; set; }
    }
    public class PassengerType
    {
        public string Quantity { get; set; }
        public string Code { get; set; }
        public string Age { get; set; }
        public string AgeSpecified { get; set; }
        public string Total { get; set; }
    }
}