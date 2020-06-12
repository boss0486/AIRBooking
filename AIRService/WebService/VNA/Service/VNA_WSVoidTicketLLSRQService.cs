using AIRService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.WS.Service
{
    class VNA_WSVoidTicketLLSRQService
    {
        public AIRService.WebService.WSVoidTicketLLSRQ.VoidTicketRS VoidETicket(TokenModel model, string eticket)
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
            AIRService.WebService.WSVoidTicketLLSRQ.MessageHeader messageHeader = new AIRService.WebService.WSVoidTicketLLSRQ.MessageHeader();
            messageHeader.MessageData = new AIRService.WebService.WSVoidTicketLLSRQ.MessageData();
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new AIRService.WebService.WSVoidTicketLLSRQ.Service();
            messageHeader.Action = "VoidTicketLLSRQ";
            messageHeader.From = new AIRService.WebService.WSVoidTicketLLSRQ.From();

            messageHeader.From.PartyId = new AIRService.WebService.WSVoidTicketLLSRQ.PartyId[1];
            var partyID = new AIRService.WebService.WSVoidTicketLLSRQ.PartyId();
            partyID.Value = "WebServiceClient";
            messageHeader.From.PartyId[0] = partyID;

            messageHeader.To = new AIRService.WebService.WSVoidTicketLLSRQ.To();
            messageHeader.To.PartyId = new AIRService.WebService.WSVoidTicketLLSRQ.PartyId[1];
            partyID = new AIRService.WebService.WSVoidTicketLLSRQ.PartyId();
            partyID.Value = "WebServiceSupplier";
            messageHeader.To.PartyId[0] = partyID;

            AIRService.WebService.WSVoidTicketLLSRQ.Security1 security = new AIRService.WebService.WSVoidTicketLLSRQ.Security1();
            security.BinarySecurityToken = model.Token;
            AIRService.WebService.WSVoidTicketLLSRQ.VoidTicketRQ voidTicketRQ = new AIRService.WebService.WSVoidTicketLLSRQ.VoidTicketRQ();
            voidTicketRQ.Ticketing = new AIRService.WebService.WSVoidTicketLLSRQ.VoidTicketRQTicketing();
            voidTicketRQ.Ticketing.eTicketNumber = eticket;
            AIRService.WebService.WSVoidTicketLLSRQ.VoidTicketPortTypeClient client = new AIRService.WebService.WSVoidTicketLLSRQ.VoidTicketPortTypeClient();
            var data = client.VoidTicketRQ(ref messageHeader, ref security, voidTicketRQ);
            return data;
        }
    }
}
