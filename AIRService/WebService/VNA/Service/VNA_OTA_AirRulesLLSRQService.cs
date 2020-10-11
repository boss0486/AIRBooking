using AIRService.Entities;
using AIRService.WS.Helper;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
//
namespace AIRService.WS.Service
{
    public class VNA_OTA_AirRulesLLSRQService
    {
        //public AIRService.WebService.VNA_OTA_AirRulesLLSRQ.OTA_AirRulesRS GetOTA_AirRulesLLSRQ(VNA_OTA_AirRulesLLSRQ model)
        //{
        //    WebService.VNA_OTA_AirRulesLLSRQ.OTA_AirRulesRQ oTA_AirRules = new WebService.VNA_OTA_AirRulesLLSRQ.OTA_AirRulesRQ();
        //    WebService.VNA_OTA_AirRulesLLSRQ.OTA_AirRulesPortTypeClient client = new WebService.VNA_OTA_AirRulesLLSRQ.OTA_AirRulesPortTypeClient();
        //    // header info
        //    WebService.VNA_OTA_AirRulesLLSRQ.MessageHeader messageHeader = new WebService.VNA_OTA_AirRulesLLSRQ.MessageHeader
        //    {
        //        MessageData = new WebService.VNA_OTA_AirRulesLLSRQ.MessageData()
        //    };
        //    messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
        //    messageHeader.ConversationId = model.ConversationID;
        //    messageHeader.Service = new WebService.VNA_OTA_AirRulesLLSRQ.Service();
        //    messageHeader.Action = "OTA_AirRulesLLSRQ";
        //    messageHeader.From = new WebService.VNA_OTA_AirRulesLLSRQ.From
        //    {
        //        PartyId = new WebService.VNA_OTA_AirRulesLLSRQ.PartyId[1]
        //    };
        //    var partyID = new WebService.VNA_OTA_AirRulesLLSRQ.PartyId
        //    {
        //        Value = "WebServiceClient"
        //    };
        //    messageHeader.From.PartyId[0] = partyID;

        //    messageHeader.To = new WebService.VNA_OTA_AirRulesLLSRQ.To
        //    {
        //        PartyId = new WebService.VNA_OTA_AirRulesLLSRQ.PartyId[1]
        //    };
        //    partyID = new WebService.VNA_OTA_AirRulesLLSRQ.PartyId
        //    {
        //        Value = "WebServiceSupplier"
        //    };
        //    messageHeader.To.PartyId[0] = partyID;

        //    //  header info
        //    WebService.VNA_OTA_AirRulesLLSRQ.Security1 security = new WebService.VNA_OTA_AirRulesLLSRQ.Security1
        //    {
        //        BinarySecurityToken = model.Token
        //    };
        //    //
        //    oTA_AirRules.OriginDestinationInformation = new WebService.VNA_OTA_AirRulesLLSRQ.OTA_AirRulesRQOriginDestinationInformation
        //    {
        //        FlightSegment = new WebService.VNA_OTA_AirRulesLLSRQ.OTA_AirRulesRQOriginDestinationInformationFlightSegment()
        //    };
        //    oTA_AirRules.OriginDestinationInformation.FlightSegment.DepartureDateTime = model.DepartureDateTime.ToString("MM-dd");
        //    //
        //    oTA_AirRules.OriginDestinationInformation.FlightSegment.OriginLocation = new WebService.VNA_OTA_AirRulesLLSRQ.OTA_AirRulesRQOriginDestinationInformationFlightSegmentOriginLocation()
        //    {
        //        LocationCode = model.OriginLocation
        //    };
        //    //
        //    oTA_AirRules.OriginDestinationInformation.FlightSegment.DestinationLocation = new WebService.VNA_OTA_AirRulesLLSRQ.OTA_AirRulesRQOriginDestinationInformationFlightSegmentDestinationLocation()
        //    {
        //        LocationCode = model.DestinationLocation
        //    };
        //    var data = client.OTA_AirRulesRQ(ref messageHeader, ref security, oTA_AirRules);
        //    return data;
        //}


        public string GetOTA_AirRulesLLSRQ(VNA_OTA_AirRulesLLSRQ model)
        {

            HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            soapEnvelopeXml.Load(path);
            soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "OTA_AirRulesLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "OTA_AirRulesLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
            XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
            var stringXML = "";
            stringXML += "<OTA_AirRulesRQ ReturnHostCommand='true' Version='2.3.0' xmlns='http://webservices.sabre.com/sabreXML/2011/10' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>";
            stringXML += "    <OriginDestinationInformation>";
            stringXML += "        <FlightSegment DepartureDateTime='" + model.DepartureDateTime.ToString("MM-dd") + "'>";
            stringXML += "            <DestinationLocation LocationCode='" + model.DestinationLocation + "' />";
            stringXML += "            <OriginLocation LocationCode='" + model.OriginLocation + "' />";
            stringXML += "        </FlightSegment>";
            stringXML += "    </OriginDestinationInformation>";
            stringXML += "    <RuleReqInfo>";
            stringXML += "        <FareBasis Code='" + model.FareBasis + "' />";
            stringXML += "    </RuleReqInfo>";
            stringXML += "</OTA_AirRulesRQ>";

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
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    // Save the document to a file and auto-indent the output.
                    string fileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "-" + model.FareBasis + "-" + model.OriginLocation + "-" + model.DestinationLocation + "-" + "_ticketting-condition.xml";
                    var urlFile = HttpContext.Current.Server.MapPath(@"~/WS/" + fileName);
                    XmlWriter writer = XmlWriter.Create(urlFile, settings);
                    soapEnvelopeXml.Save(writer);

                    return soapResult;
                }
            }

        }


        public static XmlNode GetOTA_AirRulesLLSRQForSearch(VNA_OTA_AirRulesLLSRQ model)
        {

            HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            soapEnvelopeXml.Load(path);
            soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "OTA_AirRulesLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "OTA_AirRulesLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
            XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
            var stringXML = "";
            stringXML += "<OTA_AirRulesRQ ReturnHostCommand='true' Version='2.3.0' xmlns='http://webservices.sabre.com/sabreXML/2011/10' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>";
            stringXML += "    <OriginDestinationInformation>";
            stringXML += "        <FlightSegment DepartureDateTime='" + model.DepartureDateTime.ToString("MM-dd") + "'>";
            stringXML += "            <DestinationLocation LocationCode='" + model.DestinationLocation + "' />";
            stringXML += "            <OriginLocation LocationCode='" + model.OriginLocation + "' />";
            stringXML += "        </FlightSegment>";
            stringXML += "    </OriginDestinationInformation>";
            stringXML += "    <RuleReqInfo>";
            stringXML += "        <FareBasis Code='" + model.FareBasis + "' />";
            stringXML += "    </RuleReqInfo>";
            stringXML += "</OTA_AirRulesRQ>";

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
                    if (soapEnvelopeXml == null)
                        return null;
                    //
                    XmlNode rootXmlNode = soapEnvelopeXml.GetElementsByTagName("Rules")[0];
                    if (rootXmlNode != null && rootXmlNode.HasChildNodes)
                        return rootXmlNode;
                    //
                    return null;
                }
            }

        }

    }
}


//  var listXmlNode = soapEnvelopeXml.GetElementsByTagName("Taxes");