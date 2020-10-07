using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class FlightSearch
    {
        public long TimeHanndle { get; set; }
        public int ADT { get; set; }
        public int CNN { get; set; }
        public int INF { get; set; }
        public List<FlightSegment> FlightSegment { get; set; }

    }
    public class FlightSegment
    {
        public int FlightNo { get; set; }
        public string AirEquipType { get; set; }
        public string DestinationLocation { get; set; }
        public string OriginLocation { get; set; }
        public string ArrivalDateTime { get; set; }
        public string DepartureDateTime { get; set; }

        public int FlightType { get; set; }
        public int NumberInParty { get; set; }
        public List<FlightSegment_ResBookDesigCode> FareDetails { get; set; }
        public string RPH { get; set; }
    }

    public class FlightSegment_ResBookDesigCode
    {
        public int ResBookDesigCodeID { get; set; }
        public string ResBookDesigCode { get; set; }
        public List<FareItem> FareItem { get; set; }
    }



    public class Response_FlightSearch
    {
        public string AirEquipType { get; set; }
        public string ArrivalDateTime { get; set; }
        public string DepartureDateTime { get; set; }
        public string DestinationLocation { get; set; }
        public int FlightNo { get; set; }
        public int FlightType { get; set; }
        public int NumberInParty { get; set; }
        public string OriginLocation { get; set; }
        public string RPH { get; set; }
        public List<FlightSegment_ResBookDesigCode> FareDetails { get; set; }
    }



    public class FareDetailsModel1
    {
        public string RPH { get; set; }
        public string ResBookDesigCode { get; set; }
        public string PassengerType { get; set; }
        public double FareAmount { get; set; }
        public string Code { get; set; }
    }

    public class FightGroupByTimeModel
    {
        public string ArrivalDateTime { get; set; }
        public string DepartureDateTime { get; set; }
    }
}
