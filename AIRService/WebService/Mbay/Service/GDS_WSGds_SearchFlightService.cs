using AIRService.WebService.Mbay.Authen;
using AIRService.WebService.Mbay.WSGds_SearchFlightV2;
using AIRService.WS.Mbay.Entities;
using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WebService.Mbay.Service
{
    public class GDS_WSGds_SearchFlightService
    {
        public AIRService.WebService.Mbay.WSGds_SearchFlightV2.FareData[] FUNC_GDS_WSGds_SearchFlight(FlightSearchModel model)
        {
            GDS_WSAuthencation gDS_WSAuthencation = new GDS_WSAuthencation();
            AirDataWSSoapClient airDataWSSoapClient = new AirDataWSSoapClient();
            Authentication authentication = new Authentication();
            authentication.HeaderUser = gDS_WSAuthencation.HEADERUSER;
            authentication.HeaderPassword = gDS_WSAuthencation.HEADERPASSWORD;

            int itineraryType = 1;
            string departureAirportCode = model.OriginLocation;
            string destinationAirportCode = model.DestinationLocation;
            string departureDate = model.DepartureDateTime.ToString("dd/MM/yyyy");
            string returnDate = model.ReturnDateTime.ToString("dd/MM/yyyy");
            int adult = model.ADT;
            int children = model.CNN;
            int infant = model.CNN;
            var data = airDataWSSoapClient.Gds_SearchFlight(Authentication: authentication, Email: gDS_WSAuthencation.USERNAME, Password: gDS_WSAuthencation.PASSWORD,
                ItineraryType: itineraryType, DepartureAirportCode: departureAirportCode, DestinationAirportCode: destinationAirportCode, DepartureDate: departureDate,
                ReturnDate: returnDate, Adult: adult, Children: children, Infant: infant);
            return data;
        }

    }
}
