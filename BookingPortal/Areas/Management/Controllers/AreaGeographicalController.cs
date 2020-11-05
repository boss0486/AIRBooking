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
    [RoutePrefix("AreaGeographical")]
    public class AreaGeographicalController : CMSController
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
            if (!string.IsNullOrWhiteSpace(id))
            {
                AreaGeographicalService service = new AreaGeographicalService();
                AreaGeographical areaGeographical = service.GetAreaGeographicalByID(id);
                if (areaGeographical != null)
                    return View(areaGeographical);
            }

            return View();
        }

        public ActionResult Details(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                AreaGeographicalService service = new AreaGeographicalService();
                AreaGeographicalResult areaGeographical = service.ViewAreaGeographicalByID(id);
                if (areaGeographical != null)
                    return View(areaGeographical);
            }
            return View();
        }
        //##########################################################################################################################################################################################################################################################


        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(AirportSearch model)
        {
            try
            {
                using (var service = new AreaGeographicalService())
                    return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(AreaGeographicalCreateModel model)
        {
            try
            {
                using (var service = new AreaGeographicalService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(AreaGeographicalUpdateModel model)
        {
            try
            {
                using (var flightService = new AreaGeographicalService())
                    return flightService.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(AreaGeographicalIDModel model)
        {
            try
            {
                using (var flightService = new AreaGeographicalService())
                    return flightService.Delete(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
    }
}