using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiPortalBooking.Models.VNA_WS_Model
{
    public class VoidTicketModel
    {
        public AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionRS JsonResultVoidTicket { get; set; }
        public AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionRS JsonResultOTA_Cancel { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}