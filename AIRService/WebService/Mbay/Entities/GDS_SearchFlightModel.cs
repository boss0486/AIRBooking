using AIRService.Models;
using AIRService.WebService.WSOTA_AirTaxRQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRService.WS.Mbay.Entities
{
    public class GDS_FlightSearchModel
    {
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime? ReturnDateTime { get; set; }
        public int ADT { get; set; }
        public int CNN { get; set; }
        public int INF { get; set; }
        public bool IsRoundTrip { get; set; }
    }
}