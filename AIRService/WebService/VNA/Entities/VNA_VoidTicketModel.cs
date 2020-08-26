using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models.VNA_WS_Model
{
    public class VNA_VoidTicketModel
    {
        public AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionRS JsonResultVoidTicket { get; set; }
        public AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionRS JsonResultOTA_Cancel { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}