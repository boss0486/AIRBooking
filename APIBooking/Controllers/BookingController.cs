using AIRService.Service;
using AIRService.WS.Entities;
using ApiPortalBooking.Models;
using ApiPortalBooking.Models.VNA_WS_Model;
using Notifies.Helper;
using System;
using System.Web.Mvc;

namespace APIBooking.Controllers
{
    public class BookingController : Controller
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
        public ActionResult Search(FlightSearchModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.FlightSearch(model);
            }
            catch (Exception ex)
            {
                //return Notifization.NotService;
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
        public ActionResult FeeBase(FlightFareModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.FlightFee(model);
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
        [HttpPost]
        public ActionResult IFee(FlightFareModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.FlightFee(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        /// <summary>
        /// book 
        /// </summary>
        /// <param name="model">FlightSearchModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Book(BookModel model)
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
        public ActionResult ExTicket(BookVeResult model)
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

    }
}