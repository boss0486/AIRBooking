using AIRService.Models;
using AIRService.WebService.WSOTA_AirBookLLSRQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class PassengerDetailsModel : TokenModel
    {
        public string ContactEmail { get; set; }
        public OTA_AirBookRS AirBook { get; set; }
        public List<PassengerDetailData> lPassenger { get; set; }
    }
    public class PassengerDetailData
    {
        public string PassengerType { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
        /// <summary>
        /// F, Or M
        /// </summary>
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }

    }

    public class PassengerDetail_PersonNameModel
    {
        public string NameNumber { get; set; }
        public bool IsInfant { get; set; }
        public string PassengerType { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class Response_PassengerDetailsModel
    {
        public string PNR { get; set; }
    }
    public class PNRModel
    {
        public string PNR { get; set; }
    }
}