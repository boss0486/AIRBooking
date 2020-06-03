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
        public string AirEquipType { get; set; }
        public string ArrivalDateTime { get; set; }
        public string DepartureDateTime { get; set; }
        public string DestinationLocation { get; set; }
        public int FlightNo { get; set; }
        public int FlightType { get; set; }
        public int NumberInParty { get; set; }
        public string OriginLocation { get; set; }
        public List<FareDetailsModel> PriceDetails { get; set; }
        public string ResBookDesigCode { get; set; }
        public string RPH { get; set; }
    }

}