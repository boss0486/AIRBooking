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
    public class VNAWSAirTicketLLSRQService
    {
        public string VNA_AirTicketLLSRQ(AirTicketLLSRQModel model)
        {
            #region test code
            HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            soapEnvelopeXml.Load(path);
            soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "AirTicketLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "AirTicketLLSRQ";
            soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
            XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();


            var stringXML = "";
            #region cmt
            stringXML += " <AirTicketRQ xmlns=\"http://webservices.sabre.com/sabreXML/2011/10\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" NumResponses=\"1\" ReturnHostCommand=\"true\" Version=\"2.12.0\">";
            stringXML += "    <OptionalQualifiers>";
            stringXML += "        <FOP_Qualifiers>";
            stringXML += "            <SabreSonicTicketing>";
            stringXML += "                <BasicFOP>";
            stringXML += "                    <CC_Info Suppress=\"true\">";
            stringXML += "                        <PaymentCard Code=\"BT\" ExpireDate=\"" + model.ExpireDate + "\" ManualApprovalCode=\"" + model.ApproveCode + "\" Number=\"8738210537959681\"/>";
            stringXML += "                    </CC_Info>";
            stringXML += "                </BasicFOP>";
            stringXML += "            </SabreSonicTicketing>";
            stringXML += "        </FOP_Qualifiers>";
            stringXML += "        <MiscQualifiers>";
            stringXML += "            <Ticket Type=\"VCR\"/>";
            stringXML += "        </MiscQualifiers>";
            stringXML += "    </OptionalQualifiers>";
            stringXML += "</AirTicketRQ>";
            #endregion
            #region test
            // stringXML += "<AirTicketRQ xmlns=\"http://webservices.sabre.com/sabreXML/2011/10\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" NumResponses=\"1\" Version=\"2.12.0\">";
            //stringXML += "    <DesignatePrinter>";
            //stringXML += "        <Printers>";
            //stringXML += "            <Ticket CountryCode=\"GF\" LNIATA=\"58BEAB\"/>";
            //stringXML += "        </Printers>";
            //stringXML += "    </DesignatePrinter>";
            //stringXML += "    <OptionalQualifiers>";
            //stringXML += "        <FOP_Qualifiers>";
            //stringXML += "            <SabreSonicTicketing>";
            //stringXML += "                <BasicFOP>";//ManualApprovalCode=\""+model.approveCode+"\" Type=\"CK\"
            //stringXML += "                    <CC_Info Suppress=\"true\">";
            //stringXML += "                        <PaymentCard Code=\"BT\" ExpireDate=\"2020-12\" ManualApprovalCode=\""+model.approveCode+"\" Number=\"8738210537959681\"/>";
            //stringXML += "                   </CC_Info>";
            //stringXML += "              </BasicFOP>";
            //stringXML += "            </SabreSonicTicketing>";
            //stringXML += "         </FOP_Qualifiers>";
            //stringXML += "        <MiscQualifiers>";
            //stringXML += "            <Ticket Type=\"VCR\"/>";
            //stringXML += "        </MiscQualifiers>";
            //stringXML += "    </OptionalQualifiers>";
            //stringXML += "</AirTicketRQ>";
            #endregion
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
            return "";
            #endregion
        }
    }
}
