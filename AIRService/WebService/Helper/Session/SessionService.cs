using AIRService.Models;
using AIRService.WebService.VNA.Authen;
using System;
using System.Threading.Tasks;

namespace AIR.Helper.Session
{
    public class SessionService
    {
        readonly WSAuthencation wSAuthencation = new WSAuthencation();

        public TokenModel Create()
        {
            TokenModel tokenModel = new TokenModel();
            AIRService.WebService.WSSessionCreateRQ.MessageHeader messageHeader = new AIRService.WebService.WSSessionCreateRQ.MessageHeader
            {
                MessageData = new AIRService.WebService.WSSessionCreateRQ.MessageData
                {
                    Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z"
                },
                ConversationId = "fOV1LWT3EJIUnGC0Yh1",
                Service = new AIRService.WebService.WSSessionCreateRQ.Service(),
                Action = "SessionCreateRQ",
                From = new AIRService.WebService.WSSessionCreateRQ.From
                {
                    PartyId = new AIRService.WebService.WSSessionCreateRQ.PartyId[1]
                }
            };
            var partyID = new AIRService.WebService.WSSessionCreateRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;
            messageHeader.To = new AIRService.WebService.WSSessionCreateRQ.To
            {
                PartyId = new AIRService.WebService.WSSessionCreateRQ.PartyId[1]
            };
            partyID = new AIRService.WebService.WSSessionCreateRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;
            AIRService.WebService.WSSessionCreateRQ.Security security = new AIRService.WebService.WSSessionCreateRQ.Security
            {
                UsernameToken = new AIRService.WebService.WSSessionCreateRQ.SecurityUsernameToken
                {
                    Organization = wSAuthencation.ORGANIZATION,
                    Username = wSAuthencation.USERNAME,
                    Password = wSAuthencation.PASSWORD,
                    Domain = wSAuthencation.DOMAIN
                }
            };
            AIRService.WebService.WSSessionCreateRQ.SessionCreateRQ sessionCreateRQ = new AIRService.WebService.WSSessionCreateRQ.SessionCreateRQ
            {
                returnContextID = true,
                returnContextIDSpecified = true
            };
            AIRService.WebService.WSSessionCreateRQ.SessionCreatePortTypeClient client = new AIRService.WebService.WSSessionCreateRQ.SessionCreatePortTypeClient();
            client.SessionCreateRQ(ref messageHeader, ref security, sessionCreateRQ);
            tokenModel.ConversationID = messageHeader.ConversationId;
            tokenModel.Token = security.BinarySecurityToken;
            return tokenModel;
        }

        public TokenModel GetSession()
        {
            try
            {
                AIRService.WebService.WSSessionCreateRQ.MessageHeader messageHeader = new AIRService.WebService.WSSessionCreateRQ.MessageHeader
                {
                    MessageData = new AIRService.WebService.WSSessionCreateRQ.MessageData()
                };
                messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
                messageHeader.ConversationId = Guid.NewGuid().ToString(); // "fOV1LWT3EJIUnGC0Yh1";
                messageHeader.Service = new AIRService.WebService.WSSessionCreateRQ.Service();
                messageHeader.Action = "SessionCreateRQ";
                messageHeader.From = new AIRService.WebService.WSSessionCreateRQ.From
                {
                    PartyId = new AIRService.WebService.WSSessionCreateRQ.PartyId[1]
                };
                var partyID = new AIRService.WebService.WSSessionCreateRQ.PartyId
                {
                    Value = "WebServiceClient"
                };
                messageHeader.From.PartyId[0] = partyID;
                messageHeader.To = new AIRService.WebService.WSSessionCreateRQ.To
                {
                    PartyId = new AIRService.WebService.WSSessionCreateRQ.PartyId[1]
                };
                partyID = new AIRService.WebService.WSSessionCreateRQ.PartyId
                {
                    Value = "WebServiceSupplier"
                };
                messageHeader.To.PartyId[0] = partyID;
                AIRService.WebService.WSSessionCreateRQ.Security security = new AIRService.WebService.WSSessionCreateRQ.Security
                {
                    UsernameToken = new AIRService.WebService.WSSessionCreateRQ.SecurityUsernameToken
                    {
                        Organization = wSAuthencation.ORGANIZATION,
                        Username = wSAuthencation.USERNAME,
                        Password = wSAuthencation.PASSWORD,
                        Domain = wSAuthencation.DOMAIN
                    }
                };
                AIRService.WebService.WSSessionCreateRQ.SessionCreateRQ sessionCreateRQ = new AIRService.WebService.WSSessionCreateRQ.SessionCreateRQ
                {
                    returnContextID = true,
                    returnContextIDSpecified = true
                };
                AIRService.WebService.WSSessionCreateRQ.SessionCreatePortTypeClient client = new AIRService.WebService.WSSessionCreateRQ.SessionCreatePortTypeClient();
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
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">TokenModel</param>
        /// <returns>Approved || </returns>
        public Boolean CloseSession(TokenModel model)
        {
            try
            {
                AIRService.WebService.WSSessionCloseRQ.MessageHeader messageHeader = new AIRService.WebService.WSSessionCloseRQ.MessageHeader
                {
                    MessageData = new AIRService.WebService.WSSessionCloseRQ.MessageData
                    {
                        Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z"
                    },
                    ConversationId = model.ConversationID,
                    Service = new AIRService.WebService.WSSessionCloseRQ.Service(),
                    Action = "SessionCloseRQ",
                    From = new AIRService.WebService.WSSessionCloseRQ.From
                    {
                        PartyId = new AIRService.WebService.WSSessionCloseRQ.PartyId[1]
                    }
                };
                var partyID = new AIRService.WebService.WSSessionCloseRQ.PartyId
                {
                    Value = "WebServiceClient"
                };
                messageHeader.From.PartyId[0] = partyID;
                messageHeader.To = new AIRService.WebService.WSSessionCloseRQ.To
                {
                    PartyId = new AIRService.WebService.WSSessionCloseRQ.PartyId[1]
                };
                partyID = new AIRService.WebService.WSSessionCloseRQ.PartyId
                {
                    Value = "WebServiceSupplier"
                };
                messageHeader.To.PartyId[0] = partyID;
                AIRService.WebService.WSSessionCloseRQ.Security security = new AIRService.WebService.WSSessionCloseRQ.Security
                {
                    BinarySecurityToken = model.Token
                };
                //
                AIRService.WebService.WSSessionCloseRQ.SessionCloseRQ sessionCloseRQ = new AIRService.WebService.WSSessionCloseRQ.SessionCloseRQ();
                AIRService.WebService.WSSessionCloseRQ.SessionClosePortTypeClient client = new AIRService.WebService.WSSessionCloseRQ.SessionClosePortTypeClient();
                var data = client.SessionCloseRQ(ref messageHeader, ref security, sessionCloseRQ);
                if (data == null)
                    return false;
                if (string.IsNullOrWhiteSpace(data.status))
                    return false;
                if (data.status.ToLower().Equals("approved"))
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //public async Task<TokenModel> CreateSession()
        //{
        //    var result = new TokenModel();
        //    try
        //    {
        //        AIRService.WebService.WSSessionCreateRQ.MessageHeader messageHeader = new AIRService.WebService.WSSessionCreateRQ.MessageHeader();
        //        messageHeader.MessageData = new AIRService.WebService.WSSessionCreateRQ.MessageData();
        //        messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
        //        messageHeader.ConversationId = "fOV1LWT3EJIUnGC0Yh1";
        //        messageHeader.Service = new AIRService.WebService.WSSessionCreateRQ.Service();
        //        messageHeader.Action = "SessionCreateRQ";
        //        messageHeader.From = new AIRService.WebService.WSSessionCreateRQ.From();

        //        messageHeader.From.PartyId = new AIRService.WebService.WSSessionCreateRQ.PartyId[1];
        //        var partyID = new AIRService.WebService.WSSessionCreateRQ.PartyId();
        //        partyID.Value = "WebServiceClient";
        //        messageHeader.From.PartyId[0] = partyID;

        //        messageHeader.To = new AIRService.WebService.WSSessionCreateRQ.To();
        //        messageHeader.To.PartyId = new AIRService.WebService.WSSessionCreateRQ.PartyId[1];
        //        partyID = new AIRService.WebService.WSSessionCreateRQ.PartyId();
        //        partyID.Value = "WebServiceSupplier";
        //        messageHeader.To.PartyId[0] = partyID;

        //        AIRService.WebService.WSSessionCreateRQ.Security security = new AIRService.WebService.WSSessionCreateRQ.Security();
        //        security.UsernameToken = new AIRService.WebService.WSSessionCreateRQ.SecurityUsernameToken();
        //        security.UsernameToken.Organization = ORGANIZATION;
        //        security.UsernameToken.Username = USERNAME;
        //        security.UsernameToken.Password = PASSWORD;
        //        security.UsernameToken.Domain = DOMAIN;

        //        AIRService.WebService.WSSessionCreateRQ.SessionCreateRQ sessionCreateRQ = new AIRService.WebService.WSSessionCreateRQ.SessionCreateRQ();
        //        sessionCreateRQ.returnContextID = true;
        //        sessionCreateRQ.returnContextIDSpecified = true;

        //        AIRService.WebService.WSSessionCreateRQ.SessionCreatePortTypeClient client = new AIRService.WebService.WSSessionCreateRQ.SessionCreatePortTypeClient();

        //        var data = await client.SessionCreateRQAsync(messageHeader, security, sessionCreateRQ);

        //        result.ConversationId = messageHeader.ConversationId;
        //        result.token = security.BinarySecurityToken;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return result;
        //}
        //#region Methods

        //public SessionCreateRQService Create()
        //{
        //    SessionCreateRQService serviceObj = new SessionCreateRQService();
        //    SessionCreateRS resp = new SessionCreateRS();
        //    try
        //    {
        //        // Set user information, including security credentials and the IPCC.
        //        string username = "7971";
        //        string password = "ws011213";
        //        string ipcc = "4REG";
        //        string domain = "DEFAULT";

        //        Security security = new Security();
        //        SecurityUsernameToken securityUserToken = new SecurityUsernameToken();
        //        securityUserToken.Username = username;
        //        securityUserToken.Password = password;
        //        securityUserToken.Organization = ipcc;
        //        securityUserToken.Domain = domain;
        //        security.UsernameToken = securityUserToken;

        //        SessionCreateRQ req = new SessionCreateRQ();
        //        SessionCreateRQPOS pos = new SessionCreateRQPOS();
        //        SessionCreateRQPOSSource source = new SessionCreateRQPOSSource();
        //        source.PseudoCityCode = ipcc;
        //        pos.Source = source;
        //        req.POS = pos;

        //        serviceObj.MessageHeaderValue = Get("SessionCreateRQ", "SessionCreate");
        //        serviceObj.SecurityValue = security;

        //        resp = serviceObj.SessionCreateRQ(req);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.InnerException.ToString());
        //    }

        //    return serviceObj;
        //}

        //public static MessageHeader Get(string actionName, string headerServiceValue)
        //{
        //    //Create the message header and provide the conversation ID.
        //    MessageHeader msgHeader = new MessageHeader();
        //    msgHeader.ConversationId = "TestSession";          // Set the ConversationId

        //    From from = new From();
        //    PartyId fromPartyId = new PartyId();
        //    PartyId[] fromPartyIdArr = new PartyId[1];
        //    fromPartyId.Value = "WebServiceClient";
        //    fromPartyIdArr[0] = fromPartyId;
        //    from.PartyId = fromPartyIdArr;
        //    msgHeader.From = from;

        //    To to = new To();
        //    PartyId toPartyId = new PartyId();
        //    PartyId[] toPartyIdArr = new PartyId[1];
        //    toPartyId.Value = "WebServiceSupplier";
        //    toPartyIdArr[0] = toPartyId;
        //    to.PartyId = toPartyIdArr;
        //    msgHeader.To = to;

        //    //Add the value for eb:CPAId, which is the IPCC.
        //    //Add the value for the action code of this Web service, SessionCreateRQ.

        //    msgHeader.CPAId = "4REG";
        //    msgHeader.Action = actionName;
        //    Service service = new Service();
        //    service.Value = headerServiceValue;
        //    msgHeader.Service = service;

        //    MessageData msgData = new MessageData();
        //    msgData.MessageId = "mid:20001209-133003-2333@clientofsabre.com1";
        //    //msgData.Timestamp = tstamp;
        //    msgHeader.MessageData = msgData;
        //    return msgHeader;
        //}

        //#endregion Methods

    }
}
