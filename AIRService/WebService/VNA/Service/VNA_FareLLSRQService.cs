using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AIRService.WS.Service
{
    class VNAFareLLSRQService
    {
        public AIRService.WebService.VNA_FareLLSRQ.FareRS FareLLS(FareLLSModel model)
        {
            try
            {
                #region wsdl
                AIRService.WebService.VNA_FareLLSRQ.MessageHeader messageHeader = new AIRService.WebService.VNA_FareLLSRQ.MessageHeader
                {
                    MessageData = new AIRService.WebService.VNA_FareLLSRQ.MessageData
                    {
                        Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z"
                    },
                    ConversationId = model.ConversationID,
                    Service = new AIRService.WebService.VNA_FareLLSRQ.Service(),
                    Action = "FareLLSRQ",
                    From = new AIRService.WebService.VNA_FareLLSRQ.From()
                };

                messageHeader.From.PartyId = new AIRService.WebService.VNA_FareLLSRQ.PartyId[1];
                var partyID = new AIRService.WebService.VNA_FareLLSRQ.PartyId
                {
                    Value = "WebServiceClient"
                };
                messageHeader.From.PartyId[0] = partyID;

                messageHeader.To = new AIRService.WebService.VNA_FareLLSRQ.To
                {
                    PartyId = new AIRService.WebService.VNA_FareLLSRQ.PartyId[1]
                };
                partyID = new AIRService.WebService.VNA_FareLLSRQ.PartyId
                {
                    Value = "WebServiceSupplier"
                };
                messageHeader.To.PartyId[0] = partyID;

                AIRService.WebService.VNA_FareLLSRQ.Security1 security = new AIRService.WebService.VNA_FareLLSRQ.Security1
                {
                    BinarySecurityToken = model.Token
                };
                AIRService.WebService.VNA_FareLLSRQ.FarePortTypeClient client = new AIRService.WebService.VNA_FareLLSRQ.FarePortTypeClient();
                AIRService.WebService.VNA_FareLLSRQ.FareRQ fareRQ = new AIRService.WebService.VNA_FareLLSRQ.FareRQ
                {
                    OptionalQualifiers = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiers()
                };

                fareRQ.OptionalQualifiers.FlightQualifiers = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersFlightQualifiers
                {
                    VendorPrefs = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersFlightQualifiersAirline[1]
                };
                var VendorPrefs = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersFlightQualifiersAirline
                {
                    Code = model.AirlineCode
                };
                fareRQ.OptionalQualifiers.FlightQualifiers.VendorPrefs[0] = VendorPrefs;
                fareRQ.OptionalQualifiers.TimeQualifiers = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersTimeQualifiers
                {
                    TravelDateOptions = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersTimeQualifiersTravelDateOptions()
                };
                fareRQ.OptionalQualifiers.TimeQualifiers.TravelDateOptions.Start = model.DepartureDateTime.ToString("MM-dd");
                fareRQ.OptionalQualifiers.PricingQualifiers = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersPricingQualifiers
                {
                    PassengerType = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersPricingQualifiersPassengerType[model.PassengerType.Count]
                };
                var index = 0;
                fareRQ.OptionalQualifiers.PricingQualifiers.FareOptions = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersPricingQualifiersFareOptions
                {
                    Private = true
                };
                foreach (var item in model.PassengerType)
                {
                    var type = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersPricingQualifiersPassengerType
                    {
                        Code = item
                    };
                    fareRQ.OptionalQualifiers.PricingQualifiers.PassengerType[index] = type;
                    index++;
                }

                fareRQ.OriginDestinationInformation = new AIRService.WebService.VNA_FareLLSRQ.FareRQOriginDestinationInformation
                {
                    FlightSegment = new AIRService.WebService.VNA_FareLLSRQ.FareRQOriginDestinationInformationFlightSegment()
                };
                fareRQ.OriginDestinationInformation.FlightSegment.OriginLocation = new AIRService.WebService.VNA_FareLLSRQ.FareRQOriginDestinationInformationFlightSegmentOriginLocation
                {
                    LocationCode = model.OriginLocation
                };

                fareRQ.OriginDestinationInformation.FlightSegment.DestinationLocation = new AIRService.WebService.VNA_FareLLSRQ.FareRQOriginDestinationInformationFlightSegmentDestinationLocation
                {
                    LocationCode = model.DestinationLocation
                };
                //
                var data = client.FareRQ(ref messageHeader, ref security, fareRQ);
                if (data.FareBasis == null || data.FareBasis.Count() == 0)
                    return null;
                // ok
                return data;
                #endregion
                #region xml
                //HttpWebRequest request = CreateWebRequest(URL_WS);
                //XmlDocument soapEnvelopeXml = new XmlDocument();
                //var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
                //soapEnvelopeXml.Load(path);
                //soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
                //soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "FareLLSRQ";
                //soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "FareLLSRQ";
                //soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.token;
                //soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationId;
                //XmlDocument AddonXml = new XmlDocument();
                //XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
                //AddonXml.Load(path);
                //string xmlString = @"";
                //xmlString += "<FareRQ Version=\"2.9.0\" xmlns=\"http://webservices.sabre.com/sabreXML/2011/10\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">";
                //xmlString += "   <OptionalQualifiers>";
                //xmlString += "       <FlightQualifiers>";
                //xmlString += "           <VendorPrefs>";
                //xmlString += "               <Airline Code=\"VN\"/>";
                //xmlString += "           </VendorPrefs>";
                //xmlString += "       </FlightQualifiers>";
                //xmlString += "       <PricingQualifiers>";
                //foreach (var item in model.PassengerType)
                //{
                //    xmlString += "           <PassengerType Code=\"" + item + "\"/>";
                //}
                ////xmlString += "       <FareOptions Private=\"true\"/>";
                //xmlString += "       </PricingQualifiers>";
                //xmlString += "       <TimeQualifiers>";
                //xmlString += "           <TravelDateOptions Start=\"" + model.DepartureDateTime.ToString("MM-dd") + "\"/>";
                //xmlString += "       </TimeQualifiers>";
                //xmlString += "   </OptionalQualifiers>";
                //xmlString += "   <OriginDestinationInformation>";
                //xmlString += "       <FlightSegment>";
                //xmlString += "           <DestinationLocation LocationCode=\"" + model.DestinationLocation + "\"/>";
                //xmlString += "           <OriginLocation LocationCode=\"" + model.OriginLocation + "\"/>";
                //xmlString += "       </FlightSegment>";
                //xmlString += "   </OriginDestinationInformation>";
                //xmlString += "</FareRQ>";
                //child.InnerXml = xmlString;
                //soapEnvelopeXml.GetElementsByTagName("soapenv:Body")[0].AppendChild(child);
                ////soapEnvelopeXml.LoadXml(xmlString);
                //using (Stream stream = request.GetRequestStream())
                //{
                //    soapEnvelopeXml.Save(stream);
                //}
                ////var result = new AirFareByCityPairsResult();

                //using (WebResponse response = request.GetResponse())
                //{
                //    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                //    {
                //        string soapResult = rd.ReadToEnd();
                //        soapEnvelopeXml = new XmlDocument();
                //        soapEnvelopeXml.LoadXml(soapResult);
                //        return null;
                //    }
                //}

                #endregion
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<VNAResbookDesigCode> ListVNAResbookDesigCode()
        {
            List<VNAResbookDesigCode> vnaResbookDesigCode = new List<VNAResbookDesigCode>
            {
                new VNAResbookDesigCode { ID = 01, Title = "J" },
                new VNAResbookDesigCode { ID = 02, Title = "C" },
                new VNAResbookDesigCode { ID = 03, Title = "D" },
                new VNAResbookDesigCode { ID = 04, Title = "I" },
                new VNAResbookDesigCode { ID = 05, Title = "O" },
                new VNAResbookDesigCode { ID = 06, Title = "Y" },
                new VNAResbookDesigCode { ID = 07, Title = "B" },
                new VNAResbookDesigCode { ID = 08, Title = "M" },
                new VNAResbookDesigCode { ID = 09, Title = "S" },
                new VNAResbookDesigCode { ID = 10, Title = "H" },
                new VNAResbookDesigCode { ID = 11, Title = "K" },
                new VNAResbookDesigCode { ID = 12, Title = "L" },
                new VNAResbookDesigCode { ID = 13, Title = "Q" },
                new VNAResbookDesigCode { ID = 14, Title = "N" },
                new VNAResbookDesigCode { ID = 15, Title = "R" },
                new VNAResbookDesigCode { ID = 16, Title = "T" },
                new VNAResbookDesigCode { ID = 17, Title = "E" },
                new VNAResbookDesigCode { ID = 18, Title = "A" },
                new VNAResbookDesigCode { ID = 19, Title = "G" },
                new VNAResbookDesigCode { ID = 20, Title = "P" },
                new VNAResbookDesigCode { ID = 21, Title = "X" },
                new VNAResbookDesigCode { ID = 22, Title = "V" }
            };
            return vnaResbookDesigCode;
        }

        public static int GetResbookDesigCodeIDByKey(string key)
        {
            VNAFareLLSRQService vnaFareLLSRQService = new VNAFareLLSRQService();
            VNAResbookDesigCode vnaResbookDesigCode = vnaFareLLSRQService.ListVNAResbookDesigCode().Where(m => m.Title == key).FirstOrDefault();
            if (vnaResbookDesigCode != null)
                return vnaResbookDesigCode.ID;
            else
                return -1;
        }
    }

    public class VNAResbookDesigCode
    {
        public int ID { get; set; }
        public string Title { get; set; }
    }
}
