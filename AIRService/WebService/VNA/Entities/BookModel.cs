using AIRService.WS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class BookModel
    {
        public List<PassengerData> Passengers { get; set; }
        public List<AirBookModelSegment> Segments { get; set; }
        public string Email { get; set; }

    }
}