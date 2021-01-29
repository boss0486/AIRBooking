﻿using AIR.Helper.Session;
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
    [RoutePrefix("AirBook")]
    public class AirBookController : CMSController
    {
        public ActionResult Search()
        {
            SegmentSearchModel model = new SegmentSearchModel
            {
                OriginLocation = " ",
                DestinationLocation = " ",
                DepartureDateTime = "",
                ReturnDateTime = "",
                ADT = 1,
                CNN = 0,
                INF = 0,
                IsRoundTrip = true
            };
            try
            {
                //do something
                HttpCookie flightSearch = HttpContext.Request.Cookies["FlightSearch"];
                if (flightSearch != null)
                {
                    var dataSearch = flightSearch.Value;
                    model = JsonConvert.DeserializeObject<SegmentSearchModel>(dataSearch);
                }
            }
            catch
            {
                //
            }
            return View(model);
        }

        public ActionResult Booking()
        {
            List<FlightPassengerTypeInfo> flightPassengerTypeInfos = new List<FlightPassengerTypeInfo>();
            try
            {
                //do something
                HttpCookie flightSearch = HttpContext.Request.Cookies["FlightSearch"];
                var dataSearch = flightSearch.Value;
                var model = JsonConvert.DeserializeObject<SegmentSearchModel>(dataSearch);
                if (model != null)
                {
                    if (model.ADT > 0)
                    {
                        flightPassengerTypeInfos.Add(new FlightPassengerTypeInfo
                        {
                            PassengerType = "ADT",
                            PassengerName = "Người lớn",
                            Quantity = model.ADT
                        });
                    }
                    if (model.CNN > 0)
                    {
                        flightPassengerTypeInfos.Add(new FlightPassengerTypeInfo
                        {
                            PassengerType = "CNN",
                            PassengerName = "Trẻ em",
                            Quantity = model.CNN
                        });
                    }
                    if (model.INF > 0)
                    {
                        flightPassengerTypeInfos.Add(new FlightPassengerTypeInfo
                        {
                            PassengerType = "INF",
                            PassengerName = "Em bé",
                            Quantity = model.INF
                        });
                    }
                }
            }
            catch
            {
                //
            }
            return View(flightPassengerTypeInfos);
        }
        public ActionResult DataList()
        {
            return View();
        }

        [IsManage(skip: true)]
        public ActionResult Details(string id)
        {
            BookOrderService bookOrderService = new BookOrderService();
            ViewBookOrder model = bookOrderService.ViewBookOrderByID(id);
            if (model != null)
                return View(model);
            //
            return View();
        }
        // api ******************************************************************************************************************************

        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult BookList(BookOrderSearch model)
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

        [HttpPost]
        [Route("Action/GetCompany")]
        public ActionResult GetCompany()
        {
            CompanyService companyService = new CompanyService(); 
            return Notifization.Data("", companyService.DataOption());
        }

        [HttpPost]
        [Route("Action/GetCompByAgentID")]
        public ActionResult GetCompByAgentID(BookAgentID bookAgentId)
        {
            CompanyService companyService = new CompanyService();
            List<ClientOption> dtList = companyService.GetCompByAgentID(bookAgentId.AgentID);
            return Notifization.Data("", dtList);
        }
        // GET: Booking ******************************************************************************************************************************



        [HttpPost]
        [Route("Action/Test")]
        public ActionResult Test(FareLLSModel model)
        {
            VNA_SearchService searchService = new VNA_SearchService();
            return searchService.Test01(model);
        }

        /// <summary>
        /// search =>> One way || Round trip
        /// </summary>
        /// <param name="model">FlightSearchModel</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Route("Action/Search")]
        public ActionResult Search(SegmentSearchModel model)
        {
            try
            {
                //
                bool _isRoundTrip = model.IsRoundTrip;
                string _destinationLocation = model.DestinationLocation;
                string _originLocation = model.OriginLocation;
                string _departureDateTime = model.DepartureDateTime;
                //
                string _returnDateTimeTemp = model.ReturnDateTime;
                bool isHasTax = model.IsHasTax;
                int itineraryType = model.ItineraryType;

                if (string.IsNullOrWhiteSpace(_departureDateTime) || !Helper.Page.Validate.TestDate_MMDDYYYY(_departureDateTime))
                    return Notifization.Invalid("Departure date invalid, format: MM/dd/yyyy" + _departureDateTime);
                //
                string _returnDateTime = DateTime.Now.Date.AddDays(-1).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                if (model.IsRoundTrip)
                {
                    if (string.IsNullOrWhiteSpace(_returnDateTimeTemp) || !Helper.Page.Validate.TestDate_MMDDYYYY(_returnDateTimeTemp.ToString()))
                        return Notifization.Invalid("Return date invalid, format: MM/dd/yyyy");
                    //
                    _returnDateTime = _returnDateTimeTemp;
                }
                //
                HttpContext.Response.Cookies.Add(new HttpCookie("FlightSearch", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new SegmentSearchModel
                {
                    OriginLocation = _originLocation,
                    DestinationLocation = _destinationLocation,
                    DepartureDateTime = Helper.TimeData.TimeFormat.FormatToViewDate(_departureDateTime, Helper.Language.LanguagePage.GetLanguageCode),
                    ReturnDateTime = Helper.TimeData.TimeFormat.FormatToViewDate(_returnDateTime, Helper.Language.LanguagePage.GetLanguageCode),
                    ADT = model.ADT,
                    CNN = model.CNN,
                    INF = model.INF,
                    IsRoundTrip = model.IsRoundTrip

                })));
                //
                var vnaSearchService = new VNA_SearchService();
                return vnaSearchService.BookSearch(new FlightSearchModel
                {
                    OriginLocation = _originLocation,
                    DestinationLocation = _destinationLocation,
                    DepartureDateTime = Convert.ToDateTime(_departureDateTime),
                    ReturnDateTime = Convert.ToDateTime(_returnDateTime),
                    ADT = model.ADT,
                    CNN = model.CNN,
                    INF = model.INF,
                    IsRoundTrip = model.IsRoundTrip,
                    IsHasTax = isHasTax,
                    ItineraryType = itineraryType
                });
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        /// <summary>
        /// One way || Round trip
        /// </summary>
        /// <param name="model">FlightSearchModel</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Action/Cost")]
        public ActionResult Cost(FlightSearchModel model)
        {
            try
            {
                var vnaSearchService = new VNA_SearchService();
                return vnaSearchService.FlightCost(model);
            }
            catch (Exception ex)
            {
                //return Notifization.NotService;
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/GetFeeBasic")]
        public ActionResult GetFeeBasic(List<TaxFeeModel> models)
        {
            try
            {
                var vnaSearchService = new VNA_SearchService();
                return vnaSearchService.FlightFeeBasic(models);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        /// <summary>
        /// book 
        /// </summary>
        /// <param name="model">FlightSearchModel</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Action/Booking")]
        public ActionResult TicketOrder(BookOrderSaveModel model)
        {
            try
            {
                var vnaSearchService = new VNA_SearchService();
                return vnaSearchService.BookOrderSave(model);
            }
            catch (Exception ex)
            {
                Helper.SystemLogg.WriteLog("Booking: " + ex);
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/ExTicket")]
        public ActionResult ExTicket(BookOrderIDModel model)
        {
            try
            {
                var vnaSearchService = new VNA_SearchService();
                return vnaSearchService.CheckReleaseTicket(model);
            }
            catch (Exception ex)
            {
                Helper.SystemLogg.WriteLog("ExTicket: " + ex);
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }
         
        [HttpPost]
        [Route("Action/VoidBook")]
        public ActionResult VoidBook(BookOrderIDModel model)
        {
            try
            {
                var vnaSearchService = new VNA_SearchService();
                return vnaSearchService.VoidBook(model);
            }
            catch (Exception ex)
            {
                Helper.SystemLogg.WriteLog("BookVoid: " + ex);
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/TicketInfo")]
        public ActionResult TicketInfo(PNRModel model)
        {
            try
            {
                var vnaSearchService = new VNA_SearchService();
                return vnaSearchService.TicketInfo(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }
         
        [HttpPost]
        [Route("Action/TicketCondition")]
        public ActionResult TicketCondition(TicketConditionModel model)
        {
            try
            {
                var vnaSearchService = new VNA_SearchService();
                return vnaSearchService.GetTicketCondition(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }



        // API MAY BAY ********************************************************************************************************************************************
        [HttpPost]
        public ActionResult GdsSearch(FlightSearchModel model)
        {
            try
            {
                AIRService.WebService.Mbay.Service.GDS_WSGds_SearchFlightService gds_WSGds_SearchFlightService = new AIRService.WebService.Mbay.Service.GDS_WSGds_SearchFlightService();
                //return gds_WSGds_SearchFlightService.FUNC_GDS_WSGds_SearchFlight(model);

                return Notifization.Data("", gds_WSGds_SearchFlightService.FUNC_GDS_WSGds_SearchFlight(model));
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }
        // API MAY BAY ********************************************************************************************************************************************

        ///
        [HttpPost]
        [Route("Action/TestLogin")]
        public ActionResult TestLogin()
        {
            return Notifization.Data(":::", VNA_AuthencationService.GetSession());
        }

        //

    }
}