using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNAPaymentRQService
    {
        public WebService.WSPaymentRQ.PaymentRS Payment(PaymentModel model)
        {
            WebService.WSPaymentRQ.MessageHeader messageHeader = new WebService.WSPaymentRQ.MessageHeader();
            messageHeader.MessageData = new WebService.WSPaymentRQ.MessageData();
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new WebService.WSPaymentRQ.Service();
            messageHeader.Action = "PaymentRQ";
            messageHeader.From = new WebService.WSPaymentRQ.From();

            messageHeader.From.PartyId = new WebService.WSPaymentRQ.PartyId[1];
            var partyID = new WebService.WSPaymentRQ.PartyId();
            partyID.Value = "WebServiceClient";
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new WebService.WSPaymentRQ.To();
            messageHeader.To.PartyId = new WebService.WSPaymentRQ.PartyId[1];
            partyID = new WebService.WSPaymentRQ.PartyId();
            partyID.Value = "WebServiceSupplier";
            messageHeader.To.PartyId[0] = partyID;

            WebService.WSPaymentRQ.Security security = new WebService.WSPaymentRQ.Security();
            security.BinarySecurityToken = model.Token;

            WebService.WSPaymentRQ.PaymentRQ payment = new WebService.WSPaymentRQ.PaymentRQ();
            payment.SystemDateTime = DateTime.UtcNow;
            payment.SystemDateTimeSpecified = true;

            payment.Action = new WebService.WSPaymentRQ.PaymentRQAction();
            payment.Action.Type = "Auth";
            payment.Action.Value = "Auth";

            payment.POS = new WebService.WSPaymentRQ.POS_Type();
            payment.POS.AgentSine = "A44";
            payment.POS.StationNumber = "3795968";
            payment.POS.ISOCountry = "VN";
            payment.POS.ChannelID = "AGY";
            payment.POS.PseudoCityCode = "LZJ";
            payment.POS.LocalDateTime = DateTime.Now;
            payment.POS.LocalDateTimeSpecified = true;

            payment.MerchantDetail = new WebService.WSPaymentRQ.MerchantDetailType();
            payment.MerchantDetail.MerchantID = "VN";

            payment.OrderDetail = new WebService.WSPaymentRQ.PaymentRQOrderDetail();
            payment.OrderDetail.RecordLocator = model.pnr;
            payment.OrderDetail.OrderID = model.pnr + new Random().Next(10000, 1000000);

            var lpassenger = new List<WebService.WSPaymentRQ.PassengerDetailType>();
            foreach (var item in model.PaymentOrderDetail)
            {
                var passenger = new WebService.WSPaymentRQ.PassengerDetailType();
                passenger.PsgrType = item.PsgrType;
                passenger.FirstName = item.FirstName;
                passenger.LastName = item.LastName;
                passenger.Document = new WebService.WSPaymentRQ.PassengerDetailTypeDocument[1];
                var document = new WebService.WSPaymentRQ.PassengerDetailTypeDocument();
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

            payment.AccountDetail = new WebService.WSPaymentRQ.AccountDetailType[1];
            var account = new WebService.WSPaymentRQ.AccountDetailType();

            account.AccountType = "BT";
            account.AccountNbr = "8738210537959681";
            account.CurrencyCode = "VND";
            account.UserName = "37959681";
            payment.AccountDetail[0] = account;
            payment.PaymentDetail = new WebService.WSPaymentRQ.PaymentDetailType[1];
            var detail = new WebService.WSPaymentRQ.PaymentDetailType();
            detail.FOP = new WebService.WSPaymentRQ.FOPType();
            detail.FOP.Type = "BT";
            detail.AmountDetail = new WebService.WSPaymentRQ.PaymentDetailTypeAmountDetail();
            detail.AmountDetail.Amount = Convert.ToDecimal(model.Total);
            detail.AmountDetail.CurrencyCode = "VND";
            payment.PaymentDetail[0] = detail;

            WebService.WSPaymentRQ.PaymentServicePortTypeClient client = new WebService.WSPaymentRQ.PaymentServicePortTypeClient();
            var data = client.PaymentServiceRQ(ref messageHeader, ref security, payment);
            return data;
        }
    }
}
