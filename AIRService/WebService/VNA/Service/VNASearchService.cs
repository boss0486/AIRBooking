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
            SessionService sessionService = new SessionService();
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
        // cost 
        public ActionResult FlightCost(FlightSearchModel model)
        {
            // check model 
            if (model == null)
                return Notifization.Invalid(NotifizationText.Invalid);

            int _adt = model.ADT; // Adults
            int _cnn = model.CNN; // minors
            int _inf = model.INF; // Infant

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
            SessionService sessionService = new SessionService();
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
            SessionService sessionService = new SessionService();
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


        public ActionResult BookVe(BookModel model)
        {
            try
            {
                //using (var sessionService = new SessionService())
                //{
                var sessionService = new SessionService(); 
                    var tokenModel = new TokenModel();
                    var _session = sessionService.GetSession();
                    if (_session == null)
                        return null;
                    // get token
                    string _token = _session.Token;
                    string _conversationId = _session.ConversationID;
                    if (string.IsNullOrWhiteSpace(_token))
                        return null;

                    //tokenModel = wss.CreateSession();
                    var airBookModel = new AirBookModel
                    {
                        ConversationID = _conversationId,
                        Token = _token
                    };

                    //- SessionCreateRQ
                    //- OTA_AirBookLLSRQ
                    //- OTA_AirPriceLLSRQ
                    //- PassengerDetailsRQ
                    //- EndTransactionLLSRQ
                    //- SessionCloseRQ
                    //seach data in web service
                    List<AirBookModelSegment> airBookModelSegments = new List<AirBookModelSegment>();
                    AirBookModelSegment airBookModelSegmen = new AirBookModelSegment();

                    // #1. flight information - ***********************************************************************************************************************************
                    VNAWSOTA_AirBookLLSRQSevice vNAWSOTA_AirBookLLSRQSevice = new VNAWSOTA_AirBookLLSRQSevice();
                    List<AirBookModelSegment> lstSegments = model.lFlight;
                    if (lstSegments == null || lstSegments.Count == 0)
                        return Notifization.Invalid(NotifizationText.Invalid);
                    //
                    AIRService.WebService.WSOTA_AirBookLLSRQ.OTA_AirBookRS oTA_AirBookRS = vNAWSOTA_AirBookLLSRQSevice.FUNC_OTA_AirBookRS(new AirBookModel
                    {
                        Segments = lstSegments,
                        ConversationID = _conversationId,
                        Token = _token
                    });
                    //
                    if (oTA_AirBookRS.ApplicationResults.Success == null)
                        return Notifization.Invalid(NotifizationText.Invalid);
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
                        return Notifization.Invalid(NotifizationText.Invalid);
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
                                DateOfBirth = iter.DateOfBirth,
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
                   AIRService.WebService.WSPassengerDetailsRQ.PassengerDetailsRS passengerDetailsRS = vNAWSPassengerDetailsRQService.PassengerDetail(passengerDetailModel);
                    //string a = vNAWSPassengerDetailsRQService.PassengerDetail(passengerDetailModel);
                    // end  transaction
                    VNATransaction vNATransaction = new VNATransaction();
                    //vNATransaction.EndTransaction(tokenModel);
                    var End = vNATransaction.EndTransaction(_session);
                    End = vNATransaction.EndTransaction(_session);
                    //  retur value
                    return Notifization.Data("OK", passengerDetailsRS);
                //}
            }
            catch (Exception ex)
            {
                return Notifization.TEST("OK" + ex);
            }
        }
    }
}
