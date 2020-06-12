using AIRService.WS.Helper;
using ApiPortalBooking.Models.VNA_WS_Model;
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
    class VNA_WSKT_AsrServicesService
    {
        public WebService.WSKT_AsrServices._DailySalesSummaryRS AsrServices(ReportModel model)
        {
            #region wsdl
            //TKT_AsrServicesRQ.MessageHeader messageHeader = new TKT_AsrServicesRQ.MessageHeader();
            //messageHeader.MessageData = new TKT_AsrServicesRQ.MessageData();
            //messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            //messageHeader.ConversationId = model.ConversationId;
            //messageHeader.Service = new TKT_AsrServicesRQ.Service();
            //messageHeader.Action = "TKT_AsrServicesRQ";
            //messageHeader.From = new TKT_AsrServicesRQ.From();

            //messageHeader.From.PartyId = new TKT_AsrServicesRQ.PartyId[1];
            //var partyID = new TKT_AsrServicesRQ.PartyId();
            //partyID.Value = "WebServiceClient";
            //messageHeader.From.PartyId[0] = partyID;

            //messageHeader.To = new TKT_AsrServicesRQ.To();
            //messageHeader.To.PartyId = new TKT_AsrServicesRQ.PartyId[1];
            //partyID = new TKT_AsrServicesRQ.PartyId();
            //partyID.Value = "WebServiceSupplier";
            //messageHeader.To.PartyId[0] = partyID;

            //TKT_AsrServicesRQ.Security security = new TKT_AsrServicesRQ.Security();
            //security.BinarySecurityToken = new TKT_AsrServicesRQ.SecurityBinarySecurityToken();
            //security.BinarySecurityToken.Value = model.token;

            //var report = new TKT_AsrServicesRQ.DailySalesSummaryRQRequest();

            //report.DailySalesSummaryRQ = new TKT_AsrServicesRQ._DailySalesSummaryRQ();
            //report.DailySalesSummaryRQ.SelectionCriteria = new TKT_AsrServicesRQ._DailySSSelectionCriteria();
            //report.DailySalesSummaryRQ.SelectionCriteria.ReportDate = model.ReportDate;
            //report.DailySalesSummaryRQ.SelectionCriteria.ReportDateSpecified = true;
            //report.DailySalesSummaryRQ.SelectionCriteria.PseudoCityCode = "AJZ";
            //report.DailySalesSummaryRQ.SelectionCriteria.Airline = new TKT_AsrServicesRQ._AirlineChoice();
            //report.DailySalesSummaryRQ.version = "1.2.1";
            //report.MessageHeader = messageHeader;
            //report.Security = security;
            //var header = new TKT_AsrServicesRQ._CachedReportRQ();

            //TKT_AsrServicesRQ.DailySalesSummaryPortTypeClient client = new TKT_AsrServicesRQ.DailySalesSummaryPortTypeClient();
            //var data = client.DailySalesSummaryRQ(ref messageHeader, ref security, report.DailySalesSummaryRQ);
            #endregion
            #region xml
            HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            soapEnvelopeXml.Load(path);
            soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "TKT_AsrServicesRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "TKT_AsrServicesRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
            XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
            var stringXML = "";
            stringXML += "<SalesSummaryRQ";
            stringXML += "   version =\"1.2.1\" xmlns=\"http://www.sabre.com/ns/Ticketing/AsrServices/1.0\">";
            stringXML += "   <Header/>";
            stringXML += "   <SelectionCriteria>";
            // stringXML += "    <AccountingReportOperation>display</AccountingReportOperation>";
            stringXML += "    <TicketingProvider>VN</TicketingProvider>";
            stringXML += "    <StationNumber>37959681</StationNumber>";
            stringXML += "    <EmployeeNumber>146067</EmployeeNumber>";
            stringXML += "    <ReportDate>" + model.ReportDate.ToString("yyyy-MM-dd") + "</ReportDate>";
            stringXML += "   </SelectionCriteria>";
            stringXML += "  </SalesSummaryRQ>";
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
                }
            }
            #endregion
            return null;
        }
    }
}
