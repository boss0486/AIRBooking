using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNAWSOTA_CancelLLSRQService
    {
        public AIRService.WebService.WSOTA_CancelLLSRQ.OTA_CancelRS OTA_CancelLLS(TokenModel model)
        {
            AIRService.WebService.WSOTA_CancelLLSRQ.MessageHeader messageHeader = new AIRService.WebService.WSOTA_CancelLLSRQ.MessageHeader();
            messageHeader.MessageData = new AIRService.WebService.WSOTA_CancelLLSRQ.MessageData();
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new AIRService.WebService.WSOTA_CancelLLSRQ.Service();
            messageHeader.Action = "OTA_CancelLLSRQ";
            messageHeader.From = new AIRService.WebService.WSOTA_CancelLLSRQ.From();

            messageHeader.From.PartyId = new AIRService.WebService.WSOTA_CancelLLSRQ.PartyId[1];
            var partyID = new AIRService.WebService.WSOTA_CancelLLSRQ.PartyId();
            partyID.Value = "WebServiceClient";
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new AIRService.WebService.WSOTA_CancelLLSRQ.To();
            messageHeader.To.PartyId = new AIRService.WebService.WSOTA_CancelLLSRQ.PartyId[1];
            partyID = new AIRService.WebService.WSOTA_CancelLLSRQ.PartyId();
            partyID.Value = "WebServiceSupplier";
            messageHeader.To.PartyId[0] = partyID;

            AIRService.WebService.WSOTA_CancelLLSRQ.Security1 security = new AIRService.WebService.WSOTA_CancelLLSRQ.Security1();
            security.BinarySecurityToken = model.Token;

            AIRService.WebService.WSOTA_CancelLLSRQ.OTA_CancelRQ oTA_CancelRQ = new AIRService.WebService.WSOTA_CancelLLSRQ.OTA_CancelRQ();
            oTA_CancelRQ.Segment = new AIRService.WebService.WSOTA_CancelLLSRQ.OTA_CancelRQSegment[1];
            var Segment = new AIRService.WebService.WSOTA_CancelLLSRQ.OTA_CancelRQSegment();
            Segment.Type = AIRService.WebService.WSOTA_CancelLLSRQ.OTA_CancelRQSegmentType.entire;
            Segment.TypeSpecified = true;
            oTA_CancelRQ.Segment[0] = Segment;
            AIRService.WebService.WSOTA_CancelLLSRQ.OTA_CancelPortTypeClient client = new AIRService.WebService.WSOTA_CancelLLSRQ.OTA_CancelPortTypeClient();
            var data = client.OTA_CancelRQ(ref messageHeader, ref security, oTA_CancelRQ);
            return data;
        }
    }
}
