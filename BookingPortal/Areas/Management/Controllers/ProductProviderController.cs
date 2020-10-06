using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helper;
using Helper.Page;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebApplication.Management.Controllers
{
    [IsManage]
    [RouteArea("Management")]
    [RoutePrefix("ProductProvider")]
    public class ProductProviderController : Controller
    {
        // GET: BackEnd/ProductProvider
        public ActionResult DataList()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Update()
        {
            return View();
        }
        public ActionResult Details()
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
                using (var service = new ProductProviderService())
                    return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(ProductProviderCreateModel model)
        {
            try
            {
                using (var service = new ProductProviderService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(ProductProviderUpdateModel model)
        {
            try
            {
                using (var service = new ProductProviderService())
                    return service.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }
        }
        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(ProductProviderIDModel model)
        {
            try
            {
                using (var service = new ProductProviderService())
                    return service.Delete(model.ID);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }

    }
}