using System;
using ApiPortalBooking.Models;
using AIRService.Entities;
using AIRService.Models;
using AIR.Helper.Session;
using System.Web.Mvc;
using AIRService.WS.Service;
using System.Collections.Generic;
using AIRService.WebService.VNA.Enum;
using AIRService.WS.Entities;
using System.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using ApiPortalBooking.Models.VNA_WS_Model;
using WebCore.Services;
using WebCore.Entities;
using System.IO;
using System.Web;
using WebCore.ENM;
using Helper;
using AIRService.WebService.VNA_OTA_AirTaxRQ;
using Helper.Page;
using AIRService.WebService.VNA.Authen;
using System.Web.Helpers;
using System.Web.UI.WebControls;
using System.Xml;
using AIRService.WS.Helper;
using Helper.TimeData;

namespace AIRService.Service
{
    public class VNA_SearchService
    {

        // test fare
        public ActionResult Test01()
        {
            VNAFareLLSRQService vnaFareLLSRQService = new VNAFareLLSRQService();

            return Notifization.Data("ok", vnaFareLLSRQService.FareLLS2());
        }

        // search  
        public ActionResult FlightSearch(FlightSearchModel model)
        {
            TokenModel tokenModel = VNA_AuthencationService.GetSession();
            // create session 
            if (tokenModel == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                // check model
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                int _adt = model.ADT; // Adults
                int _cnn = model.CNN; // minors
                int _inf = model.INF; // Infant

                bool _isRoundTrip = model.IsRoundTrip;
                string _destinationLocation = model.DestinationLocation;
                string _originLocation = model.OriginLocation;
                DateTime _departureDateTime = model.DepartureDateTime;
                DateTime? _returnDateTime = model.ReturnDateTime;
                int itineraryType = model.ItineraryType;

                if (_isRoundTrip && _departureDateTime > Convert.ToDateTime(_returnDateTime))
                    return Notifization.Invalid("Ngày đi phải nhỏ hơn ngày về");
                //
                if (_adt <= 0)
                    return Notifization.Invalid("Số lượng người lớn phải > 0");
                //
                if (_inf > _adt)
                    return Notifization.Invalid("Số lượng em bé không hợp lệ");

                // get token
                string _token = tokenModel.Token;
                string _conversationId = tokenModel.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.Error(MessageText.NotService);
                // seach data in web service
                // tax Ax:
                AirportService airportService = new AirportService();
                double axFeeGo = airportService.GetAlls(m => m.IATACode == _originLocation).FirstOrDefault().AxFee;
                double axFeeReturn = airportService.GetAlls(m => m.IATACode == _destinationLocation).FirstOrDefault().AxFee;
                //
                VNA_AirAvailLLSRQService airAvailLLSRQService = new VNA_AirAvailLLSRQService();
                VNAFareLLSRQService vnaFareLLSRQService = new VNAFareLLSRQService();
                // group by time
                List<Response_FlightSearch> response_FlightSearch = new List<Response_FlightSearch>();
                AirTicketCondition04Service airTicketCondition04Service = new AirTicketCondition04Service();
                AirTicketCondition05Service airTicketCondition05Service = new AirTicketCondition05Service();
                //
                AirTicketCondition04 airTicketCondition04 = airTicketCondition04Service.GetAlls(m => m.IsApplied).FirstOrDefault();
                //
                // Flight >> go
                // ****************************************************************************************************************************
                //
                string _currencyCode = "VND";
                List<FlightSegment> lstFlightSegment = new List<FlightSegment>();
                List<FlightSearch> lstFlightSearch = new List<FlightSearch>();
                //
                List<XMLObject.AirAvailLLSRQ.OriginDestinationOption> originDestinationOptionListGo = new List<XMLObject.AirAvailLLSRQ.OriginDestinationOption>();
                string _flightGo = model.OriginLocation;
                string _flightTo = model.DestinationLocation;
                //
                XMLObject.AirAvailLLSRQ.OTA_AirAvailRS airAvailResultGo = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ2(new AirAvailLLSRQModel
                {
                    Token = _token,
                    ConversationID = _conversationId,
                    DepartureDateTime = _departureDateTime,
                    DestinationLocation = _destinationLocation,
                    OriginLocation = _originLocation
                });
                //
                if (airAvailResultGo.ApplicationResults.Status == "Complete")
                {
                    originDestinationOptionListGo.AddRange(airAvailResultGo.OriginDestinationOptions.OriginDestinationOption);
                    // loop next 1
                    var airAvailResult1 = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ3(new AirAvailLLSRQModel
                    {
                        Token = _token,
                        ConversationID = _conversationId,
                        DepartureDateTime = _departureDateTime,
                        DestinationLocation = _destinationLocation,
                        OriginLocation = _originLocation
                    });
                    if (airAvailResult1.ApplicationResults.Status == "Complete")
                        originDestinationOptionListGo.AddRange(airAvailResult1.OriginDestinationOptions.OriginDestinationOption);
                    //
                    // loop next 2 
                    var airAvailResult2 = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ3(new AirAvailLLSRQModel
                    {
                        Token = _token,
                        ConversationID = _conversationId,
                        DepartureDateTime = _departureDateTime,
                        DestinationLocation = _destinationLocation,
                        OriginLocation = _originLocation
                    });
                    if (airAvailResult2.ApplicationResults.Status == "Complete")
                        originDestinationOptionListGo.AddRange(airAvailResult2.OriginDestinationOptions.OriginDestinationOption);
                    //
                    // loop next 3 
                    var airAvailResult3 = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ3(new AirAvailLLSRQModel
                    {
                        Token = _token,
                        ConversationID = _conversationId,
                        DepartureDateTime = _departureDateTime,
                        DestinationLocation = _destinationLocation,
                        OriginLocation = _originLocation
                    });
                    if (airAvailResult3.ApplicationResults.Status == "Complete")
                        originDestinationOptionListGo.AddRange(airAvailResult3.OriginDestinationOptions.OriginDestinationOption);
                    //
                }
                // danh sach chuyen bay 
                if (originDestinationOptionListGo.Count > 0)
                {
                    // attachment INF (em bé)
                    int _passengerTotal = model.ADT + model.CNN + model.INF;
                    //  passenger total is availability 
                    int _availabilityTotal = model.ADT + model.CNN;
                    //
                    //var _availabilityData =   List<>
                    FareLLSModel fareLLSModel = new FareLLSModel
                    {
                        Token = _token,
                        ConversationID = _conversationId,
                        DepartureDateTime = _departureDateTime,
                        DestinationLocation = _destinationLocation,
                        OriginLocation = _originLocation,
                        AirlineCode = "VN",
                        PassengerType = new List<string>()
                    };
                    if (model.ADT > 0)
                        fareLLSModel.PassengerType.Add("ADT");
                    if (model.CNN > 0)
                        fareLLSModel.PassengerType.Add("CNN");
                    if (model.INF > 0)
                        fareLLSModel.PassengerType.Add("INF");

                    fareLLSModel.CurrencyCode = _currencyCode;
                    fareLLSModel.AxFee = axFeeGo;

                    // fareRSFareBasis
                    List<WebService.VNA_FareLLSRQ.FareRSFareBasis> fareRSFareBases = vnaFareLLSRQService.FareLLS(fareLLSModel);
                    if (fareRSFareBases == null)
                        return Notifization.Invalid(MessageText.Invalid + "111111");
                    // get tax 
                    string flightLocation = _originLocation + "-" + _destinationLocation;
                    AirTicketCondition05 airTicketCondition05 = airTicketCondition05Service.GetAlls(m => m.IsApplied && m.FlightLocationID == flightLocation).FirstOrDefault();

                    //string json = JsonConvert.SerializeObject(fareRSFareBases);
                    //string fileName = "fa-test.json";
                    //var urlFile = HttpContext.Current.Server.MapPath(@"~/WS/" + fileName);
                    ////write string to file
                    //System.IO.File.WriteAllText(urlFile, json);

                    int _year = _departureDateTime.Year;
                    foreach (var originDestinationOption in originDestinationOptionListGo)
                    {
                        //#1. Availability: get all Flight is Availability
                        //#2.
                        XMLObject.AirAvailLLSRQ.FlightSegment _lstFlightSegment = originDestinationOption.FlightSegment;
                        if (_lstFlightSegment == null)
                            continue;
                        if (_lstFlightSegment.MarketingAirline.Code != "VN")
                            continue;
                        //
                        if (_lstFlightSegment.DestinationLocation.LocationCode != _destinationLocation || _lstFlightSegment.OriginLocation.LocationCode != _originLocation)
                            continue;
                        // gioi han du lieu , do du lieu tra ve nhieu chang
                        // _lstFlightSegment = _lstFlightSegment.Where(m => m.OriginLocation.LocationCode == _originLocation && m.DestinationLocation.LocationCode == _destinationLocation).ToList();
                        int planeNo = Convert.ToInt32(_lstFlightSegment.FlightNumber);
                        DateTime _datedepart = DateTime.Parse(_year + "-" + _lstFlightSegment.DepartureDateTime + ":00");
                        DateTime _arrivalDateTime = _datedepart;
                        // lay tat ca hang ghe dang ban (maketting) has Availability > 0 va >= tong so luong hanh khach (adt + cnn)
                        List<string> _lstResBookDesigCode = _lstFlightSegment.BookingClassAvail.Where(m => Convert.ToInt32(m.Availability) > 0 && Convert.ToInt32(m.Availability) >= _availabilityTotal).Select(m => m.ResBookDesigCode).ToList();
                        if (_lstResBookDesigCode.Count() == 0)
                            continue;
                        // 
                        List<FlightSegment_ResBookDesigCode> fareDetails = GetFlightFares(_lstResBookDesigCode, fareLLSModel, fareRSFareBases, new FlightAirTicketCondition
                        {
                            OriginLocation = _originLocation,
                            DestinationLocation = _destinationLocation,
                            DepartureDateTime = _departureDateTime,
                            ArrivalDateTime = _arrivalDateTime,
                            AirEquipType = _lstFlightSegment.Equipment.AirEquipType,
                            FlightNo = planeNo,
                            ItineraryType = itineraryType,
                            AirTicketCondition04 = airTicketCondition04,
                            AirTicketCondition05 = airTicketCondition05,
                        }, tokenModel);
                        //if (fareDetails.Count() == 0)
                        //    continue;
                        ////
                        _arrivalDateTime = _datedepart.AddMinutes(Convert.ToDouble(originDestinationOption.FlightSegment.FlightDetails.TotalTravelTime));
                        //
                        lstFlightSegment.Add(new FlightSegment
                        {
                            AirEquipType = _lstFlightSegment.Equipment.AirEquipType,
                            ArrivalDateTime = _arrivalDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                            DepartureDateTime = _datedepart.ToString("dd/MM/yyyy HH:mm:ss"),
                            DestinationLocation = _destinationLocation,
                            FlightNo = planeNo,
                            FlightType = (int)VNAEnum.FlightDirection.FlightGo,
                            NumberInParty = _availabilityTotal,
                            OriginLocation = _originLocation,
                            FareDetails = fareDetails,
                            RPH = originDestinationOption.RPH
                        });
                    }

                    var lstFlightSegmentGroupByTime = lstFlightSegment.GroupBy(m => new { m.DepartureDateTime, m.ArrivalDateTime }).Select(m => new { m.Key.ArrivalDateTime, m.Key.DepartureDateTime }).ToList();
                    // 
                    foreach (var item in lstFlightSegmentGroupByTime)
                    {
                        foreach (var flightSegment in lstFlightSegment)
                        {
                            if (item.DepartureDateTime == flightSegment.DepartureDateTime && item.ArrivalDateTime == flightSegment.ArrivalDateTime)
                            {
                                response_FlightSearch.Add(new Response_FlightSearch
                                {
                                    AirEquipType = flightSegment.AirEquipType,
                                    ArrivalDateTime = flightSegment.ArrivalDateTime,
                                    DepartureDateTime = flightSegment.DepartureDateTime,
                                    DestinationLocation = _destinationLocation,
                                    FlightNo = flightSegment.FlightNo,
                                    FlightType = flightSegment.FlightType,
                                    NumberInParty = _availabilityTotal,
                                    OriginLocation = _originLocation,
                                    FareDetails = flightSegment.FareDetails,
                                    RPH = flightSegment.RPH
                                });
                            }
                        }
                    }
                }
                //
                if (!model.IsRoundTrip)
                {
                    if (response_FlightSearch.Count() == 0)
                        return Notifization.NotFound(MessageText.NotFound + 1);
                    //
                    response_FlightSearch = response_FlightSearch.OrderBy(m => m.FlightType).OrderBy(m => m.DepartureDateTime).ToList();
                    return Notifization.Data("OK -> IsRoundTrip: false ", response_FlightSearch);
                }
                //////////// Flight >> return
                //////////// ****************************************************************************************************************************
                // reset data
                lstFlightSegment = new List<FlightSegment>();
                _originLocation = model.DestinationLocation;
                _destinationLocation = model.OriginLocation;
                _departureDateTime = model.ReturnDateTime;
                List<XMLObject.AirAvailLLSRQ.OriginDestinationOption> originDestinationOptionListReturn = new List<XMLObject.AirAvailLLSRQ.OriginDestinationOption>();
                XMLObject.AirAvailLLSRQ.OTA_AirAvailRS airAvailResultReturn = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ2(new AirAvailLLSRQModel
                {
                    Token = _token,
                    ConversationID = _conversationId,
                    DepartureDateTime = _departureDateTime,
                    DestinationLocation = _destinationLocation,
                    OriginLocation = _originLocation
                });

                if (airAvailResultReturn.ApplicationResults.Status == "Complete")
                {
                    originDestinationOptionListReturn.AddRange(airAvailResultReturn.OriginDestinationOptions.OriginDestinationOption);
                    // loop next 1
                    var airAvailResult1 = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ3(new AirAvailLLSRQModel
                    {
                        Token = _token,
                        ConversationID = _conversationId,
                        DepartureDateTime = _departureDateTime,
                        DestinationLocation = _destinationLocation,
                        OriginLocation = _originLocation
                    });
                    if (airAvailResult1.ApplicationResults.Status == "Complete")
                        originDestinationOptionListReturn.AddRange(airAvailResult1.OriginDestinationOptions.OriginDestinationOption);
                    //
                    // loop next 2 
                    var airAvailResult2 = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ3(new AirAvailLLSRQModel
                    {
                        Token = _token,
                        ConversationID = _conversationId,
                        DepartureDateTime = _departureDateTime,
                        DestinationLocation = _destinationLocation,
                        OriginLocation = _originLocation
                    });
                    if (airAvailResult2.ApplicationResults.Status == "Complete")
                        originDestinationOptionListReturn.AddRange(airAvailResult2.OriginDestinationOptions.OriginDestinationOption);
                    //
                    // loop next 3 
                    var airAvailResult3 = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ3(new AirAvailLLSRQModel
                    {
                        Token = _token,
                        ConversationID = _conversationId,
                        DepartureDateTime = _departureDateTime,
                        DestinationLocation = _destinationLocation,
                        OriginLocation = _originLocation
                    });
                    if (airAvailResult3.ApplicationResults.Status == "Complete")
                        originDestinationOptionListReturn.AddRange(airAvailResult3.OriginDestinationOptions.OriginDestinationOption);
                    //
                }

                if (originDestinationOptionListReturn.Count > 0)
                {
                    // attachment INF (em bé)
                    int _passengerTotal = model.ADT + model.CNN + model.INF;
                    //  passenger total is availability 
                    int _availabilityTotal = model.ADT + model.CNN;
                    //
                    //var _availabilityData =   List<>
                    FareLLSModel fareLLSModel = new FareLLSModel
                    {
                        Token = _token,
                        ConversationID = _conversationId,
                        DepartureDateTime = _departureDateTime,
                        DestinationLocation = _destinationLocation,
                        OriginLocation = _originLocation,
                        AirlineCode = "VN",
                        PassengerType = new List<string>()
                    };
                    if (model.ADT > 0)
                        fareLLSModel.PassengerType.Add("ADT");
                    if (model.CNN > 0)
                        fareLLSModel.PassengerType.Add("CNN");
                    if (model.INF > 0)
                        fareLLSModel.PassengerType.Add("INF");
                    //
                    fareLLSModel.CurrencyCode = _currencyCode;
                    fareLLSModel.AxFee = axFeeReturn;

                    // fareRSFareBasis
                    List<WebService.VNA_FareLLSRQ.FareRSFareBasis> fareRSFareBases = vnaFareLLSRQService.FareLLS(fareLLSModel);
                    if (fareRSFareBases == null)
                        return Notifization.Invalid(MessageText.Invalid + "111111");
                    // get tax 
                    string flightLocation = _originLocation + "-" + _destinationLocation;
                    AirTicketCondition05 airTicketCondition05 = airTicketCondition05Service.GetAlls(m => m.IsApplied && m.FlightLocationID == flightLocation).FirstOrDefault();

                    int _year = _departureDateTime.Year;
                    foreach (var originDestinationOption in originDestinationOptionListReturn)
                    {
                        //#1. Availability: get all Flight is Availability
                        //#2.
                        XMLObject.AirAvailLLSRQ.FlightSegment _lstFlightSegment = originDestinationOption.FlightSegment;
                        if (_lstFlightSegment == null)
                            continue;
                        if (_lstFlightSegment.MarketingAirline.Code != "VN")
                            continue;
                        //
                        if (_lstFlightSegment.DestinationLocation.LocationCode != _destinationLocation || _lstFlightSegment.OriginLocation.LocationCode != _originLocation)
                            continue;
                        // 
                        int planeNo = Convert.ToInt32(_lstFlightSegment.FlightNumber);
                        DateTime _datedepart = DateTime.Parse(_year + "-" + _lstFlightSegment.DepartureDateTime + ":00");
                        DateTime _arrivalDateTime = _datedepart;
                        // lay tat ca hang ghe dang ban (maketting) has Availability > 0 va >= tong so luong hanh khach (adt + cnn)
                        List<string> _lstResBookDesigCode = _lstFlightSegment.BookingClassAvail.Where(m => Convert.ToInt32(m.Availability) > 0 && Convert.ToInt32(m.Availability) >= _availabilityTotal).Select(m => m.ResBookDesigCode).ToList();
                        if (_lstResBookDesigCode.Count() == 0)
                            continue;
                        //
                        List<FlightSegment_ResBookDesigCode> fareDetails = GetFlightFares(_lstResBookDesigCode, fareLLSModel, fareRSFareBases, new FlightAirTicketCondition
                        {
                            OriginLocation = _originLocation,
                            DestinationLocation = _destinationLocation,
                            DepartureDateTime = _departureDateTime,
                            ArrivalDateTime = _arrivalDateTime,
                            AirEquipType = _lstFlightSegment.Equipment.AirEquipType,
                            FlightNo = planeNo,
                            ItineraryType = itineraryType,
                            AirTicketCondition04 = airTicketCondition04,
                            AirTicketCondition05 = airTicketCondition05
                        }, tokenModel);
                        //
                        //if (fareDetails.Count() == 0)
                        //    continue;
                        //
                        _arrivalDateTime = _datedepart.AddMinutes(Convert.ToDouble(_lstFlightSegment.FlightDetails.TotalTravelTime));
                        //
                        lstFlightSegment.Add(new FlightSegment
                        {
                            AirEquipType = _lstFlightSegment.Equipment.AirEquipType,
                            ArrivalDateTime = _arrivalDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                            DepartureDateTime = _datedepart.ToString("dd/MM/yyyy HH:mm:ss"),
                            DestinationLocation = _destinationLocation,
                            FlightNo = planeNo,
                            FlightType = (int)VNAEnum.FlightDirection.FlightReturn,
                            NumberInParty = _availabilityTotal,
                            OriginLocation = _originLocation,
                            FareDetails = fareDetails,
                            RPH = originDestinationOption.RPH
                        });
                    }
                    //
                    var lstFlightSegmentGroupByTime = lstFlightSegment.GroupBy(m => new { m.DepartureDateTime, m.ArrivalDateTime }).Select(m => new { m.Key.ArrivalDateTime, m.Key.DepartureDateTime }).ToList();
                    // 
                    foreach (var item in lstFlightSegmentGroupByTime)
                    {
                        foreach (var flightSegment in lstFlightSegment)
                        {
                            if (item.DepartureDateTime == flightSegment.DepartureDateTime && item.ArrivalDateTime == flightSegment.ArrivalDateTime)
                            {
                                response_FlightSearch.Add(new Response_FlightSearch
                                {
                                    AirEquipType = flightSegment.AirEquipType,
                                    ArrivalDateTime = flightSegment.ArrivalDateTime,
                                    DepartureDateTime = flightSegment.DepartureDateTime,
                                    DestinationLocation = _destinationLocation,
                                    FlightNo = flightSegment.FlightNo,
                                    FlightType = flightSegment.FlightType,
                                    NumberInParty = _availabilityTotal,
                                    OriginLocation = _originLocation,
                                    FareDetails = flightSegment.FareDetails,
                                    RPH = flightSegment.RPH
                                });
                            }
                        }
                    }
                }
                if (response_FlightSearch.Count() == 0)
                    return Notifization.NotFound(MessageText.NotFound + 2);
                //
                response_FlightSearch = response_FlightSearch.OrderBy(m => m.FlightType).OrderBy(m => m.DepartureDateTime).ToList();
                return Notifization.Data("OK -> IsRoundTrip: true ", response_FlightSearch);
            }
        }
        // Get list ResBookDesigCode is valid
        public List<FlightSegment_ResBookDesigCode> GetFlightFares(List<string> lstResBookDesigCode, FareLLSModel fareLLSModel, List<WebService.VNA_FareLLSRQ.FareRSFareBasis> fareRSFareBasis, FlightAirTicketCondition flightAirTicketCondition, TokenModel tokenModel)
        {
            List<FlightSegment_ResBookDesigCode> result = new List<FlightSegment_ResBookDesigCode>();
            foreach (var rbdc in lstResBookDesigCode)
            {
                List<FareItem> flightCostDetails = GetFareDetails(new VNAFareLLSRQModel
                {
                    FareLLS = fareLLSModel,
                    FareRSFareBasis = fareRSFareBasis,
                    ResBookDesigCode = rbdc
                }, flightAirTicketCondition, tokenModel);

                if (flightCostDetails == null)
                    continue;
                //
                result.Add(new FlightSegment_ResBookDesigCode
                {
                    ResBookDesigCodeID = VNALibrary.GetResbookDesigCodeIDByKey(rbdc),
                    ResBookDesigCode = rbdc,
                    SellingFare = "",
                    FareItem = flightCostDetails
                });
            }
            result = result.OrderByDescending(m => m.ResBookDesigCodeID).ToList();
            return result;
        }
        public List<FareItem> GetFareDetails(VNAFareLLSRQModel model, FlightAirTicketCondition flightAirTicketCondition, TokenModel tokenModel)
        {
            List<WebService.VNA_FareLLSRQ.FareRSFareBasis> fareRSFareBases = model.FareRSFareBasis.ToList();
            if (fareRSFareBases == null || fareRSFareBases.Count == 0)
                return new List<FareItem>();
            // 
            FareLLSModel fareLLSModel = model.FareLLS;
            if (fareLLSModel == null)
                return new List<FareItem>();
            //
            List<string> lstPassengerType = fareLLSModel.PassengerType.ToList().OrderBy(m => m).ToList();
            if (lstPassengerType.Count == 0)
                return new List<FareItem>();
            // 
            string rbdc = model.ResBookDesigCode;
            int rbdcEnum = VNALibrary.GetResbookDesigCodeIDByKey(rbdc);
            List<FareItem> fareDetailsModels = new List<FareItem>();
            //string test = ""; 
            // check condition 04 
            int planeNo = flightAirTicketCondition.FlightNo;
            bool conditionState04 = false;
            bool conditionState05 = false;
            AirTicketCondition04 airTicketCondition04 = flightAirTicketCondition.AirTicketCondition04;
            if (airTicketCondition04 != null)
            {
                if (planeNo >= airTicketCondition04.PlaneNoFrom && planeNo <= airTicketCondition04.PlaneNoTo)
                    conditionState04 = true;
            }
            // check condition 05
            AirTicketCondition05 airTicketCondition05 = flightAirTicketCondition.AirTicketCondition05;
            if (airTicketCondition05 != null)
            {
                if (!string.IsNullOrWhiteSpace(airTicketCondition05.ResBookDesigCode))
                {
                    List<string> resBookDesig = airTicketCondition05.ResBookDesigCode.Split(',').ToList();
                    if (resBookDesig.Contains(rbdc))
                        conditionState05 = true;
                }
            }
            //
            List<WebService.VNA_FareLLSRQ.FareRSFareBasis> fareRSFareBases1 = fareRSFareBases.Where(m => m.Code.Last() == 'F').ToList();
            string test = "0";

            if (conditionState04 || conditionState05)
            {
                fareRSFareBases1 = fareRSFareBases.Where(m => m.Code.Last() != 'F').ToList();
                test = "1";
            }
            //
            foreach (var passengerType in lstPassengerType)
            {
                List<FlightCost> flightCosts = new List<FlightCost>();
                List<double> lstFare = new List<double>();
                double minAmount = 0;
                int cnt = 0;
                foreach (var fareRSFareBasis in fareRSFareBases1)
                {
                    if (passengerType == fareRSFareBasis.PassengerType[0].Code && fareRSFareBasis.AdditionalInformation.ResBookDesigCode == rbdc && fareRSFareBasis.AdditionalInformation.PrivateInd != null)
                    {
                        string strAmount = fareRSFareBasis.AdditionalInformation.Fare[0].Amount;
                        if (!string.IsNullOrWhiteSpace(strAmount) && Convert.ToDouble(strAmount) > 0)
                        {
                            bool active = false;
                            double _amount = Convert.ToDouble(strAmount);
                            if (cnt == 0)
                            {
                                minAmount = _amount;
                                active = true;
                            }
                            //
                            if (minAmount != 0 && minAmount > _amount)
                            {
                                minAmount = _amount;
                                active = true;
                            }
                            //
                            double _fareTotal = 0;
                            if (passengerType == "ADT")
                                _fareTotal = (_amount * 1.1) + 350000 + fareLLSModel.AxFee + 20000;
                            //
                            if (passengerType == "CNN")
                                _fareTotal = (_amount * 0.9) * 1.1 + 350000 + (fareLLSModel.AxFee / 2) + (20000 / 2);
                            //
                            if (passengerType == "INF")
                                _fareTotal = (_amount * 0.1) * 1.1;
                            //
                            fareDetailsModels.Add(new FareItem
                            {
                                RPH = Convert.ToInt32(fareRSFareBasis.RPH),
                                PassengerType = passengerType,
                                FareAmount = _amount,
                                FareTotal = Helper.Page.Library.FormatNumberRoundUp(_fareTotal, 1000),
                                Code = fareRSFareBasis.Code,
                                IsActive = active,
                                Test = test
                            });
                            cnt++;
                        }
                    }
                }
            }
            //
            if (fareDetailsModels.Count == 0)
                return null;
            // 
            return fareDetailsModels;
        }
        // cost 
        public ActionResult FlightCost(FlightSearchModel model)
        {
            TokenModel tokenModel = VNA_AuthencationService.GetSession();
            // create session 
            if (tokenModel == null)
                return Notifization.Error("Cannot create session");
            // create session 
            string _token = tokenModel.Token;
            string _conversationId = tokenModel.ConversationID;
            if (string.IsNullOrWhiteSpace(_token))
                return Notifization.NotService;
            //
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                // check model 
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                int _adt = model.ADT; // Adults
                int _cnn = model.CNN; // minors
                int _inf = model.INF; // Infant
                //
                string _destinationLocation = model.DestinationLocation;
                string _originLocation = model.OriginLocation;
                DateTime _departureDateTime = model.DepartureDateTime;
                DateTime? _returnDateTime = model.ReturnDateTime;
                //
                if (_adt <= 0)
                    return Notifization.Invalid("Adults must be > 0");
                //
                if (_inf > _adt)
                    return Notifization.Invalid("Infant invalid");
                // seach data in web service
                VNAFareLLSRQService vNAFareLLSRQService = new VNAFareLLSRQService();
                // Flight >> go
                // ****************************************************************************************************************************
                string _flightGo = model.OriginLocation;
                string _flightTo = model.DestinationLocation;
                var fareLLSModel = new FareLLSModel
                {
                    Token = _token,
                    ConversationID = _conversationId,
                    DepartureDateTime = _departureDateTime,
                    DestinationLocation = _flightTo,
                    OriginLocation = _flightGo,
                    AirlineCode = "VN",
                    PassengerType = new List<string>()
                };
                if (model.ADT > 0)
                    fareLLSModel.PassengerType.Add("ADT");
                if (model.CNN > 0)
                    fareLLSModel.PassengerType.Add("CNN");
                if (model.INF > 0)
                    fareLLSModel.PassengerType.Add("INF");
                //
                var dataFareLLS = vNAFareLLSRQService.FareLLS(fareLLSModel);
                if (dataFareLLS == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                return Notifization.Data("OK", dataFareLLS);
            }
        }


        /// Dieu kien ve ****************************************************************************************************************************************
        /// #1.Get data
        /// #2.Save file, fare ofticket check in file (1)
        public ActionResult GetTicketCondition(TicketConditionModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid + "1");
                //
                TokenModel tokenModel = VNA_AuthencationService.GetSession();
                // create session 
                if (tokenModel == null)
                    return Notifization.Error("Cannot create session");
                // create session 
                string _token = tokenModel.Token;
                string _conversationId = tokenModel.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.NotService;
                //
                if (!Helper.Page.Validate.IsDateTime(model.DepartureDateTime))
                    return Notifization.Invalid(MessageText.Invalid + "2");
                //
                using (var sessionService = new VNA_SessionService(tokenModel))
                {
                    DateTime departureDateTime = Convert.ToDateTime(model.DepartureDateTime);
                    VNA_OTA_AirRulesLLSRQService vNA_OTA_AirRulesLLSRQService = new VNA_OTA_AirRulesLLSRQService();
                    var data = vNA_OTA_AirRulesLLSRQService.GetOTA_AirRulesLLSRQ(new VNA_OTA_AirRulesLLSRQ
                    {
                        Token = _token,
                        ConversationID = _conversationId,
                        DepartureDateTime = departureDateTime,
                        DestinationLocation = model.DestinationLocation,
                        OriginLocation = model.OriginLocation,
                        FareBasis = model.FareBasis
                    });
                    return Notifization.Data("Ok" + data, data);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("E>>:" + ex);
            }
        }
        /// ****************************************************************************************************************************************

        public List<FlightTax> GetTax(TokenModel tokenModel, List<TaxFeeModel> models)
        {
            List<FlightTax> flightTaxes = new List<FlightTax>();
            if (models == null)
                return flightTaxes;
            // 
            VNA_OTA_AirTaxRQService vNAOTA_AirTaxRQService = new VNA_OTA_AirTaxRQService();
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                foreach (var flight in models)
                {
                    if (flight.FareBase.Count > 0)
                    {
                        //variable
                        int _rph = flight.RPH;
                        string originLocation = flight.OriginLocation;
                        string destinationLocation = flight.DestinationLocation;
                        DateTime departureDateTime = flight.DepartureDateTime;
                        DateTime arrivalDateTime = flight.ArrivalDateTime;
                        int airEquipType = flight.AirEquipType;
                        string resBookDesigCode = flight.ResBookDesigCode;
                        string currency = flight.CurrencyCode;
                        int flightNumber = flight.FlightNumber;

                        if (string.IsNullOrWhiteSpace(resBookDesigCode))
                            return flightTaxes;

                        resBookDesigCode = resBookDesigCode.Trim();
                        //
                        List<FlightTaxInfo> flightTaxInfos = new List<FlightTaxInfo>();
                        foreach (var item in flight.FareBase)
                        {

                            float _amount = item.Amount;
                            AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRS airTaxRS = vNAOTA_AirTaxRQService.AirTax(tokenModel, new Resquet_WsTaxModel
                            {
                                RPH = _rph,
                                OriginLocation = originLocation,
                                DestinationLocation = destinationLocation,
                                DepartureDateTime = departureDateTime,
                                ResBookDesigCode = resBookDesigCode,
                                ArrivalDateTime = arrivalDateTime,
                                AirEquipType = airEquipType,
                                FlightNumber = flightNumber,
                                BaseFare = new Resquet_WsTax_BaseFareModel
                                {
                                    RPH = item.RPH,
                                    PassengerType = item.PassengerType,
                                    Amount = _amount,
                                    CurrencyCode = currency
                                }
                            });

                            if (airTaxRS.Items.Count() > 0 && airTaxRS.Items[1] != null)
                            {
                                string jsonData = JsonConvert.SerializeObject(airTaxRS.Items[1]);
                                FeeDetailsModel feeDetailsModel = JsonConvert.DeserializeObject<FeeDetailsModel>(jsonData);
                                flightTaxInfos.Add(new FlightTaxInfo
                                {
                                    RPHSpecified = feeDetailsModel.ItineraryInfo[0].RPHSpecified,
                                    PassengerType = feeDetailsModel.ItineraryInfo[0].PTC_FareBreakdown.PassengerType,
                                    Total = feeDetailsModel.ItineraryInfo[0].TaxInfo.Total,
                                    Taxes = feeDetailsModel.ItineraryInfo[0].TaxInfo.Taxes
                                });
                            }
                        }
                        //
                        flightTaxes.Add(new FlightTax
                        {
                            RPH = _rph,
                            FlightNumber = flightNumber,
                            AirEquipType = airEquipType,
                            ResBookDesigCode = resBookDesigCode,
                            FlightTaxInfos = flightTaxInfos
                        });

                    }
                }
                //
                return flightTaxes;


            }
        }

        public ActionResult FlightFeeBasic(List<TaxFeeModel> models)
        {
            if (models == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            TokenModel tokenModel = VNA_AuthencationService.GetSession();
            // create session 
            if (tokenModel == null)
                return Notifization.Error("Cannot create session");
            // create session 
            string _token = tokenModel.Token;
            string _conversationId = tokenModel.ConversationID;
            if (string.IsNullOrWhiteSpace(_token))
                return Notifization.NotService;
            // 
            VNA_OTA_AirTaxRQService vnaOTA_AirTaxRQService = new VNA_OTA_AirTaxRQService();
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                List<FlightTax> flightTaxFees = new List<FlightTax>();
                foreach (var flight in models)
                {
                    if (flight.FareBase == null)
                        continue;
                    //
                    if (flight.FareBase.Count() > 0)
                    {
                        //variable
                        int _rph = flight.RPH;
                        string originLocation = flight.OriginLocation;
                        string destinationLocation = flight.DestinationLocation;
                        DateTime departureDateTime = flight.DepartureDateTime;
                        DateTime arrivalDateTime = flight.ArrivalDateTime;
                        int airEquipType = flight.AirEquipType;
                        string resBookDesigCode = flight.ResBookDesigCode;
                        string currency = flight.CurrencyCode;
                        int flightNumber = flight.FlightNumber;
                        if (string.IsNullOrWhiteSpace(resBookDesigCode))
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        resBookDesigCode = resBookDesigCode.Trim();
                        List<FlightTaxInfo> flightTaxInfos = new List<FlightTaxInfo>();

                        foreach (var item in flight.FareBase)
                        {
                            float _amount = item.Amount;
                            AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRS airTaxRS = vnaOTA_AirTaxRQService.AirTax(tokenModel, new Resquet_WsTaxModel
                            {
                                RPH = _rph,
                                OriginLocation = originLocation,
                                DestinationLocation = destinationLocation,
                                DepartureDateTime = departureDateTime,
                                ResBookDesigCode = resBookDesigCode,
                                ArrivalDateTime = arrivalDateTime,
                                AirEquipType = airEquipType,
                                FlightNumber = flightNumber,
                                BaseFare = new Resquet_WsTax_BaseFareModel
                                {
                                    RPH = item.RPH,
                                    PassengerType = item.PassengerType,
                                    Amount = _amount,
                                    CurrencyCode = currency
                                }
                            });
                            //
                            if (airTaxRS.Items.Count() > 0 && airTaxRS.Items[1] != null)
                            {
                                string jsonData = JsonConvert.SerializeObject(airTaxRS.Items[1]);
                                FeeDetailsModel feeDetailsModel = JsonConvert.DeserializeObject<FeeDetailsModel>(jsonData);
                                flightTaxInfos.Add(new FlightTaxInfo
                                {
                                    RPHSpecified = feeDetailsModel.ItineraryInfo[0].RPHSpecified,
                                    PassengerType = feeDetailsModel.ItineraryInfo[0].PTC_FareBreakdown.PassengerType,
                                    Total = feeDetailsModel.ItineraryInfo[0].TaxInfo.Total,
                                    Taxes = feeDetailsModel.ItineraryInfo[0].TaxInfo.Taxes
                                });
                            }
                        }
                        //
                        flightTaxFees.Add(new FlightTax
                        {
                            RPH = _rph,
                            FlightNumber = flightNumber,
                            AirEquipType = airEquipType,
                            ResBookDesigCode = resBookDesigCode,
                            FlightTaxInfos = flightTaxInfos
                        });

                    }
                }
                //
                if (flightTaxFees.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                return Notifization.Data("OK", flightTaxFees);
            }
        }
        /// BOOK TICKET ****************************************************************************************************************************************
        /// <summary>
        /// THE STEP ACTION BOOKING ********************* 
        /// #1.SessionCreateRQ
        /// #2.OTA_AirBookLLSRQ
        /// #3.OTA_AirPriceLLSRQ
        /// #4.PassengerDetailsRQ
        /// #5.EndTransactionLLSRQ
        /// #6.SessionCloseRQ
        /// END STEP ACTION BOOKING *********************
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult TicketOrder(Request_BookModel model)
        {
            try
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                //
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                TokenModel tokenModel = VNA_AuthencationService.GetSession();
                // create session 
                if (tokenModel == null)
                    return Notifization.Invalid(MessageText.Invalid);
                // create session 
                string _token = tokenModel.Token;
                string _conversationId = tokenModel.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.NotService;
                //
                string timeZoneLocal = model.TimeZoneLocal;
                // ticketing & contact  ************************************************************************************************
                BookTicketingRq ticketingInfo = model.TicketingInfo;
                BookContactRqModel Contacts = model.Contacts;
                BookCompanyContactModel bookCompany = new BookCompanyContactModel();
                if (ticketingInfo == null || Contacts == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                string ticketingId = Helper.Current.UserLogin.IdentifierID;
                string ticketingName = string.Empty;
                if (Helper.Current.UserLogin.IsCMSUser || Helper.Current.UserLogin.IsAdminInApplication || Helper.Current.UserLogin.IsSupplierLogged() || Helper.Current.UserLogin.IsAdminCustomerLogged())
                {
                    ticketingId = ticketingInfo.TiketingID;
                    //
                    UserInfoService userInfoService = new UserInfoService();
                    ticketingName = userInfoService.GetFullName(ticketingId);
                }
                if (string.IsNullOrWhiteSpace(ticketingId))
                    return Notifization.Invalid("Lỗi xác định thông tin nhân viên đặt vé");
                //
                string agentId = ClientLoginService.GetClientIDByUserID(ticketingId);
                ClientLoginService clientLoginService = new ClientLoginService();
                string agentCode = clientLoginService.GetClientCodeByID(agentId);
                int passengerGroup = model.PassengerGroup;
                // call service get PNR code 
                string strEmail = string.Empty;
                string strPhone = string.Empty;
                // 
                if (model.Contacts == null)
                    return Notifization.Invalid("Thông tin liên hệ không hợp lệ");
                //
                if (passengerGroup == (int)ClientLoginEnum.PassengerGroup.Comp)
                {
                    BookCompanyRqContact bookCompanyContact = Contacts.BookCompanyContact;
                    if (bookCompanyContact == null)
                        return Notifization.Invalid("Thông tin liên hệ không hợp lệ");
                    //
                    string companyId = bookCompanyContact.CompanyID;
                    if (string.IsNullOrWhiteSpace(companyId))
                        return Notifization.Invalid("Vui lòng chọn công ty");
                    //
                    CustomerService customerService = new CustomerService();
                    Customer customerComp = customerService.GetAlls(m => m.ID == companyId).FirstOrDefault();
                    if (customerComp == null)
                        return Notifization.Invalid("Thông tin công ty không tồn tại");
                    // 
                    strEmail = customerComp.ContactEmail;
                    strPhone = customerComp.ContactPhone;
                    //
                    bookCompany.CompanyID = customerComp.ID;
                    bookCompany.CompanyCode = customerComp.CodeID;
                    bookCompany.Email = strEmail;
                    bookCompany.Phone = strPhone;
                    bookCompany.Name = customerComp.ContactName;
                }
                else if (passengerGroup == (int)ClientLoginEnum.PassengerGroup.Nomal)
                {
                    BookKhachLeRqContact bookKhachLeContact = Contacts.BookKhachLeContact;
                    string name = bookKhachLeContact.Name;
                    string contactPhone = bookKhachLeContact.Phone;
                    string contactEmail = bookKhachLeContact.Email;
                    if (string.IsNullOrWhiteSpace(name))
                        return Notifization.Invalid("Không được để trống tên liên hệ");
                    //
                    if (string.IsNullOrWhiteSpace(contactPhone))
                        return Notifization.Invalid("Không được để trống số đ.thoại");
                    //
                    if (!Validate.TestPhone(contactPhone))
                        return Notifization.Invalid("Số đ.thoại không hợp lệ");
                    //
                    if (!string.IsNullOrWhiteSpace(contactEmail))
                    {
                        if (!Validate.TestEmail(contactEmail))
                            return Notifization.Invalid("Địa chỉ e-mail không hợp lệ");
                        else
                            strEmail = contactEmail;
                    }
                    //
                    strPhone = contactPhone;
                }
                //
                foreach (var item in model.Passengers)
                {
                    if (!Helper.Page.Validate.TestTextNoneUnicode(item.FullName))
                        return Notifization.Invalid("Tên khách hàng '" + item.FullName + "' không hợp lệ");
                    //
                }
                //
                BookEmailConfig bookEmailConfig = new BookEmailConfig();
                if (string.IsNullOrWhiteSpace(strEmail))
                    strEmail = bookEmailConfig.Email;
                //
                if (string.IsNullOrWhiteSpace(strPhone))
                    strPhone = bookEmailConfig.Phone;
                //
                using (var sessionService = new VNA_SessionService(tokenModel))
                {
                    if (string.IsNullOrWhiteSpace(_token))
                        return Notifization.Invalid("Can not create token");
                    //
                    using (var vnaTransaction = new VNA_EndTransaction(tokenModel))
                    {
                        var airBookModel = new AirBookModel
                        {
                            ConversationID = _conversationId,
                            Token = _token
                        };
                        //seach data in web service

                        // #1. flight information - ***********************************************************************************************************************************
                        VNA_OTA_AirBookLLSRQSevice vNAWSOTA_AirBookLLSRQSevice = new VNA_OTA_AirBookLLSRQSevice();
                        List<BookSegmentModel> lstSegments = model.Flights;
                        if (lstSegments == null || lstSegments.Count == 0)
                            return Notifization.Invalid(MessageText.Invalid);
                        //
                        AIRService.WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRS ota_AirBookRS = vNAWSOTA_AirBookLLSRQSevice.FUNC_OTA_AirBookRS(new AirBookModel
                        {
                            Segments = lstSegments,
                            ConversationID = _conversationId,
                            Token = _token
                        });
                        // 
                        if (ota_AirBookRS.ApplicationResults.Success == null)
                            return Notifization.Invalid("AirBook invalid");
                        //
                        var _originDestinationOption = ota_AirBookRS.OriginDestinationOption.ToList();
                        if (_originDestinationOption.Count == 0)
                            return Notifization.NotFound(MessageText.NotFound);
                        // #2. Get price - ***************************************************************************************************************************************************
                        airBookModel.Segments = model.Flights;
                        var airPriceModel = new AirPriceModel
                        {
                            ConversationID = _conversationId,
                            Token = _token,
                            lFlight = model.Flights
                        };
                        //
                        foreach (var item in model.Passengers)
                        {
                            if (item.PassengerType.ToUpper() == "ADT")
                            {
                                airPriceModel.ADT++;
                                continue;
                            }
                            if (item.PassengerType.ToUpper() == "CNN")
                            {
                                airPriceModel.CNN++;
                                continue;
                            }
                            if (item.PassengerType.ToUpper() == "INF")
                            {
                                airPriceModel.INF++;
                                continue;
                            }
                        }
                        //VNA_WSOTA_AirPriceLLSRQService vNAWSOTA_AirPriceLLSRQService = new VNA_WSOTA_AirPriceLLSRQService();
                        //var airPriceData = vNAWSOTA_AirPriceLLSRQService.AirPrice(airPriceModel);
                        // #3. Get PNA code - **************************************************************************************************************************************************
                        //
                        string file = HttpContext.Current.Server.MapPath(@"/Team/temp.json");
                        PricingInPNR_Test airPriceData = new PricingInPNR_Test();
                        using (StreamReader r = new StreamReader(file))
                        {
                            string json = r.ReadToEnd();
                            airPriceData = JsonConvert.DeserializeObject<PricingInPNR_Test>(json);
                        }
                        //
                        double airAgentFeeAmount = 0;
                        AirAgentFeeService airAgentFeeService = new AirAgentFeeService();
                        AirAgentFee airAgentFee = airAgentFeeService.GetAgentFee(agentId);
                        if (airAgentFee != null)
                            airAgentFeeAmount = airAgentFee.InlandFee;
                        //
                        //////////double feeTotal = double.Parse(airPriceData.PriceQuote.PricedItinerary.TotalAmount);
                        //////////// check blance
                        //////////if (Helper.Current.UserLogin.IsCustomerLogged())
                        //////////{
                        //////////    var currentUserId = Helper.Current.UserLogin.IdentifierID;
                        //////////    WalletUserService walletUserService = new WalletUserService();

                        //////////    WalletUserMessageModel walletMessageModel = walletUserService.GetBalanceByUserID(currentUserId);
                        //////////    double balanceUser = walletMessageModel.Balance;
                        //////////    if (!walletMessageModel.Status)
                        //////////        return Notifization.Invalid("Lỗi kiểm tra số dư hạn mức");
                        //////////    // 
                        //////////    if (balanceUser < feeTotal)
                        //////////        return Notifization.TEST("Số dư hạn mức không đủ");
                        //////////}
                        //////////return Notifization.TEST("Test");


                        // #3. Get PNA code - **************************************************************************************************************************************************
                        var result = new BookVeResult
                        {
                            Passengers = new List<PassengerFare>()
                        };
                        List<FareTax> fareTaxs = new List<FareTax>();
                        List<FareFlight> fareFlights = new List<FareFlight>();
                        foreach (var item in airPriceData.PriceQuote.PricedItinerary.AirItineraryPricingInfo)
                        {
                            string passengerType = item.PassengerTypeQuantity.Code;
                            int bkIndex = 0;
                            foreach (var fareBreakdown in item.PTC_FareBreakdown)
                            {
                                //
                                int flightType = (int)VNAEnum.FlightDirection.None;
                                if (bkIndex == 0)
                                    flightType = (int)VNAEnum.FlightDirection.FlightGo;
                                if (bkIndex == 1)
                                    flightType = (int)VNAEnum.FlightDirection.FlightReturn;
                                //
                                fareFlights.Add(new FareFlight
                                {
                                    FlightType = flightType,
                                    PassengerType = passengerType,
                                    Code = fareBreakdown.FareBasis.Code,
                                    Amount = Convert.ToDouble(fareBreakdown.FareBasis.FareAmount)
                                });
                                bkIndex++;
                            }
                            // add tax
                            foreach (var tax in item.ItinTotalFare.Taxes.Tax)
                            {
                                fareTaxs.Add(new FareTax
                                {
                                    FlightType = 0,
                                    PassengerType = passengerType,
                                    Text = tax.TaxName,
                                    TaxCode = tax.TaxCode,
                                    Amount = double.Parse(tax.Amount)
                                });
                            }
                        }
                        //
                        result.FareTaxs = fareTaxs;
                        result.FareFlights = fareFlights;
                        result.FareTotal = double.Parse(airPriceData.PriceQuote.PricedItinerary.TotalAmount);
                        model.Passengers = model.Passengers.OrderBy(m => m.PassengerType).ToList();
                        // create model passenger details model
                        List<PassengerDetailsRQ> passengerDetailsRQ = new List<PassengerDetailsRQ>();
                        List<BookTicketPassenger> Request_Passengers = model.Passengers;
                        foreach (var item in Request_Passengers)
                        {
                            passengerDetailsRQ.Add(new PassengerDetailsRQ
                            {
                                PassengerType = item.PassengerType,
                                FullName = item.FullName,
                                DateOfBirth = item.DateOfBirth,
                                Gender = item.Gender
                            });
                        }

                        //
                        var passengerDetailModel = new DetailsRQ
                        {
                            Email = bookEmailConfig.Email,
                            Phone = bookEmailConfig.Phone,
                            ConversationID = _conversationId,
                            Token = _token,
                            Passengers = passengerDetailsRQ,
                            AirBook = ota_AirBookRS
                        };
                        //call service  passenger details in ws.service
                        //VNA_PassengerDetailsRQService vNAWSPassengerDetailsRQService = new VNA_PassengerDetailsRQService();
                        //string pnrCode = vNAWSPassengerDetailsRQService.PassengerDetail2(passengerDetailModel);
                        //End transaction 
                        var pnrCode =  Helper.Security.Library.OTPCode;
                        vnaTransaction.EndTransaction();
                        if (string.IsNullOrWhiteSpace(pnrCode))
                            return Notifization.Invalid("Cannot get PNA code");
                        //set PNR code
                        result.PNR = pnrCode;
                        // save order | input data ***************************************************************************************************
                        OrderTicketService orderTicketService = new OrderTicketService();
                        List<RequestOrderFlightModel> requestOrderFlightModels = new List<RequestOrderFlightModel>();
                        List<RequestOrderPriceModel> requestOrderPriceModels = new List<RequestOrderPriceModel>();
                        List<RequestOrderTaxModel> requestOrderTaxModels = new List<RequestOrderTaxModel>();
                        foreach (var item in model.Flights)
                        {
                            requestOrderFlightModels.Add(new RequestOrderFlightModel
                            {
                                ADT = airPriceModel.ADT,
                                CNN = airPriceModel.CNN,
                                INF = airPriceModel.INF,
                                NumberInParty = item.NumberInParty,
                                OriginLocation = item.OriginLocation,
                                DestinationLocation = item.DestinationLocation,
                                DepartureDateTime = item.DepartureDateTime,
                                ArrivalDateTime = item.ArrivalDateTime,
                                ResBookDesigCode = item.ResBookDesigCode,
                                FlightNumber = item.FlightNumber,
                                AirEquipType = item.AirEquipType,
                            });
                        }
                        foreach (var item in fareFlights)
                        {
                            requestOrderPriceModels.Add(new RequestOrderPriceModel
                            {
                                FlightType = item.FlightType,
                                PassengerType = item.PassengerType,
                                Amount = item.Amount,
                                Unit = "VND",
                            });
                        }
                        //
                        foreach (var item in fareTaxs)
                        {
                            requestOrderTaxModels.Add(new RequestOrderTaxModel
                            {
                                PassengerType = item.PassengerType,
                                Title = item.Text,
                                TaxCode = item.TaxCode,
                                Amount = item.Amount,
                                Unit = "VND",
                            });
                        }
                        // save book information | output data ***************************************************************************************************
                        BookTicketService bookTicketService = new BookTicketService();
                        List<BookTicketPassenger> bookTicketPassengers = new List<BookTicketPassenger>();
                        foreach (var item in model.Passengers)
                        {
                            bookTicketPassengers.Add(new BookTicketPassenger
                            {
                                DateOfBirth = item.DateOfBirth,
                                FullName = item.FullName,
                                Gender = item.Gender,
                                PassengerType = item.PassengerType
                            });
                        }
                        // call save booking 
                        var bookTicketId = bookTicketService.BookTicket(new BookTicketOrder
                        {
                            PNR = result.PNR,
                            PassengerGroup = model.PassengerGroup,
                            Summary = model.Summary,
                            ItineraryType = model.ItineraryType,
                            OrderDate = Convert.ToDateTime(TimeHelper.GetDateTime),
                            Flights = model.Flights,
                            Passengers = bookTicketPassengers,
                            FareTaxs = result.FareTaxs,
                            FareFlights = fareFlights,
                            AgentInfo = new BookAgentInfo
                            {
                                AgentID = agentId,
                                AgentCode = agentCode,
                                TiketingID = ticketingId,
                                TiketingName = ticketingName,
                                AgentFee = airAgentFeeAmount
                            },
                            Contacts = new BookOrderContact
                            {
                                BookKhachLeContact = model.Contacts.BookKhachLeContact,
                                BookCompanyContact = bookCompany
                            }
                        });
                        result.TicketID = bookTicketId;
                        //
                        return Notifization.Data("Đặt chỗ thành công:" + bookTicketId + " PNR: " + pnrCode, result); // "/backend/flightbook/details/" + bookIdFist
                    } // end using tran session
                } // end using session
            }
            catch (Exception ex)
            {
                return Notifization.TEST("OK" + ex);
            }
        }

        /// <summary>
        /// Xuất vé
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult ReleaseTicket(PNRModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid + "1");
                //
                TokenModel tokenModel = VNA_AuthencationService.GetSession();
                // create session 
                if (tokenModel == null)
                    return Notifization.Error("Cannot create session");
                // create session 
                string _token = tokenModel.Token;
                string _conversationId = tokenModel.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.NotService;
                // 
                string pnr = model.PNR;
                if (string.IsNullOrWhiteSpace(pnr))
                    return Notifization.Invalid("Mã PNR không hợp lệ");
                //
                List<ExTitketPassengerFareModel> exTitketPassengerFareModels = new List<ExTitketPassengerFareModel>();
                BookPassengerService appBookPassengerService = new BookPassengerService();
                BookPriceService appBookPriceService = new BookPriceService();
                BookTaxService appBookFareService = new BookTaxService();
                var passengers = appBookPassengerService.GetAlls(m => m.PNR.ToLower() == pnr.ToLower()).OrderBy(m => m.PassengerType).ToList();
                if (passengers.Count == 0)
                    return Notifization.Invalid("Thông tin hành khách không hợp lệ");
                // 
                foreach (var item in passengers)
                {
                    var bookPrice = appBookPriceService.GetAlls(m => m.PNR.ToLower() == pnr.ToLower() && m.PassengerType.ToLower() == (item.PassengerType.ToLower())).Sum(m => m.Amount);
                    var bookTax = appBookFareService.GetAlls(m => m.PNR.ToLower() == pnr.ToLower() && m.PassengerType.ToLower() == (item.PassengerType.ToLower())).Sum(m => m.Amount);
                    exTitketPassengerFareModels.Add(new ExTitketPassengerFareModel
                    {
                        FullName = item.FullName,
                        PassengerType = item.PassengerType,
                        DateOfBirth = item.DateOfBirth.ToString(),
                        PriceTotal = bookPrice,
                        TaxTotal = bookTax
                    });
                }
                //
                double fareTotal = exTitketPassengerFareModels.Sum(m => m.TaxTotal + m.PriceTotal);
                using (var sessionService = new VNA_SessionService(tokenModel))
                {

                    var paymentModel = new PaymentModel
                    {
                        //paymentModel.RevResult = ReservationData.RevResult;
                        ConversationID = _conversationId,
                        Token = _token,
                        pnr = pnr,
                        PaymentOrderDetail = new List<PaymentOrderDetail>(),
                        Total = fareTotal
                    };
                    foreach (var item in exTitketPassengerFareModels)
                    {
                        string givenName = AIRService.WS.Helper.VNALibrary.GetGivenName(item.FullName);
                        string surName = AIRService.WS.Helper.VNALibrary.GetGivenName(item.FullName);
                        var paymentOrderDetail = new PaymentOrderDetail
                        {
                            PsgrType = item.PassengerType,
                            FirstName = givenName,
                            LastName = surName,
                            BaseFare = item.PriceTotal,
                            Taxes = item.TaxTotal ///////////////////////////////////////////////////////////////////////////////////////////////////
                        };
                        paymentModel.PaymentOrderDetail.Add(paymentOrderDetail);
                    }
                    VNA_PaymentRQService vNAPaymentRQService = new VNA_PaymentRQService();
                    var paymentData = vNAPaymentRQService.Payment(paymentModel);

                    //Login -> DisginPinter -> GetRev -> OTA_AirPriceLLSRQ -> PaymentRQ -> AirTicketLLSRQ -> Endtransession
                    #region xuất vé
                    //wss.SarbreCommand(tokenModel, "IG");
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                    designatePrinter.ConversationID = tokenModel.ConversationID;
                    designatePrinter.Token = tokenModel.Token;
                    //
                    VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    if (printer.ApplicationResults.status != AIRService.WebService.VNA_DesignatePrinterLLSRQ.CompletionCodes.Complete)
                        return Notifization.Error("Server not responding from designate printer");
                    //
                    var jjson = new JavaScriptSerializer().Serialize(printer);
                    var getReservationModel = new GetReservationModel();
                    getReservationModel.ConversationID = tokenModel.ConversationID;
                    getReservationModel.Token = tokenModel.Token;
                    getReservationModel.PNR = pnr;
                    //paymentRQ
                    //wss.GetReservationWS(getReservationModel);
                    VNA_WSGetReservationRQService vNAWSGetReservationRQService = new VNA_WSGetReservationRQService();
                    var ReservationData = vNAWSGetReservationRQService.GetReservation(getReservationModel);
                    if (paymentData.Result != null && paymentData.Result.ResultCode == "SUCCESS")
                    {
                        if (paymentData.Items.Count() == 0)
                            return Notifization.Error("Server not responding from getreservation");
                        //
                        var data = (WebService.VNA_PaymentRQ.AuthorizationResultType)paymentData.Items[0];
                        if (string.IsNullOrWhiteSpace(data.ApprovalCode))
                            return Notifization.NotFound("ApprovalCode is null");
                        //
                        var airTicket = new AirTicketLLSRQModel
                        {
                            ConversationID = tokenModel.ConversationID,
                            Token = tokenModel.Token,
                            PNR = pnr,
                            approveCode = data.ApprovalCode,
                            lPricingInPNR = exTitketPassengerFareModels
                        };
                        VNAWSAirTicketLLSRQService vNAWSAirTicketLLSRQService = new VNAWSAirTicketLLSRQService();
                        var airTicketData = vNAWSAirTicketLLSRQService.AirTicket(airTicket);
                        VNA_EndTransaction vNATransaction = new VNA_EndTransaction();
                        var end = vNATransaction.EndTransaction(tokenModel);
                        if (end.ApplicationResults.Success != null)
                        {
                            BookOrderService bookPNRCodeService = new BookOrderService();
                            var bookPNRCode = bookPNRCodeService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == pnr.ToLower()).FirstOrDefault();
                            if (bookPNRCode != null)
                            {
                                bookPNRCode.Enabled = (int)BookOrderEnum.BookOrderStatus.ExTicket;
                                bookPNRCodeService.Update(bookPNRCode);
                            }
                        }
                        // 
                        return Notifization.Data("OK", end);
                    }
                    #endregion
                    return Notifization.Error(MessageText.Invalid);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("OK" + ex);
            }
        }

        /// <summary>
        ///  Hủy vé
        /// </summary>
        /// <param name="model">PNR code</param>
        /// <returns></returns>
        public ActionResult TicketInfo(PNRModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid + "1");
                //
                TokenModel tokenModel = VNA_AuthencationService.GetSession();
                // create session 
                if (tokenModel == null)
                    return Notifization.Error("Cannot create session");
                // create session 
                string _token = tokenModel.Token;
                string _conversationId = tokenModel.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.NotService;
                // 
                string pnr = model.PNR;
                if (string.IsNullOrWhiteSpace(pnr))
                    return Notifization.Invalid("PNR code in valid");
                //
                using (var sessionService = new VNA_SessionService(tokenModel))
                {
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel
                    {
                        ConversationID = tokenModel.ConversationID,
                        Token = tokenModel.Token
                    };
                    VNA_EndTransaction vNATransaction = new VNA_EndTransaction();
                    //WSDesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new WSDesignatePrinterLLSRQService();
                    //var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    //if (printer.ApplicationResults.status != AIRService.WebService.WSDesignatePrinterLLSRQ.CompletionCodes.Complete)
                    //    return Notifization.Invalid(MessageText.Invalid);
                    var getReservationModel = new GetReservationModel
                    {
                        ConversationID = tokenModel.ConversationID,
                        Token = tokenModel.Token,
                        PNR = pnr
                    };
                    VNA_WSGetReservationRQService vNAWSGetReservationRQService = new VNA_WSGetReservationRQService();
                    ApiPortalBooking.Models.VNA_WS_Model.VNA.GetReservationData getReservationData = vNAWSGetReservationRQService.GetReservation(getReservationModel);
                    vNATransaction.EndTransaction(tokenModel);
                    return Notifization.Data("OK", getReservationData);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("OK" + ex);
            }
        }
        public ActionResult VoidTicket(PNRModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid + "1");
                //
                TokenModel tokenModel = VNA_AuthencationService.GetSession();
                // create session 
                if (tokenModel == null)
                    return Notifization.Error("Cannot create session");
                // create session 
                string _token = tokenModel.Token;
                string _conversationId = tokenModel.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.NotService;
                // 
                string pnr = model.PNR;
                if (string.IsNullOrWhiteSpace(pnr))
                    return Notifization.Invalid("PNR code in valid");
                //
                using (var sessionService = new VNA_SessionService(tokenModel))
                {
                    var voidTicketModel = new VNA_VoidTicketModel();
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel
                    {
                        ConversationID = tokenModel.ConversationID,
                        Token = tokenModel.Token
                    };
                    VNA_EndTransaction vNATransaction = new VNA_EndTransaction();
                    VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    if (printer.ApplicationResults.status != AIRService.WebService.VNA_DesignatePrinterLLSRQ.CompletionCodes.Complete)
                        return Notifization.Invalid(MessageText.Invalid);
                    //
                    var getReservationModel = new GetReservationModel
                    {
                        ConversationID = tokenModel.ConversationID,
                        Token = tokenModel.Token,
                        PNR = pnr
                    };
                    VNA_WSGetReservationRQService vNAWSGetReservationRQService = new VNA_WSGetReservationRQService();
                    var dataPnr = vNAWSGetReservationRQService.GetReservation(getReservationModel);
                    var ldata = new List<WebService.VNA_VoidTicketLLSRQ.VoidTicketRS>();
                    if (dataPnr.TicketDetails != null && dataPnr.TicketDetails.Count > 0)
                    {
                        VNA_VoidTicketLLSRQService vNAWSVoidTicketLLSRQService = new VNA_VoidTicketLLSRQService();
                        foreach (var item in dataPnr.TicketDetails.Where(m => m.TransactionIndicator == "TE"))
                        {
                            var data = vNAWSVoidTicketLLSRQService.VoidETicket(tokenModel, item.TicketNumber);
                            ldata.Add(data);
                        }
                        voidTicketModel.JsonResultVoidTicket = vNATransaction.EndTransaction(tokenModel);
                        voidTicketModel.Message = "Sucess";
                    }
                    else
                    {
                        voidTicketModel.Message = "Cannot find e-ticket data";
                    }
                    if (voidTicketModel.JsonResultVoidTicket != null && voidTicketModel.JsonResultVoidTicket.ApplicationResults.Success != null)
                    {
                        getReservationModel.ConversationID = tokenModel.ConversationID;
                        getReservationModel.Token = tokenModel.Token;
                        getReservationModel.PNR = pnr;
                        var dataPnr2 = vNAWSGetReservationRQService.GetReservation(getReservationModel);
                        VNA_OTA_CancelLLSRQService vNAWSOTA_CancelLLSRQService = new VNA_OTA_CancelLLSRQService();
                        var data = vNAWSOTA_CancelLLSRQService.OTA_CancelLLS(tokenModel);
                        var end = vNATransaction.EndTransaction(tokenModel);
                        voidTicketModel.JsonResultOTA_Cancel = end;
                    }
                    return Notifization.Data("OK", voidTicketModel);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("OK" + ex);
            }
        }
        public ActionResult TestTicket(string pnr)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pnr))
                    return Notifization.Invalid(MessageText.Invalid + "1");
                //
                TokenModel tokenModel = VNA_AuthencationService.GetSession();
                // create session 
                if (tokenModel == null)
                    return Notifization.Error("Cannot create session");
                // create session 
                string _token = tokenModel.Token;
                string _conversationId = tokenModel.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.NotService;
                // 
                using (var sessionService = new VNA_SessionService(tokenModel))
                {
                    VNA_WSGetReservationRQService vNAWSGetReservationRQService = new VNA_WSGetReservationRQService();
                    VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel
                    {
                        ConversationID = tokenModel.ConversationID,
                        Token = tokenModel.Token
                    };
                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    if (printer.ApplicationResults.status != AIRService.WebService.VNA_DesignatePrinterLLSRQ.CompletionCodes.Complete)
                        return Notifization.Invalid(MessageText.Invalid);
                    //
                    var getReservationModel = new GetReservationModel
                    {
                        ConversationID = tokenModel.ConversationID,
                        Token = tokenModel.Token,
                        PNR = pnr
                    };
                    var dataPnr = vNAWSGetReservationRQService.GetReservation(getReservationModel);
                    return Notifization.Data("OK", dataPnr);
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("OK" + ex);
            }
        }
    }
}
