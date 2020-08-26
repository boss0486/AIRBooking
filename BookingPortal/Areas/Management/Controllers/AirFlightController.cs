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

    [IsManage(act: false)]
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
            AirAirport airAirport = new AirAirport();
            if (!string.IsNullOrWhiteSpace(id))
            {
                AirportService airportService = new AirportService();
                airAirport = airportService.GetAirAportModel(id);
            }
            return View(airAirport);
        }
        public ActionResult Details(string id)
        {
            AirAirport airAirport = new AirAirport();
            if (!string.IsNullOrWhiteSpace(id))
            {
                AirportService airportService = new AirportService();
                airAirport = airportService.GetAirAportModel(id);
            }
            return View(airAirport);
        }
        //##########################################################################################################################################################################################################################################################


        [HttpPost]
        [IsManage(act: true, text: "DataList")]
        [Route("Action/DataList")]
        public ActionResult DataList(AirportSearch model)
        {
            try
            {
                using (var flightService = new AirportService())
                    return flightService.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [IsManage(act: true, text: "Create")]
        [Route("Action/Create")]
        public ActionResult Create(AirportCreateModel model)
        {
            try
            {
                using (var flightService = new AirportService())
                    return flightService.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }

        [HttpPost]
        [IsManage(act: true, text: "Update")]
        [Route("Action/Update")]
        public ActionResult Update(AirportUpdateModel model)
        {
            try
            {
                using (var flightService = new AirportService())
                    return flightService.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }
    }
}