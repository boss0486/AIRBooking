using AIRService.Models;
using AIRService.WebService.VNA.Authen;
using AIRService.WS.VNAHelper;
using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace AIRService.Service
{
    class VNA_OTA_AirTaxRQService
    {
        public AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRS AirTax(TokenModel token, Resquet_WsTaxModel model)
        {



            WebService.VNA_OTA_AirTaxRQ.MessageHeader messageHeader = new WebService.VNA_OTA_AirTaxRQ.MessageHeader
            {
                MessageData = new WebService.VNA_OTA_AirTaxRQ.MessageData()
            };
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = token.ConversationID;
            messageHeader.Service = new WebService.VNA_OTA_AirTaxRQ.Service();
            messageHeader.Action = "OTA_AirTaxRQ";
            messageHeader.From = new WebService.VNA_OTA_AirTaxRQ.From
            {
                PartyId = new WebService.VNA_OTA_AirTaxRQ.PartyId[1]
            };
            var partyID = new WebService.VNA_OTA_AirTaxRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new WebService.VNA_OTA_AirTaxRQ.To
            {
                PartyId = new WebService.VNA_OTA_AirTaxRQ.PartyId[1]
            };
            partyID = new WebService.VNA_OTA_AirTaxRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;
            //  header info
            WebService.VNA_OTA_AirTaxRQ.Security security = new WebService.VNA_OTA_AirTaxRQ.Security
            {
                BinarySecurityToken = token.Token
            };
            AIRService.WebService.VNA_OTA_AirTaxRQ.TaxPortTypeClient taxPortTypeClient = new AIRService.WebService.VNA_OTA_AirTaxRQ.TaxPortTypeClient();
            ////////  
            AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQ _airTaxRQ = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQ();
            _airTaxRQ.ItineraryInfos = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfo[1];
            //
            var _itineraryInfo = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfo();
            _itineraryInfo.ReservationItems = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItems();
            _itineraryInfo.ReservationItems.Item = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItem();
            _itineraryInfo.ReservationItems.Item.RPH = (Int16)model.BaseFare.RPH;
            _itineraryInfo.ReservationItems.Item.SalePseudoCityCode = "LZJ";
            _itineraryInfo.ReservationItems.Item.TicketingCarrier = "VN";
            _itineraryInfo.ReservationItems.Item.ValidatingCarrier = "VN";
            //
            _airTaxRQ.POS = new WebService.VNA_OTA_AirTaxRQ.AirTaxRQPOS();
            _airTaxRQ.POS.Source = new WebService.VNA_OTA_AirTaxRQ.AirTaxRQPOSSource();
            _airTaxRQ.POS.Source.PseudoCityCode = "LZJ";
            //
            _itineraryInfo.ReservationItems.Item.FlightSegment = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegment[1];
            //
            var flightSegment = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegment();
            flightSegment.DepartureDateTime = model.DepartureDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            flightSegment.ArrivalDateTime = model.ArrivalDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            //
            flightSegment.FlightNumber = (Int16)model.FlightNumber;
            flightSegment.ResBookDesigCode = model.ResBookDesigCode;
            flightSegment.ForceConnectionInd = true;
            flightSegment.ForceStopOverInd = true;
            //
            flightSegment.DepartureAirport = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentDepartureAirport();
            flightSegment.DepartureAirport.CodeContext = "IATA";
            flightSegment.DepartureAirport.LocationCode = model.OriginLocation;
            //
            flightSegment.ArrivalAirport = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentArrivalAirport();
            flightSegment.ArrivalAirport.CodeContext = "IATA";
            flightSegment.ArrivalAirport.LocationCode = model.DestinationLocation;
            //
            flightSegment.Equipment = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentEquipment();
            flightSegment.Equipment.AirEquipType = Convert.ToString(model.AirEquipType);
            flightSegment.MarketingAirline = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentMarketingAirline();
            flightSegment.MarketingAirline.Code = "VN";
            _itineraryInfo.ReservationItems.Item.FlightSegment[0] = flightSegment;
            //
            _itineraryInfo.ReservationItems.Item.AirFareInfo = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfo();
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdown();
            //
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerType = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerType();
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerType.Code = model.BaseFare.PassengerType;
            //
            //_itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.FareBasisCode = model.FareBasisCode;
            //
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerFare();
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerFareBaseFare();
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.Amount = (float)model.BaseFare.Amount;
            _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.CurrencyCode = model.BaseFare.CurrencyCode;
            //
            _airTaxRQ.ItineraryInfos[0] = _itineraryInfo;
            var data = taxPortTypeClient.TaxRQ(ref messageHeader, ref security, _airTaxRQ);
            return data;

        }
        // ######################################################################################################################################################################
        public List<AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRS> AirTaxList(TokenModel token, List<Resquet_WsTaxModel> models)
        {
            WebService.VNA_OTA_AirTaxRQ.MessageHeader messageHeader = new WebService.VNA_OTA_AirTaxRQ.MessageHeader
            {
                MessageData = new WebService.VNA_OTA_AirTaxRQ.MessageData()
            };
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = token.ConversationID;
            messageHeader.Service = new WebService.VNA_OTA_AirTaxRQ.Service();
            messageHeader.Action = "OTA_AirTaxRQ";
            messageHeader.From = new WebService.VNA_OTA_AirTaxRQ.From
            {
                PartyId = new WebService.VNA_OTA_AirTaxRQ.PartyId[1]
            };
            var partyID = new WebService.VNA_OTA_AirTaxRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new WebService.VNA_OTA_AirTaxRQ.To
            {
                PartyId = new WebService.VNA_OTA_AirTaxRQ.PartyId[1]
            };
            partyID = new WebService.VNA_OTA_AirTaxRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;
            //  header info
            WebService.VNA_OTA_AirTaxRQ.Security security = new WebService.VNA_OTA_AirTaxRQ.Security
            {
                BinarySecurityToken = token.Token
            };
            AIRService.WebService.VNA_OTA_AirTaxRQ.TaxPortTypeClient taxPortTypeClient = new AIRService.WebService.VNA_OTA_AirTaxRQ.TaxPortTypeClient();
            ////////  
            AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQ _airTaxRQ = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQ();
            _airTaxRQ.ItineraryInfos = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfo[models.Count];
            int cnt = 0;

            List<AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRS> airTaxRS = new List<WebService.VNA_OTA_AirTaxRQ.AirTaxRS>();
            var _itineraryInfo = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfo();
            _itineraryInfo.ReservationItems = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItems();
            _itineraryInfo.ReservationItems.Item = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItem();
            _itineraryInfo.ReservationItems.Item.FlightSegment = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegment[models.Count];
            foreach (var model in models)
            {

                _itineraryInfo.ReservationItems.Item.SalePseudoCityCode = "LZJ";
                _itineraryInfo.ReservationItems.Item.TicketingCarrier = "VN";
                _itineraryInfo.ReservationItems.Item.ValidatingCarrier = "VN";
                //
                _airTaxRQ.POS = new WebService.VNA_OTA_AirTaxRQ.AirTaxRQPOS();
                _airTaxRQ.POS.Source = new WebService.VNA_OTA_AirTaxRQ.AirTaxRQPOSSource();
                _airTaxRQ.POS.Source.PseudoCityCode = "LZJ";
                // 
                //
                _itineraryInfo.ReservationItems.Item.RPH = (Int16)model.RPH;
                var flightSegment = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegment();
                flightSegment.DepartureDateTime = model.DepartureDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
                flightSegment.ArrivalDateTime = model.ArrivalDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
                //
                flightSegment.FlightNumber = (Int16)model.FlightNumber;
                flightSegment.ResBookDesigCode = model.ResBookDesigCode;
                flightSegment.ForceConnectionInd = true;
                flightSegment.ForceStopOverInd = true;
                //
                flightSegment.DepartureAirport = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentDepartureAirport();
                flightSegment.DepartureAirport.CodeContext = "IATA";
                flightSegment.DepartureAirport.LocationCode = model.OriginLocation;
                //
                flightSegment.ArrivalAirport = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentArrivalAirport();
                flightSegment.ArrivalAirport.CodeContext = "IATA";
                flightSegment.ArrivalAirport.LocationCode = model.DestinationLocation;
                //
                flightSegment.Equipment = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentEquipment();
                flightSegment.Equipment.AirEquipType = Convert.ToString(model.AirEquipType);
                flightSegment.MarketingAirline = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemFlightSegmentMarketingAirline();
                flightSegment.MarketingAirline.Code = "VN";
                _itineraryInfo.ReservationItems.Item.FlightSegment[cnt] = flightSegment;
                //
                _itineraryInfo.ReservationItems.Item.AirFareInfo = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfo();
                _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdown();
                //
                _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerType = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerType();
                _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerType.Code = model.BaseFare.PassengerType;
                //
                //_itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.FareBasisCode = model.FareBasisCode;
                //
                _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerFare();
                _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare = new AIRService.WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfoReservationItemsItemAirFareInfoPTC_FareBreakdownPassengerFareBaseFare();
                _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.Amount = (float)model.BaseFare.Amount;
                _itineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.CurrencyCode = model.BaseFare.CurrencyCode;
                //
                _airTaxRQ.ItineraryInfos[cnt] = _itineraryInfo;
                var data = taxPortTypeClient.TaxRQ(ref messageHeader, ref security, _airTaxRQ);
                airTaxRS.Add(data);
                cnt++;
            }
            return airTaxRS;
        }
        // ######################################################################################################################################################################


        public TaxModel AirTax(TokenModel model, WebService.VNA_OTA_AirTaxRQ.AirTaxRQItineraryInfo AirTaxRQItineraryInfo)
        {

            #region xml request
            HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            soapEnvelopeXml.Load(path);
            soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "OTA_AirTaxRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "OTA_AirTaxRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
            XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
            var stringXML = "";
            #region xml
            stringXML += " <AirTaxRQ xmlns=\"http://webservices.sabre.com/sabreXML/2003/07\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" Version=\"2.0.2\"> ";
            stringXML += "     <POS>";
            stringXML += "        <Source PseudoCityCode=\"LZJ\"/>";
            stringXML += "     </POS>";
            stringXML += "     <ItineraryInfos>";
            stringXML += "        <ItineraryInfo>";
            stringXML += "           <ReservationItems>";
            stringXML += "              <Item RPH=\"1\" TicketingCarrier=\"VN\" ValidatingCarrier=\"VN\" SalePseudoCityCode=\"LZJ\">";
            stringXML += "                 <FlightSegment DepartureDateTime=\"" + AirTaxRQItineraryInfo.ReservationItems.Item.FlightSegment[0].DepartureDateTime + "\" ArrivalDateTime=\"" + AirTaxRQItineraryInfo.ReservationItems.Item.FlightSegment[0].ArrivalDateTime + "\" FlightNumber=\"" + AirTaxRQItineraryInfo.ReservationItems.Item.FlightSegment[0].FlightNumber + "\" ResBookDesigCode=\"" + AirTaxRQItineraryInfo.ReservationItems.Item.FlightSegment[0].ResBookDesigCode + "\"> ";
            stringXML += "                    <DepartureAirport LocationCode=\"" + AirTaxRQItineraryInfo.ReservationItems.Item.FlightSegment[0].DepartureAirport.LocationCode + "\" CodeContext=\"IATA\"/>";
            stringXML += "                    <ArrivalAirport LocationCode=\"" + AirTaxRQItineraryInfo.ReservationItems.Item.FlightSegment[0].DepartureAirport.LocationCode + "\" CodeContext=\"IATA\"/>";
            stringXML += "                    <Equipment AirEquipType=\"" + AirTaxRQItineraryInfo.ReservationItems.Item.FlightSegment[0].Equipment.AirEquipType + "\"/>";
            stringXML += "                    <MarketingAirline Code=\"VN\"/>";
            stringXML += "                    <OperatingAirline Code=\"VN\"/>";
            stringXML += "                 </FlightSegment>";
            stringXML += "                 <AirFareInfo>";
            stringXML += "                    <PTC_FareBreakdown>";
            stringXML += "                       <PassengerType Code=\"" + AirTaxRQItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerType.Code + "\"/>";
            stringXML += "                       <FareBasisCode>" + AirTaxRQItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.FareBasisCode + "</FareBasisCode>";
            stringXML += "                       <PassengerFare>";
            stringXML += "                          <BaseFare Amount=\"" + AirTaxRQItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.Amount + "\" CurrencyCode=\"" + AirTaxRQItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.CurrencyCode + "\"/>";
            stringXML += "                       </PassengerFare>";
            stringXML += "                    </PTC_FareBreakdown>";
            stringXML += "                 </AirFareInfo>";
            stringXML += "              </Item>";
            stringXML += "           </ReservationItems>";
            stringXML += "        </ItineraryInfo>";
            stringXML += "     </ItineraryInfos>";
            stringXML += "  </AirTaxRQ>";
            child.InnerXml = stringXML;
            #endregion
            soapEnvelopeXml.GetElementsByTagName("soapenv:Body")[0].AppendChild(child);

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
            //try
            //{
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();
                    soapEnvelopeXml = new XmlDocument();
                    soapEnvelopeXml.LoadXml(soapResult);
                    var data = new TaxModel
                    {
                        Total = VNALibrary.ConvertToDouble(soapEnvelopeXml.GetElementsByTagName("TaxInfo")[0].Attributes["Total"].Value),
                        PassengerType = AirTaxRQItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerType.Code,
                        FareAmmout = AirTaxRQItineraryInfo.ReservationItems.Item.AirFareInfo.PTC_FareBreakdown.PassengerFare.BaseFare.Amount
                    };
                    var listXmlNode = soapEnvelopeXml.GetElementsByTagName("Taxes");

                    foreach (XmlNode node in listXmlNode)
                    {
                        foreach (XmlNode item in node)
                        {
                            if (item.Attributes["TaxCode"].Value == "YRI")
                            {
                                data.ServiceFee = VNALibrary.ConvertToDouble(item.Attributes["Amount"].Value);
                                continue;
                            }
                            if (item.Attributes["TaxCode"].Value == "UE3")
                            {
                                data.Tax = VNALibrary.ConvertToDouble(item.Attributes["Amount"].Value);
                                continue;
                            }
                            if (item.Attributes["TaxCode"].Value == "AX")
                            {
                                data.ChargeDomesticFee = VNALibrary.ConvertToDouble(item.Attributes["Amount"].Value);
                                continue;
                            }
                            if (item.Attributes["TaxCode"].Value == "C4")
                            {
                                data.ScreeningFee = VNALibrary.ConvertToDouble(item.Attributes["Amount"].Value);
                                continue;
                            }
                        }
                    }
                    return data;
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            #endregion


        }
    }
    public class TaxModel
    {
        public string PassengerType { get; set; }
        public double Total { get; set; }
        public double ServiceFee { get; set; }
        public double Tax { get; set; }//CHARGE DOMESTIC
        public double ChargeDomesticFee { get; set; }
        public double ScreeningFee { get; set; }
        public double FareAmmout { get; set; }
    }
}
