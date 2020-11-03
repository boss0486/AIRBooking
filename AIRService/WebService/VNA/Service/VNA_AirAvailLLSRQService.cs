using AIRService.Entities;
using AIRService.WS.Helper;
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
        public AIRService.WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRS FUNC_OTA_AirAvailLLSRQ(AirAvailLLSRQModel model)
        {
            WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQ oTA_AirAvailRQ = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQ();
            WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailPortTypeClient client = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailPortTypeClient();
            // header info
            WebService.VNA_OTA_AirAvailLLSRQ.MessageHeader messageHeader = new WebService.VNA_OTA_AirAvailLLSRQ.MessageHeader
            {
                MessageData = new WebService.VNA_OTA_AirAvailLLSRQ.MessageData()
            };
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new WebService.VNA_OTA_AirAvailLLSRQ.Service();
            messageHeader.Action = "OTA_AirAvailLLSRQ";
            messageHeader.From = new WebService.VNA_OTA_AirAvailLLSRQ.From
            {
                PartyId = new WebService.VNA_OTA_AirAvailLLSRQ.PartyId[1]
            };
            var partyID = new WebService.VNA_OTA_AirAvailLLSRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new WebService.VNA_OTA_AirAvailLLSRQ.To
            {
                PartyId = new WebService.VNA_OTA_AirAvailLLSRQ.PartyId[1]
            };
            partyID = new WebService.VNA_OTA_AirAvailLLSRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;

            //  header info
            WebService.VNA_OTA_AirAvailLLSRQ.Security1 security = new WebService.VNA_OTA_AirAvailLLSRQ.Security1
            {
                BinarySecurityToken = model.Token
            };

            //
            oTA_AirAvailRQ.OptionalQualifiers = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOptionalQualifiers
            {
                AdditionalAvailability = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOptionalQualifiersAdditionalAvailability
                {
                    Ind = true
                }
            };
            oTA_AirAvailRQ.OriginDestinationInformation = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformation
            {
                FlightSegment = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformationFlightSegment()
            };
            oTA_AirAvailRQ.OriginDestinationInformation.FlightSegment.DepartureDateTime = model.DepartureDateTime.ToString("MM-dd");

            oTA_AirAvailRQ.OriginDestinationInformation.FlightSegment.OriginLocation = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformationFlightSegmentOriginLocation
            {
                LocationCode = model.OriginLocation
            };

            oTA_AirAvailRQ.OriginDestinationInformation.FlightSegment.DestinationLocation = new WebService.VNA_OTA_AirAvailLLSRQ.OTA_AirAvailRQOriginDestinationInformationFlightSegmentDestinationLocation
            {
                LocationCode = model.DestinationLocation
            };
            var data = client.OTA_AirAvailRQ(ref messageHeader, ref security, oTA_AirAvailRQ);
            return data;
        }

        public XMLObject.AirAvailLLSRQ.OTA_AirAvailRS FUNC_OTA_AirAvailLLSRQ2(AirAvailLLSRQModel model)
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
            stringXML += "<ns:OTA_AirAvailRQ ReturnHostCommand='true'  Version='2.4.0'  xmlns:ns='http://webservices.sabre.com/sabreXML/2011/10' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>";
            stringXML += " <ns:OriginDestinationInformation>";
            stringXML += "        <ns:FlightSegment DepartureDateTime='" + model.DepartureDateTime.ToString("MM-dd") + "' ResBookDesigCode=''>";
            stringXML += "            <ns:DestinationLocation LocationCode='" + model.DestinationLocation + "' />";
            stringXML += "            <ns:OriginLocation LocationCode='" + model.OriginLocation + "' />";
            stringXML += "        </ns:FlightSegment>";
            stringXML += "    </ns:OriginDestinationInformation>";
            stringXML += "</ns:OTA_AirAvailRQ>";


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


        public XMLObject.AirAvailLLSRQ.OTA_AirAvailRS FUNC_OTA_AirAvailLLSRQ3(AirAvailLLSRQModel model)
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

namespace XMLObject.AirAvailLLSRQ
{
    [XmlRoot(ElementName = "HostCommand", Namespace = "http://services.sabre.com/STL/v01")]
    public class HostCommand
    {
        [XmlAttribute(AttributeName = "LNIATA")]
        public string LNIATA { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "SystemSpecificResults", Namespace = "http://services.sabre.com/STL/v01")]
    public class SystemSpecificResults
    {
        [XmlElement(ElementName = "HostCommand", Namespace = "http://services.sabre.com/STL/v01")]
        public HostCommand HostCommand { get; set; }
    }

    [XmlRoot(ElementName = "Success", Namespace = "http://services.sabre.com/STL/v01")]
    public class Success
    {
        [XmlElement(ElementName = "SystemSpecificResults", Namespace = "http://services.sabre.com/STL/v01")]
        public SystemSpecificResults SystemSpecificResults { get; set; }
        [XmlAttribute(AttributeName = "timeStamp")]
        public string TimeStamp { get; set; }
    }

    [XmlRoot(ElementName = "ApplicationResults", Namespace = "http://services.sabre.com/STL/v01")]
    public class ApplicationResults
    {
        [XmlElement(ElementName = "Success", Namespace = "http://services.sabre.com/STL/v01")]
        public Success Success { get; set; }
        [XmlAttribute(AttributeName = "status")]
        public string Status { get; set; }
    }

    [XmlRoot(ElementName = "BookingClassAvail", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class BookingClassAvail
    {
        [XmlAttribute(AttributeName = "AggregatedContent")]
        public string AggregatedContent { get; set; }
        [XmlAttribute(AttributeName = "Availability")]
        public string Availability { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
        [XmlAttribute(AttributeName = "ResBookDesigCode")]
        public string ResBookDesigCode { get; set; }
    }

    [XmlRoot(ElementName = "OperationTime", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OperationTime
    {
        [XmlAttribute(AttributeName = "Fri")]
        public string Fri { get; set; }
        [XmlAttribute(AttributeName = "Mon")]
        public string Mon { get; set; }
        [XmlAttribute(AttributeName = "Sat")]
        public string Sat { get; set; }
        [XmlAttribute(AttributeName = "Sun")]
        public string Sun { get; set; }
        [XmlAttribute(AttributeName = "Thur")]
        public string Thur { get; set; }
        [XmlAttribute(AttributeName = "Tue")]
        public string Tue { get; set; }
        [XmlAttribute(AttributeName = "Weds")]
        public string Weds { get; set; }
    }

    [XmlRoot(ElementName = "OperationTimes", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OperationTimes
    {
        [XmlElement(ElementName = "OperationTime", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OperationTime OperationTime { get; set; }
    }

    [XmlRoot(ElementName = "OperationSchedule", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OperationSchedule
    {
        [XmlElement(ElementName = "OperationTimes", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OperationTimes OperationTimes { get; set; }
    }

    [XmlRoot(ElementName = "DaysOfOperation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class DaysOfOperation
    {
        [XmlElement(ElementName = "OperationSchedule", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OperationSchedule OperationSchedule { get; set; }
    }

    [XmlRoot(ElementName = "DestinationLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class DestinationLocation
    {
        [XmlAttribute(AttributeName = "LocationCode")]
        public string LocationCode { get; set; }
    }

    [XmlRoot(ElementName = "Equipment", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class Equipment
    {
        [XmlAttribute(AttributeName = "AirEquipType")]
        public string AirEquipType { get; set; }
    }

    [XmlRoot(ElementName = "FlightDetails", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class FlightDetails
    {
        [XmlAttribute(AttributeName = "Canceled")]
        public string Canceled { get; set; }
        [XmlAttribute(AttributeName = "Charter")]
        public string Charter { get; set; }
        [XmlAttribute(AttributeName = "GroundTime")]
        public string GroundTime { get; set; }
        [XmlAttribute(AttributeName = "TotalTravelTime")]
        public string TotalTravelTime { get; set; }
        [XmlAttribute(AttributeName = "CodeshareBlockDisplay")]
        public string CodeshareBlockDisplay { get; set; }
    }

    [XmlRoot(ElementName = "MarketingAirline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class MarketingAirline
    {
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "FlightNumber")]
        public string FlightNumber { get; set; }
    }

    [XmlRoot(ElementName = "OriginLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OriginLocation
    {
        [XmlAttribute(AttributeName = "LocationCode")]
        public string LocationCode { get; set; }
    }

    [XmlRoot(ElementName = "FlightSegment", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class FlightSegment
    {
        [XmlElement(ElementName = "BookingClassAvail", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<BookingClassAvail> BookingClassAvail { get; set; }
        [XmlElement(ElementName = "DaysOfOperation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public DaysOfOperation DaysOfOperation { get; set; }
        [XmlElement(ElementName = "DestinationLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public DestinationLocation DestinationLocation { get; set; }
        [XmlElement(ElementName = "Equipment", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public Equipment Equipment { get; set; }
        [XmlElement(ElementName = "FlightDetails", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public FlightDetails FlightDetails { get; set; }
        [XmlElement(ElementName = "MarketingAirline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public MarketingAirline MarketingAirline { get; set; }
        [XmlElement(ElementName = "OriginLocation", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OriginLocation OriginLocation { get; set; }
        [XmlAttribute(AttributeName = "ArrivalDateTime")]
        public string ArrivalDateTime { get; set; }
        [XmlAttribute(AttributeName = "DOT_Ind")]
        public string DOT_Ind { get; set; }
        [XmlAttribute(AttributeName = "DepartureDateTime")]
        public string DepartureDateTime { get; set; }
        [XmlAttribute(AttributeName = "FlightNumber")]
        public string FlightNumber { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
        [XmlAttribute(AttributeName = "SmokingAllowed")]
        public string SmokingAllowed { get; set; }
        [XmlAttribute(AttributeName = "StopQuantity")]
        public string StopQuantity { get; set; }
        [XmlAttribute(AttributeName = "eTicket")]
        public string ETicket { get; set; }
        [XmlElement(ElementName = "Meal", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<Meal> Meal { get; set; }
        [XmlElement(ElementName = "DisclosureAirline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public DisclosureAirline DisclosureAirline { get; set; }
    }

    [XmlRoot(ElementName = "OriginDestinationOption", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OriginDestinationOption
    {
        [XmlElement(ElementName = "FlightSegment", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public FlightSegment FlightSegment { get; set; }
        [XmlAttribute(AttributeName = "RPH")]
        public string RPH { get; set; }
    }

    [XmlRoot(ElementName = "Meal", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class Meal
    {
        [XmlAttribute(AttributeName = "MealCode")]
        public string MealCode { get; set; }
    }

    [XmlRoot(ElementName = "DisclosureAirline", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class DisclosureAirline
    {
        [XmlElement(ElementName = "Text", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "Code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "CompanyShortName")]
        public string CompanyShortName { get; set; }
    }

    [XmlRoot(ElementName = "OriginDestinationOptions", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OriginDestinationOptions
    {
        [XmlElement(ElementName = "OriginDestinationOption", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public List<OriginDestinationOption> OriginDestinationOption { get; set; }
        [XmlAttribute(AttributeName = "OriginTimeZone")]
        public string OriginTimeZone { get; set; }
        [XmlAttribute(AttributeName = "TimeZoneDifference")]
        public string TimeZoneDifference { get; set; }
    }

    [XmlRoot(ElementName = "OTA_AirAvailRS", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
    public class OTA_AirAvailRS
    {
        [XmlElement(ElementName = "ApplicationResults", Namespace = "http://services.sabre.com/STL/v01")]
        public ApplicationResults ApplicationResults { get; set; }
        [XmlElement(ElementName = "OriginDestinationOptions", Namespace = "http://webservices.sabre.com/sabreXML/2011/10")]
        public OriginDestinationOptions OriginDestinationOptions { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "xs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xs { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "stl", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Stl { get; set; }
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }
    }

}
