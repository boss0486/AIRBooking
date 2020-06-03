using AIRService.Service;
using ApiPortalBooking.Models;
using Notifies.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIBooking.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        // GET: Booking
        [HttpGet]
        //[Route("Api/[controller]/[action]")]
        public ActionResult Test()
        {
            //var a = new VNAService();
            //return a.TicketSearch(model);
            return Notifization.TEST("ok");
        }

        /// <summary>
        /// One way || Round trip
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
                return Notifization.NotService;
            }
        }

    }
}
