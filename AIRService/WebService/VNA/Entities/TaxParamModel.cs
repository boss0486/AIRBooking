using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models.VNA_WS_Model
{
    public class TaxParamModel
    {
        public string DepartureDateTime { get; set; }
        public string ArrivalDateTime { get; set; }
        public short FlightNumber { get; set; }
        public string ResBookDesigCode { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public string AirEquipType { get; set; }
        public float baseFare { get; set; }
        public string PassengerType { get; set; }
        public string FareBasisCode { get; set; }
        public string CurrencyCode { get; set; }
    }
}