using AIRService.Entities;
using System;
//
namespace AIRService.WS.Service
{
    public class VNA_AirAvailLLSRQService
    {
        public AIRService.WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailRS FUNC_OTA_AirAvailLLSRQ(AirAvailLLSRQModel model)
        {
            WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailRQ oTA_AirAvailRQ = new WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailRQ();
            WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailPortTypeClient client = new WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailPortTypeClient();
            // header info
            WebService.WSOTA_AirAvailLLSRQ.MessageHeader messageHeader = new WebService.WSOTA_AirAvailLLSRQ.MessageHeader
            {
                MessageData = new WebService.WSOTA_AirAvailLLSRQ.MessageData()
            };
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new WebService.WSOTA_AirAvailLLSRQ.Service();
            messageHeader.Action = "OTA_AirAvailLLSRQ";
            messageHeader.From = new WebService.WSOTA_AirAvailLLSRQ.From
            {
                PartyId = new WebService.WSOTA_AirAvailLLSRQ.PartyId[1]
            };
            var partyID = new WebService.WSOTA_AirAvailLLSRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new WebService.WSOTA_AirAvailLLSRQ.To
            {
                PartyId = new WebService.WSOTA_AirAvailLLSRQ.PartyId[1]
            };
            partyID = new WebService.WSOTA_AirAvailLLSRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;

            //  header info
            WebService.WSOTA_AirAvailLLSRQ.Security1 security = new WebService.WSOTA_AirAvailLLSRQ.Security1
            {
                BinarySecurityToken = model.Token
            };

            //
            oTA_AirAvailRQ.OptionalQualifiers = new WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailRQOptionalQualifiers
            {
                AdditionalAvailability = new WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailRQOptionalQualifiersAdditionalAvailability
                {
                    Ind = true
                }
            };

            oTA_AirAvailRQ.OriginDestinationInformation = new WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformation
            {
                FlightSegment = new WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformationFlightSegment()
            };
            oTA_AirAvailRQ.OriginDestinationInformation.FlightSegment.DepartureDateTime = model.DepartureDateTime.ToString("MM-dd");

            oTA_AirAvailRQ.OriginDestinationInformation.FlightSegment.OriginLocation = new WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformationFlightSegmentOriginLocation
            {
                LocationCode = model.OriginLocation
            };

            oTA_AirAvailRQ.OriginDestinationInformation.FlightSegment.DestinationLocation = new WebService.WSOTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformationFlightSegmentDestinationLocation
            {
                LocationCode = model.DestinationLocation
            };
            var data = client.OTA_AirAvailRQ(ref messageHeader, ref security, oTA_AirAvailRQ);
            return data;
        }
    }
}
