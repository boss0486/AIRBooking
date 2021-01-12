using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCore.Entities;

namespace ApiPortalBooking.Models
{
    public class AirTicketLLSRQModel : TokenModel
    {
        public string PNR { get; set; }
        public string ApproveCode { get; set; }
        public string ExpireDate { get; set; }
        public List<ExTitketPassengerFareModel> ListPricingInPNR { get; set; }
    }
}