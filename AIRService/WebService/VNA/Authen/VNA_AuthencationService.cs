using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WebService.VNA.Authen
{
    public class VNA_AuthencationService
    {
        public TokenModel Create()
        {
            TokenModel tokenModel = new TokenModel();
            AIRService.WebService.VNA_SessionCreateRQ.MessageHeader messageHeader = new AIRService.WebService.VNA_SessionCreateRQ.MessageHeader
            {
                MessageData = new AIRService.WebService.VNA_SessionCreateRQ.MessageData
                {
                    Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z"
                },
                ConversationId = "fOV1LWT3EJIUnGC0Yh1",
                Service = new AIRService.WebService.VNA_SessionCreateRQ.Service(),
                Action = "SessionCreateRQ",
                From = new AIRService.WebService.VNA_SessionCreateRQ.From
                {
                    PartyId = new AIRService.WebService.VNA_SessionCreateRQ.PartyId[1]
                }
            };
            var partyID = new AIRService.WebService.VNA_SessionCreateRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;
            messageHeader.To = new AIRService.WebService.VNA_SessionCreateRQ.To
            {
                PartyId = new AIRService.WebService.VNA_SessionCreateRQ.PartyId[1]
            };
            partyID = new AIRService.WebService.VNA_SessionCreateRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;
            VNA_Authencation wSAuthencation = new VNA_Authencation();
            AIRService.WebService.VNA_SessionCreateRQ.Security security = new AIRService.WebService.VNA_SessionCreateRQ.Security
            {
                UsernameToken = new AIRService.WebService.VNA_SessionCreateRQ.SecurityUsernameToken
                {
                    Organization = wSAuthencation.ORGANIZATION,
                    Username = wSAuthencation.USERNAME,
                    Password = wSAuthencation.PASSWORD,
                    Domain = wSAuthencation.DOMAIN
                }
            };
            AIRService.WebService.VNA_SessionCreateRQ.SessionCreateRQ sessionCreateRQ = new AIRService.WebService.VNA_SessionCreateRQ.SessionCreateRQ
            {
                returnContextID = true,
                returnContextIDSpecified = true
            };
            AIRService.WebService.VNA_SessionCreateRQ.SessionCreatePortTypeClient client = new AIRService.WebService.VNA_SessionCreateRQ.SessionCreatePortTypeClient();
            client.SessionCreateRQ(ref messageHeader, ref security, sessionCreateRQ);
            tokenModel.ConversationID = messageHeader.ConversationId;
            tokenModel.Token = security.BinarySecurityToken;
            return tokenModel;
        }

        public static TokenModel GetSession()
        {
            AIRService.WebService.VNA_SessionCreateRQ.MessageHeader messageHeader = new AIRService.WebService.VNA_SessionCreateRQ.MessageHeader
            {
                MessageData = new AIRService.WebService.VNA_SessionCreateRQ.MessageData()
            };
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = "fOV1LWT3EJIUnGC0Yh1";// Guid.NewGuid().ToString(); // "fOV1LWT3EJIUnGC0Yh1";
            messageHeader.Service = new AIRService.WebService.VNA_SessionCreateRQ.Service();
            messageHeader.Action = "SessionCreateRQ";
            messageHeader.From = new AIRService.WebService.VNA_SessionCreateRQ.From
            {
                PartyId = new AIRService.WebService.VNA_SessionCreateRQ.PartyId[1]
            };
            var partyID = new AIRService.WebService.VNA_SessionCreateRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;
            messageHeader.To = new AIRService.WebService.VNA_SessionCreateRQ.To
            {
                PartyId = new AIRService.WebService.VNA_SessionCreateRQ.PartyId[1]
            };
            partyID = new AIRService.WebService.VNA_SessionCreateRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;
            VNA_Authencation wSAuthencation = new VNA_Authencation();
            AIRService.WebService.VNA_SessionCreateRQ.Security security = new AIRService.WebService.VNA_SessionCreateRQ.Security
            {
                UsernameToken = new AIRService.WebService.VNA_SessionCreateRQ.SecurityUsernameToken
                {
                    Organization = wSAuthencation.ORGANIZATION,
                    Username = wSAuthencation.USERNAME,
                    Password = wSAuthencation.PASSWORD,
                    Domain = wSAuthencation.DOMAIN
                }
            };
            AIRService.WebService.VNA_SessionCreateRQ.SessionCreateRQ sessionCreateRQ = new AIRService.WebService.VNA_SessionCreateRQ.SessionCreateRQ
            {
                returnContextID = true,
                returnContextIDSpecified = true
            };
            AIRService.WebService.VNA_SessionCreateRQ.SessionCreatePortTypeClient client = new AIRService.WebService.VNA_SessionCreateRQ.SessionCreatePortTypeClient();
            var data = client.SessionCreateRQ(ref messageHeader, ref security, sessionCreateRQ);
            if (data == null)
                return null;
            // set data for model ->> return
            var result = new TokenModel
            {
                ConversationID = messageHeader.ConversationId,
                Token = security.BinarySecurityToken
            };
            return result;
        }
        public static Boolean CloseSession(TokenModel model)
        {
            if (model == null)
                return false;
            //
            AIRService.WebService.VNA_SessionCloseRQ.MessageHeader messageHeader = new AIRService.WebService.VNA_SessionCloseRQ.MessageHeader
            {
                MessageData = new AIRService.WebService.VNA_SessionCloseRQ.MessageData
                {
                    Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z"
                },
                ConversationId = model.ConversationID,
                Service = new AIRService.WebService.VNA_SessionCloseRQ.Service(),
                Action = "SessionCloseRQ",
                From = new AIRService.WebService.VNA_SessionCloseRQ.From
                {
                    PartyId = new AIRService.WebService.VNA_SessionCloseRQ.PartyId[1]
                }
            };
            var partyID = new AIRService.WebService.VNA_SessionCloseRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;
            messageHeader.To = new AIRService.WebService.VNA_SessionCloseRQ.To
            {
                PartyId = new AIRService.WebService.VNA_SessionCloseRQ.PartyId[1]
            };
            partyID = new AIRService.WebService.VNA_SessionCloseRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;
            AIRService.WebService.VNA_SessionCloseRQ.Security security = new AIRService.WebService.VNA_SessionCloseRQ.Security
            {
                BinarySecurityToken = model.Token
            };
            //
            AIRService.WebService.VNA_SessionCloseRQ.SessionCloseRQ sessionCloseRQ = new AIRService.WebService.VNA_SessionCloseRQ.SessionCloseRQ();
            AIRService.WebService.VNA_SessionCloseRQ.SessionClosePortTypeClient client = new AIRService.WebService.VNA_SessionCloseRQ.SessionClosePortTypeClient();
            var data = client.SessionCloseRQ(ref messageHeader, ref security, sessionCloseRQ);
            if (data == null)
                return false;
            if (string.IsNullOrWhiteSpace(data.status))
                return false;
            if (data.status.ToLower() == "approved")
                return true;
            return false;
        }

    }
}
