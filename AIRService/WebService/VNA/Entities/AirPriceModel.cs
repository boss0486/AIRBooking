using AIRService.Models;
using AIRService.WS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class AirPriceModel : TokenModel
    {
        public int ADT { get; set; }
        public int CNN { get; set; }
        public int INF { get; set; }
        public List<AirBookModelSegment> Segments { get; set; }

    }
}