using AIRService.Models;
using ApiPortalBooking.Models.VNA_WS_Model;
using ApiPortalBooking.Models.VNA_WS_Model.VNA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class AirTicketLLSRQModel : TokenModel
    {
        public string PNR { get; set; }
        public string approveCode { get; set; }
        public List<PricingInPNR> lPricingInPNR { get; set; }
    }
}