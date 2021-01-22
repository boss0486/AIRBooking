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
    class VNA_WSGetReservationRQService
    {
        public XMLObject.ReservationRq.GetReservationRS GetReservation(GetReservationModel model)
        {
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
                    //Helper.XMLHelper.WriteXml("getreservationrq-test-xuat-ve.xml", soapEnvelopeXml);
                    XmlNode xmlnode = soapEnvelopeXml.GetElementsByTagName("soap-env:Body")[0];
                    XMLObject.ReservationRq.GetReservationRS reservationRS = new XMLObject.ReservationRq.GetReservationRS();
                    if (xmlnode != null)
                        reservationRS = XMLHelper.Deserialize<XMLObject.ReservationRq.GetReservationRS>(xmlnode.InnerXml);
                    //
                    return reservationRS;
                }
            }
        }

        public XMLObject.ReservationRq2.GetReservationRS GetReservation2(GetReservationModel model)
        {
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
                    //Helper.XMLHelper.WriteXml("getreservationrq-test-xuat-ve.xml", soapEnvelopeXml);
                    XmlNode xmlnode = soapEnvelopeXml.GetElementsByTagName("soap-env:Body")[0];
                    XMLObject.ReservationRq2.GetReservationRS reservationRS = new XMLObject.ReservationRq2.GetReservationRS();
                    if (xmlnode != null)
                        reservationRS = XMLHelper.Deserialize<XMLObject.ReservationRq2.GetReservationRS>(xmlnode.InnerXml);
                    //
                    return reservationRS;
                }
            }
        }
        //}
        //catch (Exception ex)
        //{

        //    throw ex;
        //}

    }
}