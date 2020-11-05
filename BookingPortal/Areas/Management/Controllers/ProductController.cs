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
    [RoutePrefix("Product")]
    public class ProductController : CMSController
    {
        public ActionResult DataList()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Update(string id)
        {
            ProductService service = new ProductService();
            Product model = service.GetProductByID(id);
            if (model != null)
                return View(model);
            //
            return View(model);
        }
        public ActionResult Detail(string id)
        {
            ProductService service = new ProductService();
            Product model = service.GetProductByID(id);
            if (model != null)
                return View(model);
            //
            return View(model);
        }
        //##########################################################################################################################################################################################################################################################
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(ProductSearchModel model)
        {
            try
            {
                using (var productService = new ProductService())
                    return productService.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Create")]
        public ActionResult Create(ProductCreateModel model)
        {
            try
            {
                using (var service = new ProductService())
                    return service.Create(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Update")]
        public ActionResult Update(ProductUpdateModel model)
        {
            try
            {
                using (var service = new ProductService())
                    return service.Update(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/Delete")]
        public ActionResult Delete(ProductIDModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.Invalid();
                using (var productService = new ProductService())
                    return productService.Delete(model.ID);
            }
            catch (Exception)
            {
                return Notifization.NotService;
            }
        }
    }
}