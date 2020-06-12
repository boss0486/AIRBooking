using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNA_WSDesignatePrinterLLSRQService
    {
        public WebService.WSDesignatePrinterLLSRQ.DesignatePrinterRS DesignatePrinterLLS(DesignatePrinterLLSModel model)
        {
            WebService.WSDesignatePrinterLLSRQ.MessageHeader messageHeader = new WebService.WSDesignatePrinterLLSRQ.MessageHeader();
            messageHeader.MessageData = new WebService.WSDesignatePrinterLLSRQ.MessageData();
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new WebService.WSDesignatePrinterLLSRQ.Service();
            messageHeader.Action = "DesignatePrinterLLSRQ";
            messageHeader.From = new WebService.WSDesignatePrinterLLSRQ.From();
            //
            messageHeader.From.PartyId = new WebService.WSDesignatePrinterLLSRQ.PartyId[1];
            var partyID = new WebService.WSDesignatePrinterLLSRQ.PartyId();
            partyID.Value = "WebServiceClient";
            messageHeader.From.PartyId[0] = partyID;
            //
            messageHeader.To = new WebService.WSDesignatePrinterLLSRQ.To();
            messageHeader.To.PartyId = new WebService.WSDesignatePrinterLLSRQ.PartyId[1];
            partyID = new WebService.WSDesignatePrinterLLSRQ.PartyId();
            partyID.Value = "WebServiceSupplier";
            messageHeader.To.PartyId[0] = partyID;
            //
            WebService.WSDesignatePrinterLLSRQ.Security1 security = new WebService.WSDesignatePrinterLLSRQ.Security1();
            security.BinarySecurityToken = model.Token;
            WebService.WSDesignatePrinterLLSRQ.DesignatePrinterRQ designatePrinterRQ = new WebService.WSDesignatePrinterLLSRQ.DesignatePrinterRQ();
            designatePrinterRQ.Printers = new WebService.WSDesignatePrinterLLSRQ.DesignatePrinterRQPrinters();
            designatePrinterRQ.ReturnHostCommand = true;
            designatePrinterRQ.ReturnHostCommandSpecified = true;
            designatePrinterRQ.Printers.Ticket = new WebService.WSDesignatePrinterLLSRQ.DesignatePrinterRQPrintersTicket();
            designatePrinterRQ.Printers.Ticket.CountryCode = "GF";
            designatePrinterRQ.Printers.Ticket.LNIATA = "58BEAB";
            WebService.WSDesignatePrinterLLSRQ.DesignatePrinterPortTypeClient client = new WebService.WSDesignatePrinterLLSRQ.DesignatePrinterPortTypeClient();
            var data = client.DesignatePrinterRQ(ref messageHeader, ref security, designatePrinterRQ);
            return data;
        }
    }
}
