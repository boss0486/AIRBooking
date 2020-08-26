using AIRService.Models;
using AIRService.WebService.WSOTA_AirTaxRQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class Request_FlightSearchModel
    {
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public string DepartureDateTime { get; set; }
        public string ReturnDateTime { get; set; }
        public int ADT { get; set; }
        public int CNN { get; set; }
        public int INF { get; set; }
        public bool IsRoundTrip { get; set; }
    }
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
        public int RPH { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int AirEquipType { get; set; }
        public string PassengerType { get; set; }
        public int FlightNumber { get; set; }
        public string FareBasisCode { get; set; }
        public string ResBookDesigCode { get; set; }
        public Resquet_WsTax_BaseFareModel BaseFare { get; set; }
    }

    public class Resquet_WsTaxModel
    {

        public int RPH { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int AirEquipType { get; set; }
        public string PassengerType { get; set; }
        public string ResBookDesigCode { get; set; }
        public int FlightNumber { get; set; }
        public Double TotalTravelTime { get; set; }

        public string FareBasisCode { get; set; }

        public Resquet_WsTax_BaseFareModel BaseFare { get; set; }
    }

    //
    //public class FlightFare_TaxBasicModel
    //{
    //    public List<FlightFare_PassengerTypeModel> TaxBasicModel { get; set; }
    //}
    //



    public class FeeTaxBasicModel
    {
        public int RPH { get; set; }
        public int AirEquipType { get; set; }
        public int FlightNumber { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public List<FlightFare_FareBaseModel> FareBase { get; set; }
        public string ResBookDesigCode { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class FlightFare_FareBaseModel
    {
        public int RPH { get; set; }
        public string PassengerType { get; set; }
        public float Amount { get; set; }
        public string FareBaseCode { get; set; }
    }

    //public class Resquet_WsTax_PassengerTypeModel : TokenModel
    //{

    //    public string RPH { get; set; }
    //    public string OriginLocation { get; set; }
    //    public string DestinationLocation { get; set; }
    //    public DateTime DepartureDateTime { get; set; }
    //    public DateTime? ReturnDateTime { get; set; }
    //    public DateTime ArrivalDateTime { get; set; }
    //    public string AirEquipType { get; set; }
    //    public string PassengerType { get; set; }
    //    public string ResBookDesigCode { get; set; }
    //    public short FlightNumber { get; set; }
    //    public Double TotalTravelTime { get; set; }

    //    public string FareBasisCode { get; set; }

    //    public Resquet_WsTax_BaseFareModel BaseFare { get; set; }
    //}

    public class Resquet_WsTax_BaseFareModel
    {
        
        public int RPH { get; set; }
        public float Amount { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class FlightTax
    {
        public int RPH { get; set; }
        public int FlightNumber { get; set; }
        public int AirEquipType { get; set; }
        public string ResBookDesigCode { get; set; }
        public List<FlightTaxInfo> FlightTaxInfos { get; set; }
    }

    public class FlightTaxInfo
    {
        public FarePassengerType PassengerType { get; set; }
        public Boolean RPHSpecified { get; set; }
        public Double Total { get; set; }
        public List<Taxes> Taxes { get; set; }
        public List<InteralFee> IFee { get; set; }
    }
    public class InteralFee
    {
        public string Text { get; set; }
        public string TaxCode { get; set; }
        public Double Amount { get; set; }
    }

    /// ##############################################################################################################
    /// For view 
    public class FlightPassengerTypeInfo
    {
        public string PassengerType { get; set; }
        public string PassengerName { get; set; }
        public int Quantity { get; set; }
    }




    /// ##############################################################################################################
    public class ItineraryInfo
    {
        public string RPH { get; set; }
        public PTC_FareBreakdown PTC_FareBreakdown { get; set; }
        public TaxInfo TaxInfo { get; set; }
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
        public FarePassengerType PassengerType { get; set; }
    }
    public class FarePassengerType
    {
        public string Quantity { get; set; }
        public string Code { get; set; }
        public string Age { get; set; }
        public string AgeSpecified { get; set; }
        public string Total { get; set; }
    }
}