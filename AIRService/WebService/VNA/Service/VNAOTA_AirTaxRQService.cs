using AIRService.WebService.VNA.Authen;
using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.Service
{
    class VNAOTA_AirTaxRQService
    {
        public AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRS GetTaxByFilght(Resquet_WsTaxModel model)
        {
            WebService.WSOTA_AirTaxRQ.MessageHeader messageHeader = new WebService.WSOTA_AirTaxRQ.MessageHeader
            {
                MessageData = new WebService.WSOTA_AirTaxRQ.MessageData()
            };
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new WebService.WSOTA_AirTaxRQ.Service();
            messageHeader.Action = "OTA_AirTaxRQ";
            messageHeader.From = new WebService.WSOTA_AirTaxRQ.From
            {
                PartyId = new WebService.WSOTA_AirTaxRQ.PartyId[1]
            };
            var partyID = new WebService.WSOTA_AirTaxRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new WebService.WSOTA_AirTaxRQ.To
            {
                PartyId = new WebService.WSOTA_AirTaxRQ.PartyId[1]
            };
            partyID = new WebService.WSOTA_AirTaxRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;
            //  header info
            WebService.WSOTA_AirTaxRQ.Security security = new WebService.WSOTA_AirTaxRQ.Security
            {
                BinarySecurityToken = model.Token
            };
            AIRService.WebService.WSOTA_AirTaxRQ.TaxPortTypeClient taxPortTypeClient = new AIRService.WebService.WSOTA_AirTaxRQ.TaxPortTypeClient();
            ////////  
            AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQ _airTaxRQ = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQ();
            _airTaxRQ.ItineraryInfos = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfo[1];
            //
            var _itineraryInfo = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfo();
            _itineraryInfo.ReservationItems = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItems();
            _itineraryInfo.ReservationItems.Item = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItem();
            _itineraryInfo.ReservationItems.Item.RPH = short.Parse(model.RPH);
            _itineraryInfo.ReservationItems.Item.SalePseudoCityCode = "LZJ";
            _itineraryInfo.ReservationItems.Item.TicketingCarrier = "VN";
            _itineraryInfo.ReservationItems.Item.ValidatingCarrier = "VN";
            //
            _airTaxRQ.POS = new WebService.WSOTA_AirTaxRQ.AirTaxRQPOS();
            _airTaxRQ.POS.Source = new WebService.WSOTA_AirTaxRQ.AirTaxRQPOSSource();
            _airTaxRQ.POS.Source.PseudoCityCode = "LZJ";
            //
            _itineraryInfo.ReservationItems.Item.FlightSegment = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegment[1];
            //
            var flightSegment = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegment();
            flightSegment.DepartureDateTime = model.DepartureDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            flightSegment.ArrivalDateTime = model.ArrivalDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            //
            flightSegment.FlightNumber = model.FlightNumber;
            flightSegment.ResBookDesigCode = model.ResBookDesigCode;
            flightSegment.ForceConnectionInd = true;
            flightSegment.ForceStopOverInd = true;
            //
            flightSegment.DepartureAirport = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentDepartureAirport();
            flightSegment.DepartureAirport.CodeContext = "IATA";
            flightSegment.DepartureAirport.LocationCode = model.OriginLocation;
            //
            flightSegment.ArrivalAirport = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentArrivalAirport();
            flightSegment.ArrivalAirport.CodeContext = "IATA";
            flightSegment.ArrivalAirport.LocationCode = model.DestinationLocation;
            //
            flightSegment.Equipment = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentEquipment();
            flightSegment.Equipment.AirEquipType = model.AirEquipType;
            flightSegment.MarketingAirline = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentMarketingAirline();
            flightSegment.MarketingAirline.Code = "VN";
            _itineraryInfo.ReservationItems.Item.FlightSegment[0] = flightSegment;
            //
            _itineraryInfo.ReservationItems.Item.AirFareInfo = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfo();
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdown();
            //
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerType = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerType();
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerType.Code = model.PassengerType;
            //
            //_itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.FareBasisCode = model.FareBasisCode;
            //
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerFare();
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare = new AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerFareBaseFare();
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.Amount = model.BaseFare.Amount;
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.CurrencyCode = model.BaseFare.CurrencyCode;
            //
            _airTaxRQ.ItineraryInfos[0] = _itineraryInfo;
            var data = taxPortTypeClient.TaxRQ(ref messageHeader, ref security, _airTaxRQ);
            return data;

        }
    }
}
