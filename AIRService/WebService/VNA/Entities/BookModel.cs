using AIRService.WS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class BookModel
    {
        public List<PassengerDetailData> lPassenger { get; set; }
        public List<AirBookModelSegment> lFlight { get; set; }
        public string ContactEmail { get; set; }

    }
}