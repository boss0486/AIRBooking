using AIR.Helper.Session;
using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNAWSOTA_AirPriceLLSRQService
    {
        public AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRS FUNCOTA_AirPriceRQ(AirPriceModel model)
        {
           
            // handle
            //try
            //{
            AIRService.WebService.WSOTA_AirPriceLLSRQ.MessageHeader messageHeader = new AIRService.WebService.WSOTA_AirPriceLLSRQ.MessageHeader();
            messageHeader.MessageData = new AIRService.WebService.WSOTA_AirPriceLLSRQ.MessageData();
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new AIRService.WebService.WSOTA_AirPriceLLSRQ.Service();
            messageHeader.Action = "OTA_AirPriceLLSRQ";
            messageHeader.From = new AIRService.WebService.WSOTA_AirPriceLLSRQ.From();

            messageHeader.From.PartyId = new AIRService.WebService.WSOTA_AirPriceLLSRQ.PartyId[1];
            var partyID = new AIRService.WebService.WSOTA_AirPriceLLSRQ.PartyId();
            partyID.Value = "WebServiceClient";
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new AIRService.WebService.WSOTA_AirPriceLLSRQ.To();
            messageHeader.To.PartyId = new AIRService.WebService.WSOTA_AirPriceLLSRQ.PartyId[1];
            partyID = new AIRService.WebService.WSOTA_AirPriceLLSRQ.PartyId();
            partyID.Value = "WebServiceSupplier";
            messageHeader.To.PartyId[0] = partyID;

            AIRService.WebService.WSOTA_AirPriceLLSRQ.Security1 security = new AIRService.WebService.WSOTA_AirPriceLLSRQ.Security1();
            security.BinarySecurityToken = model.Token;

            var listCustomer = new List<AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType>();
            model.ADT = 1;
            if (model.ADT > 0)
            {
                var data = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType();
                data.Code = "ADT";
                data.Quantity = "1";
                listCustomer.Add(data);
            }
            //if (model.CNN > 0)
            //{
            //    var data = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType();
            //    data.Code = "CNN";
            //    data.Quantity = model.CNN.ToString();
            //    listCustomer.Add(data);
            //}
            //if (model.INF > 0)
            //{
            //    var data = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType();
            //    data.Code = "INF";
            //    data.Quantity = model.INF.ToString();
            //    //data.Force = true;
            //    //data.ForceSpecified = true;
            //    listCustomer.Add(data);
            //}
            AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQ oTA_AirPriceRQ = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQ();
            oTA_AirPriceRQ.PriceRequestInformation = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformation();
            oTA_AirPriceRQ.PriceRequestInformation.Retain = true;
            oTA_AirPriceRQ.PriceRequestInformation.RetainSpecified = true;
            oTA_AirPriceRQ.PriceRequestInformation.OptionalQualifiers = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiers();
            oTA_AirPriceRQ.PriceRequestInformation.OptionalQualifiers.PricingQualifiers = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiers();

           // var listCustomer = new List<AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType>();
            oTA_AirPriceRQ.PriceRequestInformation.OptionalQualifiers.PricingQualifiers.PassengerType = listCustomer.ToArray();
            //oTA_AirPriceRQ.PriceRequestInformation.OptionalQualifiers.PricingQualifiers.PassengerType =  new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType[1];

            //oTA_AirPriceRQ.PriceRequestInformation.OptionalQualifiers.PricingQualifiers.PassengerType[0].Code  = "ADT";
            //oTA_AirPriceRQ.PriceRequestInformation.OptionalQualifiers.PricingQualifiers.PassengerType[0].Quantity = "1";


            //var lSegment = new List<AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQFlightSegment>();
            //var rph = 0;
            //foreach (var item in model.Segments)
            //{
            //    rph++;
            //    var Segment = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQFlightSegment();
            //    Segment.ArrivalDateTime = item.ArrivalDateTime.ToString("yyyy-MM-dd'T'HH:mm");
            //    Segment.DepartureDateTime = item.DepartureDateTime.ToString("yyyy-MM-dd'T'HH:mm");
            //    Segment.FlightNumber = item.FlightNumber;
            //    Segment.ResBookDesigCode = item.ResBookDesigCode;
            //    Segment.RPH = rph.ToString();
            //    Segment.DestinationLocation = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQFlightSegmentDestinationLocation()
            //    {
            //        LocationCode = item.DestinationLocation
            //    };
            //    Segment.OriginLocation = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQFlightSegmentOriginLocation()
            //    {
            //        LocationCode = item.OriginLocation
            //    };
            //    Segment.MarketingCarrier = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQFlightSegmentMarketingCarrier()
            //    {
            //        Code = "VN",
            //        FlightNumber = item.FlightNumber
            //    };
            //    Segment.ConnectionInd = "O";
            //    lSegment.Add(Segment);
            //}
            //oTA_AirPriceRQ.ReturnHostCommand = true;
            //oTA_AirPriceRQ.ReturnHostCommandSpecified = true;
            //oTA_AirPriceRQ.OriginDestinationInformation = lSegment.ToArray();
            AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPricePortTypeClient client = new AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPricePortTypeClient();
            var result = client.OTA_AirPriceRQ(ref messageHeader, ref security, oTA_AirPriceRQ);
            //sessionService.CloseSession(_session);
            return result;
            //}
            //catch (Exception)
            //{
            //    sessionService.CloseSession(_session);
            //    return null;
            //}
        }
    }
}
