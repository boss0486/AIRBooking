using AIRService.Entities;
using AIRService.Models;
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
   public class VNA_VoidTicketLLSRQService
    {
        public XMLObject.VoidTicketRq.VoidTicketRS VoidTicketLLSRQ(TokenModel model, string eticket)
        { 
            HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            soapEnvelopeXml.Load(path);
            soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "VoidTicketLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "VoidTicketLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;

            XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
            var stringXML = "";
            //stringXML += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            stringXML += $"<VoidTicketRQ Version=\"2.0.2\" xmlns=\"http://webservices.sabre.com/sabreXML/2011/10\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">";
            stringXML += $"<Ticketing eTicketNumber=\"{ eticket}\" />";
            stringXML += "</VoidTicketRQ> ";
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
                     
                    XMLObject.VoidTicketRq.VoidTicketRS voidTicketRS = new XMLObject.VoidTicketRq.VoidTicketRS();
                    XmlNode xmlnode = soapEnvelopeXml.GetElementsByTagName("soap-env:Body")[0];
                    if (xmlnode != null)
                        voidTicketRS = XMLHelper.Deserialize<XMLObject.VoidTicketRq.VoidTicketRS>(xmlnode.InnerXml);
                    //
                    return voidTicketRS;
                }
            }  
        }
    }
}
