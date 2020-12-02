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
    [RoutePrefix("AreaNational")]
    public class AreaNationalController : CMSController
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
                AreaNationalService service = new AreaNationalService();
                AreaNational areaNational = service.GetAreaNationalByID(id);
                if (areaNational != null)
                    return View(areaNational);
            }

            return View();
        }

        public ActionResult Details(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                AreaNationalService service = new AreaNationalService();
                AreaNationalResult areaNational = service.ViewAreaNationalByID(id);
                if (areaNational != null)
                    return View(areaNational);
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
                using (var service = new AreaNationalService())
                    return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(AreaNationalCreateModel model)
        {
            try
            {
                using (var service = new AreaNationalService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(AreaNationalUpdateModel model)
        {
            try
            {
                using (var flightService = new AreaNationalService())
                    return flightService.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(AreaNationalIDModel model)
        {
            try
            {
                using (var flightService = new AreaNationalService())
                    return flightService.Delete(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
    }
}