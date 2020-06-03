using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models
{
    public class PassengerDetailsModel : TokenModel
    {
        //public string ContactEmail { get; set; }
        //public OTA_AirBookLLS.OTA_AirBookRS AirBook { get; set; }
        //public List<PassengerDetailData> lPassenger { get; set; }
    }
    public class PassengerData
    {
        public string PassengerType { get; set; }
        public bool Infant { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// F, Or M
        /// </summary>
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
       
    }
   
}