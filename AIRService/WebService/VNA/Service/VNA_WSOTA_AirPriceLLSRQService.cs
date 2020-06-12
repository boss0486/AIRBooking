using AIR.Helper.Session;
using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNA_WSOTA_AirPriceLLSRQService
    {
        public AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRS AirPrice(AirPriceModel model)
        {
            try
            {
                WebService.WSOTA_AirPriceLLSRQ.MessageHeader messageHeader = new WebService.WSOTA_AirPriceLLSRQ.MessageHeader
                {
                    MessageData = new WebService.WSOTA_AirPriceLLSRQ.MessageData()
                };
                messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
                messageHeader.ConversationId = model.ConversationID;
                messageHeader.Service = new WebService.WSOTA_AirPriceLLSRQ.Service();
                messageHeader.Action = "OTA_AirPriceLLSRQ";
                messageHeader.From = new WebService.WSOTA_AirPriceLLSRQ.From
                {
                    PartyId = new WebService.WSOTA_AirPriceLLSRQ.PartyId[1]
                };
                var partyID = new WebService.WSOTA_AirPriceLLSRQ.PartyId
                {
                    Value = "WebServiceClient"
                };
                messageHeader.From.PartyId[0] = partyID;
                //
                messageHeader.To = new WebService.WSOTA_AirPriceLLSRQ.To
                {
                    PartyId = new WebService.WSOTA_AirPriceLLSRQ.PartyId[1]
                };
                partyID = new WebService.WSOTA_AirPriceLLSRQ.PartyId
                {
                    Value = "WebServiceSupplier"
                };
                messageHeader.To.PartyId[0] = partyID;
                //
                WebService.WSOTA_AirPriceLLSRQ.Security1 security = new WebService.WSOTA_AirPriceLLSRQ.Security1
                {
                    BinarySecurityToken = model.Token
                };
                var listCustomer = new List<WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType>();
                if (model.ADT > 0)
                {
                    var data = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType
                    {
                        Code = "ADT",
                        Quantity = model.ADT.ToString()
                    };
                    listCustomer.Add(data);
                }
                if (model.CNN > 0)
                {
                    var data = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType
                    {
                        Code = "CNN",
                        Quantity = model.CNN.ToString()
                    };
                    listCustomer.Add(data);
                }
                if (model.INF > 0)
                {
                    var data = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiersPassengerType
                    {
                        Code = "INF",
                        Quantity = model.INF.ToString()
                    };
                    listCustomer.Add(data);
                }
                //
                WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQ oTA_AirPriceRQ = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQ
                {
                    PriceRequestInformation = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformation()
                };
                oTA_AirPriceRQ.PriceRequestInformation.Retain = true;
                oTA_AirPriceRQ.PriceRequestInformation.RetainSpecified = true;
                oTA_AirPriceRQ.PriceRequestInformation.OptionalQualifiers = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiers
                {
                    PricingQualifiers = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQPriceRequestInformationOptionalQualifiersPricingQualifiers()
                };
                oTA_AirPriceRQ.PriceRequestInformation.OptionalQualifiers.PricingQualifiers.PassengerType = listCustomer.ToArray();
                var lSegment = new List<WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQFlightSegment>();
                var rph = 0;
                foreach (var item in model.lFlight)
                {
                    rph++;
                    var Segment = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQFlightSegment
                    {
                        ArrivalDateTime = item.ArrivalDateTime.ToString("yyyy-MM-dd'T'HH:mm"),
                        DepartureDateTime = item.DepartureDateTime.ToString("yyyy-MM-dd'T'HH:mm"),
                        FlightNumber = item.FlightNumber,
                        ResBookDesigCode = item.ResBookDesigCode,
                        RPH = rph.ToString(),
                        DestinationLocation = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQFlightSegmentDestinationLocation()
                        {
                            LocationCode = item.DestinationLocation
                        },
                        OriginLocation = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQFlightSegmentOriginLocation()
                        {
                            LocationCode = item.OriginLocation
                        },
                        MarketingCarrier = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRQFlightSegmentMarketingCarrier()
                        {
                            Code = "VN",
                            FlightNumber = item.FlightNumber
                        },
                        ConnectionInd = "O"
                    };
                    lSegment.Add(Segment);
                }
                oTA_AirPriceRQ.ReturnHostCommand = true;
                oTA_AirPriceRQ.ReturnHostCommandSpecified = true;
                oTA_AirPriceRQ.OriginDestinationInformation = lSegment.ToArray();
                oTA_AirPriceRQ.TimeStamp = DateTime.Now;
                oTA_AirPriceRQ.TimeStampSpecified = true;
                WebService.WSOTA_AirPriceLLSRQ.OTA_AirPricePortTypeClient client = new WebService.WSOTA_AirPriceLLSRQ.OTA_AirPricePortTypeClient();
                var result = client.OTA_AirPriceRQ(ref messageHeader, ref security, oTA_AirPriceRQ);
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
