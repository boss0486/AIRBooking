using ApiPortalBooking.Models.VNA_WS_Model.VNA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models.VNA_WS_Model.VNA
{

    public class GetReservationData
    {
        public RevResult RevResult { get; set; }
        public List<TicketingInfoTicketDetails> TicketDetails { get; set; }
    }
    public class TicketingInfoTicketDetails
    {
        public int id { get; set; }
        public int index { get; set; }
        public string elementId { get; set; }
        public string OriginalTicketDetails { get; set; }
        public string TransactionIndicator { get; set; }
        public string TicketNumber { get; set; }
        public string PassengerName { get; set; }
        public string AgencyLocation { get; set; }
        public string DutyCode { get; set; }
        public string AgentSine { get; set; }
        public DateTime Timestamp { get; set; }

    }
}

