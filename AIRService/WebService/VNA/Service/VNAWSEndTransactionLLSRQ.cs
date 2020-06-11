
using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNAEndTransaction : IDisposable
    {
        private TokenModel _tokenModel = null;
        private bool m_Disposed = false;

        public VNAEndTransaction()
        {
            _tokenModel = null;
        }

        // used for using 
        public VNAEndTransaction(TokenModel tokenModel)
        {
            _tokenModel = tokenModel;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_Disposed)
            {
                if (disposing)
                {
                    // các đối tượng có Dispose gọi ở đây
                    EndTransaction(_tokenModel);
                }
                // giải phóng các tài nguyên không quản lý được cửa lớp
                m_Disposed = true;
            }
        }

        ~VNAEndTransaction()
        {
            Dispose(false);
        }
        // *****************************************************************************************************************************************************************************

        public AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionRS EndTransaction(TokenModel model = null)
        {
            try
            {
                if (model == null)
                    model = _tokenModel;
                //
                AIRService.WebService.WSEndTransactionLLSRQ.MessageHeader messageHeader = new AIRService.WebService.WSEndTransactionLLSRQ.MessageHeader();
                messageHeader.MessageData = new AIRService.WebService.WSEndTransactionLLSRQ.MessageData();
                messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
                messageHeader.ConversationId = model.ConversationID;
                messageHeader.Service = new AIRService.WebService.WSEndTransactionLLSRQ.Service();
                messageHeader.Action = "EndTransactionLLSRQ";
                messageHeader.From = new AIRService.WebService.WSEndTransactionLLSRQ.From();

                messageHeader.From.PartyId = new AIRService.WebService.WSEndTransactionLLSRQ.PartyId[1];
                var partyID = new AIRService.WebService.WSEndTransactionLLSRQ.PartyId();
                partyID.Value = "WebServiceClient";
                messageHeader.From.PartyId[0] = partyID;

                messageHeader.To = new AIRService.WebService.WSEndTransactionLLSRQ.To();
                messageHeader.To.PartyId = new AIRService.WebService.WSEndTransactionLLSRQ.PartyId[1];
                partyID = new AIRService.WebService.WSEndTransactionLLSRQ.PartyId();
                partyID.Value = "WebServiceSupplier";
                messageHeader.To.PartyId[0] = partyID;

                AIRService.WebService.WSEndTransactionLLSRQ.Security1 security = new AIRService.WebService.WSEndTransactionLLSRQ.Security1();
                security.BinarySecurityToken = model.Token;

                AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionRQ endTransactionRQ = new AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionRQ();
                endTransactionRQ.EndTransaction = new AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionRQEndTransaction();
                endTransactionRQ.EndTransaction.Ind = true;

                endTransactionRQ.Source = new AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionRQSource();
                endTransactionRQ.Source.ReceivedFrom = "GOINGO";

                endTransactionRQ.EndTransaction.Email = new AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionRQEndTransactionEmail();
                endTransactionRQ.EndTransaction.Email.eTicket = new AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionRQEndTransactionEmailETicket();
                endTransactionRQ.EndTransaction.Email.eTicket.Ind = true;



                AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionPortTypeClient client = new AIRService.WebService.WSEndTransactionLLSRQ.EndTransactionPortTypeClient();
                var data = client.EndTransactionRQ(ref messageHeader, ref security, endTransactionRQ);
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
