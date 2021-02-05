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
using AIRService.WS.VNAHelper;
using Helper.TimeData;
using Helper.Language;
using Itenso.TimePeriod;

namespace AIRService.Service
{
    public class VNA_SearchService
    {

        // test fare
        public ActionResult Test01(FareLLSModel model)
        {
            VNAFareLLSRQService vnaFareLLSRQService = new VNAFareLLSRQService();
            return Notifization.Data("ok", vnaFareLLSRQService.FareLLS2(model));
        }

        // search  
        public ActionResult BookSearch(SegmentSearchModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            bool isHasTax = model.IsHasTax;
            int itineraryType = model.ItineraryType;
            TokenModel tokenModel = VNA_AuthencationService.GetSession();
            // create session 
            if (tokenModel == null)
                return Notifization.Invalid(MessageText.Invalid);
            // get token
            string _token = tokenModel.Token;
            string _conversationId = tokenModel.ConversationID;
            if (string.IsNullOrWhiteSpace(_token))
                return Notifization.Invalid(MessageText.NotService);
            //
            VNA_AirAvailLLSRQService airAvailLLSRQService = new VNA_AirAvailLLSRQService();
            VNAFareLLSRQService vnaFareLLSRQService = new VNAFareLLSRQService();
            List<RsSegment> response_FlightSearch = new List<RsSegment>();
            AirTicketCondition04Service airTicketCondition04Service = new AirTicketCondition04Service();
            AirTicketCondition05Service airTicketCondition05Service = new AirTicketCondition05Service();
            AirportConfigService airportConfigService = new AirportConfigService();
            AirportService airportService = new AirportService();
            AirTicketCondition04 airTicketCondition04 = airTicketCondition04Service.GetAlls(m => m.IsApplied).FirstOrDefault();
            List<FlightSegment> lstFlightSegment = new List<FlightSegment>();
            using (var sessionService = new VNA_SessionService(tokenModel))
            {

                string _destinationLocation = model.DestinationLocation;
                string _originLocation = model.OriginLocation;
                DateTime _departureDateTime = model.DepartureDateTime;
                int _adt = model.ADT; // Adults
                int _cnn = model.CNN; // minors
                int _inf = model.INF; // Infant
                int _availabilityTotal = model.ADT + model.CNN;
                string segmentText = $"{_originLocation}-{_destinationLocation}";
                if (_adt <= 0)
                    return Notifization.Invalid($"#{segmentText}: số lượng người lớn phải > 0");
                //
                if (_inf > _adt)
                    return Notifization.Invalid($"#{segmentText}: số lượng em bé không hợp lệ");
                // tax Ax:              
                Airport airportGo = airportService.GetAlls(m => m.IATACode == _originLocation).FirstOrDefault();
                if (airportGo == null)
                    return Notifization.Invalid($"#{segmentText}: không tìm thấy sân bay");
                //
                double axFeeGo = airportConfigService.GetAlls(m => m.AirportID == airportGo.ID).FirstOrDefault().AxFee;
                // ****************************************************************************************************************************
                string _currencyCode = "VND";
                List<XMLObject.AirAvailLLSRQ.FlightSegment> flightSegments = new List<XMLObject.AirAvailLLSRQ.FlightSegment>();
                //
                AirAvailLLSRQModel airAvailLLSRQModel = new AirAvailLLSRQModel
                {
                    Token = _token,
                    ConversationID = _conversationId,
                    DepartureDateTime = _departureDateTime,
                    DestinationLocation = _destinationLocation,
                    OriginLocation = _originLocation
                };
                XMLObject.AirAvailLLSRQ.OTA_AirAvailRS airAvailResult = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ(airAvailLLSRQModel);
                //
                if (airAvailResult.ApplicationResults.Status != "Complete")
                    return Notifization.Invalid(MessageText.Invalid);
                //
                airAvailResult.OriginDestinationOptions.OriginDestinationOption.Select(m => m.FlightSegment).ToList();
                flightSegments.AddRange(airAvailResult.OriginDestinationOptions.OriginDestinationOption.Select(m => m.FlightSegment));
                // loop next 1
                var airAvailResult1 = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQLoop(airAvailLLSRQModel);
                if (airAvailResult1.ApplicationResults.Status == "Complete")
                    flightSegments.AddRange(airAvailResult1.OriginDestinationOptions.OriginDestinationOption.Select(m => m.FlightSegment));
                // loop next 2 
                var airAvailResult2 = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQLoop(airAvailLLSRQModel);
                if (airAvailResult2.ApplicationResults.Status == "Complete")
                    flightSegments.AddRange(airAvailResult2.OriginDestinationOptions.OriginDestinationOption.Select(m => m.FlightSegment));
                // loop next 3 
                XMLObject.AirAvailLLSRQ.OTA_AirAvailRS airAvailResult3 = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQLoop(airAvailLLSRQModel);
                if (airAvailResult3.ApplicationResults.Status == "Complete")
                    flightSegments.AddRange(airAvailResult3.OriginDestinationOptions.OriginDestinationOption.Select(m => m.FlightSegment));
                //
                //XMLHelper.WriteXml("search-ko-bay-thang11", XMLHelper.GetXMLFromObject(flightSegmentsGo));
                string test = string.Empty;
                if (flightSegments.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);
                //
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
                if (_adt > 0)
                    fareLLSModel.PassengerType.Add("ADT");
                if (_cnn > 0)
                    fareLLSModel.PassengerType.Add("CNN");
                if (_inf > 0)
                    fareLLSModel.PassengerType.Add("INF");
                //
                fareLLSModel.CurrencyCode = _currencyCode;
                fareLLSModel.AxFee = axFeeGo;
                //
                XMLObject.FareLLSRQ.FareRS fareRS = vnaFareLLSRQService.FareLLS2(fareLLSModel);
                if (fareRS == null)
                    return Notifization.Invalid(MessageText.Invalid + "111111");
                // 
                List<XMLObject.FareLLSRQ.FareBasis> fareBasis = fareRS.FareBasis;
                if (fareBasis == null)
                    return Notifization.Invalid(MessageText.Invalid + "111111");
                // get tax 
                string flightLocation = _originLocation + "-" + _destinationLocation;
                AirTicketCondition05 airTicketCondition05 = airTicketCondition05Service.GetAlls(m => m.IsApplied && m.FlightLocationID == flightLocation).FirstOrDefault();
                int _year = _departureDateTime.Year;
                foreach (var segment in flightSegments)
                {
                    int flightNumber = Convert.ToInt32(segment.FlightNumber);
                    if (segment.DestinationLocation.LocationCode != _destinationLocation || segment.OriginLocation.LocationCode != _originLocation)
                    {
                        test += $"Go:{segment.OriginLocation.LocationCode}-{segment.DestinationLocation.LocationCode}|";
                    }
                    else
                    {
                        DateTime _datedepart = DateTime.Parse(_year + "-" + segment.DepartureDateTime + ":00");
                        DateTime _arrivalDateTime = _datedepart;
                        // lay tat ca hang ghe dang ban (maketting) has Availability > 0 va >= tong so luong hanh khach (adt + cnn)
                        List<string> _lstResBookDesigCode = segment.BookingClassAvail.Where(m => Convert.ToInt32(m.Availability) >= _availabilityTotal).Select(m => m.ResBookDesigCode).ToList();
                        if (_lstResBookDesigCode.Count() == 0)
                            continue;
                        // 
                        List<FlightSegment_ResBookDesigCode> fareDetails = GetFlightFares(_lstResBookDesigCode, fareLLSModel, fareBasis, new FlightAirTicketCondition
                        {
                            OriginLocation = _originLocation,
                            DestinationLocation = _destinationLocation,
                            DepartureDateTime = _departureDateTime,
                            ArrivalDateTime = _arrivalDateTime,
                            AirEquipType = segment.Equipment.AirEquipType,
                            FlightNo = flightNumber,
                            ItineraryType = itineraryType,
                            AirTicketCondition04 = airTicketCondition04,
                            AirTicketCondition05 = airTicketCondition05,
                        });
                        //
                        _arrivalDateTime = _datedepart.AddMinutes(Convert.ToDouble(segment.FlightDetails.TotalTravelTime));
                        //
                        lstFlightSegment.Add(new FlightSegment
                        {
                            AirEquipType = segment.Equipment.AirEquipType,
                            ArrivalDateTime = _arrivalDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                            DepartureDateTime = _datedepart.ToString("dd/MM/yyyy HH:mm:ss"),
                            DestinationLocation = _destinationLocation,
                            FlightNo = flightNumber,
                            FlightType = (int)VNAEnum.FlightDirection.FlightGo,
                            NumberInParty = _availabilityTotal,
                            OriginLocation = _originLocation,
                            FareDetails = fareDetails,
                            RPH = segment.RPH
                        });
                    }
                }

                // end for
                //List<FlightSegment> lstFlightSegment1 = lstFlightSegment.Where(m => m.OriginLocation == item.OriginLocation && m.DestinationLocation == item.DestinationLocation).ToList();
                List<RsDetails> rsDetails = new List<RsDetails>();
                var lstFlightSegmentGroupByTime = lstFlightSegment.GroupBy(m => new { m.SegmentText, m.DepartureDateTime, m.ArrivalDateTime }).Select(m => new { m.Key.SegmentText, m.Key.ArrivalDateTime, m.Key.DepartureDateTime }).ToList();
                foreach (var itemData in lstFlightSegmentGroupByTime)
                {
                    foreach (var flightSegment in lstFlightSegment)
                    {
                        if (itemData.DepartureDateTime == flightSegment.DepartureDateTime && itemData.ArrivalDateTime == flightSegment.ArrivalDateTime)
                        {
                            rsDetails.Add(new RsDetails
                            {
                                AirEquipType = flightSegment.AirEquipType,
                                DepartureDateTime = flightSegment.DepartureDateTime,
                                ArrivalDateTime = flightSegment.ArrivalDateTime,
                                FlightNo = flightSegment.FlightNo,
                                FlightType = flightSegment.FlightType,
                                NumberInParty = flightSegment.NumberInParty,
                                FareDetails = flightSegment.FareDetails,
                                RPH = flightSegment.RPH
                            });
                        }
                    }
                }
                //

                RsSegment rsSegment = new RsSegment
                {
                    DepartureDateTime = Helper.TimeData.TimeFormat.FormatToViewDateTime(model.DepartureDateTime, LanguagePage.GetLanguageCode),
                    DestinationLocation = model.DestinationLocation,
                    OriginLocation = model.OriginLocation,
                    ADT = model.ADT,
                    Details = rsDetails.OrderBy(m => m.DepartureDateTime).ToList()
                };
                //
                if (rsSegment == null)
                    return Notifization.NotFound(MessageText.NotFound + 1);
                //
                return Notifization.Data("OK", rsSegment);
            }
        }
        // Get list ResBookDesigCode is valid
        public List<FlightSegment_ResBookDesigCode> GetFlightFares(List<string> lstResBookDesigCode, FareLLSModel fareLLSModel, List<XMLObject.FareLLSRQ.FareBasis> fareRSFareBasis, FlightAirTicketCondition flightAirTicketCondition)
        {
            List<FlightSegment_ResBookDesigCode> result = new List<FlightSegment_ResBookDesigCode>();
            foreach (var rbdc in lstResBookDesigCode)
            {
                List<FareItem> flightCostDetails = GetFareDetails(new VNAFareLLSRQModel
                {
                    FareLLS = fareLLSModel,
                    FareRSFareBasis = fareRSFareBasis,
                    ResBookDesigCode = rbdc
                }, flightAirTicketCondition);

                if (flightCostDetails == null)
                    continue;
                //
                result.Add(new FlightSegment_ResBookDesigCode
                {
                    //ResBookDesigCodeID = 0,
                    ResBookDesigCode = rbdc,
                    SellingFare = "",
                    FareItem = flightCostDetails
                });
            }
            result = result.OrderByDescending(m => m.ResBookDesigCode).ToList();
            return result;
        }
        public List<FareItem> GetFareDetails(VNAFareLLSRQModel model, FlightAirTicketCondition flightAirTicketCondition)
        {
            List<XMLObject.FareLLSRQ.FareBasis> fareRSFareBases = model.FareRSFareBasis;
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
            //int rbdcEnum = VNALibrary.GetResbookDesigCodeIDByKey(rbdc);
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
            List<XMLObject.FareLLSRQ.FareBasis> fareRSFareBases1 = fareRSFareBases.Where(m => m.Code.Last() == 'F').ToList();
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
                    if (passengerType == fareRSFareBasis.PassengerType.Code && fareRSFareBasis.AdditionalInformation.ResBookDesigCode == rbdc && fareRSFareBasis.AdditionalInformation.PrivateInd != null)
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
                return Notifization.Invalid("Cannot create session");
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
                    return Notifization.Invalid("Cannot create session");
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

        //public List<FlightTax> GetTax(TokenModel tokenModel, List<TaxFeeModel> models)
        //{
        //    List<FlightTax> flightTaxes = new List<FlightTax>();
        //    if (models == null)
        //        return flightTaxes;
        //    // 
        //    VNA_OTA_AirTaxRQService vNAOTA_AirTaxRQService = new VNA_OTA_AirTaxRQService();
        //    using (var sessionService = new VNA_SessionService(tokenModel))
        //    {
        //        foreach (var flight in models)
        //        {
        //            if (flight.FareBase.Count > 0)
        //            {
        //                //variable
        //                int _rph = flight.RPH;
        //                string originLocation = flight.OriginLocation;
        //                string destinationLocation = flight.DestinationLocation;
        //                DateTime departureDateTime = flight.DepartureDateTime;
        //                DateTime arrivalDateTime = flight.ArrivalDateTime;
        //                int airEquipType = flight.AirEquipType;
        //                string resBookDesigCode = flight.ResBookDesigCode;
        //                string currency = flight.CurrencyCode;
        //                int flightNumber = flight.FlightNumber;

        //                if (string.IsNullOrWhiteSpace(resBookDesigCode))
        //                    return flightTaxes;

        //                resBookDesigCode = resBookDesigCode.Trim();
        //                //
        //                List<FlightTaxInfo> flightTaxInfos = new List<FlightTaxInfo>();
        //                foreach (var item in flight.FareBase)
        //                {

        //                    float _amount = item.Amount;
        //                    AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRS airTaxRS = vNAOTA_AirTaxRQService.AirTax(tokenModel, new Resquet_WsTaxModel
        //                    {
        //                        RPH = _rph,
        //                        OriginLocation = originLocation,
        //                        DestinationLocation = destinationLocation,
        //                        DepartureDateTime = departureDateTime,
        //                        ResBookDesigCode = resBookDesigCode,
        //                        ArrivalDateTime = arrivalDateTime,
        //                        AirEquipType = airEquipType,
        //                        FlightNumber = flightNumber,
        //                        BaseFare = new Resquet_WsTax_BaseFareModel
        //                        {
        //                            RPH = item.RPH,
        //                            PassengerType = item.PassengerType,
        //                            Amount = _amount,
        //                            CurrencyCode = currency
        //                        }
        //                    });

        //                    if (airTaxRS.Items.Count() > 0 && airTaxRS.Items[1] != null)
        //                    {
        //                        string jsonData = JsonConvert.SerializeObject(airTaxRS.Items[1]);
        //                        FeeDetailsModel feeDetailsModel = JsonConvert.DeserializeObject<FeeDetailsModel>(jsonData);
        //                        flightTaxInfos.Add(new FlightTaxInfo
        //                        {
        //                            RPHSpecified = feeDetailsModel.ItineraryInfo[0].RPHSpecified,
        //                            PassengerType = feeDetailsModel.ItineraryInfo[0].PTC_FareBreakdown.PassengerType,
        //                            Total = feeDetailsModel.ItineraryInfo[0].TaxInfo.Total,
        //                            Taxes = feeDetailsModel.ItineraryInfo[0].TaxInfo.Taxes
        //                        });
        //                    }
        //                }
        //                //
        //                flightTaxes.Add(new FlightTax
        //                {
        //                    RPH = _rph,
        //                    FlightNumber = flightNumber,
        //                    AirEquipType = airEquipType,
        //                    ResBookDesigCode = resBookDesigCode,
        //                    FlightTaxInfos = flightTaxInfos
        //                });

        //            }
        //        }
        //        //
        //        return flightTaxes;


        //    }
        //}

        public ActionResult FlightFeeBasic(List<Resquest_SegmentTaxModel> models)
        {
            if (models == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //

            VNA_OTA_AirTaxRQService vnaOTA_AirTaxRQService = new VNA_OTA_AirTaxRQService();

            List<FlightTaxRs> flightTaxFees = new List<FlightTaxRs>();
            foreach (var model in models)
            {
                if (model.FareBase == null)
                    continue;
                //
                if (model.FareBase.Count() > 0)
                {
                    //variable
                    int _rph = model.RPH;
                    string originLocation = model.OriginLocation;
                    string destinationLocation = model.DestinationLocation;
                    DateTime departureDateTime = Convert.ToDateTime(model.DepartureDateTime);
                    DateTime arrivalDateTime = Convert.ToDateTime(model.ArrivalDateTime);
                    int airEquipType = model.AirEquipType;
                    string resBookDesigCode = model.ResBookDesigCode;
                    string currency = model.CurrencyCode;
                    int flightNumber = model.FlightNumber;
                    if (string.IsNullOrWhiteSpace(resBookDesigCode))
                        return Notifization.Invalid(MessageText.Invalid);
                    //
                    resBookDesigCode = resBookDesigCode.Trim();
                    List<FlightTaxInfo> flightTaxInfos = new List<FlightTaxInfo>();

                    foreach (var item in model.FareBase)
                    {
                        double _amount = item.Amount;
                        TokenModel tokenModel = VNA_AuthencationService.GetSession();
                        // create session 
                        if (tokenModel == null)
                            continue;
                        // create session 
                        string _token = tokenModel.Token;
                        string _conversationId = tokenModel.ConversationID;
                        if (string.IsNullOrWhiteSpace(_token))
                            continue;
                        // 
                        using (var sessionService = new VNA_SessionService(tokenModel))
                        {
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
                            //vnaTransaction.EndTransaction(tokenModel);
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
                                    Taxes = feeDetailsModel.ItineraryInfo[0].TaxInfo.Taxes,
                                    Quantity = item.Quantity,
                                    FareBasic = _amount
                                });
                            }
                        }

                    }

                    // 
                    flightTaxFees.Add(new FlightTaxRs
                    {
                        RPH = _rph,
                        AirEquipType = airEquipType,
                        FlightNumber = flightNumber,
                        ResBookDesigCode = resBookDesigCode,
                        ArrivalDateTime = TimeFormat.FormatToViewDateTime(arrivalDateTime, LanguagePage.GetLanguageCode),
                        DepartureDateTime = TimeFormat.FormatToViewDateTime(departureDateTime, LanguagePage.GetLanguageCode),
                        DestinationLocation = destinationLocation,
                        OriginLocation = originLocation,
                        FlightTaxInfos = flightTaxInfos,

                    });

                }
            }
            //
            if (flightTaxFees.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            return Notifization.Data("OK", flightTaxFees);

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
        public ActionResult Booking(BookOrderSaveModel model)
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
            CompanyContact bookCompany = new CompanyContact();
            if (ticketingInfo == null || Contacts == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string ticketingId = Helper.Current.UserLogin.IdentifierID;
            if (!Helper.Current.UserLogin.IsClientInApplication())
                ticketingId = ticketingInfo.TiketingID;
            //
            if (string.IsNullOrWhiteSpace(ticketingId))
                return Notifization.Invalid("Nhân viên không hợp lệ");
            //
            UserInfoService userInfoService = new UserInfoService();
            UserInfo userInfo = userInfoService.GetAlls(m => m.UserID == ticketingId).FirstOrDefault();
            if (userInfo == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string ticketingName = userInfo.FullName;
            string ticketingPhone = userInfo.Phone;
            string ticketingEmail = userInfo.Email;
            //
            string agentId = AirAgentService.GetAgentIDByUserID(ticketingId);
            ClientLoginService clientLoginService = new ClientLoginService();
            AirAgentService airAgentService = new AirAgentService();
            AirAgent airAgent = airAgentService.GetAlls(m => m.ID == agentId).FirstOrDefault();
            string agentCode = airAgent.CodeID;
            string agentName = airAgent.Title;
            //
            string agentProviderId = airAgent.ID;
            if (!string.IsNullOrWhiteSpace(airAgent.ParentID))
                agentProviderId = airAgent.ParentID;
            //

            // call service get PNR code 
            string strEmail = string.Empty;
            string strPhone = string.Empty;
            // 
            if (model.Contacts == null)
                return Notifization.Invalid("Thông tin liên hệ không hợp lệ 1");
            //
            int customerType = model.CustomerType;
            if (customerType == (int)ClientLoginEnum.CustomerType.Company)
            {
                CompanyContactRq companyContact = Contacts.CompanyContact;
                if (companyContact == null)
                    return Notifization.Invalid("Thông tin liên hệ không hợp lệ");
                //
                string companyId = companyContact.CompanyID;
                if (string.IsNullOrWhiteSpace(companyId))
                    return Notifization.Invalid("Vui lòng chọn công ty");
                //
                CompanyService companyService = new CompanyService();
                Company company = companyService.GetAlls(m => m.ID == companyId).FirstOrDefault();
                if (company == null)
                    return Notifization.Invalid("Thông tin công ty không tồn tại");
                // 
                strEmail = company.ContactEmail;
                strPhone = company.ContactPhone;
                //
                bookCompany.CompanyID = company.ID;
                bookCompany.CompanyCode = company.CodeID;
                bookCompany.Email = strEmail;
                bookCompany.Phone = strPhone;
                bookCompany.Name = company.ContactName;
            }
            else if (customerType == (int)ClientLoginEnum.CustomerType.Haunt)
            {
                HauntContact hauntContact = Contacts.HauntContact;
                string name = hauntContact.Name;
                string contactPhone = hauntContact.Phone;
                string contactEmail = hauntContact.Email;
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
                    return Notifization.Invalid(MessageText.Invalid);
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
                    List<BookSegmentModel> lstSegments = model.Segments;
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
                        return Notifization.Invalid("Lỗi dịch vụ đặt chỗ");
                    //
                    var _originDestinationOption = ota_AirBookRS.OriginDestinationOption.ToList();
                    if (_originDestinationOption.Count == 0)
                        return Notifization.NotFound(MessageText.NotFound);
                    // #2. Get price - ***************************************************************************************************************************************************
                    airBookModel.Segments = model.Segments;
                    var airPriceModel = new AirPriceModel
                    {
                        ConversationID = _conversationId,
                        Token = _token,
                        Segments = model.Segments
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
                    //
                    VNA_OTA_AirPriceLLSRQService vNAWSOTA_AirPriceLLSRQService = new VNA_OTA_AirPriceLLSRQService();
                    var airPriceData = vNAWSOTA_AirPriceLLSRQService.AirPrice(airPriceModel);

                    // #3. Get PNA code - **************************************************************************************************************************************************
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
                    model.Passengers = model.Passengers.OrderBy(m => m.PassengerType).ToList();
                    // create model passenger details model
                    List<PassengerDetailsRQ> passengerDetailsRQ = new List<PassengerDetailsRQ>();
                    List<BookTicketPassenger> request_Passengers = model.Passengers;
                    foreach (var item in request_Passengers)
                    {
                        // 
                        string fullName = Helper.Page.Library.FormatStandardNameToUni2NONE(item.FullName);
                        passengerDetailsRQ.Add(new PassengerDetailsRQ
                        {
                            PassengerType = item.PassengerType,
                            FullName = fullName,
                            DateOfBirth = item.DateOfBirth,
                            Gender = item.Gender
                        });
                    }
                    // ######################################################################################

                    //call service  passenger details in ws.service
                    VNA_PassengerDetailsRQService vNAWSPassengerDetailsRQService = new VNA_PassengerDetailsRQService();

                    XMLObject.AirTicketRq.PassengerDetailsRS passengerDetailsRS = vNAWSPassengerDetailsRQService.PassengerDetail(new DetailsRQ
                    {
                        Email = bookEmailConfig.Email,
                        Phone = bookEmailConfig.Phone,
                        ConversationID = _conversationId,
                        Token = _token,
                        Passengers = passengerDetailsRQ,
                        AirBook = ota_AirBookRS
                    });

                    //XMLObject.AirTicketRq.PassengerDetailsRS passengerDetailsRS = new XMLObject.AirTicketRq.PassengerDetailsRS();
                    //XmlDocument soapEnvelopeXml = new XmlDocument();
                    //var path = HttpContext.Current.Server.MapPath(@"~/Team/2021-01-12-18-18-50-B9NIYQGK2J-pnr-rs.xml");
                    //soapEnvelopeXml.Load(path);
                    //XmlNode xmlnode = soapEnvelopeXml.GetElementsByTagName("soap-env:Body")[0];
                    //if (xmlnode != null)
                    //    passengerDetailsRS = XMLHelper.Deserialize<XMLObject.AirTicketRq.PassengerDetailsRS>(xmlnode.InnerXml);
                    ////  
                    if (passengerDetailsRS == null)
                        return Notifization.Invalid("Lỗi dịch vụ đặt chỗ");
                    //
                    string pnrCode = passengerDetailsRS.ItineraryRef.ID;
                    //End transaction 
                    vnaTransaction.EndTransaction();
                    //
                    if (string.IsNullOrWhiteSpace(pnrCode))
                        return Notifization.Invalid("Lỗi dịch vụ đặt chỗ");
                    // save order | input data ***************************************************************************************************
                    OrderTicketService orderTicketService = new OrderTicketService();
                    List<RequestOrderFlightModel> requestOrderFlightModels = new List<RequestOrderFlightModel>();
                    List<RequestOrderPriceModel> requestOrderPriceModels = new List<RequestOrderPriceModel>();
                    List<RequestOrderTaxModel> requestOrderTaxModels = new List<RequestOrderTaxModel>();
                    foreach (var item in model.Segments)
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
                    // save book information | output data **************************************************************************************************
                    BookTicketService bookTicketService = new BookTicketService();
                    List<BookTicketPassenger> bookTicketPassengers = new List<BookTicketPassenger>();

                    var index = 0;
                    foreach (var item in model.Passengers)
                    {
                        index++;
                        string passengerName = Helper.Page.Library.FormatStandardNameToUni2NONE(item.FullName).ToUpper();
                        string givenName = AIRService.WS.VNAHelper.VNALibrary.GetGivenName(passengerName);
                        string surName = AIRService.WS.VNAHelper.VNALibrary.GetSurName(passengerName);
                        string middleName = AIRService.WS.VNAHelper.VNALibrary.GetMiddleName(passengerName);
                        //
                        if (!string.IsNullOrWhiteSpace(givenName))
                            givenName = givenName.ToUpper();
                        if (!string.IsNullOrWhiteSpace(middleName))
                            middleName = middleName.ToUpper();
                        if (!string.IsNullOrWhiteSpace(surName))
                            surName = surName.ToUpper();

                        //var isInf = "false";
                        //if (item.PassengerType.ToUpper() == "ADT")
                        //{
                        //    record = 1;
                        //}
                        //if (item.PassengerType.ToUpper() == "CNN")
                        //{
                        //    //isHaveCNNOrINF = true;
                        //    record = 2;
                        //}
                        //if (item.PassengerType.ToUpper() == "INF")
                        //{
                        //    isInf = "true";
                        //    //isHaveCNNOrINF = true;
                        //    record = 3;
                        //} 
                        //
                        string strIndex = "";
                        if (index < 10)
                            strIndex += "0" + index;
                        //
                        var nameNumber = $"{strIndex}" + ".01";
                        char mdd;
                        if (!string.IsNullOrWhiteSpace(middleName))
                            mdd = middleName[0];
                        else
                            mdd = givenName[0];
                        // 
                        bookTicketPassengers.Add(new BookTicketPassenger
                        {
                            DateOfBirth = item.DateOfBirth,
                            FullName = item.FullName,
                            Gender = item.Gender,
                            PassengerType = item.PassengerType,
                            PassengerName = $"{surName}/{mdd}",
                            ElementID = "",
                            NameNumber = nameNumber
                        });
                    }
                    // call save booking 
                    double airAgentFee = 0; //  
                    string bookTicketId = bookTicketService.BookSave(new BookTicketOrder
                    {
                        PNR = pnrCode,
                        CustomerType = model.CustomerType,
                        Summary = model.Summary,
                        ItineraryType = model.ItineraryType,
                        OrderDate = TimeHelper.UtcDateTime,
                        Flights = model.Segments,
                        Passengers = bookTicketPassengers,
                        FareTaxs = fareTaxs,
                        FareFlights = fareFlights,
                        OrderStatus = (int)WebCore.ENM.BookOrderEnum.BookOrderStatus.Booking,
                        AgentInfo = new BookAgentInfo
                        {
                            AgentID = agentId,
                            AgentName = agentName,
                            AgentCode = agentCode,
                            AgentFee = airAgentFee,
                            TiketingID = ticketingId,
                            TiketingName = ticketingName,
                            TiketingPhone = ticketingPhone,
                            TiketingEmail = ticketingEmail,
                            ProviderID = agentProviderId
                        },
                        Contacts = new BookOrderContact
                        {
                            HauntContact = model.Contacts.HauntContact,
                            CompanyContact = bookCompany
                        }
                    });

                    Helper.Security.Cookies.RemoveCookis("FlightSearch");
                    Helper.Security.Cookies.RemoveCookis("FlightOrder");
                    //
                    return Notifization.Success($"Đặt chỗ thành công", $"/management/airbook/details/{bookTicketId}");
                } // end using tran session
            } // end using session

        }

        // check xuat ve
        public ActionResult CheckReleaseTicket(BookOrderIDModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            BookOrderService bookOrderService = new BookOrderService();
            BookTicketService bookTicketService = new BookTicketService();
            WalletAgentService walletClientService = new WalletAgentService();
            BookAgentService bookAgentService = new BookAgentService();
            BookTaxService bookTaxService = new BookTaxService();
            BookPriceService bookPriceService = new BookPriceService();
            //
            string orderId = model.ID;
            if (string.IsNullOrWhiteSpace(orderId))
                return Notifization.Invalid(MessageText.Invalid);
            //
            BookOrder bookOrder = bookOrderService.GetAlls(m => m.ID == orderId).FirstOrDefault();
            if (bookOrder == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            BookAgent bookAgent = bookAgentService.GetAlls(m => m.BookOrderID == orderId).FirstOrDefault();
            if (bookAgent == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string pnr = bookOrder.PNR;
            string agentId = bookAgent.AgentID;

            double priceTotal = bookPriceService.GetAlls(m => m.BookOrderID == orderId).Sum(m => m.Amount);
            double taxTotal = bookTaxService.GetAlls(m => m.BookOrderID == orderId).Sum(m => m.Amount);

            double amount = priceTotal + taxTotal + bookAgent.ProviderFee + bookAgent.AgentFee + bookAgent.AgentPrice;

            // xuat ve: 
            // #1: kiem tra hom nay co trong khoang cuối tháng trước -> ngày 15 tháng hiện tại hay ko
            //     + nếu trong khoảng -> kiem tra da thanh toan han muc cua thang truoc hay chua
            //           + đã thanh toán -> kiểm tra hạn mức
            //                 + còn hạn mức:  xuất vé
            //                 - hết hạn mức: Thông báo hạn mức ko đủ
            //           + chưa thanh toán -> Thông báo yêu cầu thanh toán
            //     - nếu ko trong khoảng -> kiểm tra hạn mức
            //           + còn hạn mức:  xuất vé
            //           - hết hạn mức: Thông báo hạn mức ko đủ

            DateTime today = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));// Convert.ToDateTime(TimeHelper.UtcDateTime.ToString("yyyy-MM-dd"));
            //kiem tra da thanh toan han muc cua thang truoc hay chua
            string userLog = Helper.Current.UserLogin.IdentifierID;
            AirAgentService airAgentService = new AirAgentService();
            AirAgent airAgent = airAgentService.GetAlls(m => m.ID == agentId).FirstOrDefault();
            double totalSpent = 0;
            if (!string.IsNullOrWhiteSpace(airAgent.ParentID))
            {
                // kiểm tra dữ liệu các tháng trước đó  
                DateTime agentCreatedDate = airAgent.CreatedDate;
                DateTime end = today.AddMonths(-2);// ko xử lý tháng cuối cùng
                DateDiff dateDiff = new DateDiff(agentCreatedDate, end);
                DateTime start = end.AddMonths(-dateDiff.Months);


                List<DateTime> dateTimeRange = Enumerable.Range(0, 1 + end.Subtract(start).Days).Select(offset => start.AddDays(offset)).Where(m => m.Day == 1).ToList();
                AgentSpendingLimitPaymentService agentSpendingLimitPaymentService = new AgentSpendingLimitPaymentService();
                foreach (var item in dateTimeRange)
                {
                    // kiem tra da tieu han muc chua, chua tieu bo qua
                    BookOrder bookOrderOnMonth = bookOrderService.GetAlls(m => m.IssueDate.Year == item.Year && m.IssueDate.Month == item.Month).FirstOrDefault();
                    if (bookOrderOnMonth == null)
                        continue;
                    //
                    AgentSpendingLimitPayment agentSpendingLimitPayment = agentSpendingLimitPaymentService.GetAlls(m => m.AgentID == agentId && m.Year == item.Year && m.Month == item.Month).FirstOrDefault();
                    if (agentSpendingLimitPayment == null)
                        return Notifization.Invalid("Hạn mức tháng:" + Helper.TimeData.TimeFormat.FormatToViewYearMonth(item, LanguagePage.GetLanguageCode) + " chưa thanh toán");
                    //
                }
                // ngay dau tien cua thang truoc
                DateTime firstDayOfMonth = today.AddMonths(-1);
                firstDayOfMonth = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.Month, 01);
                int paymentDayTotal = today.Subtract(firstDayOfMonth).Days;
                int termPayment = airAgent.TermPayment;
                if (termPayment <= 0)
                    return Notifization.Invalid("Hạn mức không hợp lệ");
                //  
                //#1: check kỳ hạn thanh toan han muc 
                if (paymentDayTotal > termPayment)
                    return Notifization.Invalid("Hạn mức tháng:" + Helper.TimeData.TimeFormat.FormatToViewYearMonth(firstDayOfMonth, LanguagePage.GetLanguageCode) + " chưa thanh toán");
                //
                // lấy tổng đã tiêu tu tháng trước den nay (< 45 ngay)
                string sqlQuery = $@" SELECT (
                ISNULL((select sum (Amount) from App_BookPrice where BookOrderID = o.ID),0) +
                ISNULL((select sum (Amount) from App_BookTax where BookOrderID = o.ID),0) +
                ISNULL((select sum (AgentPrice + ProviderFee + AgentFee) from App_BookAgent where BookOrderID = o.ID),0) 
                ) as 'Spent'              
                FROM App_BookOrder AS o
                INNER JOIN App_BookAgent AS a ON a.BookOrderID = o.ID 
                WHERE o.ID != @OrderID AND a.AgentID = @AgentID AND a.TicketingID = @TicketingID 
                AND cast(IssueDate as Date) >= cast('{firstDayOfMonth}' as Date) AND cast(IssueDate as Date) <= cast('{today}' as Date)";
                totalSpent = bookOrderService.Query<double>(sqlQuery, new { OrderID = orderId, AgentID = agentId, TicketingID = userLog }).FirstOrDefault();
            }
            AgentSpendingLimitService agentSpendingLimitService = new AgentSpendingLimitService();
            // For ticketing 
            UserSpendingService userSpendingService = new UserSpendingService();
            UserSpending userSpending = userSpendingService.GetAlls(m => m.TicketingID == userLog).FirstOrDefault();
            if (userSpending == null)
                return Notifization.Invalid("Hạn mức chưa được cấp phép");

            double userSpendingLimit = userSpending.Amount;
            if (userSpendingLimit < totalSpent + amount)
                return Notifization.Invalid("Hạn mức không đủ");
            // xuất vé
            return ReleaseTicket(orderId);
        }

        private ActionResult ReleaseTicket(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
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
                return Notifization.Invalid(MessageText.Invalid);
            //   
            List<ExTitketPassengerFareModel> exTitketPassengerFareModels = new List<ExTitketPassengerFareModel>();
            BookPassengerService bookPassengerService = new BookPassengerService();
            BookPriceService bookPriceService = new BookPriceService();
            BookTaxService bookFareService = new BookTaxService();
            BookOrderService bookOrderService = new BookOrderService();
            BookOrder bookOrder = bookOrderService.GetAlls(m => m.ID == orderId).FirstOrDefault();
            if (bookOrder == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            if (bookOrder.OrderStatus != (int)BookOrderEnum.BookOrderStatus.Booking)
                return Notifization.Invalid("Trạng thái vé không hợp lệ");
            //
            string pnr = bookOrder.PNR;
            if (string.IsNullOrWhiteSpace(pnr))
                return Notifization.Invalid("Mã PNR không hợp lệ");
            //
            List<BookPassenger> bookPassengers = bookPassengerService.GetAlls(m => m.BookOrderID == orderId).OrderBy(m => m.PassengerType).ToList();
            if (bookPassengers.Count == 0)
                return Notifization.Invalid("Thông tin hành khách không hợp lệ");
            //   
            VNA_WSGetReservationRQService vNAWSGetReservationRQService = new VNA_WSGetReservationRQService();
            XMLObject.ReservationRq2.GetReservationRS reservationRS = vNAWSGetReservationRQService.GetReservation(new GetReservationModel
            {
                ConversationID = tokenModel.ConversationID,
                Token = tokenModel.Token,
                PNR = pnr
            });
            // 
            XMLObject.ReservationRq2.AlreadyTicketed alreadyTicketed = reservationRS.Reservation.PassengerReservation.TicketingInfo.AlreadyTicketed;
            if (alreadyTicketed != null)
                return Notifization.Invalid("Vé đã xuất, vui lòng kiểm tra lại");
            //
            foreach (var item in bookPassengers)
            {
                double bookPrice = bookPriceService.GetAlls(m => m.BookOrderID == orderId && m.PassengerType == item.PassengerType).Sum(m => m.Amount);
                double bookTax = bookFareService.GetAlls(m => m.BookOrderID == orderId && m.PassengerType == item.PassengerType).Sum(m => m.Amount);
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
                    string givenName = AIRService.WS.VNAHelper.VNALibrary.GetGivenName(item.FullName);
                    string surName = AIRService.WS.VNAHelper.VNALibrary.GetGivenName(item.FullName);
                    var paymentOrderDetail = new PaymentOrderDetail
                    {
                        PsgrType = item.PassengerType,
                        FirstName = givenName,
                        LastName = surName,
                        BaseFare = item.PriceTotal,
                        Taxes = item.TaxTotal
                    };
                    paymentModel.PaymentOrderDetail.Add(paymentOrderDetail);
                }
                VNA_PaymentRQService vNAPaymentRQService = new VNA_PaymentRQService();
                WebService.VNA_PaymentRQ.PaymentRS paymentRS = vNAPaymentRQService.Payment(paymentModel);
                //
                if (paymentRS.Result == null || paymentRS.Result.ResultCode != "SUCCESS")
                    return Notifization.Invalid(MessageText.Invalid);
                //
                if (paymentRS.Items.Count() == 0)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                AIRService.WebService.VNA_PaymentRQ.AuthorizationResultType authorizationResultType = (AIRService.WebService.VNA_PaymentRQ.AuthorizationResultType)paymentRS.Items[0];
                if (authorizationResultType == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                if (string.IsNullOrWhiteSpace(authorizationResultType.ApprovalCode))
                    return Notifization.Invalid(MessageText.Invalid);
                //
                //Login -> DisginPinter -> GetRev -> OTA_AirPriceLLSRQ -> PaymentRQ -> AirTicketLLSRQ -> Endtransession
                VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(new DesignatePrinterLLSModel
                {
                    ConversationID = tokenModel.ConversationID,
                    Token = tokenModel.Token
                });
                if (printer.ApplicationResults.Error != null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                if (printer.ApplicationResults.status != AIRService.WebService.VNA_DesignatePrinterLLSRQ.CompletionCodes.Complete)
                    return Notifization.Invalid(MessageText.Invalid);
                // 
                VNAWSAirTicketLLSRQService vNAWSAirTicketLLSRQService = new VNAWSAirTicketLLSRQService();
                var airTicketData = vNAWSAirTicketLLSRQService.VNA_AirTicketLLSRQ(new AirTicketLLSRQModel
                {
                    ConversationID = tokenModel.ConversationID,
                    Token = tokenModel.Token,
                    PNR = pnr,
                    ApproveCode = authorizationResultType.ApprovalCode,
                    ExpireDate = $"{Helper.TimeData.TimeHelper.UtcDateTime.Year}-12",
                    ListPricingInPNR = exTitketPassengerFareModels
                });

                VNA_EndTransaction vNATransaction = new VNA_EndTransaction();
                vNATransaction.EndTransaction(tokenModel);
                //
                XMLObject.ReservationRq2.GetReservationRS reservationRSCheck = vNAWSGetReservationRQService.GetReservation(new GetReservationModel
                {
                    ConversationID = tokenModel.ConversationID,
                    Token = tokenModel.Token,
                    PNR = pnr
                });
                //
                alreadyTicketed = reservationRSCheck.Reservation.PassengerReservation.TicketingInfo.AlreadyTicketed;
                if (alreadyTicketed != null)
                {
                    //step 01
                    XMLObject.ReservationRq2.TicketingInfo ticketingInfo = reservationRSCheck.Reservation.PassengerReservation.TicketingInfo;
                    if (ticketingInfo != null)
                    {
                        List<XMLObject.ReservationRq2.TicketDetails> ticketDetails = ticketingInfo.TicketDetails;
                        if (ticketDetails.Count() > 0)
                        {
                            foreach (var item in bookPassengers)
                            {
                                XMLObject.ReservationRq2.TicketDetails ticket = ticketDetails.Where(m => m.PassengerName == item.PassengerName.ToUpper()).FirstOrDefault();
                                if (ticket != null)
                                {
                                    BookPassenger bookPassenger = bookPassengerService.GetAlls(m => m.ID == item.ID).FirstOrDefault();
                                    bookPassenger.TicketNumber = ticket.TicketNumber;
                                    bookPassengerService.Update(bookPassenger);
                                }
                            }
                        }
                    }
                    // step 02
                    //List<XMLObject.ReservationRq2.Passenger> passengers = reservationRSCheck.Reservation.PassengerReservation.Passengers.Passenger;
                    //if (passengers.Count() > 0)
                    //{
                    //    foreach (var item in bookPassengers)
                    //    {
                    //        XMLObject.ReservationRq2.Passenger passenger = passengers.Where(m => m.NameId == item.NameNumber).FirstOrDefault();
                    //        string ticketNumber = string.Empty;
                    //        //
                    //        List<XMLObject.ReservationRq2.GenericSpecialRequest> genericSpecialRequests = passenger.SpecialRequests.GenericSpecialRequest;
                    //        if (genericSpecialRequests.Count() > 0)
                    //            ticketNumber = genericSpecialRequests[0].TicketNumber;
                    //        //
                    //        BookPassenger bookPassenger = bookPassengerService.GetAlls(m => m.BookOrderID == bookOrder.ID && m.ID == item.ID).FirstOrDefault();
                    //        bookPassenger.TicketNumber = ticketNumber;
                    //        bookPassengerService.Update(bookPassenger);
                    //        // 
                    //        List<BookPrice> bookPrices = bookPriceService.GetAlls(m => m.BookOrderID == bookOrder.ID && m.PassengerType == item.PassengerType).ToList();
                    //        if (bookPrices.Count() > 0)
                    //        {
                    //            int index = 1;
                    //            foreach (var itemPrice in bookPrices)
                    //            {
                    //                BookPrice bookPrice = bookPrices.Where(m => m.FlightType == index).FirstOrDefault();
                    //                bookPrice.TicketNumber = Helper.Page.Library.LastText(genericSpecialRequests[index - 1].FullText, '/');
                    //                bookPrice.FullText = genericSpecialRequests[index - 1].FullText;
                    //                bookPriceService.Update(bookPrice);
                    //                index++;
                    //            }
                    //        }
                    //    }
                    //}
                    // save
                    bookOrder.OrderStatus = (int)BookOrderEnum.BookOrderStatus.Exported;
                    bookOrder.ExportDate = DateTime.Now;
                    bookOrderService.Update(bookOrder);
                    //
                    BookAgentService bookAgentService = new BookAgentService();
                    BookAgent bookAgent = bookAgentService.GetAlls(m => m.BookOrderID == bookOrder.ID).FirstOrDefault();
                    if (bookAgent != null)
                    {
                        UserSpendingHistoryService.WalletUserSpendingHistory(new WalletUserSpendingHistoryCreateModel
                        {
                            AgentID = bookAgent.AgentID,
                            UserID = bookAgent.TicketingID,
                            Amount = fareTotal,
                            TransactionType = (int)TransactionEnum.TransactionType.OUT
                        });
                    }
                }
                //
                return Notifization.Success("Thành công. Đã xuất vé");
            }
        }
        /// <summary>
        ///  Hủy vé
        /// </summary>
        /// <param name="model">PNR code</param>
        /// <returns></returns>

        public ActionResult VoidBook(BookOrderIDModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            TokenModel tokenModel = VNA_AuthencationService.GetSession();
            // create session 
            if (tokenModel == null)
                return Notifization.Invalid(MessageText.Invalid);
            // create session 
            string _token = tokenModel.Token;
            string _conversationId = tokenModel.ConversationID;
            if (string.IsNullOrWhiteSpace(_token))
                return Notifization.Invalid(MessageText.Invalid);
            // 
            string orderId = model.ID;
            BookOrderService bookOrderService = new BookOrderService();
            BookOrder bookOrder = bookOrderService.GetAlls(m => m.ID == orderId).FirstOrDefault();
            if (bookOrder == null)
                return Notifization.Invalid(MessageText.Invalid);
            string pnr = bookOrder.PNR;
            if (string.IsNullOrWhiteSpace(pnr))
                return Notifization.Invalid("Mã PNR không hợp lệ");
            // 
            BookTicketService bookTicketService = new BookTicketService();
            BookTicket bookTicket = bookTicketService.GetAlls(m => m.BookOrderID == orderId).OrderBy(m => m.DepartureDateTime).FirstOrDefault();
            if (bookTicket == null)
                return Notifization.Invalid("Lỗi thời gian khởi hành");
            // check time void book 
            AirportConfigService airportConfigService = new AirportConfigService();
            AirResBookDesigService airResBookDesigService = new AirResBookDesigService();
            int voidBookTime = 0;

            AirResBookDesig airResBookDesig = airResBookDesigService.GetAlls(m => m.CodeID == bookTicket.ResBookDesigCode).FirstOrDefault();
            if (airResBookDesig == null)
                return Notifization.Invalid("Lỗi thời gian hủy hành trình");
            //
            voidBookTime = airResBookDesig.VoidBookTime;
            //DepartureDateTime  
            DateTime dateTime = Helper.TimeData.TimeHelper.UtcDateTime;
            DateTime departureDateTime = bookTicket.DepartureDateTime;
            DateTime IssueDate = bookOrder.IssueDate;
            // in day 
            DateTime datetime = Helper.TimeData.TimeHelper.UtcDateTime;
            if (datetime > departureDateTime)
                return Notifization.Invalid($"T.gian khởi hành không hợp lệ");
            //
            TimeSpan difference = departureDateTime - datetime;
            int leftBookTime = Convert.ToInt32(difference.TotalHours);
            if (leftBookTime < voidBookTime)
                return Notifization.Invalid($"T.gian hủy chỗ quá giới hạn: {voidBookTime} giờ");
            // 
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel
                {
                    ConversationID = tokenModel.ConversationID,
                    Token = tokenModel.Token
                };
                VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                if (printer.ApplicationResults.status != AIRService.WebService.VNA_DesignatePrinterLLSRQ.CompletionCodes.Complete)
                    return Notifization.Invalid(MessageText.Invalid);
                // 
                VNA_WSGetReservationRQService vNAWSGetReservationRQService = new VNA_WSGetReservationRQService();
                XMLObject.ReservationRq2.GetReservationRS reservationRS = vNAWSGetReservationRQService.GetReservation(new GetReservationModel
                {
                    ConversationID = tokenModel.ConversationID,
                    Token = tokenModel.Token,
                    PNR = pnr
                });
                //
                XMLObject.ReservationRq2.TicketingTimeLimit ticketingTimeLimit = reservationRS.Reservation.PassengerReservation.TicketingInfo.TicketingTimeLimit;
                if (ticketingTimeLimit == null)
                    return Notifization.Invalid("Lệnh đặt chỗ không tồn tại");
                //
                VNA_OTA_CancelLLSRQService vNAWSOTA_CancelLLSRQService = new VNA_OTA_CancelLLSRQService();
                var data = vNAWSOTA_CancelLLSRQService.OTA_CancelLLS(tokenModel);
                VNA_EndTransaction vNATransaction = new VNA_EndTransaction();
                var end = vNATransaction.EndTransaction(tokenModel);
                if (data.ApplicationResults.Success != null)
                {
                    BookPassengerService bookPassengerService = new BookPassengerService();
                    bookPassengerService.Execute("UPDATE App_BookPassenger SET IsVoided = 1 WHERE BookOrderID = @OrderID", new { OrderID = orderId });
                    bookOrder.OrderStatus = (int)BookOrderEnum.BookOrderStatus.Cancel;
                    bookOrderService.Update(bookOrder);
                    return Notifization.Success("Đã hủy thành công");
                }
                return Notifization.Invalid("Lỗi dịch vụ");
            }
        }
        public ActionResult VoidTicket(BookPassengerIDModel model)
        {

            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            TokenModel tokenModel = VNA_AuthencationService.GetSession();
            // create session 
            if (tokenModel == null)
                return Notifization.Invalid(MessageText.Invalid);
            // create session 
            string _token = tokenModel.Token;
            string _conversationId = tokenModel.ConversationID;
            if (string.IsNullOrWhiteSpace(_token))
                return Notifization.Invalid(MessageText.Invalid);
            //  
            string bookPassengerId = model.ID;
            if (string.IsNullOrWhiteSpace(bookPassengerId))
                return Notifization.Invalid(MessageText.Invalid);

            BookPriceService bookPriceService = new BookPriceService();
            BookTaxService bookTaxService = new BookTaxService();
            BookPassengerService bookPassengerService = new BookPassengerService();
            BookOrderService bookOrderService = new BookOrderService();
            BookTicketService bookTicketService = new BookTicketService();
            BookAgentService bookAgentService = new BookAgentService();
            BookPassenger bookPassenger = bookPassengerService.GetAlls(m => m.ID == bookPassengerId).FirstOrDefault();
            if (bookPassenger == null)
                return Notifization.Invalid(MessageText.Invalid);
            // 
            string orderId = bookPassenger.BookOrderID;

            string ticketNo = bookPassenger.TicketNumber;
            if (string.IsNullOrWhiteSpace(ticketNo))
                return Notifization.Invalid("Số vé không hợp lệ");
            //
            BookOrder bookOrder = bookOrderService.GetAlls(m => m.ID == orderId).FirstOrDefault();
            if (bookOrder == null)
                return Notifization.Invalid(MessageText.Invalid);
            //DepartureDateTime 

            BookTicket bookTicket = bookTicketService.GetAlls(m => m.BookOrderID == orderId).OrderBy(m => m.DepartureDateTime).FirstOrDefault();
            if (bookTicket == null)
                return Notifization.Invalid("Lỗi kiểm tra thời gian khởi hành");
            // 
            DateTime departureDateTime = bookTicket.DepartureDateTime;

            // check time void ticket 
            AirportConfigService airportConfigService = new AirportConfigService();
            AirportService airportService = new AirportService();
            int voidTicketTime = 0;
            Airport airportGo = airportService.GetAlls(m => m.IATACode == bookTicket.OriginLocation).FirstOrDefault();
            if (airportGo == null)
                return Notifization.Invalid("Lỗi không tìm thấy sân bay");
            // 
            AirportConfig airportConfig = airportConfigService.GetAlls(m => m.IATACode == bookTicket.OriginLocation).FirstOrDefault();
            if (airportConfig != null)
                voidTicketTime = airportConfig.VoidTicketTime;
            //
            DateTime dateTime = Helper.TimeData.TimeHelper.UtcDateTime;
            if (departureDateTime.Minute - dateTime.Minute < voidTicketTime * 60)
                return Notifization.Invalid($"T.gian hủy vé quá giới hạn: {voidTicketTime} giờ");
            // 
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                VNA_WSGetReservationRQService vNAWSGetReservationRQService = new VNA_WSGetReservationRQService();
                VNA_VoidTicketLLSRQService vNA_VoidTicketLLSRQService = new VNA_VoidTicketLLSRQService();
                VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(new DesignatePrinterLLSModel
                {
                    ConversationID = tokenModel.ConversationID,
                    Token = tokenModel.Token
                });
                if (printer.ApplicationResults.Error != null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                if (printer.ApplicationResults.status != AIRService.WebService.VNA_DesignatePrinterLLSRQ.CompletionCodes.Complete)
                    return Notifization.Invalid(MessageText.Invalid);
                // 
                XMLObject.ReservationRq2.GetReservationRS reservationRS = vNAWSGetReservationRQService.GetReservation(new GetReservationModel
                {
                    ConversationID = tokenModel.ConversationID,
                    Token = tokenModel.Token,
                    PNR = bookOrder.PNR
                });

                XMLObject.ReservationRq2.AlreadyTicketed alreadyTicketed = reservationRS.Reservation.PassengerReservation.TicketingInfo.AlreadyTicketed;
                if (alreadyTicketed == null)
                    return Notifization.Invalid("Lỗi chưa xuất vé");
                //
                XMLObject.VoidTicketRq.VoidTicketRS voidTicketRS = vNA_VoidTicketLLSRQService.VoidTicketLLSRQ(tokenModel, ticketNo);
                VNA_EndTransaction vNATransaction = new VNA_EndTransaction();
                vNATransaction.EndTransaction(tokenModel);
                //
                if (voidTicketRS.ApplicationResults.Error != null)
                    return Notifization.Invalid($"Lỗi hủy số vé: {ticketNo}");
                //  
                // update trang thai ve
                if (!bookPassenger.IsVoided)
                {
                    bookPassenger.IsVoided = true;
                    bookPassenger.VoidDateTime = Helper.TimeData.TimeHelper.UtcDateTime;
                    bookPassenger.VoidUserdID = Helper.Current.UserLogin.IdentifierID;
                    bookPassengerService.Update(bookPassenger);
                    // tra lai tien cho ticketing
                    UserSpendingHistory walletUserSpendingHistory = new UserSpendingHistory();
                    string passengerType = bookPassenger.PassengerType;
                    double totalPrice = bookPriceService.GetAlls(m => m.BookOrderID == orderId && m.PassengerType == passengerType).Sum(m => m.Amount);
                    double totalTax = bookTaxService.GetAlls(m => m.BookOrderID == orderId && m.PassengerType == passengerType).Sum(m => m.Amount);
                    BookAgent bookAgent = bookAgentService.GetAlls(m => m.BookOrderID == orderId).FirstOrDefault();
                    UserSpendingHistoryService.WalletUserSpendingHistory(new WalletUserSpendingHistoryCreateModel
                    {
                        UserID = bookAgent.TicketingID,
                        Amount = totalTax + totalPrice,
                        AgentID = bookAgent.ID,
                        TransactionType = (int)TransactionEnum.TransactionType.IN
                    });
                }

                return Notifization.Success($"Đã hủy số vé: {ticketNo}");
            }
        }
        public ActionResult VoidItinerary(BookOrderIDModel model)
        {

            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            TokenModel tokenModel = VNA_AuthencationService.GetSession();
            // create session 
            if (tokenModel == null)
                return Notifization.Invalid(MessageText.Invalid);
            // create session 
            string _token = tokenModel.Token;
            string _conversationId = tokenModel.ConversationID;
            if (string.IsNullOrWhiteSpace(_token))
                return Notifization.Invalid(MessageText.Invalid);
            //  
            string orderId = model.ID;
            if (string.IsNullOrWhiteSpace(orderId))
                return Notifization.Invalid(MessageText.Invalid);

            BookPriceService bookPriceService = new BookPriceService();
            BookTaxService bookTaxService = new BookTaxService();
            BookPassengerService bookPassengerService = new BookPassengerService();
            BookOrderService bookOrderService = new BookOrderService();
            BookTicketService bookTicketService = new BookTicketService();
            BookAgentService bookAgentService = new BookAgentService();

            BookOrder bookOrder = bookOrderService.GetAlls(m => m.ID == orderId).FirstOrDefault();
            if (bookOrder == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string pnr = bookOrder.PNR;
            //DepartureDateTime 

            BookPassenger bookPassenger = bookPassengerService.GetAlls(m => m.ID == orderId).FirstOrDefault();
            if (bookPassenger == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string ticketNo = bookPassenger.TicketNumber;
            if (string.IsNullOrWhiteSpace(ticketNo))
                return Notifization.Invalid("Số vé không hợp lệ");
            // 
            BookTicket bookTicket = bookTicketService.GetAlls(m => m.BookOrderID == orderId).OrderBy(m => m.DepartureDateTime).FirstOrDefault();
            if (bookTicket == null)
                return Notifization.Invalid("Lỗi kiểm tra thời gian khởi hành");
            // 
            DateTime departureDateTime = bookTicket.DepartureDateTime;

            // check time void ticket 
            AirportConfigService airportConfigService = new AirportConfigService();
            AirportService airportService = new AirportService();
            int voidTicketTime = 0;
            Airport airportGo = airportService.GetAlls(m => m.IATACode == bookTicket.OriginLocation).FirstOrDefault();
            if (airportGo == null)
                return Notifization.Invalid("Lỗi không tìm thấy sân bay");
            // 
            AirportConfig airportConfig = airportConfigService.GetAlls(m => m.IATACode == bookTicket.OriginLocation).FirstOrDefault();
            if (airportConfig != null)
                voidTicketTime = airportConfig.VoidTicketTime;
            //
            DateTime datetime = Helper.TimeData.TimeHelper.UtcDateTime;
            if (datetime > departureDateTime)
                return Notifization.Invalid($"T.gian khởi hành không hợp lệ");
            //
            TimeSpan difference = departureDateTime - datetime;
            int leftBookTime = Convert.ToInt32(difference.TotalHours);
            int voidBookTime = 0;
            AirResBookDesigService airResBookDesigService = new AirResBookDesigService();
            AirResBookDesig airResBookDesig = airResBookDesigService.GetAlls(m => m.CodeID == bookTicket.ResBookDesigCode).FirstOrDefault();
            if (airResBookDesig == null)
                return Notifization.Invalid("Lỗi thời gian hủy hành trình");
            //
            voidBookTime = airResBookDesig.VoidBookTime;
            if (leftBookTime < voidBookTime)
                return Notifization.Invalid($"T.gian hủy chỗ quá giới hạn: {voidBookTime} giờ");
            // 
            using (var sessionService = new VNA_SessionService(tokenModel))
            {
                DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel
                {
                    ConversationID = tokenModel.ConversationID,
                    Token = tokenModel.Token
                };
                VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                if (printer.ApplicationResults.status != AIRService.WebService.VNA_DesignatePrinterLLSRQ.CompletionCodes.Complete)
                    return Notifization.Invalid(MessageText.Invalid);
                // 
                VNA_WSGetReservationRQService vNAWSGetReservationRQService = new VNA_WSGetReservationRQService();
                XMLObject.ReservationRq2.GetReservationRS reservationRS = vNAWSGetReservationRQService.GetReservation(new GetReservationModel
                {
                    ConversationID = tokenModel.ConversationID,
                    Token = tokenModel.Token,
                    PNR = pnr
                });
                //
                XMLObject.ReservationRq2.TicketingTimeLimit ticketingTimeLimit = reservationRS.Reservation.PassengerReservation.TicketingInfo.TicketingTimeLimit;
                if (ticketingTimeLimit == null)
                    return Notifization.Invalid("Lệnh đặt chỗ không tồn tại");
                // 
                VNA_OTA_CancelLLSRQService vNAWSOTA_CancelLLSRQService = new VNA_OTA_CancelLLSRQService();
                var data = vNAWSOTA_CancelLLSRQService.OTA_CancelLLS(tokenModel);
                //
                DateTime dateTime = Helper.TimeData.TimeHelper.UtcDateTime;
                if (departureDateTime.Minute - dateTime.Minute < voidTicketTime * 60)
                    return Notifization.Invalid($"T.gian hủy vé quá giới hạn: {voidTicketTime} giờ");
                //  
                if (printer.ApplicationResults.Error != null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                if (printer.ApplicationResults.status != AIRService.WebService.VNA_DesignatePrinterLLSRQ.CompletionCodes.Complete)
                    return Notifization.Invalid(MessageText.Invalid);
                // 
                XMLObject.ReservationRq2.AlreadyTicketed alreadyTicketed = reservationRS.Reservation.PassengerReservation.TicketingInfo.AlreadyTicketed;
                if (alreadyTicketed == null)
                    return Notifization.Invalid("Lỗi chưa xuất vé");
                //
                VNA_VoidTicketLLSRQService vNA_VoidTicketLLSRQService = new VNA_VoidTicketLLSRQService();
                XMLObject.VoidTicketRq.VoidTicketRS voidTicketRS = vNA_VoidTicketLLSRQService.VoidTicketLLSRQ(tokenModel, ticketNo);
                VNA_EndTransaction vNATransaction = new VNA_EndTransaction();
                vNATransaction.EndTransaction(tokenModel);
                //
                if (voidTicketRS.ApplicationResults.Error != null)
                    return Notifization.Invalid($"Lỗi hủy số vé: {ticketNo}");
                //  
                // update trang thai ve 
                if (!bookPassenger.IsVoided)
                {
                    bookPassenger.IsVoided = true;
                    bookPassenger.VoidDateTime = Helper.TimeData.TimeHelper.UtcDateTime;
                    bookPassenger.VoidUserdID = Helper.Current.UserLogin.IdentifierID;
                    bookPassengerService.Update(bookPassenger);
                    // tra lai tien cho ticketing
                    UserSpendingHistory walletUserSpendingHistory = new UserSpendingHistory();
                    string passengerType = bookPassenger.PassengerType;
                    double totalPrice = bookPriceService.GetAlls(m => m.BookOrderID == orderId && m.PassengerType == passengerType).Sum(m => m.Amount);
                    double totalTax = bookTaxService.GetAlls(m => m.BookOrderID == orderId && m.PassengerType == passengerType).Sum(m => m.Amount);
                    BookAgent bookAgent = bookAgentService.GetAlls(m => m.BookOrderID == orderId).FirstOrDefault();
                    UserSpendingHistoryService.WalletUserSpendingHistory(new WalletUserSpendingHistoryCreateModel
                    {
                        UserID = bookAgent.TicketingID,
                        Amount = totalTax + totalPrice,
                        AgentID = bookAgent.ID,
                        TransactionType = (int)TransactionEnum.TransactionType.IN
                    });
                }
                return Notifization.Success($"Đã hủy");
            }
        }

        public ActionResult TicketInfo(PNRModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            TokenModel tokenModel = VNA_AuthencationService.GetSession();
            // create session 
            if (tokenModel == null)
                return Notifization.Invalid("Cannot create session");
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
                List<BookSegmentModel> bookSegmentModels = new List<BookSegmentModel>
                    {
                        //new BookSegmentModel
                        //{
                        //    DepartureDateTime = Convert.ToDateTime("2021-02-03 09:25:00"),
                        //    ArrivalDateTime =   Convert.ToDateTime("2021-02-03 11:50:00"),
                        //    FlightNumber = 7262,
                        //    NumberInParty = 2,
                        //    ResBookDesigCode = "N",
                        //    AirEquipType = 787,
                        //    DestinationLocation = "HAN",
                        //    OriginLocation = "SGN"
                        //}
                        //,
                        new BookSegmentModel
                        {
                            DepartureDateTime = Convert.ToDateTime("2021-02-22T13:45:00"),
                            ArrivalDateTime = Convert.ToDateTime("2021-02-22T16:00:00"),
                            FlightNumber = 7265,
                            NumberInParty = 2,
                            ResBookDesigCode = "Q",
                            AirEquipType = 787,
                            DestinationLocation = "SGN",
                            OriginLocation = "HAN"
                        }
                    };
                VNA_OTA_AirBookLLSRQSevice vNAWSOTA_AirBookLLSRQSevice = new VNA_OTA_AirBookLLSRQSevice();
                List<BookSegmentModel> lstSegments = bookSegmentModels;
                if (lstSegments == null || lstSegments.Count == 0)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                //AIRService.WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRS ota_AirBookRS = vNAWSOTA_AirBookLLSRQSevice.FUNC_OTA_AirBookRS(new AirBookModel
                //{
                //    Segments = lstSegments,
                //    ConversationID = _conversationId,
                //    Token = _token
                //});
                //// 
                //if (ota_AirBookRS.ApplicationResults.Success == null)
                //    return Notifization.Invalid("Lỗi dịch vụ đặt chỗ");
                ////
                //var _originDestinationOption = ota_AirBookRS.OriginDestinationOption.ToList();
                //if (_originDestinationOption.Count == 0)
                //    return Notifization.NotFound(MessageText.NotFound);
                ////
                AirPriceModel airPriceModel = new AirPriceModel
                {
                    ADT = 1,
                    CNN = 1,
                    INF = 1,
                    ConversationID = _conversationId,
                    Token = _token,
                    Segments = new List<BookSegmentModel>
                    {
                        new BookSegmentModel
                        {
                            DepartureDateTime = Convert.ToDateTime("2021-02-08 00:10:00"),
                            ArrivalDateTime = Convert.ToDateTime("2021-02-08 02:30:00"),
                            FlightNumber = 7228,
                            NumberInParty = 2,
                            ResBookDesigCode = "K",
                            AirEquipType = 321,
                            DestinationLocation = "HAN",
                            OriginLocation = "SGN"
                        }
                    }

                };


                VNA_OTA_AirPriceLLSRQService vNAWSOTA_AirPriceLLSRQService = new VNA_OTA_AirPriceLLSRQService();
                var airPriceData = vNAWSOTA_AirPriceLLSRQService.AirPrice(airPriceModel);
                return Notifization.Data("OK", airPriceData);








                //VNA_WSGetReservationRQService vNAWSGetReservationRQService = new VNA_WSGetReservationRQService();
                //XMLObject.ReservationRq2.GetReservationRS reservationRS = vNAWSGetReservationRQService.GetReservation(getReservationModel);
                //VNA_EndTransaction vNATransaction = new VNA_EndTransaction();
                //vNATransaction.EndTransaction(tokenModel);
                //return Notifization.Data("OK", reservationRS);
            }
        }
        public ActionResult PnrSync(SyncModel model)
        {
            string pnr = model.PNR;
            string ticketingId = Helper.Current.UserLogin.IdentifierID;
            if (string.IsNullOrWhiteSpace(pnr))
                return Notifization.Invalid(MessageText.Invalid + "1");
            // 
            BookOrderService bookOrderService = new BookOrderService();
            BookOrder bookOrder = bookOrderService.GetAlls(m => !string.IsNullOrWhiteSpace(m.PNR) && m.PNR.ToLower() == pnr.ToLower()).FirstOrDefault();
            if (bookOrder != null)
                return Notifization.Invalid(MessageText.Duplicate);
            // 
            TokenModel tokenModel = VNA_AuthencationService.GetSession();
            // create session 
            if (tokenModel == null)
                return Notifization.Invalid("Cannot create session");
            // create session 
            string _token = tokenModel.Token;
            if (string.IsNullOrWhiteSpace(_token))
                return Notifization.NotService;
            //
            string _conversationId = tokenModel.ConversationID;
            if (!Helper.Current.UserLogin.IsClientInApplication())
                ticketingId = model.TicketingID;
            //
            if (string.IsNullOrWhiteSpace(ticketingId))
                return Notifization.Invalid("Nhân viên không hợp lệ");
            //
            UserInfoService userInfoService = new UserInfoService();
            UserInfo userInfo = userInfoService.GetAlls(m => m.UserID == ticketingId).FirstOrDefault();
            if (userInfo == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string agentId = AirAgentService.GetAgentIDByUserID(ticketingId);
            AirAgentService airAgentService = new AirAgentService();
            AirAgent airAgent = airAgentService.GetAlls(m => m.ID == agentId).FirstOrDefault();
            string agentCode = airAgent.CodeID;
            string agentName = airAgent.Title;
            //
            string agentProviderId = airAgent.ID;
            if (!string.IsNullOrWhiteSpace(airAgent.ParentID))
                agentProviderId = airAgent.ParentID;
            //
            string ticketingName = userInfo.FullName;
            string ticketingPhone = userInfo.Phone;
            string ticketingEmail = userInfo.Email;
            //
            CompanyContact bookCompany = new CompanyContact();
            HauntContact hauntContact = new HauntContact();
            int customerType = model.CustomerType;
            string strEmail = string.Empty;
            string strPhone = string.Empty;
            if (customerType == (int)ClientLoginEnum.CustomerType.Company)
            {
                string companyId = model.CompanyID;
                if (string.IsNullOrWhiteSpace(companyId))
                    return Notifization.Invalid("Vui lòng chọn công ty");
                //
                CompanyService companyService = new CompanyService();
                Company customerComp = companyService.GetAlls(m => m.ID == companyId).FirstOrDefault();
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
                XMLObject.ReservationRq2.GetReservationRS getReservationRS = vNAWSGetReservationRQService.GetReservation(new GetReservationModel
                {
                    ConversationID = tokenModel.ConversationID,
                    Token = tokenModel.Token,
                    PNR = pnr
                });
                //XMLObject.ReservationRq2.GetReservationRS getReservationRS = new XMLObject.ReservationRq2.GetReservationRS();
                //XmlDocument soapEnvelopeXml = new XmlDocument();
                //var path = HttpContext.Current.Server.MapPath(@"~/Team/Log-2021-01-26-15-34-05-SD77UU30chua-xuat-ve.xml");
                //soapEnvelopeXml.Load(path);
                //XmlNode xmlnode = soapEnvelopeXml.GetElementsByTagName("soap-env:Body")[0];
                //if (xmlnode != null)
                //    getReservationRS = XMLHelper.Deserialize<XMLObject.ReservationRq2.GetReservationRS>(xmlnode.InnerXml);
                ////  
                //
                if (getReservationRS == null)
                    return Notifization.NotFound(MessageText.NotFound);
                // thong tin chang bay

                BookOrderSaveModel bookOrderSaveModel = new BookOrderSaveModel();

                XMLObject.ReservationRq2.Segments segments = getReservationRS.Reservation.PassengerReservation.Segments;
                XMLObject.ReservationRq2.Passengers passengers = getReservationRS.Reservation.PassengerReservation.Passengers;
                if (segments == null || passengers == null)
                    return Notifization.Invalid(MessageText.Invalid + "2");
                //
                List<XMLObject.ReservationRq2.Segment> segmentsList = segments.Segment;
                List<XMLObject.ReservationRq2.Passenger> passengersList = passengers.Passenger;
                //  
                if (segmentsList.Count() == 0 || passengersList.Count() == 0)
                    return Notifization.Invalid("Mã pnr: đã hủy");
                // 
                XMLObject.ReservationRq2.AlreadyTicketed alreadyTicketed = getReservationRS.Reservation.PassengerReservation.TicketingInfo.AlreadyTicketed;
                XMLObject.ReservationRq2.TicketingTimeLimit ticketingTimeLimit = getReservationRS.Reservation.PassengerReservation.TicketingInfo.TicketingTimeLimit;
                List<XMLObject.ReservationRq2.Passenger> reservationPassengers = new List<XMLObject.ReservationRq2.Passenger>();
                int orderStatus = (int)WebCore.ENM.BookOrderEnum.BookOrderStatus.None;
                if (alreadyTicketed != null)
                {
                    orderStatus = (int)WebCore.ENM.BookOrderEnum.BookOrderStatus.Exported;
                    reservationPassengers = getReservationRS.Reservation.PassengerReservation.Passengers.Passenger;
                }
                else if (ticketingTimeLimit != null)
                {
                    orderStatus = (int)WebCore.ENM.BookOrderEnum.BookOrderStatus.Booking;
                    reservationPassengers = getReservationRS.Reservation.PassengerReservation.Passengers.Passenger;
                }
                //
                List<BookTicketPassenger> bookTicketPassenger = new List<BookTicketPassenger>();
                List<string> freeTexts = new List<string>();
                string test = "";
                foreach (var item in passengersList)
                {
                    test += $":{item.LastName}/{item.FirstName[0]}";
                    string passengerType = "ADT";
                    int gender = 0;
                    //
                    if (!string.IsNullOrWhiteSpace(item.WithInfant))
                    {
                        if (item.SpecialRequests.GenericSpecialRequest.Count() != 0)
                        {
                            string freeText = item.SpecialRequests.GenericSpecialRequest[0].FreeText;
                            freeTexts.Add(freeText);
                        }
                    }
                    //
                    if (item.FirstName.Contains("MSTR"))
                    {
                        passengerType = "CNN";
                        gender = 1;

                    }
                    else if (item.FirstName.Contains("MISS"))
                    {
                        passengerType = "CNN";
                        gender = 2;
                    }
                    //
                    string ticketNumber = string.Empty;
                    string fullText = string.Empty;
                    if (reservationPassengers.Count() > 0)
                    {
                        XMLObject.ReservationRq2.TicketingInfo ticketingInfo = getReservationRS.Reservation.PassengerReservation.TicketingInfo;
                        if (ticketingInfo != null)
                        {
                            List<XMLObject.ReservationRq2.TicketDetails> ticketDetails = ticketingInfo.TicketDetails;
                            if (ticketDetails.Count() > 0)
                            {

                                string nameMap = $"{item.LastName}/{item.FirstName[0]}";
                                XMLObject.ReservationRq2.TicketDetails ticket = ticketDetails.Where(m => m.PassengerName == nameMap).FirstOrDefault();
                                if (ticket != null)
                                {
                                    ticketNumber = ticket.TicketNumber;
                                    fullText = ticket.OriginalTicketDetails;
                                }
                            }
                        }
                    }
                    //
                    bookTicketPassenger.Add(new BookTicketPassenger
                    {
                        PassengerType = passengerType,
                        FullName = $"{item.LastName} {item.FirstName}",
                        Gender = gender,
                        DateOfBirth = null,
                        PassengerName = $"{item.LastName} {item.FirstName}",
                        ElementID = item.ElementId,
                        NameNumber = item.NameId,
                        FullText = fullText,
                        TicketNumber = ticketNumber
                    });

                }
                // update birth day
                foreach (var item in bookTicketPassenger)
                {
                    string freeText = freeTexts.Where(m => m.Replace("/", " ").Contains(item.FullName)).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(freeText))
                        continue;
                    //
                    string strDate = freeText.Split('/').LastOrDefault();
                    if (string.IsNullOrWhiteSpace(strDate))
                        continue;
                    // 
                    item.DateOfBirth = Convert.ToDateTime(strDate);
                    item.PassengerType = "INF";
                }

                bookOrderSaveModel.Passengers = bookTicketPassenger;

                List<string> lstSegmentText = new List<string>();
                List<BookSegmentModel> bookSegmentModels = new List<BookSegmentModel>();
                foreach (var item in segmentsList)
                {
                    string numberInParty = item.Air.NumberInParty;
                    string originLocation = item.Air.DepartureAirport;
                    string destinationLocation = item.Air.ArrivalAirport;
                    string departureDateTime = item.Air.DepartureDateTime;
                    string arrivalDateTime = item.Air.ArrivalDateTime;
                    string resBookDesigCode = item.Air.ResBookDesigCode;
                    string flightNumber = item.Air.FlightNumber;
                    string airEquipType = item.Air.EquipmentType;
                    lstSegmentText.Add(originLocation + destinationLocation);
                    string _segmentText = destinationLocation + originLocation;
                    bookSegmentModels.Add(new BookSegmentModel
                    {
                        DepartureDateTime = Convert.ToDateTime(departureDateTime),
                        ArrivalDateTime = Convert.ToDateTime(arrivalDateTime),
                        FlightNumber = Convert.ToInt32(flightNumber),
                        NumberInParty = Convert.ToInt32(numberInParty),
                        ResBookDesigCode = resBookDesigCode,
                        AirEquipType = Convert.ToInt32(airEquipType),
                        DestinationLocation = destinationLocation,
                        OriginLocation = originLocation,
                    });
                }

                bookOrderSaveModel.Segments = bookSegmentModels;
                // call save booking 
                double airAgentFee = 0; //  
                var airPriceModel = new AirPriceModel
                {
                    ConversationID = _conversationId,
                    Token = _token,
                    Segments = bookSegmentModels
                };
                // 
                foreach (var item in bookTicketPassenger)
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
                //
                VNA_OTA_AirBookLLSRQSevice vNAWSOTA_AirBookLLSRQSevice = new VNA_OTA_AirBookLLSRQSevice();
                if (bookSegmentModels == null || bookSegmentModels.Count == 0)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                AIRService.WebService.VNA_OTA_AirBookLLSRQ.OTA_AirBookRS ota_AirBookRS = vNAWSOTA_AirBookLLSRQSevice.FUNC_OTA_AirBookRS(new AirBookModel
                {
                    Segments = bookSegmentModels,
                    ConversationID = _conversationId,
                    Token = _token
                });
                // 
                if (ota_AirBookRS.ApplicationResults.Success == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                var _originDestinationOption = ota_AirBookRS.OriginDestinationOption.ToList();
                if (_originDestinationOption.Count == 0)
                    return Notifization.NotFound(MessageText.NotFound);
                // 
                VNA_OTA_AirPriceLLSRQService vNAWSOTA_AirPriceLLSRQService = new VNA_OTA_AirPriceLLSRQService();
                var airPriceData = vNAWSOTA_AirPriceLLSRQService.AirPrice(airPriceModel);
                List<FareTax> fareTaxs = new List<FareTax>();
                List<FareFlight> fareFlights = new List<FareFlight>();
                //
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
                int itineraryType = (int)BookOrderEnum.BookItineraryType.None;
                string isoCountry = getReservationRS.Reservation.POS.Source.ISOCountry;
                if (!string.IsNullOrWhiteSpace(isoCountry))
                {
                    string language = Helper.Language.LanguagePage.GetLanguageCode;
                    if (isoCountry.ToLower() == language.ToLower())
                        itineraryType = (int)BookOrderEnum.BookItineraryType.Inland;
                    else
                        itineraryType = (int)BookOrderEnum.BookItineraryType.National;
                    //
                }
                // 
                BookTicketService bookTicketService = new BookTicketService();
                string bookTicketId = bookTicketService.BookSave(new BookTicketOrder
                {
                    PNR = pnr,
                    CustomerType = model.CustomerType,
                    Summary = "Aync data",
                    ItineraryType = itineraryType,
                    OrderDate = TimeHelper.UtcDateTime,
                    Flights = bookSegmentModels,
                    Passengers = bookTicketPassenger,
                    FareTaxs = fareTaxs,
                    FareFlights = fareFlights,
                    OrderStatus = orderStatus,
                    AgentInfo = new BookAgentInfo
                    {
                        AgentID = agentId,
                        AgentName = agentName,
                        AgentCode = agentCode,
                        AgentFee = airAgentFee,
                        TiketingID = ticketingId,
                        TiketingName = ticketingName,
                        TiketingPhone = ticketingPhone,
                        TiketingEmail = ticketingEmail,
                        ProviderID = agentProviderId
                    },
                    Contacts = new BookOrderContact
                    {
                        HauntContact = new HauntContact
                        {
                            Name = bookTicketPassenger.Where(m => m.PassengerType == "ADT").FirstOrDefault().FullName,
                            Email = getReservationRS.Reservation.EmailAddresses.EmailAddress.Address,
                            Phone = getReservationRS.Reservation.PhoneNumbers.PhoneNumber.Number,
                        },
                        CompanyContact = bookCompany
                    }
                });

                return Notifization.Success(MessageText.AsyncSuccess);

            }
        }
    }
}
