using AIRService.WS.Helper;
using ApiPortalBooking.Models;
using ApiPortalBooking.Models.VNA_WS_Model.VNA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace AIRService.WS.Service
{
    class VNAWSGetReservationRQService
    {
        public ApiPortalBooking.Models.VNA_WS_Model.VNA.GetReservationData GetReservation(GetReservationModel model)
        {
            #region xml
            ApiPortalBooking.Models.VNA_WS_Model.VNA.GetReservationData data = null;
            //ReservationModel result;
            //try
            //{

                HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
                soapEnvelopeXml.Load(path);
                soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
                soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "GetReservationRQ";
                soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "GetReservationRQ";
                soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
                soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
                XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
                var stringXML = "";
                stringXML += " <ns7:GetReservationRQ xmlns:ns7=\"http://webservices.sabre.com/pnrbuilder/v1_19\" Version=\"1.19.0\">";
                stringXML += "    <ns7:Locator>" + model.PNR + "</ns7:Locator>";
                //stringXML += "    <ns7:RequestType>Stateful</ns7:RequestType>";
                //stringXML += "    <ns7:ReturnOptions  ShowTicketStatus=\"true\" PriceQuoteServiceVersion=\"3.2.0\">";
                //stringXML += "       <ns7:SubjectAreas>";
                //stringXML += "           <ns7:SubjectArea>PRICE_QUOTE</ns7:SubjectArea>";
                //stringXML += "       </ns7:SubjectAreas>";
                //stringXML += "       <ns7:ViewName>Simple</ns7:ViewName>";
                //stringXML += "       <ns7:ResponseFormat>STL</ns7:ResponseFormat>";
                //stringXML += "    </ns7:ReturnOptions>";

                stringXML += " </ns7:GetReservationRQ>";
                child.InnerXml = stringXML;
                soapEnvelopeXml.GetElementsByTagName("soapenv:Body")[0].AppendChild(child);

                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string soapResult = rd.ReadToEnd();
                        soapEnvelopeXml = new XmlDocument();
                        soapEnvelopeXml.LoadXml(soapResult);
                        var xmlnode = soapEnvelopeXml.GetElementsByTagName("stl19:TicketDetails");
                        data = new ApiPortalBooking.Models.VNA_WS_Model.VNA.GetReservationData();
                        data.RevResult = new ApiPortalBooking.Models.VNA_WS_Model.RevResult();
                        data.RevResult.AgentDutyCode = soapEnvelopeXml.GetElementsByTagName("stl19:Source")[0].Attributes["AgentDutyCode"].Value;
                        data.RevResult.BookingSource = soapEnvelopeXml.GetElementsByTagName("stl19:Source")[0].Attributes["BookingSource"].Value;
                        data.RevResult.AgentSine = soapEnvelopeXml.GetElementsByTagName("stl19:Source")[0].Attributes["AgentSine"].Value;
                        data.RevResult.PseudoCityCode = soapEnvelopeXml.GetElementsByTagName("stl19:Source")[0].Attributes["PseudoCityCode"].Value;
                        data.RevResult.ISOCountry = soapEnvelopeXml.GetElementsByTagName("stl19:Source")[0].Attributes["ISOCountry"].Value;
                        data.RevResult.AirlineVendorID = soapEnvelopeXml.GetElementsByTagName("stl19:Source")[0].Attributes["AirlineVendorID"].Value;
                        data.RevResult.HomePseudoCityCode = soapEnvelopeXml.GetElementsByTagName("stl19:Source")[0].Attributes["HomePseudoCityCode"].Value;
                        data.RevResult.PrimeHostID = soapEnvelopeXml.GetElementsByTagName("stl19:Source")[0].Attributes["PrimeHostID"].Value;
                        var OAC = soapEnvelopeXml.GetElementsByTagName("stl19:OAC")[0];
                        data.RevResult.OAC = new ApiPortalBooking.Models.VNA_WS_Model.RevResultOAC();
                        foreach (XmlNode item in OAC.ChildNodes)
                        {
                            if (item.Name.Equals("stl19:PartitionId"))
                            {
                                data.RevResult.OAC.PartitionId = item.InnerText;
                                continue;
                            }
                            if (item.Name.Equals("stl19:AccountingCityCode"))
                            {
                                data.RevResult.OAC.AccountingCityCode = item.InnerText;
                                continue;
                            }
                            if (item.Name.Equals("stl19:AccountingCode"))
                            {
                                data.RevResult.OAC.AccountingCode = item.InnerText;
                                continue;
                            }
                            if (item.Name.Equals("stl19:AccountingOfficeStationCode"))
                            {
                                data.RevResult.OAC.AccountingOfficeStationCode = item.InnerText;
                                continue;
                            }
                        }
                        if (xmlnode != null && xmlnode.Count > 0)
                        {
                            data.TicketDetails = new List<TicketingInfoTicketDetails>();
                            foreach (XmlNode item in xmlnode)
                            {
                                var details = new TicketingInfoTicketDetails();
                                details.id = item.Attributes["id"].Value != null ? int.Parse(item.Attributes["id"].Value) : 0;
                                details.index = item.Attributes["index"].Value != null ? int.Parse(item.Attributes["index"].Value) : 0;
                                details.elementId = item.Attributes["elementId"].Value != null ? item.Attributes["elementId"].Value : "";
                                foreach (XmlNode item2 in item.ChildNodes)
                                {
                                    if (item2.Name.Equals("stl19:OriginalTicketDetails"))
                                    {
                                        details.OriginalTicketDetails = item2.InnerText;
                                        continue;
                                    }
                                    if (item2.Name.Equals("stl19:TransactionIndicator"))
                                    {
                                        details.TransactionIndicator = item2.InnerText;
                                        continue;
                                    }
                                    if (item2.Name.Equals("stl19:TicketNumber"))
                                    {
                                        details.TicketNumber = item2.InnerText;
                                        continue;
                                    }
                                    if (item2.Name.Equals("stl19:PassengerName"))
                                    {
                                        details.PassengerName = item2.InnerText;
                                        continue;
                                    }
                                    if (item2.Name.Equals("stl19:AgencyLocation"))
                                    {
                                        details.AgencyLocation = item2.InnerText;
                                        continue;
                                    }
                                    if (item2.Name.Equals("stl19:DutyCode"))
                                    {
                                        details.DutyCode = item2.InnerText;
                                        continue;
                                    }
                                    if (item2.Name.Equals("stl19:AgentSine"))
                                    {
                                        details.AgentSine = item2.InnerText;
                                        continue;
                                    }
                                    if (item2.Name.Equals("stl19:Timestamp"))
                                    {
                                        details.Timestamp = DateTime.Parse(item2.InnerText);
                                        continue;
                                    }
                                }
                                data.TicketDetails.Add(details);
                            }
                        }
                    }
                }
                return data;
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}
            #endregion
        }
    }
}
