using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNA_DesignatePrinterLLSRQService
    {
        public WebService.VNA_DesignatePrinterLLSRQ.DesignatePrinterRS DesignatePrinterLLS(DesignatePrinterLLSModel model)
        {
            WebService.VNA_DesignatePrinterLLSRQ.MessageHeader messageHeader = new WebService.VNA_DesignatePrinterLLSRQ.MessageHeader();
            messageHeader.MessageData = new WebService.VNA_DesignatePrinterLLSRQ.MessageData();
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new WebService.VNA_DesignatePrinterLLSRQ.Service();
            messageHeader.Action = "DesignatePrinterLLSRQ";
            messageHeader.From = new WebService.VNA_DesignatePrinterLLSRQ.From();
            //
            messageHeader.From.PartyId = new WebService.VNA_DesignatePrinterLLSRQ.PartyId[1];
            var partyID = new WebService.VNA_DesignatePrinterLLSRQ.PartyId();
            partyID.Value = "WebServiceClient";
            messageHeader.From.PartyId[0] = partyID;
            //
            messageHeader.To = new WebService.VNA_DesignatePrinterLLSRQ.To();
            messageHeader.To.PartyId = new WebService.VNA_DesignatePrinterLLSRQ.PartyId[1];
            partyID = new WebService.VNA_DesignatePrinterLLSRQ.PartyId();
            partyID.Value = "WebServiceSupplier";
            messageHeader.To.PartyId[0] = partyID;
            //
            WebService.VNA_DesignatePrinterLLSRQ.Security1 security = new WebService.VNA_DesignatePrinterLLSRQ.Security1();
            security.BinarySecurityToken = model.Token;
            WebService.VNA_DesignatePrinterLLSRQ.DesignatePrinterRQ designatePrinterRQ = new WebService.VNA_DesignatePrinterLLSRQ.DesignatePrinterRQ();
            designatePrinterRQ.Printers = new WebService.VNA_DesignatePrinterLLSRQ.DesignatePrinterRQPrinters();
            designatePrinterRQ.ReturnHostCommand = true;
            designatePrinterRQ.ReturnHostCommandSpecified = true;
            designatePrinterRQ.Printers.Ticket = new WebService.VNA_DesignatePrinterLLSRQ.DesignatePrinterRQPrintersTicket();
            designatePrinterRQ.Printers.Ticket.CountryCode = "GF";
            designatePrinterRQ.Printers.Ticket.LNIATA = "58BEAB";
            WebService.VNA_DesignatePrinterLLSRQ.DesignatePrinterPortTypeClient client = new WebService.VNA_DesignatePrinterLLSRQ.DesignatePrinterPortTypeClient();
            var data = client.DesignatePrinterRQ(ref messageHeader, ref security, designatePrinterRQ);
            return data;
        }
    }
}
