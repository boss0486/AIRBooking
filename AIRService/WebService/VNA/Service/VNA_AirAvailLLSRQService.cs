using AIRService.Entities;
using AIRService.WS.VNAHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Serialization;
using WebCore.Entities;
//
namespace AIRService.WS.Service
{
    public class VNA_AirAvailLLSRQService
    {

        public XMLObject.AirAvailLLSRQ.OTA_AirAvailRS FUNC_OTA_AirAvailLLSRQ(AirAvailLLSRQModel model)
        {

            HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            soapEnvelopeXml.Load(path);
            soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "OTA_AirAvailLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "OTA_AirAvailLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
            XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
            var stringXML = "";
            stringXML += "<OTA_AirAvailRQ ReturnHostCommand='true'  Version='2.4.0'  xmlns='http://webservices.sabre.com/sabreXML/2011/10' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>";
            stringXML += "  <OptionalQualifiers>";
            //stringXML += "  <AdditionalAvailability AirExtras='true' DirectAccess='true'/>";
            stringXML += "      <FlightQualifiers>";
            //stringXML += "          <VendorPrefs DirectAccess='true'><Airline Code=\"VN\"/></VendorPrefs>";
            stringXML += "      </FlightQualifiers>";
            stringXML += "  </OptionalQualifiers>";
            stringXML += "  <OriginDestinationInformation>";
            stringXML += "        <FlightSegment DepartureDateTime='" + model.DepartureDateTime.ToString("MM-dd") + "' ResBookDesigCode=''>";
            stringXML += "            <DestinationLocation LocationCode='" + model.DestinationLocation + "' />";
            stringXML += "            <OriginLocation LocationCode='" + model.OriginLocation + "' />";
            stringXML += "        </FlightSegment>";
            stringXML += "  </OriginDestinationInformation>";
            stringXML += "</OTA_AirAvailRQ>";
            //
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
                    // 
                    XMLObject.AirAvailLLSRQ.OTA_AirAvailRS ota_AirAvailRS = new XMLObject.AirAvailLLSRQ.OTA_AirAvailRS();
                    XmlNode xmlnode = soapEnvelopeXml.GetElementsByTagName("soap-env:Body")[0];
                    if (xmlnode != null)
                        ota_AirAvailRS = XMLHelper.Deserialize<XMLObject.AirAvailLLSRQ.OTA_AirAvailRS>(xmlnode.InnerXml);
                    //
                    return ota_AirAvailRS;
                }
            }
        }


        public XMLObject.AirAvailLLSRQ.OTA_AirAvailRS FUNC_OTA_AirAvailLLSRQLoop(AirAvailLLSRQModel model)
        {
            #region xml
            //ReservationModel result;
            //try
            //{
            HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            soapEnvelopeXml.Load(path);
            soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "OTA_AirAvailLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "OTA_AirAvailLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
            XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
            var stringXML = "";

            stringXML += "<ns:OTA_AirAvailRQ ReturnHostCommand='true' Version='2.4.0'  xmlns:ns='http://webservices.sabre.com/sabreXML/2011/10' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>";
            stringXML += "	<ns:OptionalQualifiers>";
            stringXML += "		<ns:AdditionalAvailability Ind='true' />";
            stringXML += "	</ns:OptionalQualifiers>";
            stringXML += "</ns:OTA_AirAvailRQ>";
            //
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
                    XMLHelper.WriteXml($"{model.DestinationLocation}-{model.OriginLocation}-segment.xml", soapEnvelopeXml);
                    //
                    XMLObject.AirAvailLLSRQ.OTA_AirAvailRS ota_AirAvailRS = new XMLObject.AirAvailLLSRQ.OTA_AirAvailRS();
                    XmlNode xmlnode = soapEnvelopeXml.GetElementsByTagName("soap-env:Body")[0];
                    if (xmlnode != null)
                        ota_AirAvailRS = XMLHelper.Deserialize<XMLObject.AirAvailLLSRQ.OTA_AirAvailRS>(xmlnode.InnerXml);
                    //
                    return ota_AirAvailRS;
                }
            }
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}
            #endregion
        }
        public XMLObject.AirAvailLLSRQ.OTA_AirAvailRS FUNC_OTA_AirBookLLSRQ(AirAvailLLSRQModel model)
        {
            #region xml
            //ReservationModel result;
            //try
            //{
            HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            soapEnvelopeXml.Load(path);
            soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "OTA_AirBookLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "OTA_AirBookLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
            XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
            var stringXML = ""; 
            stringXML += $"<OTA_AirBookRQ Version='2.2.0' xmlns='http://webservices.sabre.com/sabreXML/2011/10' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>";
            stringXML += $"    <OriginDestinationInformation>";
            stringXML += $"        <FlightSegment DepartureDateTime='2019-12-21T12:25' ArrivalDateTime='2019-12-21T13:25' FlightNumber='1717' NumberInParty='2' ResBookDesigCode='Y' Status='NN'>";
            stringXML += $"            <DestinationLocation LocationCode='{model.DestinationLocation}' />";
            stringXML += $"            <Equipment AirEquipType='757' />";
            stringXML += $"            <MarketingAirline Code='AA' FlightNumber='1717' />";
            stringXML += $"            <OperatingAirline Code='AA' />";
            stringXML += $"            <OriginLocation LocationCode='{model.OriginLocation}' />";
            stringXML += $"        </FlightSegment>";
            stringXML += $"        <FlightSegment DepartureDateTime='2019-12-24T11:10' ArrivalDateTime='2019-12-24T15:50' FlightNumber='1174' NumberInParty='2' ResBookDesigCode='Y' Status='NN'>";
            stringXML += $"            <DestinationLocation LocationCode='{model.OriginLocation}' />";
            stringXML += $"            <Equipment AirEquipType='757' />";
            stringXML += $"            <MarketingAirline Code='AA' FlightNumber='1174' />";
            stringXML += $"            <OperatingAirline Code='AA' />";
            stringXML += $"            <OriginLocation LocationCode='{model.DestinationLocation}' />";
            stringXML += $"        </FlightSegment>";
            stringXML += $"    </OriginDestinationInformation>";
            stringXML += $"</OTA_AirBookRQ>";


            //
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
                    //
                    XMLObject.AirAvailLLSRQ.OTA_AirAvailRS ota_AirAvailRS = new XMLObject.AirAvailLLSRQ.OTA_AirAvailRS();
                    XmlNode xmlnode = soapEnvelopeXml.GetElementsByTagName("soap-env:Body")[0];
                    if (xmlnode != null)
                        ota_AirAvailRS = XMLHelper.Deserialize<XMLObject.AirAvailLLSRQ.OTA_AirAvailRS>(xmlnode.InnerXml);
                    //
                    return ota_AirAvailRS;
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

}


