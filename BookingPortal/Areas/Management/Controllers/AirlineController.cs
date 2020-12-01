using Helper;
using Helper.Page;
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
    [RoutePrefix("Airline")]
    public class AirlineController : CMSController
    {
        // GET: BackEnd/Airline
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
            AirlineService service = new AirlineService();
            Airline model = service.GetAirlineByID(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        public ActionResult Details(string id)
        {
            AirlineService service = new AirlineService();
            AirlineResult model = service.ViewAirlineByID(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                using (var service = new AirlineService())
                {
                    return service.DataList(model);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(AirlineCreateModel model)
        {
            try
            {
                using (var service = new AirlineService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(AirlineUpdateModel model)
        {
            try
            {
                using (var service = new AirlineService())
                    return service.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(AirlineIDModel model)
        {
            try
            {
                using (var service = new AirlineService())
                    return service.Delete(model);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }
    }
}