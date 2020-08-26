using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNA_VoidTicketLLSRQService
    {
        public AIRService.WebService.VNA_VoidTicketLLSRQ.VoidTicketRS VoidETicket(TokenModel model, string eticket)
        {

            #region test code
            //HttpWebRequest request = CreateWebRequest(URL_WS);
            //XmlDocument soapEnvelopeXml = new XmlDocument();
            //var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            //soapEnvelopeXml.Load(path);
            //soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            //soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "VoidTicketRQ";
            //soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "VoidTicketRQ";
            //soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.token;
            //soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationId;
            //XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();


            //var stringXML = "";
            //#region cmt

            //#endregion

            //child.InnerXml = stringXML;
            //soapEnvelopeXml.GetElementsByTagName("soapenv:Body")[0].AppendChild(child);

            //using (Stream stream = request.GetRequestStream())
            //{
            //    soapEnvelopeXml.Save(stream);
            //}
            //using (WebResponse response = request.GetResponse())
            //{
            //    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
            //    {
            //        string soapResult = rd.ReadToEnd();
            //        soapEnvelopeXml = new XmlDocument();
            //        soapEnvelopeXml.LoadXml(soapResult);

            //    }
            //}
            //return "";
            #endregion
            AIRService.WebService.VNA_VoidTicketLLSRQ.MessageHeader messageHeader = new AIRService.WebService.VNA_VoidTicketLLSRQ.MessageHeader();
            messageHeader.MessageData = new AIRService.WebService.VNA_VoidTicketLLSRQ.MessageData();
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new AIRService.WebService.VNA_VoidTicketLLSRQ.Service();
            messageHeader.Action = "VoidTicketLLSRQ";
            messageHeader.From = new AIRService.WebService.VNA_VoidTicketLLSRQ.From();

            messageHeader.From.PartyId = new AIRService.WebService.VNA_VoidTicketLLSRQ.PartyId[1];
            var partyID = new AIRService.WebService.VNA_VoidTicketLLSRQ.PartyId();
            partyID.Value = "WebServiceClient";
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new AIRService.WebService.VNA_VoidTicketLLSRQ.To();
            messageHeader.To.PartyId = new AIRService.WebService.VNA_VoidTicketLLSRQ.PartyId[1];
            partyID = new AIRService.WebService.VNA_VoidTicketLLSRQ.PartyId();
            partyID.Value = "WebServiceSupplier";
            messageHeader.To.PartyId[0] = partyID;

            AIRService.WebService.VNA_VoidTicketLLSRQ.Security1 security = new AIRService.WebService.VNA_VoidTicketLLSRQ.Security1();
            security.BinarySecurityToken = model.Token;
            AIRService.WebService.VNA_VoidTicketLLSRQ.VoidTicketRQ voidTicketRQ = new AIRService.WebService.VNA_VoidTicketLLSRQ.VoidTicketRQ();
            voidTicketRQ.Ticketing = new AIRService.WebService.VNA_VoidTicketLLSRQ.VoidTicketRQTicketing();
            voidTicketRQ.Ticketing.eTicketNumber = eticket;
            AIRService.WebService.VNA_VoidTicketLLSRQ.VoidTicketPortTypeClient client = new AIRService.WebService.VNA_VoidTicketLLSRQ.VoidTicketPortTypeClient();
            var data = client.VoidTicketRQ(ref messageHeader, ref security, voidTicketRQ);
            return data;
        }
    }
}
