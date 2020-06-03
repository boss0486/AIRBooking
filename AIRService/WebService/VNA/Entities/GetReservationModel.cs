using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class GetReservationModel : TokenModel
    {
        public string PNR { get; set; }
    }
}