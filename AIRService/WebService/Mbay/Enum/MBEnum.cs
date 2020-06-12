using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WebService.Mbay.Enum
{
    class GDSEnum
    {
        public enum FlightType
        {
            None = 0,
            OnWay = 1,
            RoundTrip = 2
        }
        public enum FlightDirection
        {
            None = 0,
            FlightGo = 1,
            FlightTo = 2
        }
    }
}
