using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNA_PaymentRQService
    {
        public WebService.VNA_PaymentRQ.PaymentRS Payment(PaymentModel model)
        {
            WebService.VNA_PaymentRQ.MessageHeader messageHeader = new WebService.VNA_PaymentRQ.MessageHeader();
            messageHeader.MessageData = new WebService.VNA_PaymentRQ.MessageData();
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new WebService.VNA_PaymentRQ.Service();
            messageHeader.Action = "PaymentRQ";
            messageHeader.From = new WebService.VNA_PaymentRQ.From();

            messageHeader.From.PartyId = new WebService.VNA_PaymentRQ.PartyId[1];
            var partyID = new WebService.VNA_PaymentRQ.PartyId();
            partyID.Value = "WebServiceClient";
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new WebService.VNA_PaymentRQ.To();
            messageHeader.To.PartyId = new WebService.VNA_PaymentRQ.PartyId[1];
            partyID = new WebService.VNA_PaymentRQ.PartyId();
            partyID.Value = "WebServiceSupplier";
            messageHeader.To.PartyId[0] = partyID;

            WebService.VNA_PaymentRQ.Security security = new WebService.VNA_PaymentRQ.Security();
            security.BinarySecurityToken = model.Token;

            WebService.VNA_PaymentRQ.PaymentRQ payment = new WebService.VNA_PaymentRQ.PaymentRQ();
            payment.SystemDateTime = DateTime.UtcNow;
            payment.SystemDateTimeSpecified = true;

            payment.Action = new WebService.VNA_PaymentRQ.PaymentRQAction();
            payment.Action.Type = "Auth";
            payment.Action.Value = "Auth";

            payment.POS = new WebService.VNA_PaymentRQ.POS_Type();
            payment.POS.AgentSine = "A44";
            payment.POS.StationNumber = "3795968";
            payment.POS.ISOCountry = "VN";
            payment.POS.ChannelID = "AGY";
            payment.POS.PseudoCityCode = "LZJ";
            payment.POS.LocalDateTime = DateTime.Now;
            payment.POS.LocalDateTimeSpecified = true;

            payment.MerchantDetail = new WebService.VNA_PaymentRQ.MerchantDetailType();
            payment.MerchantDetail.MerchantID = "VN";

            payment.OrderDetail = new WebService.VNA_PaymentRQ.PaymentRQOrderDetail();
            payment.OrderDetail.RecordLocator = model.pnr;
            payment.OrderDetail.OrderID = model.pnr + new Random().Next(10000, 1000000);

            var lpassenger = new List<WebService.VNA_PaymentRQ.PassengerDetailType>();
            foreach (var item in model.PaymentOrderDetail)
            {
                var passenger = new WebService.VNA_PaymentRQ.PassengerDetailType();
                passenger.PsgrType = item.PsgrType;
                passenger.FirstName = item.FirstName;
                passenger.LastName = item.LastName;
                passenger.Document = new WebService.VNA_PaymentRQ.PassengerDetailTypeDocument[1];
                var document = new WebService.VNA_PaymentRQ.PassengerDetailTypeDocument();
                document.DocType = "TKT";
                document.eTicketInd = true;
                document.BaseFare = Convert.ToDecimal(item.BaseFare);
                document.BaseFareSpecified = true;
                document.Taxes = Convert.ToDecimal(item.Taxes);
                document.TaxesSpecified = true;
                passenger.Document[0] = document;
                lpassenger.Add(passenger);
            }
            payment.OrderDetail.PassengerDetail = lpassenger.ToArray();

            payment.AccountDetail = new WebService.VNA_PaymentRQ.AccountDetailType[1];
            var account = new WebService.VNA_PaymentRQ.AccountDetailType();

            account.AccountType = "BT";
            account.AccountNbr = "8738210537959681";
            account.CurrencyCode = "VND";
            account.UserName = "37959681";
            payment.AccountDetail[0] = account;
            payment.PaymentDetail = new WebService.VNA_PaymentRQ.PaymentDetailType[1];
            var detail = new WebService.VNA_PaymentRQ.PaymentDetailType();
            detail.FOP = new WebService.VNA_PaymentRQ.FOPType();
            detail.FOP.Type = "BT";
            detail.AmountDetail = new WebService.VNA_PaymentRQ.PaymentDetailTypeAmountDetail();
            detail.AmountDetail.Amount = Convert.ToDecimal(model.Total);
            detail.AmountDetail.CurrencyCode = "VND";
            payment.PaymentDetail[0] = detail;

            WebService.VNA_PaymentRQ.PaymentServicePortTypeClient client = new WebService.VNA_PaymentRQ.PaymentServicePortTypeClient();
            var data = client.PaymentServiceRQ(ref messageHeader, ref security, payment);
            return data;
            
        }
    }
}
