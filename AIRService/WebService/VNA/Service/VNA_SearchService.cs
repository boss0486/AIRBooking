﻿using System;
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

namespace AIRService.Service
{
    public class VNA_SearchService
    {
        // search  
        public ActionResult FlightSearch(FlightSearchModel model)
        {
            TokenModel tokenModel = VNA_AuthencationService.GetSession();
            // create session 
            if (tokenModel == null)
                return Notifization.Error("Cannot create session");
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                // check model
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid);

                int _adt = model.ADT; // Adults
                int _cnn = model.CNN; // minors
                int _inf = model.INF; // Infant

                bool _isRoundTrip = model.IsRoundTrip;
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

                // get token
                string _token = tokenModel.Token;
                string _conversationId = tokenModel.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.Error("Cannot create token");
                // seach data in web service
                VNA_AirAvailLLSRQService airAvailLLSRQService = new VNA_AirAvailLLSRQService();
                VNAFareLLSRQService vnaFareLLSRQService = new VNAFareLLSRQService();
                // Flight >> go
                // ****************************************************************************************************************************
                //
                string _currencyCode = "VND";
                var lstFlightSegment = new List<FlightSegment>();
                var lstFlightSearch = new List<FlightSearch>();

                string _flightGo = model.OriginLocation;
                string _flightTo = model.DestinationLocation;
                var dataAirAvail = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ(new AirAvailLLSRQModel
                {
                    Token = _token,
                    ConversationID = _conversationId,
                    DepartureDateTime = _departureDateTime,
                    DestinationLocation = _flightTo,
                    OriginLocation = _flightGo
                });
                // check data
                if (dataAirAvail == null)
                    return Notifization.Error("AirAvail is null");
                // search light data
                var lstOrigin = dataAirAvail.OriginDestinationOptions.OriginDestinationOption.ToList();
                if (lstOrigin.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);
                // attachment INF (em bé)
                int _passengerTotal = model.ADT + model.CNN + model.INF;
                //  passenger total is availability 
                int _availabilityTotal = model.ADT + model.CNN;
                //
                //var _availabilityData =   List<>
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
                List<AIRService.WebService.VNA_FareLLSRQ.FareRS> fareRs = new List<WebService.VNA_FareLLSRQ.FareRS>();
                var dataFareLLS = vnaFareLLSRQService.FareLLS(fareLLSModel);
                if (dataFareLLS == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //

                //string json = JsonConvert.SerializeObject(dataFareLLS);
                //string fileName = "fare-test.json";
                //var urlFile = HttpContext.Current.Server.MapPath(@"~/WS/" + fileName);
                //write string to file
                //System.IO.File.WriteAllText(urlFile, json);


                fareRs.Add(dataFareLLS);
                // Fare 
                int _year = _departureDateTime.Year;
                foreach (var origin in lstOrigin)
                {
                    //#1. Availability: get all Flight is Availability
                    //#2.

                    var _lstFlightSegment = origin.FlightSegment;
                    if (_lstFlightSegment.Count() > 0)
                    {
                        foreach (var flightSegment in _lstFlightSegment)
                        {
                            var _datedepart = DateTime.Parse(_year + "-" + flightSegment.DepartureDateTime + ":00");
                            var _arrivalDateTime = _datedepart.AddMinutes(double.Parse(flightSegment.FlightDetails.TotalTravelTime));
                            // get list  ResBookDesigCode  is valid
                            var _lstBookingClassAvail = flightSegment.BookingClassAvail.ToList();
                            if (_lstBookingClassAvail.Count() > 0)
                            {
                                var fareDetails = GetFlightFares(_availabilityTotal, _lstBookingClassAvail, fareLLSModel, dataFareLLS, _currencyCode);
                                if (fareDetails.Count > 0)
                                {
                                    // check ticket condition
                                    int planeNo = Convert.ToInt32(flightSegment.FlightNumber);
                                    bool conditionState = AirTicketConditionFeeService.CheckTicketCondition(new AirTicketConditionCheckModel
                                    {
                                        PlaneNo = planeNo,
                                        DepartureDateTime = _returnDateTime
                                    });
                                    //
                                    lstFlightSegment.Add(new FlightSegment
                                    {
                                        AirEquipType = flightSegment.Equipment.AirEquipType,
                                        ArrivalDateTime = _arrivalDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                                        DepartureDateTime = _datedepart.ToString("dd/MM/yyyy HH:mm:ss"),
                                        DestinationLocation = _destinationLocation,
                                        FlightNo = planeNo,
                                        FlightType = (int)VNAEnum.FlightDirection.FlightGo,
                                        NumberInParty = _availabilityTotal,
                                        OriginLocation = _originLocation,
                                        FareDetails = fareDetails,
                                        RPH = flightSegment.RPH
                                    });
                                }

                            }

                        }
                    }
                }
                // group by time
                List<Response_FlightSearch> response_FlightSearch = new List<Response_FlightSearch>();
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
                                ArrivalDateTime = item.ArrivalDateTime,
                                DepartureDateTime = item.DepartureDateTime,
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
                //
                if (!model.IsRoundTrip)
                {
                    if (response_FlightSearch.Count() == 0)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    return Notifization.Data("OK -> IsRoundTrip: false ", response_FlightSearch);
                }
                ////////////
                //////////// Flight >> return
                //////////// ****************************************************************************************************************************
                // reset data
                lstFlightSegment = new List<FlightSegment>();
                _flightGo = model.DestinationLocation;
                _flightTo = model.OriginLocation;
                _departureDateTime = model.ReturnDateTime;
                dataAirAvail = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ(new AirAvailLLSRQModel
                {
                    Token = _token,
                    ConversationID = _conversationId,
                    DepartureDateTime = _departureDateTime,
                    DestinationLocation = _flightTo,
                    OriginLocation = _flightGo
                });
                // check data
                if (dataAirAvail == null)
                    return Notifization.Error("AirAvail is null");
                // search light data
                lstOrigin = dataAirAvail.OriginDestinationOptions.OriginDestinationOption.ToList();
                if (lstOrigin.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);
                // attachment INF (em bé)
                _passengerTotal = model.ADT + model.CNN + model.INF;
                //  passenger total is availability 
                _availabilityTotal = model.ADT + model.CNN;
                //
                //var _availabilityData =   List<>
                fareLLSModel = new FareLLSModel
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
                dataFareLLS = vnaFareLLSRQService.FareLLS(fareLLSModel);
                if (dataFareLLS == null)
                    return Notifization.Invalid(MessageText.Invalid);
                // Fare 
                fareRs.Add(dataFareLLS);

                _year = _departureDateTime.Year;
                foreach (var origin in lstOrigin)
                {
                    //#1. Availability: get all Flight is Availability
                    //#2.
                    var _lstFlightSegment = origin.FlightSegment;
                    if (_lstFlightSegment.Count() > 0)
                    {
                        foreach (var flightSegment in _lstFlightSegment)
                        {
                            var _datedepart = DateTime.Parse(_year + "-" + flightSegment.DepartureDateTime + ":00");
                            var _arrivalDateTime = _datedepart.AddMinutes(double.Parse(flightSegment.FlightDetails.TotalTravelTime));
                            // get list  ResBookDesigCode  is valid
                            var _lstBookingClassAvail = flightSegment.BookingClassAvail.ToList();
                            if (_lstBookingClassAvail.Count() > 0)
                            {
                                var fareDetails = GetFlightFares(_availabilityTotal, _lstBookingClassAvail, fareLLSModel, dataFareLLS, _currencyCode);
                                if (fareDetails.Count > 0)
                                {
                                    // check ticket condition
                                    int planeNo = Convert.ToInt32(flightSegment.FlightNumber);
                                    bool conditionState = AirTicketConditionFeeService.CheckTicketCondition(new AirTicketConditionCheckModel
                                    {
                                        PlaneNo = planeNo,
                                        DepartureDateTime = _departureDateTime
                                    });
                                    //
                                    lstFlightSegment.Add(new FlightSegment
                                    {
                                        AirEquipType = flightSegment.Equipment.AirEquipType,
                                        ArrivalDateTime = _arrivalDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                                        DepartureDateTime = _datedepart.ToString("dd/MM/yyyy HH:mm:ss"),
                                        DestinationLocation = _destinationLocation,
                                        FlightNo = Convert.ToInt32(flightSegment.FlightNumber),
                                        FlightType = (int)VNAEnum.FlightDirection.FlightReturn,
                                        NumberInParty = _availabilityTotal,
                                        OriginLocation = _originLocation,
                                        FareDetails = fareDetails,
                                        RPH = flightSegment.RPH
                                    });
                                }

                            }
                        }
                    }
                }
                //
                lstFlightSegmentGroupByTime = lstFlightSegment.GroupBy(m => new { m.DepartureDateTime, m.ArrivalDateTime }).Select(m => new { m.Key.ArrivalDateTime, m.Key.DepartureDateTime }).ToList();
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
                                ArrivalDateTime = item.ArrivalDateTime,
                                DepartureDateTime = item.DepartureDateTime,
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
                if (response_FlightSearch.Count() == 0)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                return Notifization.Data("OK -> IsRoundTrip: true ", response_FlightSearch);
            }
        }

        // Get list ResBookDesigCode is valid
        public List<FlightSegment_ResBookDesigCode> GetFlightFares(int _availabilityTotal, List<WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRSOriginDestinationOptionsOriginDestinationOptionFlightSegmentBookingClassAvail> _lstBookingClassAvail, FareLLSModel fareLLSModel, AIRService.WebService.VNA_FareLLSRQ.FareRS fareRS, string currencyCode)
        {
            List<FlightSegment_ResBookDesigCode> result = new List<FlightSegment_ResBookDesigCode>();
            foreach (var bookingClassAvail in _lstBookingClassAvail)
            {
                if (!string.IsNullOrWhiteSpace(bookingClassAvail.Availability) && int.Parse(bookingClassAvail.Availability) > _availabilityTotal)
                {
                    string _resBookDesigCode = bookingClassAvail.ResBookDesigCode;
                    // price 
                    var flightCostDetails = GetFareDetails(new VNAFareLLSRQModel
                    {
                        FareLLS = fareLLSModel,
                        FareRSData = fareRS,
                        CurrencyCode = currencyCode,
                        ResBookDesigCode = _resBookDesigCode
                    });
                    if (flightCostDetails != null && flightCostDetails.Count > 0)
                    {
                        result.Add(new FlightSegment_ResBookDesigCode
                        {
                            ResBookDesigCodeID = VNAFareLLSRQService.GetResbookDesigCodeIDByKey(_resBookDesigCode),
                            ResBookDesigCode = _resBookDesigCode,
                            FareItem = flightCostDetails
                        });
                    }
                }
            }
            result = result.OrderByDescending(m => m.ResBookDesigCodeID).ToList();
            return result;
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
        public List<FareItemModel> GetFareDetails(VNAFareLLSRQModel model)
        {
            var dataFareLLS = model.FareRSData;
            List<WebService.VNA_FareLLSRQ.FareRSFareBasis> fareRSFareBases = dataFareLLS.FareBasis.ToList();
            if (fareRSFareBases == null || fareRSFareBases.Count == 0)
                return null;
            //
            FareLLSModel fareLLSModel = model.FareLLS;
            if (fareLLSModel == null)
                return null;
            //
            var lstFareLLSModel = fareLLSModel.PassengerType.ToList().OrderBy(m => m).ToList();
            if (lstFareLLSModel.Count == 0)
                return null;
            // 
            List<FareItemModel> fareDetailsModels = new List<FareItemModel>();
            //string test = "";          
            foreach (var fareLLS in lstFareLLSModel)
            {
                List<FlightCost> flightCosts = new List<FlightCost>();
                int _cnt = 0;
                foreach (var fareRSFareBasis in fareRSFareBases)
                {
                    if (fareLLS == fareRSFareBasis.PassengerType[0].Code && fareRSFareBasis.AdditionalInformation.ResBookDesigCode == model.ResBookDesigCode && fareRSFareBasis.CurrencyCode == model.CurrencyCode)
                    {
                        //test += "| " + _cnt +":" + fareLLS
                        string strAmount = fareRSFareBasis.AdditionalInformation.Fare[0].Amount;
                        if (!string.IsNullOrWhiteSpace(strAmount) && Convert.ToDouble(strAmount) > 0)
                        {
                            double _amount = Convert.ToDouble(strAmount);
                            //flightCosts.Add(new FlightCost
                            //{
                            //    FareAmmout = _amount,
                            //    Total = _total
                            //});
                            fareDetailsModels.Add(new FareItemModel
                            {
                                RPH = fareRSFareBasis.RPH,
                                PassengerType = fareLLS,
                                FareAmount = _amount,
                                Code = fareRSFareBasis.Code
                                //ChargeDomesticFee = 0,
                                //ScreeningFee = 0.0,
                                //ServiceFee = 0.0,
                                //Tax = 0.0,
                                //Total = _total
                            });
                            _cnt++;
                        }
                    }
                }
            }
            if (fareDetailsModels.Count == 0)
                return null;
            //ok
            return fareDetailsModels;
        }

        public List<FareDetailsModel1> GetFareDetails2(VNAFareLLSRQModel model)
        {
            var dataFareLLS = model.FareRSData;
            List<WebService.VNA_FareLLSRQ.FareRSFareBasis> fareRSFareBases = dataFareLLS.FareBasis.ToList();
            if (fareRSFareBases == null || fareRSFareBases.Count == 0)
                return null;
            //
            FareLLSModel fareLLSModel = model.FareLLS;
            if (fareLLSModel == null)
                return null;
            //
            var lstFareLLSModel = fareLLSModel.PassengerType.ToList().OrderBy(m => m).ToList();
            if (lstFareLLSModel.Count == 0)
                return null;
            // 
            List<FareDetailsModel1> fareDetailsModels = new List<FareDetailsModel1>();
            //string test = "";          
            foreach (var fareLLS in lstFareLLSModel)
            {
                List<FlightCost> flightCosts = new List<FlightCost>();
                int _cnt = 0;
                foreach (var fareRSFareBasis in fareRSFareBases)
                {
                    if (fareLLS == fareRSFareBasis.PassengerType[0].Code && fareRSFareBasis.AdditionalInformation.ResBookDesigCode == model.ResBookDesigCode && fareRSFareBasis.CurrencyCode == model.CurrencyCode)
                    {
                        //test += "| " + _cnt +":" + fareLLS;
                        if (!string.IsNullOrWhiteSpace(fareRSFareBasis.BaseFare.Amount) && Convert.ToDouble(fareRSFareBasis.BaseFare.Amount) > 0)
                        {
                            double _amount = Convert.ToDouble(fareRSFareBasis.BaseFare.Amount);
                            fareDetailsModels.Add(new FareDetailsModel1
                            {
                                RPH = fareRSFareBasis.RPH,
                                ResBookDesigCode = model.ResBookDesigCode,
                                PassengerType = fareLLS,
                                FareAmount = _amount,
                                Code = fareRSFareBasis.Code
                            });
                            _cnt++;
                        }
                    }
                }
            }
            if (fareDetailsModels.Count == 0)
                return null;
            //ok
            return fareDetailsModels;
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
                if (!Helper.Page.Validate.TestDateTime(model.DepartureDateTime))
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


        ////public ActionResult TaxFee(FlightFareModel model)
        ////{
        ////    using (var sessionService = new VNA_SessionService())
        ////    {
        ////        // check model
        ////        if (model == null)
        ////            return Notifization.Invalid(MessageText.Invalid);
        ////        // 
        ////        //int _adt = model.ADT; // Adults
        ////        //int _cnn = model.CNN; // minors
        ////        //int _inf = model.INF; // Infant
        ////        string _destinationLocation = model.DestinationLocation;
        ////        string _originLocation = model.OriginLocation;
        ////        DateTime _departureDateTime = model.DepartureDateTime;
        ////        ////
        ////        //if (_adt <= 0)
        ////        //    return Notifization.Invalid("Adults must be > 0");
        ////        ////
        ////        //if (_inf > _adt)
        ////        //    return Notifization.Invalid("Infant invalid");
        ////        // create session
        ////        var _session = sessionService.GetSession();
        ////        if (_session == null)
        ////            return Notifization.NotService;
        ////        // get token
        ////        string _token = _session.Token;
        ////        string _conversationId = _session.ConversationID;
        ////        if (string.IsNullOrWhiteSpace(_token))
        ////            return Notifization.NotService;
        ////        // seach data in web service
        ////        VNAOTA_AirTaxRQService vNAOTA_AirTaxRQService = new VNAOTA_AirTaxRQService();
        ////        AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRS airTaxRS = vNAOTA_AirTaxRQService.AirTax(_session, new Resquet_WsTaxModel
        ////        {
        ////            RPH = model.RPH,
        ////            OriginLocation = _originLocation,
        ////            DestinationLocation = _destinationLocation,
        ////            DepartureDateTime = _departureDateTime,
        ////            ResBookDesigCode = model.ResBookDesigCode,
        ////            ArrivalDateTime = model.ArrivalDateTime,
        ////            AirEquipType = model.AirEquipType,
        ////            PassengerType = model.PassengerType,
        ////            FareBasisCode = model.FareBasisCode,
        ////            BaseFare = new Resquet_WsTax_BaseFareModel
        ////            {
        ////                Amount = model.BaseFare.Amount,
        ////                CurrencyCode = model.BaseFare.CurrencyCode
        ////            }
        ////        });
        ////        //
        ////        string jsonData = JsonConvert.SerializeObject(airTaxRS.Items[1]);
        ////        FeeDetailsModel feeDetailsModel = JsonConvert.DeserializeObject<FeeDetailsModel>(jsonData);
        ////        //
        ////        if (feeDetailsModel == null)
        ////            return Notifization.NotFound(MessageText.NotFound);
        ////        //
        ////        FlightTax response_FlightFee = new FlightTax
        ////        {
        ////            RPH = feeDetailsModel.ItineraryInfo[0].RPH,
        ////            RPHSpecified = feeDetailsModel.ItineraryInfo[0].RPHSpecified,
        ////            PassengerType = feeDetailsModel.ItineraryInfo[0].PTC_FareBreakdown.PassengerType,
        ////            Total = feeDetailsModel.ItineraryInfo[0].TaxInfo.Total,
        ////            Taxes = feeDetailsModel.ItineraryInfo[0].TaxInfo.Taxes
        ////        };
        ////        return Notifization.Data("", response_FlightFee);
        ////    }
        ////}
        // 
        public ActionResult TaxFeeTest(List<FlightFareModel> models)
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
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                // check model
                if (models.Count == 0)
                    return Notifization.Invalid(MessageText.Invalid);
                // seach data in web service
                VNA_OTA_AirTaxRQService vNAOTA_AirTaxRQService = new VNA_OTA_AirTaxRQService();

                List<Resquet_WsTaxModel> resquet_WsTaxes = new List<Resquet_WsTaxModel>();
                foreach (var item in models)
                {
                    resquet_WsTaxes.Add(new Resquet_WsTaxModel
                    {
                        RPH = item.RPH,
                        OriginLocation = item.OriginLocation,
                        DestinationLocation = item.DestinationLocation,
                        DepartureDateTime = item.DepartureDateTime,
                        ResBookDesigCode = item.ResBookDesigCode,
                        ArrivalDateTime = item.ArrivalDateTime,
                        AirEquipType = item.AirEquipType,
                        PassengerType = item.PassengerType,
                        FareBasisCode = item.FareBasisCode,
                        BaseFare = new Resquet_WsTax_BaseFareModel
                        {
                            Amount = item.BaseFare.Amount,
                            CurrencyCode = item.BaseFare.CurrencyCode
                        }
                    });
                }
                List<AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRS> airTaxRS = vNAOTA_AirTaxRQService.AirTaxList(tokenModel, resquet_WsTaxes);

                return Notifization.Data(":::::::::::::::::", airTaxRS);
                ////
                //string jsonData = JsonConvert.SerializeObject(airTaxRS.Items[1]);
                //FeeDetailsModel feeDetailsModel = JsonConvert.DeserializeObject<FeeDetailsModel>(jsonData);
                ////
                //if (feeDetailsModel == null)
                //    return Notifization.NotFound(MessageText.NotFound);
                ////
                //Response_FlightFee response_FlightFee = new Response_FlightFee
                //{
                //    RPH = feeDetailsModel.ItineraryInfo[0].RPH,
                //    RPHSpecified = feeDetailsModel.ItineraryInfo[0].RPHSpecified,
                //    PassengerType = feeDetailsModel.ItineraryInfo[0].PTC_FareBreakdown.PassengerType,
                //    Total = feeDetailsModel.ItineraryInfo[0].TaxInfo.Total,
                //    Taxes = feeDetailsModel.ItineraryInfo[0].TaxInfo.Taxes
                //};
                //return Notifization.Data("", response_FlightFee);
            }
        }


        public ActionResult TaxFee(List<FlightFareModel> models)
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
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                // check model
                if (models.Count == 0)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                VNA_OTA_AirTaxRQService vNAOTA_AirTaxRQService = new VNA_OTA_AirTaxRQService();
                var ListTax = new List<TaxModel>();
                foreach (var item in models)
                {
                    string _destinationLocation = item.DestinationLocation;
                    string _originLocation = item.OriginLocation;
                    DateTime departureDateTime = item.DepartureDateTime;
                    var ItineraryInfo = new WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfo
                    {
                        ReservationItems = new AirTaxRQItineraryInfoReservationItems
                        {
                            Item = new AirTaxRQItineraryInfoReservationItemsItem()
                        }
                    };
                    ItineraryInfo.ReservationItems.Item.RPH = 1;
                    ItineraryInfo.ReservationItems.Item.SalePseudoCityCode = "LZJ";
                    ItineraryInfo.ReservationItems.Item.TicketingCarrier = "VN";
                    ItineraryInfo.ReservationItems.Item.ValidatingCarrier = "VN";

                    ItineraryInfo.ReservationItems.Item.FlightSegment = new AirTaxRQItineraryInfoReservationItemsItemFlightSegment[1];

                    var FlightSegment = new AirTaxRQItineraryInfoReservationItemsItemFlightSegment
                    {
                        DepartureDateTime = item.DepartureDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss"),

                        ArrivalDateTime = item.ArrivalDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss"),
                        FlightNumber = (Int16)item.FlightNumber,
                        ResBookDesigCode = item.ResBookDesigCode,
                        ForceConnectionInd = true,
                        ForceStopOverInd = true,

                        DepartureAirport = new AirTaxRQItineraryInfoReservationItemsItemFlightSegmentDepartureAirport
                        {
                            CodeContext = "IATA",
                            LocationCode = _originLocation
                        },

                        ArrivalAirport = new AirTaxRQItineraryInfoReservationItemsItemFlightSegmentArrivalAirport
                        {
                            CodeContext = "IATA",
                            LocationCode = _destinationLocation
                        },

                        MarketingAirline = new AirTaxRQItineraryInfoReservationItemsItemFlightSegmentMarketingAirline
                        {
                            Code = "VN"
                        },

                        OperatingAirline = new AirTaxRQItineraryInfoReservationItemsItemFlightSegmentOperatingAirline()
                    };
                    FlightSegment.OperatingAirline.Code = "VN";

                    FlightSegment.Equipment = new AirTaxRQItineraryInfoReservationItemsItemFlightSegmentEquipment();
                    FlightSegment.Equipment.AirEquipType = Convert.ToString(item.AirEquipType);

                    ItineraryInfo.ReservationItems.Item.FlightSegment[0] = FlightSegment;

                    ItineraryInfo.ReservationItems.Item.AirFareInfo = new AirTaxRQItineraryInfoReservationItemsItemAirFareInfo();
                    ItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown = new AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdown();

                    ItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerType = new AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerType();
                    ItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerType.Code = item.PassengerType;

                    ItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.FareBasisCode = item.FareBasisCode;

                    ItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare = new AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerFare();
                    ItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare = new AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerFareBaseFare();
                    ItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.Amount = item.BaseFare.Amount;
                    ItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.CurrencyCode = item.BaseFare.CurrencyCode;
                    var tax = vNAOTA_AirTaxRQService.AirTax(tokenModel, ItineraryInfo);
                    ListTax.Add(tax);
                }
                return Notifization.Data("", ListTax);
                //string jsonData = JsonConvert.SerializeObject(airTaxRS.Items[1]);
                //FeeDetailsModel feeDetailsModel = JsonConvert.DeserializeObject<FeeDetailsModel>(jsonData);
                ////
                //if (feeDetailsModel == null)
                //    return Notifization.NotFound(MessageText.NotFound);
                ////
                //Response_FlightFee response_FlightFee = new Response_FlightFee
                //{
                //    RPH = feeDetailsModel.ItineraryInfo[0].RPH,
                //    RPHSpecified = feeDetailsModel.ItineraryInfo[0].RPHSpecified,
                //    PassengerType = feeDetailsModel.ItineraryInfo[0].PTC_FareBreakdown.PassengerType,
                //    Total = feeDetailsModel.ItineraryInfo[0].TaxInfo.Total,
                //    Taxes = feeDetailsModel.ItineraryInfo[0].TaxInfo.Taxes
                //};
                //return Notifization.Data("", response_FlightFee);
            }
        }
        ///  ****************************************************************************************************************************************


        //public ActionResult FlightFeeByPassenger(FlightFare_PassengerTypeModel model)
        //{
        //    using (var sessionService = new VNA_SessionService())
        //    {
        //        // check model
        //        if (model == null)
        //            return Notifization.Invalid(MessageText.Invalid);
        //        // 
        //        string _destinationLocation = model.DestinationLocation;
        //        string _originLocation = model.OriginLocation;
        //        DateTime _departureDateTime = model.DepartureDateTime;
        //        // create session
        //        var _session = sessionService.GetSession();
        //        if (_session == null)
        //            return Notifization.NotService;
        //        // get token
        //        string _token = _session.Token;
        //        string _conversationId = _session.ConversationID;
        //        if (string.IsNullOrWhiteSpace(_token))
        //            return Notifization.NotService;
        //        // seach data in web service
        //        VNAOTA_AirTaxRQService vNAOTA_AirTaxRQService = new VNAOTA_AirTaxRQService();
        //        List<Response_FlightFee> response_FlightFees = new List<Response_FlightFee>();

        //        foreach (var item in model.FareBase)
        //        {
        //            string _passengerType = item.PassengerType;
        //            float _amount = item.Amount;
        //            string _fareBasisCode = item.FareBasisCode;
        //            string _rph = model.RPH;
        //            AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRS airTaxRS = vNAOTA_AirTaxRQService.GetTaxByFilght(new Resquet_WsTaxModel
        //            {
        //                ConversationID = _conversationId,
        //                Token = _token,
        //                RPH = _rph,
        //                OriginLocation = _originLocation,
        //                DestinationLocation = _destinationLocation,
        //                DepartureDateTime = _departureDateTime,
        //                ResBookDesigCode = model.ResBookDesigCode,
        //                ArrivalDateTime = model.ArrivalDateTime,
        //                AirEquipType = model.AirEquipType,
        //                PassengerType = _passengerType,
        //                FareBasisCode = _fareBasisCode,
        //                BaseFare = new Resquet_WsTax_BaseFareModel
        //                {
        //                    Amount = _amount,
        //                    CurrencyCode = model.CurrencyCode
        //                }
        //            });
        //            if (airTaxRS.Items.Count() > 0 && airTaxRS.Items[1] != null)
        //            {
        //                string jsonData = JsonConvert.SerializeObject(airTaxRS.Items[1]);
        //                FeeDetailsModel feeDetailsModel = JsonConvert.DeserializeObject<FeeDetailsModel>(jsonData);
        //                response_FlightFees.Add(new Response_FlightFee
        //                {
        //                    RPH = feeDetailsModel.ItineraryInfo[0].RPH,
        //                    RPHSpecified = feeDetailsModel.ItineraryInfo[0].RPHSpecified,
        //                    PassengerType = feeDetailsModel.ItineraryInfo[0].PTC_FareBreakdown.PassengerType,
        //                    Total = feeDetailsModel.ItineraryInfo[0].TaxInfo.Total,
        //                    Taxes = feeDetailsModel.ItineraryInfo[0].TaxInfo.Taxes
        //                });
        //            }
        //        }
        //        //
        //        if (response_FlightFees.Count == 0)
        //            return Notifization.NotFound(MessageText.NotFound);
        //        //
        //        return Notifization.Data("OK", response_FlightFees);
        //    }
        //}

        public ActionResult FlightFeeBasic(List<FeeTaxBasicModel> models)
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
            VNA_OTA_AirTaxRQService vNAOTA_AirTaxRQService = new VNA_OTA_AirTaxRQService();
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                List<FlightTax> flightTaxFees = new List<FlightTax>();
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
                            return Notifization.Error("BookDesigCode is empty");

                        resBookDesigCode = resBookDesigCode.Trim();
                        //
                        List<FlightTaxInfo> flightTaxInfos = new List<FlightTaxInfo>();
                        foreach (var item in flight.FareBase)
                        {
                            string _passengerType = item.PassengerType;
                            float _amount = item.Amount;
                            string _fareBasisCode = item.FareBaseCode;
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
                                PassengerType = _passengerType,
                                FareBasisCode = _fareBasisCode,
                                BaseFare = new Resquet_WsTax_BaseFareModel
                                {
                                    RPH = item.RPH,
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
        public ActionResult BookVe(Request_BookModel model)
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

                foreach (var item in model.Passengers)
                {
                    if (!Helper.Page.Validate.TestTextNoneUnicode(item.FullName))
                        return Notifization.Invalid("Tên khách hàng '" + item.FullName + "' không hợp lệ");
                    //
                }
                // check blance of user booker

                var currentUserId = Helper.Current.UserLogin.IdentifierID;

                WalletUserService walletUserService = new WalletUserService();

                WalletUserMessageModel walletMessageModel = walletUserService.GetBalanceByUserID(currentUserId);
                double balanceUser = walletMessageModel.Balance;
                if (!walletMessageModel.Status)
                    return Notifization.Invalid("Lỗi kiểm tra số dư hạn mức");
                //

                if (balanceUser <= 0)
                    return Notifization.Invalid("Số dư hạn mức không đủ");
                //
                return Notifization.TEST("Test số dư hạn mức");

                using (var sessionService = new VNA_SessionService(tokenModel))
                {
                    if (string.IsNullOrWhiteSpace(_token))
                        return Notifization.Invalid("Can not create token");
                    //
                    BookEmailService appBookEmailService = new BookEmailService();
                    var appBookEmail = appBookEmailService.GetAlls(m => m.IsActive).FirstOrDefault();
                    if (appBookEmail == null)
                        return Notifization.Error("Thông tin đại lý chưa được cấu hình");

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
                            return Notifization.Invalid("Segments information invalid");
                        //
                        AIRService.WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRS oTA_AirBookRS = vNAWSOTA_AirBookLLSRQSevice.FUNC_OTA_AirBookRS(new AirBookModel
                        {
                            Segments = lstSegments,
                            ConversationID = _conversationId,
                            Token = _token
                        });

                        if (oTA_AirBookRS.ApplicationResults.Success == null)
                            return Notifization.Data("AirBook invalid", oTA_AirBookRS);

                        var _originDestinationOption = oTA_AirBookRS.OriginDestinationOption.ToList();
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

                        string file = HttpContext.Current.Server.MapPath(@"/Team/temp.json");
                        PricingInPNR_Test airPriceData = new PricingInPNR_Test();
                        using (StreamReader r = new StreamReader(file))
                        {
                            string json = r.ReadToEnd();
                            airPriceData = JsonConvert.DeserializeObject<PricingInPNR_Test>(json);
                        }
                        //
                        double feeTotal = double.Parse(airPriceData.PriceQuote.PricedItinerary.TotalAmount);
                        if (balanceUser < feeTotal)
                            return Notifization.TEST("Số dư không đủ, vui lòng nạp thêm tiền");

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
                                    PassengerType = passengerType,
                                    Text = tax.TaxName,
                                    TaxCode = tax.TaxCode,
                                    Amount = double.Parse(tax.Amount)
                                });
                            }
                        }

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
                        // call service get PNR code
                        var passengerDetailModel = new DetailsRQ
                        {
                            Email = appBookEmail.Email,
                            Phone = appBookEmail.Phone,
                            ConversationID = _conversationId,
                            Token = _token,
                            Passengers = passengerDetailsRQ,
                            AirBook = oTA_AirBookRS
                        };
                        //call service  passenger details in ws.service
                        VNA_PassengerDetailsRQService vNAWSPassengerDetailsRQService = new VNA_PassengerDetailsRQService();
                        // string pnrCode = vNAWSPassengerDetailsRQService.PassengerDetail2(passengerDetailModel);
                        //End transaction 
                        var pnrCode = "PNR-00001";
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
                                AirEquipType = item.AirEquipType
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
                        // call save order
                        var orderTicketId = orderTicketService.BookOrder(new Request_OrderTicketModel
                        {
                            Flights = requestOrderFlightModels,
                            PriceFare = requestOrderPriceModels,
                            TaxFare = requestOrderTaxModels
                        });


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
                            OrderID = orderTicketId,
                            PNR = result.PNR,
                            Flights = model.Flights,
                            Passengers = bookTicketPassengers,
                            FareTaxs = result.FareTaxs,
                            FareFlights = fareFlights,
                            Contacts = model.Contacts
                        });
                        result.TicketID = bookTicketId;

                        if (!string.IsNullOrWhiteSpace(bookTicketId))
                        {
                            // update user balance

                        }


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
        public ActionResult BookVeTest(Request_BookModel model)
        {
            try
            {
                ////using (var sessionService = new VNA_SessionService())
                ////{
                ////////var tokenModel = new TokenModel();
                ////////var _session = sessionService.GetSession();
                ////////if (_session == null)
                ////////    return Notifization.Invalid("Can not create session");
                //////////
                ////////string _token = _session.Token;
                ////////string _conversationId = _session.ConversationID;
                ////////if (string.IsNullOrWhiteSpace(_token))
                ////////    return Notifization.Invalid("Can not create token");
                //
                BookEmailService appBookEmailService = new BookEmailService();
                var appBookEmail = appBookEmailService.GetAlls(m => m.IsActive).FirstOrDefault();
                if (appBookEmail == null)
                    return Notifization.Error("Thông tin đại lý chưa được cấu hình");

                //using (var vnaTransaction = new VNA_EndTransaction(_session))
                //{
                ////////var airBookModel = new AirBookModel
                ////////{
                ////////    ConversationID = _conversationId,
                ////////    Token = _token
                ////////};
                //seach data in web service

                // #1. flight information - ***********************************************************************************************************************************
                ////////VNA_WSOTA_AirBookLLSRQSevice vNAWSOTA_AirBookLLSRQSevice = new VNA_WSOTA_AirBookLLSRQSevice();
                ////////List<BookSegmentModel> lstSegments = model.Flights;
                ////////if (lstSegments == null || lstSegments.Count == 0)
                ////////    return Notifization.Invalid("Segments information invalid");
                //////////
                ////////AIRService.WebService.WSOTA_AirBookLLSRQ.OTA_AirBookRS oTA_AirBookRS = vNAWSOTA_AirBookLLSRQSevice.FUNC_OTA_AirBookRS(new AirBookModel
                ////////{
                ////////    Segments = lstSegments,
                ////////    ConversationID = _conversationId,
                ////////    Token = _token
                ////////});

                ////////if (oTA_AirBookRS.ApplicationResults.Success == null)
                ////////    return Notifization.Data("AirBook invalid", oTA_AirBookRS);
                //
                ////////var _originDestinationOption = oTA_AirBookRS.OriginDestinationOption.ToList();
                ////////if (_originDestinationOption.Count == 0)
                ////////    return Notifization.NotFound(MessageText.NotFound);

                // #2. Get price - ***************************************************************************************************************************************************
                ////////airBookModel.Segments = model.Flights;
                var airPriceModel = new AirPriceModel();
                ////////var airPriceModel = new AirPriceModel
                ////////{
                ////////    ConversationID = _conversationId,
                ////////    Token = _token,
                ////////    lFlight = model.Flights
                ////////};

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
                ////////VNA_WSOTA_AirPriceLLSRQService vNAWSOTA_AirPriceLLSRQService = new VNA_WSOTA_AirPriceLLSRQService();
                ////////var airPriceData = vNAWSOTA_AirPriceLLSRQService.AirPrice(airPriceModel);

                // #3. Get PNA code - **************************************************************************************************************************************************

                string file = HttpContext.Current.Server.MapPath(@"/Team/temp.json");
                PricingInPNR_Test airPriceData = new PricingInPNR_Test();
                using (StreamReader r = new StreamReader(file))
                {
                    string json = r.ReadToEnd();
                    airPriceData = JsonConvert.DeserializeObject<PricingInPNR_Test>(json);
                }
                // var a = airPriceData.PriceQuote.PricedItinerary.AirItineraryPricingInfo[0];

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
                            PassengerType = passengerType,
                            Text = tax.TaxName,
                            TaxCode = tax.TaxCode,
                            Amount = double.Parse(tax.Amount)
                        });
                    }
                }
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
                // call service get PNR code
                ////////var passengerDetailModel = new DetailsRQ
                ////////{
                ////////    Email = appBookEmail.Email,
                ////////    Phone = appBookEmail.Phone,
                ////////    ConversationID = _conversationId,
                ////////    Token = _token,
                ////////    Passengers = passengerDetailsRQ,
                ////////    AirBook = oTA_AirBookRS
                ////////};
                //////////call service  passenger details in ws.service
                ////////VNA_WSPassengerDetailsRQService vNAWSPassengerDetailsRQService = new VNA_WSPassengerDetailsRQService();
                ////////string pnrCode = vNAWSPassengerDetailsRQService.PassengerDetail2(passengerDetailModel);
                //////////End transaction
                ////////vnaTransaction.EndTransaction();
                ////////if (string.IsNullOrWhiteSpace(pnrCode))
                ////////    return Notifization.Invalid("Cannot get PNA code");
                //set PNR code
                var pnrCode = "PNR-00001";
                result.PNR = pnrCode;
                ////////sessionService.CloseSession(_session);

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
                        AirEquipType = item.AirEquipType
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
                // call save order
                var orderTicketId = orderTicketService.BookOrder(new Request_OrderTicketModel
                {
                    Flights = requestOrderFlightModels,
                    PriceFare = requestOrderPriceModels,
                    TaxFare = requestOrderTaxModels
                });

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
                    OrderID = orderTicketId,
                    PNR = result.PNR,
                    Flights = model.Flights,
                    Passengers = bookTicketPassengers,
                    FareTaxs = result.FareTaxs,
                    FareFlights = fareFlights,
                    Contacts = model.Contacts
                });
                result.TicketID = bookTicketId;
                return Notifization.Data("Đặt chỗ thành công:" + bookTicketId + " PNR: " + pnrCode, result); // "/backend/flightbook/details/" + bookIdFist
                                                                                                             ////    } // end using tran session
                                                                                                             ////} // end using session
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
                            BookPNRCodeService bookPNRCodeService = new BookPNRCodeService();
                            var bookPNRCode = bookPNRCodeService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == pnr.ToLower()).FirstOrDefault();
                            if (bookPNRCode != null)
                            {
                                bookPNRCode.Enabled = (int)BookTicketEnum.BookPNRCodeStatus.ExTicket;
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
