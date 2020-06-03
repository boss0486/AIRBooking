using AIRService.Service;
using ApiPortalBooking.Models;
using Notifies.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIRApplication.Controllers
{
    public class BookingController : Controller
    {
        // POST: Booking
        [HttpPost]
        //[Route("Api/[controller]/[action]")]
        public ActionResult Search(SearchFlightModel model)
        {
            //var a = new VNAService();
            //return a.TicketSearch(model);
            return Notifization.TEST("ok");

        }
    }
}