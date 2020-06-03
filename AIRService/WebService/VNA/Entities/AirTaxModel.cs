using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class AirTaxModel : TokenModel
    {
        public string OriginLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string DestinationLocation { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public short RPH { get; set; }
        public short FlightNumber { get; set; }
        public string ResBookDesigCode { get; set; }
        public string AirEquipType { get; set; }
        public string PassengerType { get; set; }
        public string FareBasisCode { get; set; }
        public float BaseFare { get; set; }
        public string CurrencyCode { get; set; }
    }
}