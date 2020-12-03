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
    [RoutePrefix("AreaInland")]
    public class AreaInlandController : CMSController
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
                AreaInlandService service = new AreaInlandService();
                AreaInland areaInland = service.GetAreaInlandByID(id);
                if (areaInland != null)
                    return View(areaInland);
            }

            return View();
        }

        public ActionResult Details(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                AreaInlandService service = new AreaInlandService();
                AreaInlandResult areaInlandResult = service.ViewAreaInlandByID(id);
                if (areaInlandResult != null)
                    return View(areaInlandResult);
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
                using (var service = new AreaInlandService())
                    return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(AreaInlandCreateModel model)
        {
            try
            {
                using (var service = new AreaInlandService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(AreaInlandUpdateModel model)
        {
            try
            {
                using (var flightService = new AreaInlandService())
                    return flightService.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(AreaInlandIDModel model)
        {
            try
            {
                using (var flightService = new AreaInlandService())
                    return flightService.Delete(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }


        [HttpPost]
        [Route("Action/GetAreaInland")]
        public ActionResult GetAreaInland(AreaInlandNationalIDModel model)
        {
            try
            {
                using (var flightService = new AreaInlandService())
                    return Notifization.Data(MessageText.Success, flightService.DataOption(nationalId: model.NationalID));
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
    }
}