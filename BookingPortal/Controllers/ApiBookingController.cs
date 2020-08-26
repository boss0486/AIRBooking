using AIR.Helper.Session;
using AIRService.Service;
using AIRService.WebService.VNA.Authen;
using AIRService.WS.Entities;
using ApiPortalBooking.Models;
using ApiPortalBooking.Models.VNA_WS_Model;
using Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebCore.Entities;
using WebCore.Services;

namespace APIBooking.Controllers
{
    public class ApiBookingController : Controller
    {
        // GET: Booking
        [HttpGet]
        public ActionResult Test()
        {
            //var a = new VNAService();
            //return a.TicketSearch(model);
            return Notifization.TEST("ok");
        }

        /// <summary>
        /// search =>> One way || Round trip
        /// </summary>
        /// <param name="model">FlightSearchModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(Request_FlightSearchModel model)
        {
            try
            {
                //
                bool _isRoundTrip = model.IsRoundTrip;
                string _destinationLocation = model.DestinationLocation;
                string _originLocation = model.OriginLocation;
                string _departureDateTime = model.DepartureDateTime;
                //
                string _returnDateTimeTemp = model.ReturnDateTime;

                if (string.IsNullOrWhiteSpace(_departureDateTime) || !Helper.Page.Validate.TestDate_MMDDYYYY(_departureDateTime))
                    return Notifization.Invalid("Departure date invalid, format: MM/dd/yyyy" + _departureDateTime);
                //
                string _returnDateTime = DateTime.Now.Date.AddDays(-1).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (model.IsRoundTrip)
                {
                    if (string.IsNullOrWhiteSpace(_returnDateTimeTemp) || !Helper.Page.Validate.TestDate_MMDDYYYY(_returnDateTimeTemp.ToString()))
                        return Notifization.Invalid("Return date invalid, format: MM/dd/yyyy");
                    //
                    _returnDateTime = _returnDateTimeTemp;
                }
                //

                HttpContext.Response.Cookies.Add(new HttpCookie("FlightSearch", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new Request_FlightSearchModel
                {
                    OriginLocation = _originLocation,
                    DestinationLocation = _destinationLocation,
                    DepartureDateTime = Convert.ToDateTime(_departureDateTime).ToString("dd-MM-yyyy"),
                    ReturnDateTime = Convert.ToDateTime(_returnDateTime).ToString("dd-MM-yyyy"),
                    ADT = model.ADT,
                    CNN = model.CNN,
                    INF = model.INF,
                    IsRoundTrip = model.IsRoundTrip

                })));
                //
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.FlightSearch(new FlightSearchModel
                {
                    OriginLocation = _originLocation,
                    DestinationLocation = _destinationLocation,
                    DepartureDateTime = Convert.ToDateTime(_departureDateTime),
                    ReturnDateTime = Convert.ToDateTime(_returnDateTime),
                    ADT = model.ADT,
                    CNN = model.CNN,
                    INF = model.INF,
                    IsRoundTrip = model.IsRoundTrip
                });
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        /// <summary>
        /// One way || Round trip
        /// </summary>
        /// <param name="model">FlightSearchModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Cost(FlightSearchModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.FlightCost(model);
            }
            catch (Exception ex)
            {
                //return Notifization.NotService;
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        /// <summary>
        /// Fee basic
        /// </summary>
        /// <param name="model">FlightFareModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FeeBase(List<FlightFareModel> models)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.TaxFee(models);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }
        [HttpPost]
        public ActionResult FeeBase1(List<FlightFareModel> models)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.TaxFeeTest(models);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }



        [HttpPost]
        public ActionResult GetFeeBasic(List<FeeTaxBasicModel> models)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.FlightFeeBasic(models);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        /// <summary>
        /// Internal fee
        /// </summary>
        /// <param name="model">FlightSearchModel</param>
        /// <returns></returns>

        /// <summary>
        /// book 
        /// </summary>
        /// <param name="model">FlightSearchModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Book(Request_BookModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.BookVe(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        [HttpPost]
        public ActionResult ExTicket(PNRModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.ReleaseTicket(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        [HttpPost]
        public ActionResult TicketInfo(PNRModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.TicketInfo(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        [HttpPost]
        public ActionResult TicketCancel(PNRModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.VoidTicket(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        // API MAY BAY ********************************************************************************************************************************************
        // API MAY BAY ********************************************************************************************************************************************
        [HttpPost]
        public ActionResult GdsSearch(FlightSearchModel model)
        {
            try
            {
                AIRService.WebService.Mbay.Service.GDS_WSGds_SearchFlightService gds_WSGds_SearchFlightService = new AIRService.WebService.Mbay.Service.GDS_WSGds_SearchFlightService();
                //return gds_WSGds_SearchFlightService.FUNC_GDS_WSGds_SearchFlight(model);

                return Notifization.Data("", gds_WSGds_SearchFlightService.FUNC_GDS_WSGds_SearchFlight(model));
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }
        // API MAY BAY ********************************************************************************************************************************************
        // API MAY BAY ********************************************************************************************************************************************


        public ActionResult Order(string id)
        {
            try
            {
                var service = new BookTicketService();
                return Notifization.Data(":::", service.BookTicketDetails(id));
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        [HttpGet]
        public ActionResult TestLogin()
        {
            try
            {
                VNA_WSAuthencation wSAuthencation = new VNA_WSAuthencation();
                VNA_SessionService vNA_SessionService = new VNA_SessionService();
                var service = new BookTicketService();
                return Notifization.Data(":::", vNA_SessionService.GetSession());
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

    }
}