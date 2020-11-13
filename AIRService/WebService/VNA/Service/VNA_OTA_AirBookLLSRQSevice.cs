using AIR.Helper.Session;
using AIRService.WS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    public class VNA_OTA_AirBookLLSRQSevice
    {
        public AIRService.WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRS FUNC_OTA_AirBookRS(AirBookModel model)
        {
            //SessionService sessionService = new SessionService();
            //var _session = sessionService.GetSession();
            //if (_session == null)
            //    return null;
            //// get token
            string _token = model.Token;
            string _conversationId = model.ConversationID;
            //if (string.IsNullOrWhiteSpace(_token))
            //    return null;
            //handle
             
                // header info
                WebService.VNA_OTA_AirBookLLSRQ.MessageHeader messageHeader = new WebService.VNA_OTA_AirBookLLSRQ.MessageHeader();
                messageHeader.MessageData = new WebService.VNA_OTA_AirBookLLSRQ.MessageData();
                messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
                messageHeader.ConversationId = _conversationId;
                messageHeader.Service = new WebService.VNA_OTA_AirBookLLSRQ.Service();
                messageHeader.Action = "OTA_AirBookLLSRQ";
                messageHeader.From = new WebService.VNA_OTA_AirBookLLSRQ.From();

                messageHeader.From.PartyId = new WebService.VNA_OTA_AirBookLLSRQ.PartyId[1];
                var partyID = new WebService.VNA_OTA_AirBookLLSRQ.PartyId();
                partyID.Value = "WebServiceClient";
                messageHeader.From.PartyId[0] = partyID;

                messageHeader.To = new WebService.VNA_OTA_AirBookLLSRQ.To();
                messageHeader.To.PartyId = new WebService.VNA_OTA_AirBookLLSRQ.PartyId[1];
                partyID = new WebService.VNA_OTA_AirBookLLSRQ.PartyId();
                partyID.Value = "WebServiceSupplier";
                messageHeader.To.PartyId[0] = partyID;

                WebService.VNA_OTA_AirBookLLSRQ.Security1 security = new WebService.VNA_OTA_AirBookLLSRQ.Security1();
                security.BinarySecurityToken = _token
                    ;

                var oTA_AirBookRQ = new WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRQ();
                List<WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegment> lstSegments = new List<WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegment>();
                foreach (var item in model.Segments)
                {
                    var segment = new WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegment();
                    segment.DepartureDateTime = item.DepartureDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss");
                    segment.ArrivalDateTime = item.ArrivalDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss");
                    segment.FlightNumber = Convert.ToString(item.FlightNumber);
                    segment.ResBookDesigCode = item.ResBookDesigCode;
                    segment.NumberInParty = Convert.ToString(item.NumberInParty);
                    segment.Status = "NN";
                    segment.Equipment = new WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentEquipment();
                    segment.Equipment.AirEquipType = Convert.ToString(item.AirEquipType);
                    segment.OriginLocation = new WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentOriginLocation();
                    segment.OriginLocation.LocationCode = item.OriginLocation;
                    segment.DestinationLocation = new WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentDestinationLocation();
                    segment.DestinationLocation.LocationCode = item.DestinationLocation;
                    segment.MarketingAirline = new WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentMarketingAirline();
                    segment.MarketingAirline.Code = "VN";
                    segment.MarketingAirline.FlightNumber = Convert.ToString(item.FlightNumber);
                    segment.OperatingAirline = new WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRQFlightSegmentOperatingAirline();
                    segment.OperatingAirline.Code = "VN";
                    lstSegments.Add(segment);
                }
                oTA_AirBookRQ.OriginDestinationInformation = lstSegments.ToArray();
                WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookPortTypeClient client = new WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookPortTypeClient();
                var data = client.OTA_AirBookRQ(ref messageHeader, ref security, oTA_AirBookRQ);
                // close session *******************************************************************************************************************************************
                //sessionService.CloseSession(_session);
                // return data *********************************************************************************************************************************************
                return data;
            
        }
    }
}
