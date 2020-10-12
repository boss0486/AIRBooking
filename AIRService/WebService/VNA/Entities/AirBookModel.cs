using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace AIRService.WS.Entities
{
    public class AirBookModel : TokenModel
    {
        public List<BookSegmentModel> Segments { get; set; }
    }

    public class BookSegmentModel
    {
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int FlightNumber { get; set; }
        public int NumberInParty { get; set; }
        public string ResBookDesigCode { get; set; }
        public int AirEquipType { get; set; }
        public string DestinationLocation { get; set; }
        public string OriginLocation { get; set; }
    }



}