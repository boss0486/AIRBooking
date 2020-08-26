using AIRService.Models;
using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRService.WS.Entities
{
    public class VNAFareLLSRQModel
    {
        public string ResBookDesigCode { get; set; }
        public string CurrencyCode { get; set; }
        public FareLLSModel FareLLS { get; set; }
        public AIRService.WebService.VNA_FareLLSRQ.FareRS FareRSData { get; set; }
    }
}