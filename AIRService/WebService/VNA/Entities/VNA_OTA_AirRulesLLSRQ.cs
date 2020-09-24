using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRService.Entities
{
    public class VNA_OTA_AirRulesLLSRQ : TokenModel
    {
        public DateTime DepartureDateTime { get; set; }
        public string DestinationLocation { get; set; }
        public string OriginLocation { get; set; }
        public string FareBasis { get; set; }
    }
}