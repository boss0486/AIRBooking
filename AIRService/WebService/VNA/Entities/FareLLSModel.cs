using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class FareLLSModel : TokenModel
    {
        public List<string> PassengerType { get; set; }
        public string OriginLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string DestinationLocation { get; set; }
        public string AirlineCode { get; set; }
        public string CurrencyCode { get; set; }
        public Double AxFee { get; set; }
    }
    public class FareLLSModel2 : TokenModel
    {
        public List<string> PassengerType { get; set; }
        public string OriginLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string DestinationLocation { get; set; }
        public string AirlineCode { get; set; }
        public string CurrencyCode { get; set; }
        public Double AxFee { get; set; }
    }
    public class FareItem
    {
        public int RPH { get; set; }
        public string PassengerType { get; set; }
        //public double Total { get; set; }
        //public double ServiceFee { get; set; }
        //public double Tax { get; set; }//CHARGE DOMESTIC
        //public double ChargeDomesticFee { get; set; }
        //public double ScreeningFee { get; set; }
        public double FareAmount { get; set; }
        public double FareTotal { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public string Test { get; set; }

    }
}