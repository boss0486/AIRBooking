﻿using AIRService.Models;
using AIRService.WebService.VNA.Authen;
using System;
using System.Threading.Tasks;

namespace AIR.Helper.Session
{
    public class SessionService : IDisposable
    {


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SessionService()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
        // *****************************************************************************************************************************************************************************

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
                messageHeader.ConversationId = "fOV1LWT3EJIUnGC0Yh1";// Guid.NewGuid().ToString(); // "fOV1LWT3EJIUnGC0Yh1";
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
    }
}
