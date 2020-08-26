
using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNA_EndTransaction : IDisposable
    {
        private TokenModel _tokenModel = null;
        private bool m_Disposed = false;

        public VNA_EndTransaction()
        {
            _tokenModel = null;
        }

        // used for using 
        public VNA_EndTransaction(TokenModel tokenModel)
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

        ~VNA_EndTransaction()
        {
            Dispose(false);
        }
        // *****************************************************************************************************************************************************************************

        public AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionRS EndTransaction(TokenModel model = null)
        {
            try
            {
                if (model == null)
                    model = _tokenModel;
                //
                AIRService.WebService.VNA_EndTransactionLLSRQ.MessageHeader messageHeader = new AIRService.WebService.VNA_EndTransactionLLSRQ.MessageHeader();
                messageHeader.MessageData = new AIRService.WebService.VNA_EndTransactionLLSRQ.MessageData();
                messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
                messageHeader.ConversationId = model.ConversationID;
                messageHeader.Service = new AIRService.WebService.VNA_EndTransactionLLSRQ.Service();
                messageHeader.Action = "EndTransactionLLSRQ";
                messageHeader.From = new AIRService.WebService.VNA_EndTransactionLLSRQ.From();

                messageHeader.From.PartyId = new AIRService.WebService.VNA_EndTransactionLLSRQ.PartyId[1];
                var partyID = new AIRService.WebService.VNA_EndTransactionLLSRQ.PartyId();
                partyID.Value = "WebServiceClient";
                messageHeader.From.PartyId[0] = partyID;

                messageHeader.To = new AIRService.WebService.VNA_EndTransactionLLSRQ.To();
                messageHeader.To.PartyId = new AIRService.WebService.VNA_EndTransactionLLSRQ.PartyId[1];
                partyID = new AIRService.WebService.VNA_EndTransactionLLSRQ.PartyId();
                partyID.Value = "WebServiceSupplier";
                messageHeader.To.PartyId[0] = partyID;

                AIRService.WebService.VNA_EndTransactionLLSRQ.Security1 security = new AIRService.WebService.VNA_EndTransactionLLSRQ.Security1();
                security.BinarySecurityToken = model.Token;

                AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionRQ endTransactionRQ = new AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionRQ();
                endTransactionRQ.EndTransaction = new AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionRQEndTransaction();
                endTransactionRQ.EndTransaction.Ind = true;

                endTransactionRQ.Source = new AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionRQSource();
                endTransactionRQ.Source.ReceivedFrom = "GOINGO";

                endTransactionRQ.EndTransaction.Email = new AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionRQEndTransactionEmail();
                endTransactionRQ.EndTransaction.Email.eTicket = new AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionRQEndTransactionEmailETicket();
                endTransactionRQ.EndTransaction.Email.eTicket.Ind = true;



                AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionPortTypeClient client = new AIRService.WebService.VNA_EndTransactionLLSRQ.EndTransactionPortTypeClient();
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
