using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebApplication.Management.Controllers
{
    [IsManage]
    [RouteArea("Management")]
    [RoutePrefix("AirFlight")]
    public class AirFlightController : CMSController
    {
        // GET: Backend/Fight
        public ActionResult DataList()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Update(string id)
        {
            AirportResult airAirport = new AirportResult();
            if (!string.IsNullOrWhiteSpace(id))
            {
                AirportService airportService = new AirportService();
                airAirport = airportService.GetAirAportModel(id);
            }
            return View(airAirport);
        }

        public ActionResult Details(string id)
        {
            AirportResult airAirport = new AirportResult();
            if (!string.IsNullOrWhiteSpace(id))
            {
                AirportService airportService = new AirportService();
                airAirport = airportService.GetAirAportModel(id);
            }
            return View(airAirport);
        }
        //##########################################################################################################################################################################################################################################################


        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(AirportSearch model)
        {
            try
            {
                using (var service = new AirportService())
                    return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(AirportCreateModel model)
        {
            try
            {
                using (var service = new AirportService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(AirportUpdateModel model)
        {
            try
            {

                using (var service = new AirportService())
                    return service.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(AirportIDModel model)
        {
            try
            {
                using (var flightService = new AirportService())
                    return flightService.Delete(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
    }
}