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
    [RoutePrefix("AirResBookDesig")]
    public class AirResBookDesigController : CMSController
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
        public ActionResult Setting(string id)
        {
            AirResBookDesigSetting airportSetting = new AirResBookDesigSetting();
            if (!string.IsNullOrWhiteSpace(id))
            {
                AirResBookDesigService airportService = new AirResBookDesigService();
                airportSetting = airportService.GetAirResBookDesigModel(id);
            }
            return View(airportSetting);
        }
        public ActionResult Update(string id)
        {
            AirResBookDesig airAirResBookDesig = new AirResBookDesig();
            if (!string.IsNullOrWhiteSpace(id))
            {
                AirResBookDesigService airportService = new AirResBookDesigService();
                airAirResBookDesig = airportService.GetAirAportModel(id);
            }
            return View(airAirResBookDesig);
        }

        public ActionResult Details(string id)
        {
            AirResBookDesigResult airAirResBookDesig = new AirResBookDesigResult();
            if (!string.IsNullOrWhiteSpace(id))
            {
                AirResBookDesigService airportService = new AirResBookDesigService();
                airAirResBookDesig = airportService.ViewAirAportModel(id);
            }
            return View(airAirResBookDesig);
        }

        public ActionResult ExSetting()
        {
            return View();
        }

        //##########################################################################################################################################################################################################################################################


        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                using (var service = new AirResBookDesigService())
                    return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }


        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(AirResBookDesigCreateModel model)
        {
            try
            {
                using (var service = new AirResBookDesigService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(AirResBookDesigUpdateModel model)
        {
            try
            {

                using (var service = new AirResBookDesigService())
                    return service.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(AirResBookDesigIDModel model)
        {
            try
            {
                using (var flightService = new AirResBookDesigService())
                    return flightService.Delete(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Setting")]
        public ActionResult Setting(AirResBookDesigSetting model)
        {
            try
            {
                using (var service = new AirResBookDesigService())
                    return service.Setting(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        } 
    }
}