using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace AIRService.WS.Entities
{
    public class AirBookModel : TokenModel
    {
        public List<AirBookModelSegment> Segments { get; set; }
    }

    public class AirBookModelSegment
    {
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public string FlightNumber { get; set; }
        public string NumberInParty { get; set; }
        public string ResBookDesigCode { get; set; }
        public string AirEquipType { get; set; }
        public string DestinationLocation { get; set; }
        public string OriginLocation { get; set; }
    }
}