using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class FlightCost
    {
        public string PassengerType { get; set; }
        public double Total { get; set; }
        public double ServiceFee { get; set; }
        public double Tax { get; set; }//CHARGE DOMESTIC
        public double ChargeDomesticFee { get; set; }
        public double ScreeningFee { get; set; }
        public double FareAmmout { get; set; }
    }
}