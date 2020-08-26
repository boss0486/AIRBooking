using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models.VNA_WS_Model
{
    public class BookVeResult
    {
        public string PNR { get; set; }
        public double FareTotal { get; set; }
        public string TicketID { get; set; }
        public List<PassengerFare> Passengers { get; set; }
        public List<FareFlight> FareFlights { get; set; }
        public List<FareTax> FareTaxs { get; set; }
    }
    public class PassengerFare
    { 
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string PassengerType { get; set; }
        public double PriceTotal { get; set; }
        public double TaxTotal { get; set; }
    }

    public class FareTax
    {
        public string PassengerType { get; set; }
        public string Text { get; set; }
        public string TaxCode { get; set; }
        public double Amount { get; set; }
    }

    public class FareFlight
    {
        public int FlightType { get; set; }
        public string PassengerType { get; set; }
        public string Code { get; set; }
        public double Amount { get; set; }
    }


    public class PricingInPNR_Test
    {
        public AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRSPriceQuote PriceQuote { get; set; }
        public AIRService.WebService.WSOTA_AirPriceLLSRQ.ApplicationResults ApplicationResults { get; set; }

    }
}