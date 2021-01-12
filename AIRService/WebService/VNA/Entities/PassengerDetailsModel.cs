using AIRService.Models;
using AIRService.WebService.VNA_OTA_AirBookLLSRQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCore.Entities;

namespace ApiPortalBooking.Models
{
    public class DetailsRQ : TokenModel
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public OTA_AirBookRS AirBook { get; set; }
        public List<PassengerDetailsRQ> Passengers { get; set; }
    }
    public class PassengerDetailsRQ
    {
        public string PassengerType { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FullName { get; set; }
        /// <summary>
        /// F, Or M
        /// </summary>
        public int Gender { get; set; }
        public string Phone { get; set; }
    }
}