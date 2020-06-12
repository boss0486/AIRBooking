using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace AIRService.AppService.Module.Entities
{
    [Dapper.Table("AppFlightSegment ")]
    public class FlightSegment
    {
        public FlightSegment()
        {
            ID = Guid.NewGuid().ToString();
        }
        [IgnoreInsert]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string IATACode { get; set; }
        public bool IsEnabled { get; set; }
    }
}
