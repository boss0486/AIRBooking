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
    [RoutePrefix("National")]
    public class NationalController : CMSController
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
                NationalService service = new NationalService();
                National National = service.GetNationalByID(id);
                if (National != null)
                    return View(National);
            }

            return View();
        }

        public ActionResult Details(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                NationalService service = new NationalService();
                NationalResult National = service.ViewNationalByID(id);
                if (National != null)
                    return View(National);
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
                using (var service = new NationalService())
                    return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(NationalCreateModel model)
        {
            try
            {
                using (var service = new NationalService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(NationalUpdateModel model)
        {
            try
            {
                using (var flightService = new NationalService())
                    return flightService.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }

        }


        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(NationalIDModel model)
        {
            try
            {
                using (var flightService = new NationalService())
                    return flightService.Delete(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
    }
}