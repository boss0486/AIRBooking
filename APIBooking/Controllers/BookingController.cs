using AIRService.Service;
using AIRService.WS.Entities;
using ApiPortalBooking.Models;
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
                var vNASearchService = new VNASearchService();
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
                var vNASearchService = new VNASearchService();
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
                var vNASearchService = new VNASearchService();
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
                var vNASearchService = new VNASearchService();
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
                var vNASearchService = new VNASearchService();
                return vNASearchService.FlightBook(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex); 
                //return Notifization.NotService;
            }
        }

    }
}