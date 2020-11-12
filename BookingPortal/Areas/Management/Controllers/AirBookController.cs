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
    [RoutePrefix("AirBook")]
    public class AirBookController : CMSController
    {
        public ActionResult Search()
        {
            Request_FlightSearchModel model = new Request_FlightSearchModel
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
                    model = JsonConvert.DeserializeObject<Request_FlightSearchModel>(dataSearch);
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
                var model = JsonConvert.DeserializeObject<Request_FlightSearchModel>(dataSearch);
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
        public ActionResult BookList()
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
        

        // GET: ******************************************************************************************************************************
        //[HttpPost]
        //[Route("Action/GetProvider")]
        //public ActionResult GetCustomerAgent()
        //{
        //    ClientLoginService clientLoginService = new ClientLoginService();
        //    List<ClientOption> dtList = clientLoginService.GetAllProvider();
        //    string userId = Helper.Current.UserLogin.IdentifierID;
        //    return Notifization.Data("", dtList);
        //}

        [HttpPost]
        [Route("Action/BookList")]
        public ActionResult BookList(BookOrderSerch model)
        {
            try
            {
                using (var service = new BookOrderService())
                    return service.DataList(model);
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
            CustomerService customerService = new CustomerService();
            List<ClientOption> dtList = customerService.GetCompanyData();
            return Notifization.Data("", dtList);
        }

        // GET: Booking ******************************************************************************************************************************



        [HttpPost]
        [Route("Action/Test")]
        public ActionResult Test()
        {
            VNA_SearchService searchService = new VNA_SearchService();
            return searchService.Test01();
        }

        /// <summary>
        /// search =>> One way || Round trip
        /// </summary>
        /// <param name="model">FlightSearchModel</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Route("Action/Search")]
        public ActionResult Search(Request_FlightSearchModel model)
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
                HttpContext.Response.Cookies.Add(new HttpCookie("FlightSearch", new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new Request_FlightSearchModel
                {
                    OriginLocation = _originLocation,
                    DestinationLocation = _destinationLocation,
                    DepartureDateTime = Convert.ToDateTime(_departureDateTime).ToString("dd-MM-yyyy"),
                    ReturnDateTime = Convert.ToDateTime(_returnDateTime).ToString("dd-MM-yyyy"),
                    ADT = model.ADT,
                    CNN = model.CNN,
                    INF = model.INF,
                    IsRoundTrip = model.IsRoundTrip

                })));
                //
                var vnaSearchService = new VNA_SearchService();
                return vnaSearchService.FlightSearch(new FlightSearchModel
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
        public ActionResult TicketOrder(Request_BookModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.TicketOrder(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/ExTicket")]
        public ActionResult ExTicket(PNRModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.ReleaseTicket(model);
            }
            catch (Exception ex)
            {
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
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.TicketInfo(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST(":::" + ex);
                //return Notifization.NotService;
            }
        }

        [HttpPost]
        [Route("Action/TicketCancel")]
        public ActionResult TicketCancel(PNRModel model)
        {
            try
            {
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.VoidTicket(model);
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
                var vNASearchService = new VNA_SearchService();
                return vNASearchService.GetTicketCondition(model);
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

    }
}