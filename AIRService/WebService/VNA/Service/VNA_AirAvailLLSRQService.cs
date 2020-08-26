using AIRService.Entities;
using System;
//
namespace AIRService.WS.Service
{
    public class VNA_AirAvailLLSRQService
    {
        public AIRService.WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRS FUNC_OTA_AirAvailLLSRQ(AirAvailLLSRQModel model)
        {
            WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQ oTA_AirAvailRQ = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQ();
            WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailPortTypeClient client = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailPortTypeClient();
            // header info
            WebService.VNA_OTA_AirAvailLLSRQ.MessageHeader messageHeader = new WebService.VNA_OTA_AirAvailLLSRQ.MessageHeader
            {
                MessageData = new WebService.VNA_OTA_AirAvailLLSRQ.MessageData()
            };
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new WebService.VNA_OTA_AirAvailLLSRQ.Service();
            messageHeader.Action = "OTA_AirAvailLLSRQ";
            messageHeader.From = new WebService.VNA_OTA_AirAvailLLSRQ.From
            {
                PartyId = new WebService.VNA_OTA_AirAvailLLSRQ.PartyId[1]
            };
            var partyID = new WebService.VNA_OTA_AirAvailLLSRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new WebService.VNA_OTA_AirAvailLLSRQ.To
            {
                PartyId = new WebService.VNA_OTA_AirAvailLLSRQ.PartyId[1]
            };
            partyID = new WebService.VNA_OTA_AirAvailLLSRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;

            //  header info
            WebService.VNA_OTA_AirAvailLLSRQ.Security1 security = new WebService.VNA_OTA_AirAvailLLSRQ.Security1
            {
                BinarySecurityToken = model.Token
            };

            //
            oTA_AirAvailRQ.OptionalQualifiers = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOptionalQualifiers
            {
                AdditionalAvailability = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOptionalQualifiersAdditionalAvailability
                {
                    Ind = true
                }
            };

            oTA_AirAvailRQ.OriginDestinationInformation = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformation
            {
                FlightSegment = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformationFlightSegment()
            };
            oTA_AirAvailRQ.OriginDestinationInformation.FlightSegment.DepartureDateTime = model.DepartureDateTime.ToString("MM-dd");

            oTA_AirAvailRQ.OriginDestinationInformation.FlightSegment.OriginLocation = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformationFlightSegmentOriginLocation
            {
                LocationCode = model.OriginLocation
            };

            oTA_AirAvailRQ.OriginDestinationInformation.FlightSegment.DestinationLocation = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformationFlightSegmentDestinationLocation
            {
                LocationCode = model.DestinationLocation
            };
            var data = client.OTA_AirAvailRQ(ref messageHeader, ref security, oTA_AirAvailRQ);
            return data;
        }
    }
}
