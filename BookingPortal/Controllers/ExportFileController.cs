using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebCore.Entities;
using WebCore.Services;

namespace BookingPortal.Controllers
{
    public class ExportFileController : Controller
    {
        // GET: ExportFile
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExOrder(string id)
        {
            BookOrderService bookOrderService = new BookOrderService();
            ViewBookOrder model = bookOrderService.ViewBookOrderByID(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
    }
}