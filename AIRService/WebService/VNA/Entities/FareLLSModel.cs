﻿using AIRService.Models;
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
        
    }
    public class FareDetailsModel
    {
        public string RBH { get; set; }
        public string PassengerType { get; set; }
        //public double Total { get; set; }
        //public double ServiceFee { get; set; }
        //public double Tax { get; set; }//CHARGE DOMESTIC
        //public double ChargeDomesticFee { get; set; }
        //public double ScreeningFee { get; set; }
        public double FareAmount { get; set; } 
        public string Code { get; set; }
    }
}