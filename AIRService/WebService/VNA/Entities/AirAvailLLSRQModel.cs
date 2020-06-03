using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRService.Entities
{
    public class AirAvailLLSRQModel : TokenModel
    {
        public string OriginLocation { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public string DestinationLocation { get; set; }
    }
}