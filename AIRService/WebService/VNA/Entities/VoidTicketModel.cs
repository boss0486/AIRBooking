using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models.VNA_WS_Model
{
    public class VoidTicketModel
    {
        public string JsonResultVoidTicket { get; set; }
        public string JsonResultOTA_Cancel { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}