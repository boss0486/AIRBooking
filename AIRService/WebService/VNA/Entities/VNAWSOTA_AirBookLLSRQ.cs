using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Entities
{
    class VNAWSOTA_AirBookLLSRQ : TokenModel
    {
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public int FlightNumber { get; set; }
        public string ResBookDesigCode { get; set; }
        public string NumberInParty { get; set; }
        public string AirEquipType { get; set; }
        public string OriginLocation { get; set; }
        public string DestinationLocation { get; set; }
    }
}
