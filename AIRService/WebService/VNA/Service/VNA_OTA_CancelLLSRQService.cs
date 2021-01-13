using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNA_OTA_CancelLLSRQService
    {
        public AIRService.WebService.VNA_OTA_CancelLLSRQ.OTA_CancelRS OTA_CancelLLS(TokenModel model)
        {
            AIRService.WebService.VNA_OTA_CancelLLSRQ.MessageHeader messageHeader = new AIRService.WebService.VNA_OTA_CancelLLSRQ.MessageHeader
            {
                MessageData = new AIRService.WebService.VNA_OTA_CancelLLSRQ.MessageData
                {
                    Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z"
                },
                ConversationId = model.ConversationID,
                Service = new AIRService.WebService.VNA_OTA_CancelLLSRQ.Service(),
                Action = "OTA_CancelLLSRQ",
                From = new AIRService.WebService.VNA_OTA_CancelLLSRQ.From
                {
                    PartyId = new AIRService.WebService.VNA_OTA_CancelLLSRQ.PartyId[1]
                }
            };
            var partyID = new AIRService.WebService.VNA_OTA_CancelLLSRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new AIRService.WebService.VNA_OTA_CancelLLSRQ.To
            {
                PartyId = new AIRService.WebService.VNA_OTA_CancelLLSRQ.PartyId[1]
            };
            partyID = new AIRService.WebService.VNA_OTA_CancelLLSRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;

            AIRService.WebService.VNA_OTA_CancelLLSRQ.Security1 security = new AIRService.WebService.VNA_OTA_CancelLLSRQ.Security1
            {
                BinarySecurityToken = model.Token
            };

            AIRService.WebService.VNA_OTA_CancelLLSRQ.OTA_CancelRQ oTA_CancelRQ = new AIRService.WebService.VNA_OTA_CancelLLSRQ.OTA_CancelRQ
            {
                Segment = new AIRService.WebService.VNA_OTA_CancelLLSRQ.OTA_CancelRQSegment[1]
            };
            var Segment = new AIRService.WebService.VNA_OTA_CancelLLSRQ.OTA_CancelRQSegment
            {
                Type = AIRService.WebService.VNA_OTA_CancelLLSRQ.OTA_CancelRQSegmentType.entire,
                TypeSpecified = true
            };
            oTA_CancelRQ.Segment[0] = Segment;
            AIRService.WebService.VNA_OTA_CancelLLSRQ.OTA_CancelPortTypeClient client = new AIRService.WebService.VNA_OTA_CancelLLSRQ.OTA_CancelPortTypeClient();
            var data = client.OTA_CancelRQ(ref messageHeader, ref security, oTA_CancelRQ);
            return data;
        }
    }
}
