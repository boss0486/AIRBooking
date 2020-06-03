using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models.VNA_WS_Model
{
    public class RevResult
    {
        public string BookingSource { get; set; }
        public string AgentSine { get; set; }
        public string PseudoCityCode { get; set; }
        public string ISOCountry { get; set; }
        public string AgentDutyCode { get; set; }
        public string AirlineVendorID { get; set; }
        public string HomePseudoCityCode { get; set; }
        public string PrimeHostID { get; set; }
        public RevResultOAC OAC { get; set; }
    }

    public class RevResultOAC
    {
        public string PartitionId { get; set; }
        public string AccountingCityCode { get; set; }
        public string AccountingCode { get; set; }
        public string AccountingOfficeStationCode { get; set; }
    }
}