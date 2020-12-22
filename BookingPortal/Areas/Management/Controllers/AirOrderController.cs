using AIR.Helper.Session;
using AIRService.Service;
using AIRService.WebService.VNA.Authen;
using AIRService.WS.Entities;
using ApiPortalBooking.Models;
using ApiPortalBooking.Models.VNA_WS_Model;
using Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Services;

namespace WebApplication.Management.Controllers
{
    [IsManage]
    [RouteArea("Management")]
    [RoutePrefix("AirOrder")]
    public class AirOrderController : CMSController
    {

        public ActionResult DataList()
        {
            return View();
        }
        public ActionResult Details(string id)
        {
            BookOrderService bookOrderService = new BookOrderService();
            ViewBookOrder model = bookOrderService.ViewBookOrderByID(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(BookOrderSerch model)
        {
            try
            {
                using (var service = new BookOrderService())
                    return service.OrederList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/BookingList")]
        public ActionResult BookingList(BookOrderSerch model)
        {
            try
            {
                using (var service = new BookOrderService())
                    return service.BookingList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }



        // Data ****************************************************************************************************
        [HttpPost]
        [Route("Action/BookEditPrice")]
        public ActionResult BookEditPrice(BookEditPriceModel model)
        {
            BookOrderService bookOrderService = new BookOrderService();
            return bookOrderService.BookEditPrice(model);
        }
        [HttpPost]
        [Route("Action/BookEditAgentFee")]
        public ActionResult BookEditAgentFee(BookEditFeeModel model)
        {
            BookOrderService bookOrderService = new BookOrderService();
            return bookOrderService.BookEditAgentFee(model);
        }
        [HttpPost]
        [Route("Action/BookEmail")]
        public ActionResult BookEmail(BookOrderIDModel model)
        {
            try
            {
                using (var service = new BookOrderService())
                    return service.BookEmail(model.ID);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        // Data ****************************************************************************************************

        [HttpPost]
        [Route("Action/GetPassenger")]
        public ActionResult GetPassenger(BookOrderIDModel model)
        {
            try
            {
                BookOrderService service = new BookOrderService();
                return Notifization.Data(MessageText.Success, service.ViewBookPassenger(model.ID));
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }

        [HttpPost]
        [Route("Action/GetFaseBasic")]
        public ActionResult GetFaseBasic(BookOrderIDModel model)
        {
            try
            {
                BookOrderService service = new BookOrderService();
                return Notifization.Data(MessageText.Success, service.ViewBookFareBasic(model.ID));
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }


        [HttpPost]
        [Route("Action/BookingExport")]
        public ActionResult BookingExport(BookOrderSerch model)
        {
            try
            {
                BookOrderService service = new BookOrderService();
                return service.BookingExport(model);
            }
            catch (Exception ex)
            {
                return Notifization.Error("Error: " + ex);
            }
        }
        [HttpPost]
        [Route("Action/OrderExport")]
        public ActionResult OrderExport(BookOrderSerch model)
        {
            try
            {
                BookOrderService service = new BookOrderService();
                return service.OrderExport(model);
            }
            catch (Exception ex)
            {
                return Notifization.Error("Error: " + ex);
            }
        }

    }
}