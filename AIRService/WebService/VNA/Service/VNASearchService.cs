using System;
using ApiPortalBooking.Models;
using AIRService.Entities;
using AIRService.Models;
using AIR.Helper.Session;
using Notifies.Helper;
using System.Web.Mvc;
using AIRService.WS.Service;
using System.Collections.Generic;
using AIRService.WebService.VNA.Enum;
using AIRService.WS.Entities;
using System.Linq;
using AIRService.WebService.WSOTA_AirTaxRQ;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AIRService.Service;
using ApiPortalBooking.Models.VNA_WS_Model;

namespace AIRService.Service
{
    public class VNASearchService
    {
        // search  
        public ActionResult FlightSearch(FlightSearchModel model)
        {
            using (var sessionService = new SessionService())
            {


                // check model
                if (model == null)
                    return Notifization.Invalid(NotifizationText.Invalid);

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
                // create session 
                var _session = sessionService.GetSession();
                if (_session == null)
                    return Notifization.NotService;
                // get token
                string _token = _session.Token;
                string _conversationId = _session.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.NotService;
                // seach data in web service
                AirAvailLLSRQService airAvailLLSRQService = new AirAvailLLSRQService();
                VNAFareLLSRQService vNAFareLLSRQService = new VNAFareLLSRQService();
                // Flight >> go
                // ****************************************************************************************************************************
                //
                var dataAirAvail = airAvailLLSRQService.FUNC_OTA_AirAvailLLSRQ(new AirAvailLLSRQModel
                {
                    Token = _token,
                    ConversationID = _conversationId,
                    DepartureDateTime = model.DepartureDateTime,
                    DestinationLocation = model.DestinationLocation,
                    OriginLocation = model.OriginLocation
                });
                // check data
                if (dataAirAvail == null)
                    return Notifization.NotService;
                // search light data
                var lstOrigin = dataAirAvail.OriginDestinationOptions.OriginDestinationOption.ToList();
                if (lstOrigin.Count == 0)
                    return Notifization.NotFound(NotifizationText.NotFound);
                // attachment INF (em bé)
                int _passengerTotal = model.ADT + model.CNN + model.INF;
                //  passenger total is availability 
                int _availabilityTotal = model.ADT + model.CNN;
                //
                //var _availabilityData =   List<>
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
                    return Notifization.Invalid(NotifizationText.Invalid);
                // Fare 
                string _currencyCode = "VND";
                var lstFlightSegment = new List<FlightSegment>();
                var lstFlightSearch = new List<FlightSearch>();
                int _year = model.DepartureDateTime.Year;
                foreach (var origin in lstOrigin)
                {
                    //#1. Availability: get all Flight is Availability
                    //#2.
                    var _lstFlightSegment = origin.FlightSegment;
                    if (_lstFlightSegment.Count() > 0)
                    {
                        foreach (var flightSegment in _lstFlightSegment)
                        {
                            var _lstBookingClassAvail = flightSegment.BookingClassAvail;
                            if (_lstBookingClassAvail.Count() > 0)
                            {
                                foreach (var bookingClassAvail in _lstBookingClassAvail)
                                {
                                    if (!string.IsNullOrWhiteSpace(bookingClassAvail.Availability) && int.Parse(bookingClassAvail.Availability) > _availabilityTotal)
                                    {
                                        var _datedepart = DateTime.Parse(_year + "-" + flightSegment.DepartureDateTime + ":00");
                                        var _arrivalDateTime = _datedepart.AddMinutes(double.Parse(flightSegment.FlightDetails.TotalTravelTime));
                                        string _resBookDesigCode = bookingClassAvail.ResBookDesigCode;
                                        //  price model
                                        var flightCostDetails = GetFareDetails(new VNAFareLLSRQModel
                                        {
                                            FareLLS = fareLLSModel,
                                            FareRSData = dataFareLLS,
                                            CurrencyCode = _currencyCode,
                                            ResBookDesigCode = _resBookDesigCode
                                        });
                                        if (flightCostDetails != null && flightCostDetails.Count > 0)
                                        {
                                            lstFlightSegment.Add(new FlightSegment
                                            {
                                                AirEquipType = flightSegment.Equipment.AirEquipType,
                                                ArrivalDateTime = Convert.ToString(_arrivalDateTime),
                                                DepartureDateTime = Convert.ToString(_datedepart),
                                                DestinationLocation = _destinationLocation,
                                                FlightNo = Convert.ToInt32(flightSegment.FlightNumber),
                                                FlightType = (int)VNAEnum.FlightDirection.FlightGo,
                                                NumberInParty = _availabilityTotal,
                                                OriginLocation = _originLocation,
                                                PriceDetails = flightCostDetails,
                                                ResBookDesigCode = _resBookDesigCode,
                                                RPH = flightSegment.RPH
                                            });
                                        }
                                    }
                                    else
                                        continue;

                                }
                            }
                            else
                                continue;
                        }
                    }
                    else
                        continue;
                }
                //
                if (lstFlightSegment.Count() == 0)
                    return Notifization.NotFound(NotifizationText.NotFound);
                lstFlightSearch.Add(new FlightSearch
                {
                    TimeHanndle = 1,
                    ADT = _adt,
                    CNN = _cnn,
                    INF = _cnn,
                    FlightSegment = lstFlightSegment
                });
                return Notifization.Data("OK", lstFlightSegment);
            }
        }
        // cost 
        public ActionResult FlightCost(FlightSearchModel model)
        {
            using (var sessionService = new SessionService())
            {
                // check model 
                if (model == null)
                    return Notifization.Invalid(NotifizationText.Invalid);
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
                // create session 
                var _session = sessionService.GetSession();
                if (_session == null)
                    return Notifization.NotService;
                // get token
                string _token = _session.Token;
                string _conversationId = _session.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.NotService;
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
                    return Notifization.Invalid(NotifizationText.Invalid);
                //
                return Notifization.Data("OK", dataFareLLS);
            }
        }
        public List<FareDetailsModel> GetFareDetails(VNAFareLLSRQModel model)
        {
            var dataFareLLS = model.FareRSData;
            List<WebService.WSFareLLSRQ.FareRSFareBasis> fareRSFareBases = dataFareLLS.FareBasis.ToList();
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
            List<FareDetailsModel> fareDetailsModels = new List<FareDetailsModel>();
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
                        double _total = 0.0;

                        if (!string.IsNullOrWhiteSpace(fareRSFareBasis.BaseFare.Amount) && Convert.ToDouble(fareRSFareBasis.BaseFare.Amount) > 0)
                        {
                            double _amount = Convert.ToDouble(fareRSFareBasis.BaseFare.Amount);
                            _total += _amount;
                            //
                            flightCosts.Add(new FlightCost
                            {
                                FareAmmout = _amount,
                                Total = _total
                            });
                            fareDetailsModels.Add(new FareDetailsModel
                            {
                                RBH = fareRSFareBasis.RPH,
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
                    else
                    {
                        continue;
                    }
                }
            }
            if (fareDetailsModels.Count == 0)
                return null;
            //ok
            return fareDetailsModels;
        }
        // fare 
        public ActionResult FlightFee(FlightFareModel model)
        {
            using (var sessionService = new SessionService())
            {
                // check model
                if (model == null)
                    return Notifization.Invalid(NotifizationText.Invalid);
                // 
                //int _adt = model.ADT; // Adults
                //int _cnn = model.CNN; // minors
                //int _inf = model.INF; // Infant
                string _destinationLocation = model.DestinationLocation;
                string _originLocation = model.OriginLocation;
                DateTime _departureDateTime = model.DepartureDateTime;
                DateTime? _returnDateTime = model.ReturnDateTime;
                ////
                //if (_adt <= 0)
                //    return Notifization.Invalid("Adults must be > 0");
                ////
                //if (_inf > _adt)
                //    return Notifization.Invalid("Infant invalid");
                // create session
                var _session = sessionService.GetSession();
                if (_session == null)
                    return Notifization.NotService;
                // get token
                string _token = _session.Token;
                string _conversationId = _session.ConversationID;
                if (string.IsNullOrWhiteSpace(_token))
                    return Notifization.NotService;
                // seach data in web service
                VNAOTA_AirTaxRQService vNAOTA_AirTaxRQService = new VNAOTA_AirTaxRQService();
                AIRService.WebService.WSOTA_AirTaxRQ.AirTaxRS airTaxRS = vNAOTA_AirTaxRQService.GetTaxByFilght(new Resquet_WsTaxModel
                {
                    ConversationID = _conversationId,
                    Token = _token,
                    RPH = model.RPH,
                    OriginLocation = _originLocation,
                    DestinationLocation = _destinationLocation,
                    DepartureDateTime = _departureDateTime,
                    ResBookDesigCode = model.ResBookDesigCode,
                    ArrivalDateTime = model.ArrivalDateTime,
                    ReturnDateTime = _returnDateTime,
                    AirEquipType = model.AirEquipType,
                    PassengerType = model.PassengerType,
                    FareBasisCode = model.FareBasisCode,
                    BaseFare = new Resquet_WsTax_BaseFareModel
                    {
                        Amount = model.BaseFare.Amount,
                        CurrencyCode = model.BaseFare.CurrencyCode
                    }
                });
                //
                string jsonData = JsonConvert.SerializeObject(airTaxRS.Items[1]);
                FeeDetailsModel feeDetailsModel = JsonConvert.DeserializeObject<FeeDetailsModel>(jsonData);
                //
                if (feeDetailsModel == null)
                    return Notifization.NotFound(NotifizationText.NotFound);
                //
                Response_FlightFee response_FlightFee = new Response_FlightFee
                {
                    RPH = feeDetailsModel.ItineraryInfo[0].RPH,
                    RPHSpecified = feeDetailsModel.ItineraryInfo[0].RPHSpecified,
                    PassengerType = feeDetailsModel.ItineraryInfo[0].PTC_FareBreakdown.PassengerType,
                    Total = feeDetailsModel.ItineraryInfo[0].TaxInfo.Total,
                    Taxes = feeDetailsModel.ItineraryInfo[0].TaxInfo.Taxes
                };
                return Notifization.Data("", response_FlightFee);
            }
        }
        // book 
        public ActionResult FlightBook(BookModel model)
        {
            //////// check model
            //////if (model == null)
            //////    return Notifization.Invalid(NotifizationText.Invalid);
            //////// 
            ////////int _adt = model.ADT; // Adults
            ////////int _cnn = model.CNN; // minors
            ////////int _inf = model.INF; // Infant
            ////////string _destinationLocation = model.DestinationLocation;
            ////////string _originLocation = model.OriginLocation;
            ////////DateTime _departureDateTime = model.DepartureDateTime;
            ////////DateTime? _returnDateTime = model.ReturnDateTime;
            //////////
            ////////if (_adt <= 0)
            ////////    return Notifization.Invalid("Adults must be > 0");
            //////////
            ////////if (_inf > _adt)
            ////////    return Notifization.Invalid("Infant invalid");
            //////// create session
            //////SessionService sessionService = new SessionService();
            //////var _session = sessionService.GetSession();
            //////if (_session == null)
            //////    return null;
            //////// get token
            //////string _token = _session.Token;
            //////string _conversationId = _session.ConversationID;
            //////if (string.IsNullOrWhiteSpace(_token))
            //////    return null;
            //////// seach data in web service
            ////////List<AirBookModelSegment> airBookModelSegments = new List<AirBookModelSegment>();
            ////////AirBookModelSegment airBookModelSegmen = new AirBookModelSegment();
            ////////airBookModelSegmen.DepartureDateTime = model.
            ////////VNAWSOTA_AirBookLLSRQSevice vNAWSOTA_AirBookLLSRQSevice = new VNAWSOTA_AirBookLLSRQSevice();
            ////////List<AirBookModelSegment> lstSegments = model.Segments;
            ////////if (lstSegments == null || lstSegments.Count == 0)
            ////////    return Notifization.Invalid(NotifizationText.Invalid);

            ////////AIRService.WebService.WSOTA_AirBookLLSRQ.OTA_AirBookRS oTA_AirBookRS = vNAWSOTA_AirBookLLSRQSevice.FUNC_OTA_AirBookRS(new AirBookModel
            ////////{
            ////////    Segments = lstSegments,
            ////////    ConversationID = _conversationId,
            ////////    Token = _token
            ////////});
            //////////
            ////////if (oTA_AirBookRS.ApplicationResults.Success == null)
            ////////    return Notifization.NotFound(NotifizationText.NotFound);
            //////////
            ////////var _originDestinationOption = oTA_AirBookRS.OriginDestinationOption.ToList();
            ////////if (_originDestinationOption.Count == 0)
            ////////    return Notifization.NotFound(NotifizationText.NotFound);
            ////////// get price

            //////var _airPriceModel = new AirPriceModel
            //////{
            //////    ConversationID = _conversationId,
            //////    Token = _token,
            //////    Segments = model.lFlight
            //////};
            //////foreach (var item in model.lPassenger)
            //////{
            //////    if (item.PassengerType.ToUpper() == "ADT")
            //////    {
            //////        _airPriceModel.ADT++;
            //////        continue;
            //////    }
            //////    if (item.PassengerType.ToUpper() == "CNN")
            //////    {
            //////        _airPriceModel.CNN++;
            //////        continue;
            //////    }
            //////    if (item.PassengerType.ToUpper() == "INF")
            //////    {
            //////        _airPriceModel.INF++;
            //////        continue;
            //////    }
            //////}




            //////VNAWSOTA_AirPriceLLSRQService vNAWSOTA_AirPriceLLSRQService = new VNAWSOTA_AirPriceLLSRQService();
            //////AIRService.WebService.WSOTA_AirPriceLLSRQ.OTA_AirPriceRS oTA_AirPriceRS = vNAWSOTA_AirPriceLLSRQService.FUNCOTA_AirPriceRQ(_airPriceModel);

            ////////VNATransaction vNATransaction = new VNATransaction();
            ////////var End = vNATransaction.EndTransaction(_session);
            //////// End = vNATransaction.EndTransaction(_session);

            ////////
            ////return Notifization.Data("", oTA_AirPriceRS);
            // 

            return Notifization.NotService;

        }

        /// <summary>
        /// THE STEP ACTION BOOKING ****************************************************************************************************************************************
        /// #1.SessionCreateRQ
        /// #2.OTA_AirBookLLSRQ
        /// #3.OTA_AirPriceLLSRQ
        /// #4.PassengerDetailsRQ
        /// #5.EndTransactionLLSRQ
        /// #6.SessionCloseRQ
        /// END STEP ACTION BOOKING ****************************************************************************************************************************************
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult BookVe(BookModel model)
        {
            try
            {
                using (var sessionService = new SessionService())
                {
                    //var vnaTransaction = new VNATransaction();
                    //var tokenModel = new TokenModel();
                    var _session = sessionService.GetSession();
                    if (_session == null)
                        return Notifization.Invalid("Can not create session");
                    // get token
                    string _token = _session.Token;
                    string _conversationId = _session.ConversationID;
                    if (string.IsNullOrWhiteSpace(_token))
                        return Notifization.Invalid("Can not create token");
                    //
                    using (var vnaTransaction = new VNAEndTransaction(_session))
                    {
                        var airBookModel = new AirBookModel
                        {
                            ConversationID = _conversationId,
                            Token = _token
                        };
                        //seach data in web service
                        List<AirBookModelSegment> airBookModelSegments = new List<AirBookModelSegment>();
                        AirBookModelSegment airBookModelSegment = new AirBookModelSegment();

                        // #1. flight information - ***********************************************************************************************************************************
                        VNAWSOTA_AirBookLLSRQSevice vNAWSOTA_AirBookLLSRQSevice = new VNAWSOTA_AirBookLLSRQSevice();
                        List<AirBookModelSegment> lstSegments = model.lFlight;
                        if (lstSegments == null || lstSegments.Count == 0)
                            return Notifization.Invalid("Segments information invalid");
                        //
                        AIRService.WebService.WSOTA_AirBookLLSRQ.OTA_AirBookRS oTA_AirBookRS = vNAWSOTA_AirBookLLSRQSevice.FUNC_OTA_AirBookRS(new AirBookModel
                        {
                            Segments = lstSegments,
                            ConversationID = _conversationId,
                            Token = _token
                        });
                        //


                        if (oTA_AirBookRS.ApplicationResults.Success == null)
                            return Notifization.Data("AirBook invalid", oTA_AirBookRS);
                        //
                        var _originDestinationOption = oTA_AirBookRS.OriginDestinationOption.ToList();
                        if (_originDestinationOption.Count == 0)
                            return Notifization.NotFound(NotifizationText.NotFound);
                        // #2. Get price - ***************************************************************************************************************************************************
                        airBookModel.Segments = model.lFlight;
                        var airPriceModel = new AirPriceModel
                        {
                            ConversationID = _conversationId,
                            Token = _token,
                            lFlight = model.lFlight
                        };
                        foreach (var item in model.lPassenger)
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
                        VNAWSOTA_AirPriceLLSRQService vNAWSOTA_AirPriceLLSRQService = new VNAWSOTA_AirPriceLLSRQService();
                        var airPriceData = vNAWSOTA_AirPriceLLSRQService.AirPrice(airPriceModel);
                        //


                        if (airPriceData.ApplicationResults.Success == null)
                            return Notifization.Invalid("AirPrice invalid");
                        // #3. Get PNA code - **************************************************************************************************************************************************
                        var result = new BookVeResult
                        {
                            lPricingInPNR = new List<PricingInPNR>()
                        };
                        foreach (var item in airPriceData.PriceQuote.PricedItinerary.AirItineraryPricingInfo)
                        {
                            foreach (var iter in model.lPassenger.Where(m => m.PassengerType.ToUpper().Equals(item.PassengerTypeQuantity.Code)))
                            {
                                var price = new PricingInPNR
                                {
                                    DateOfBirth = Convert.ToString(iter.DateOfBirth),
                                    SurName = iter.Surname,
                                    GivenName = iter.GivenName,
                                    type = iter.PassengerType.ToUpper(),
                                    Fare = double.Parse(item.ItinTotalFare.BaseFare.Amount)
                                };
                                foreach (var tax in item.ItinTotalFare.Taxes.Tax)
                                {
                                    if (tax.TaxCode.Equals("YRI"))
                                    {
                                        price.ServiceFee = double.Parse(tax.Amount);
                                    }
                                    if (tax.TaxCode.Equals("UE3"))
                                    {
                                        price.TaxFee = double.Parse(tax.Amount);
                                    }
                                    if (tax.TaxCode.Equals("AX"))
                                    {
                                        price.VAT = double.Parse(tax.Amount);
                                    }
                                    if (tax.TaxCode.Equals("C4"))
                                    {
                                        price.ScanFee = double.Parse(tax.Amount);
                                    }
                                }
                                result.lPricingInPNR.Add(price);
                            }
                        }
                        result.Total = double.Parse(airPriceData.PriceQuote.PricedItinerary.TotalAmount);
                        model.lPassenger = model.lPassenger.OrderBy(m => m.PassengerType).ToList();
                        // create model passenger details model
                        var passengerDetailModel = new PassengerDetailsModel
                        {
                            ContactEmail = model.ContactEmail,
                            ConversationID = _conversationId,
                            Token = _token,
                            lPassenger = model.lPassenger,
                            AirBook = oTA_AirBookRS
                        };
                        // call service  passenger details in ws.service
                        VNAWSPassengerDetailsRQService vNAWSPassengerDetailsRQService = new VNAWSPassengerDetailsRQService();
                        //AIRService.WebService.WSPassengerDetailsRQ.PassengerDetailsRS passengerDetailsRS = vNAWSPassengerDetailsRQService.PassengerDetail(passengerDetailModel);
                        Response_PassengerDetailsModel response_PassengerDetailsModel = vNAWSPassengerDetailsRQService.PassengerDetail2(passengerDetailModel);
                        // End  transaction
                        vnaTransaction.EndTransaction();
                        if (response_PassengerDetailsModel == null)
                            return Notifization.Invalid("Passenger is not result");
                        //
                        if (string.IsNullOrWhiteSpace(response_PassengerDetailsModel.PNR))
                            return Notifization.Invalid("Cannot get PNA code");
                        //SET PNR CODE
                        result.PNR = response_PassengerDetailsModel.PNR;
                        sessionService.CloseSession(_session);
                        //  retur value
                        return Notifization.Data("OK", result);
                    }
                }
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
        public ActionResult ReleaseTicket(BookVeResult model)
        {
            try
            {
                using (var sessionService = new SessionService())
                {
                    var _session = sessionService.GetSession();
                    if (_session == null)
                        return Notifization.Invalid(NotifizationText.Invalid);
                    // get token
                    string _token = _session.Token;
                    string _conversationId = _session.ConversationID;
                    if (string.IsNullOrWhiteSpace(_token))
                        return Notifization.Invalid(NotifizationText.Invalid);
                    //
                    var tokenAppro = sessionService.GetSession();
                    var paymentModel = new PaymentModel();
                    //paymentModel.RevResult = ReservationData.RevResult;
                    paymentModel.ConversationID = tokenAppro.ConversationID;
                    paymentModel.Token = tokenAppro.Token;
                    paymentModel.pnr = model.PNR;
                    paymentModel.PaymentOrderDetail = new List<PaymentOrderDetail>();
                    paymentModel.Total = model.Total;
                    foreach (var item in model.lPricingInPNR)
                    {
                        var paymentOrderDetail = new PaymentOrderDetail();
                        paymentOrderDetail.PsgrType = item.type;
                        paymentOrderDetail.FirstName = item.GivenName;
                        paymentOrderDetail.LastName = item.SurName;
                        paymentOrderDetail.BaseFare = item.Fare;
                        paymentOrderDetail.Taxes = item.ScanFee + item.TaxFee + item.VAT + item.ServiceFee;
                        paymentModel.PaymentOrderDetail.Add(paymentOrderDetail);
                    }
                    VNAPaymentRQService vNAPaymentRQService = new VNAPaymentRQService();
                    var paymentData = vNAPaymentRQService.Payment(paymentModel);
                    sessionService.CloseSession(tokenAppro);
                    //Login -> DisginPinter -> GetRev -> OTA_AirPriceLLSRQ -> PaymentRQ -> AirTicketLLSRQ -> Endtransession
                    #region xuất vé
                    TokenModel tokenModel = _session;
                    //wss.SarbreCommand(tokenModel, "IG");
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                    designatePrinter.ConversationID = tokenModel.ConversationID;
                    designatePrinter.Token = tokenModel.Token;
                    //
                    WSDesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new WSDesignatePrinterLLSRQService();
                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    if (printer.ApplicationResults.status != AIRService.WebService.WSDesignatePrinterLLSRQ.CompletionCodes.Complete)
                        return Notifization.ERROR("Server not responding from designate printer");
                    //
                    var jjson = new JavaScriptSerializer().Serialize(printer);
                    var getReservationModel = new GetReservationModel();
                    getReservationModel.ConversationID = tokenModel.ConversationID;
                    getReservationModel.Token = tokenModel.Token;
                    getReservationModel.PNR = model.PNR;
                    //paymentRQ
                    //wss.GetReservationWS(getReservationModel);
                    VNAWSGetReservationRQService vNAWSGetReservationRQService = new VNAWSGetReservationRQService();
                    var ReservationData = vNAWSGetReservationRQService.GetReservation(getReservationModel);
                    if (paymentData.Result != null && paymentData.Result.ResultCode.Equals("SUCCESS"))
                    {
                        if (paymentData.Items.Count() == 0)
                            return Notifization.ERROR("Server not responding from getreservation");
                        //
                        var data = (WebService.WSPaymentRQ.AuthorizationResultType)paymentData.Items[0];
                        if (string.IsNullOrWhiteSpace(data.ApprovalCode))
                            return Notifization.NotFound("ApprovalCode is null");
                        //
                        var airTicket = new AirTicketLLSRQModel();
                        airTicket.ConversationID = tokenModel.ConversationID;
                        airTicket.Token = tokenModel.Token;
                        airTicket.PNR = model.PNR;
                        airTicket.approveCode = data.ApprovalCode;
                        airTicket.lPricingInPNR = model.lPricingInPNR;
                        VNAWSAirTicketLLSRQService vNAWSAirTicketLLSRQService = new VNAWSAirTicketLLSRQService();
                        var airTicketData = vNAWSAirTicketLLSRQService.AirTicket(airTicket);
                        VNAEndTransaction vNATransaction = new VNAEndTransaction();
                        var end = vNATransaction.EndTransaction(tokenModel);
                        // json = new JavaScriptSerializer().Serialize(end);
                        return Notifization.Data("OK", end);
                    }
                    #endregion
                    return Notifization.ERROR(NotifizationText.Invalid);
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
        /// <param name="mode">PNR code</param>
        /// <returns></returns>
        public ActionResult TicketInfo(PNRModel mode)
        {
            try
            {
                if (mode == null)
                    return Notifization.Invalid(NotifizationText.Invalid);
                //
                string pnr = mode.PNR;
                if (string.IsNullOrWhiteSpace(pnr))
                    return Notifization.Invalid("PNR code in valid");
                //
                using (var sessionService = new SessionService())
                {
                    TokenModel tokenModel = sessionService.GetSession();
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                    designatePrinter.ConversationID = tokenModel.ConversationID;
                    designatePrinter.Token = tokenModel.Token;
                    VNAEndTransaction vNATransaction = new VNAEndTransaction();
                    //WSDesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new WSDesignatePrinterLLSRQService();
                    //var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    //if (printer.ApplicationResults.status != AIRService.WebService.WSDesignatePrinterLLSRQ.CompletionCodes.Complete)
                    //    return Notifization.Invalid(NotifizationText.Invalid);
                    var getReservationModel = new GetReservationModel();
                    getReservationModel.ConversationID = tokenModel.ConversationID;
                    getReservationModel.Token = tokenModel.Token;
                    getReservationModel.PNR = pnr;
                    VNAWSGetReservationRQService vNAWSGetReservationRQService = new VNAWSGetReservationRQService();
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
        public ActionResult VoidTicket(PNRModel mode)
        {
            try
            {
                if (mode == null)
                    return Notifization.Invalid(NotifizationText.Invalid);
                //
                string pnr = mode.PNR;
                if (string.IsNullOrWhiteSpace(pnr))
                    return Notifization.Invalid("PNR code in valid");
                //
                using (var sessionService = new SessionService())
                {
                    var voidTicketModel = new VoidTicketModel();
                    TokenModel  tokenModel = sessionService.GetSession();
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                    designatePrinter.ConversationID = tokenModel.ConversationID;
                    designatePrinter.Token = tokenModel.Token;
                    VNAEndTransaction vNATransaction = new VNAEndTransaction();
                    WSDesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new WSDesignatePrinterLLSRQService();
                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    if (printer.ApplicationResults.status != AIRService.WebService.WSDesignatePrinterLLSRQ.CompletionCodes.Complete)
                        return Notifization.Invalid(NotifizationText.Invalid);
                    //
                    var getReservationModel = new GetReservationModel();
                    getReservationModel.ConversationID = tokenModel.ConversationID;
                    getReservationModel.Token = tokenModel.Token;
                    getReservationModel.PNR = pnr;
                    VNAWSGetReservationRQService vNAWSGetReservationRQService = new VNAWSGetReservationRQService();
                    var dataPnr = vNAWSGetReservationRQService.GetReservation(getReservationModel);
                    var ldata = new List<WebService.WSVoidTicketLLSRQ.VoidTicketRS>();
                    if (dataPnr.TicketDetails != null && dataPnr.TicketDetails.Count > 0)
                    {
                        VNAWSVoidTicketLLSRQService vNAWSVoidTicketLLSRQService = new VNAWSVoidTicketLLSRQService();
                        foreach (var item in dataPnr.TicketDetails.Where(m => m.TransactionIndicator.Equals("TE")))
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
                        tokenModel = sessionService.GetSession();
                        getReservationModel.ConversationID = tokenModel.ConversationID;
                        getReservationModel.Token = tokenModel.Token;
                        getReservationModel.PNR = pnr;
                        var dataPnr2 = vNAWSGetReservationRQService.GetReservation(getReservationModel);
                        VNAWSOTA_CancelLLSRQService vNAWSOTA_CancelLLSRQService = new VNAWSOTA_CancelLLSRQService();
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
        public ActionResult SalesReport(DateTime date)
        {
            try
            {
                using (var sessionService = new SessionService())
                {
                    TokenModel tokenModel = sessionService.GetSession();
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                    designatePrinter.ConversationID = tokenModel.ConversationID;
                    designatePrinter.Token = tokenModel.Token;
                    WSDesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new WSDesignatePrinterLLSRQService();
                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    var reportModel = new ReportModel();
                    reportModel.Token = tokenModel.Token;
                    reportModel.ConversationID = tokenModel.ConversationID;
                    reportModel.ReportDate = date;
                    //
                    VNAWSKT_AsrServicesService vNAWSKT_AsrServicesService = new VNAWSKT_AsrServicesService();
                    var data = vNAWSKT_AsrServicesService.AsrServices(reportModel);
                    return Notifization.Data("OK", data);
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
                using (var sessionService = new SessionService())
                {
                    VNAWSGetReservationRQService vNAWSGetReservationRQService = new VNAWSGetReservationRQService();
                    WSDesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new WSDesignatePrinterLLSRQService();
                    var tokenModel = sessionService.GetSession();
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                    designatePrinter.ConversationID = tokenModel.ConversationID;
                    designatePrinter.Token = tokenModel.Token;
                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    if (printer.ApplicationResults.status != AIRService.WebService.WSDesignatePrinterLLSRQ.CompletionCodes.Complete)
                        return Notifization.Invalid(NotifizationText.Invalid);
                    //
                    var getReservationModel = new GetReservationModel();
                    getReservationModel.ConversationID = tokenModel.ConversationID;
                    getReservationModel.Token = tokenModel.Token;
                    getReservationModel.PNR = pnr;
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
