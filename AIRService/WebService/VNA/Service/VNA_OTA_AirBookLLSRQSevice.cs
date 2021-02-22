using AIRService.WS.Entities;
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

namespace AIRService.WS.Service
{
    public class VNA_OTA_AirBookLLSRQSevice
    {
        public XMLObject.AirOTA_AirBookRS.OTA_AirBookRS FUNC_OTA_AirBookRS(AirBookModel model)
        {
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
            stringXML += "<OTA_AirBookRQ Version=\"2.2.0\" xmlns=\"http://webservices.sabre.com/sabreXML/2011/10\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">";
            stringXML += "    <OriginDestinationInformation>";
            foreach (var item in model.Segments)
            {
                stringXML += $"        <FlightSegment DepartureDateTime=\"{item.DepartureDateTime:yyyy-MM-dd'T'HH:mm}\" ArrivalDateTime=\"{item.ArrivalDateTime:yyyy-MM-dd'T'HH:mm}\" FlightNumber=\"{item.FlightNumber}\" NumberInParty=\"{item.NumberInParty}\" ResBookDesigCode=\"{item.ResBookDesigCode}\" Status=\"NN\">";
                stringXML += $"            <DestinationLocation LocationCode=\"{item.DestinationLocation}\" />";
                stringXML += $"            <Equipment AirEquipType=\"{item.AirEquipType}\" />";
                stringXML += $"            <MarketingAirline Code=\"VN\" FlightNumber=\"{item.FlightNumber}\" />";
                stringXML += $"            <OperatingAirline Code=\"VN\" />";
                stringXML += $"            <OriginLocation LocationCode=\"{item.OriginLocation}\" />";
                stringXML += $"        </FlightSegment>";
            }
            stringXML += "    </OriginDestinationInformation>";
            stringXML += "</OTA_AirBookRQ>";
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
                    //XMLHelper.WriteXml("chua-xuat-ve.xml", soapEnvelopeXml);
                    XmlNode xmlnode = soapEnvelopeXml.GetElementsByTagName("soap-env:Body")[0];
                    XMLObject.AirOTA_AirBookRS.OTA_AirBookRS airBookRS = new XMLObject.AirOTA_AirBookRS.OTA_AirBookRS();
                    if (xmlnode != null)
                        airBookRS = XMLHelper.Deserialize<XMLObject.AirOTA_AirBookRS.OTA_AirBookRS>(xmlnode.InnerXml);
                    //
                    return airBookRS;
                }
            }
        }
    }
}
