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


        public string FUNC_OTA_AirAvailLLSRQ2(AirAvailLLSRQModel model)
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
            soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "OTA_AirAvailLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "OTA_AirAvailLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
            XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
            var stringXML = "";



           



            stringXML += "<ns:OTA_AirAvailRQ Version='2.4.0'  xmlns:ns='http://webservices.sabre.com/sabreXML/2011/10' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>";
            stringXML += " <ns:OriginDestinationInformation>";
            stringXML += "        <ns:FlightSegment DepartureDateTime='" + model.DepartureDateTime.ToString("MM-dd") + "' ResBookDesigCode=''>";
            stringXML += "            <ns:DestinationLocation LocationCode='" + model.DestinationLocation + "' />";
            stringXML += "            <ns:OriginLocation LocationCode='" + model.OriginLocation + "' />";
            stringXML += "        </ns:FlightSegment>";
            stringXML += "    </ns:OriginDestinationInformation>";
            stringXML += "</ns:OTA_AirAvailRQ>";

            for (int i = 0; i < 5; i++)
            {
                stringXML += "		<OTA_AirAvailRQ Version='2.4.0' xmlns:ns='http://webservices.sabre.com/sabreXML/2011/10' xmlns:xs='http://www.w3.org/2001/XMLSchema' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>";
                stringXML += "			<ns:OptionalQualifiers>";
                stringXML += "				<ns:AdditionalAvailability Ind='true' />";
                stringXML += "			</ns:OptionalQualifiers>";
                stringXML += "		</OTA_AirAvailRQ>";
            }
           


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


                    //string json = JsonConvert.SerializeObject(soapEnvelopeXml.InnerXml);
                    //string fileName = "seach-test000111.json";
                    //var urlFile = HttpContext.Current.Server.MapPath(@"~/WS/" + fileName);
                    ////write string to file
                    //System.IO.File.WriteAllText(urlFile, json);

                    return soapEnvelopeXml.InnerXml;
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
