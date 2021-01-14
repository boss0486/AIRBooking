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
using System.Xml.Serialization;

namespace AIRService.WS.Service
{
    class VNAFareLLSRQService
    {
        public List<WebService.VNA_FareLLSRQ.FareRSFareBasis> FareLLS(FareLLSModel model)
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
                PassengerType = new AIRService.WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersPricingQualifiersPassengerType[model.PassengerType.Count],
                //ReturnBaseTaxTotalSpecified = true,
                CurrencyCode = "VND",

                //RetailerRuleQualifier = new WebService.VNA_FareLLSRQ.FareRQOptionalQualifiersPricingQualifiersRetailerRuleQualifier(),
                //ReturnBaseTaxTotal =  true
            };
            //fareRQ.OptionalQualifiers.PricingQualifiers.RetailerRuleQualifier.ForceSpecified = true;
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
            List<WebService.VNA_FareLLSRQ.FareRSFareBasis> fareRSFareBases = data.FareBasis.Where(m => m.CurrencyCode == model.CurrencyCode).ToList();
            return fareRSFareBases;
            #endregion

            //HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            //XmlDocument soapEnvelopeXml = new XmlDocument();
            //var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
            //soapEnvelopeXml.Load(path);
            //soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
            //soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "FareLLSRQ";
            //soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "FareLLSRQ";
            //soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
            //soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
            //XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
            //string stringXML = @"";
            //stringXML += "<FareRQ Version=\"2.9.0\" xmlns=\"http://webservices.sabre.com/sabreXML/2011/10\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">";
            //stringXML += "   <OptionalQualifiers>";
            //stringXML += "       <FlightQualifiers>";
            //stringXML += "           <VendorPrefs>";
            //stringXML += "               <Airline Code=\"VN\"/>";
            //stringXML += "           </VendorPrefs>";
            //stringXML += "       </FlightQualifiers>";
            //stringXML += "       <PricingQualifiers>";
            //foreach (var item in model.PassengerType)
            //{
            //    stringXML += "           <PassengerType Code=\"" + item + "\"/>";
            //}
            ////xmlString += "       <FareOptions Private=\"true\"/>";
            //stringXML += "       </PricingQualifiers>";
            //stringXML += "       <TimeQualifiers>";
            //stringXML += "           <TravelDateOptions Start=\"" + model.DepartureDateTime.ToString("MM-dd") + "\"/>";
            //stringXML += "       </TimeQualifiers>";
            //stringXML += "   </OptionalQualifiers>";
            //stringXML += "   <OriginDestinationInformation>";
            //stringXML += "       <FlightSegment>";
            //stringXML += "           <DestinationLocation LocationCode=\"" + model.DestinationLocation + "\"/>";
            //stringXML += "           <OriginLocation LocationCode=\"" + model.OriginLocation + "\"/>";
            //stringXML += "       </FlightSegment>";
            //stringXML += "   </OriginDestinationInformation>";
            //stringXML += "</FareRQ>";
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

        }


        public string FareLLS2()
        {
            HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
            XmlDocument soapEnvelopeXml = new XmlDocument();  
            string stringXML = @"<?xml version='1.0' encoding='UTF-8' ?><OTA_AirLowFareSearchRQ Target='Production' Version='5.1.0' xmlns='http://www.opentravel.org/OTA/2003/05'>
    <POS>
        <Source PseudoCityCode='ABM'>
        <RequestorID Type='1' ID='1'>
            <CompanyName Code='SSW'>SSW</CompanyName>
        </RequestorID>
        </Source>
    </POS>
    <OriginDestinationInformation RPH='14'>
        <DepartureDateTime>2021-01-22T07:00:00</DepartureDateTime>
        <OriginLocation LocationCode='HAN' />
        <DestinationLocation LocationCode='SGN' />
        <TPA_Extensions>
            <SegmentType Code='O' />
        </TPA_Extensions>
    </OriginDestinationInformation>
    <OriginDestinationInformation RPH='13'>
        <DepartureDateTime>2021-01-28T06:15:00</DepartureDateTime>
        <OriginLocation LocationCode='SGN' />
        <DestinationLocation LocationCode='HAN' />
        <TPA_Extensions>
            <SegmentType Code='O' />
        </TPA_Extensions>
    </OriginDestinationInformation>
    <TravelPreferences ETicketDesired='false'>
        <CabinPref Cabin='Y' />
    </TravelPreferences>
    <TravelerInfoSummary>
        <SeatsRequested>1</SeatsRequested>
        <AirTravelerAvail>
            <PassengerTypeQuantity Code='ADT' Quantity='1' />
        </AirTravelerAvail>
        <PriceRequestInformation></PriceRequestInformation>
    </TravelerInfoSummary>
    <TPA_Extensions>
        <IntelliSellTransaction>
            <RequestType Name='ADVBRD' />
            <ServiceTag Name='VA' />
        </IntelliSellTransaction>
        <SplitTaxes ByLeg='true' ByFareComponent='true' />
    </TPA_Extensions>
</OTA_AirLowFareSearchRQ>

";
            //stringXML += "<?xml version='1.0' encoding='UTF-8' ?>";
            //stringXML += "<OTA_AirLowFareSearchRQ xmlns:xs='http://www.w3.org/2001/XMLSchema' Target='Production' Version='5.4.0' xmlns='http://www.opentravel.org/OTA/2003/05' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' ResponseType='OTA' ResponseVersion='5.4.0'>";
            //stringXML += "	<POS>";
            //stringXML += "		<Source PseudoCityCode='WIN'>";
            //stringXML += "		<RequestorID Type='0.AAA.X' ID='REQ.ID'>";
            //stringXML += "			<CompanyName Code='SSW' />";
            //stringXML += "		</RequestorID>";
            //stringXML += "		</Source>";
            //stringXML += "	</POS>";
            //stringXML += "	<OriginDestinationInformation RPH='1'>";
            //stringXML += "		<DepartureDateTime>2018-02-08T00:00:00</DepartureDateTime>";
            //stringXML += "		<OriginLocation LocationCode='SYD' />";
            //stringXML += "		<DestinationLocation LocationCode='BNE' />";
            //stringXML += "		<TPA_Extensions>";
            //stringXML += "			<SegmentType Code='O' />";
            //stringXML += "		</TPA_Extensions>";
            //stringXML += "	</OriginDestinationInformation>";
            //stringXML += "	<OriginDestinationInformation RPH='2'>";
            //stringXML += "		<DepartureDateTime>2018-02-15T00:00:00</DepartureDateTime>";
            //stringXML += "		<OriginLocation LocationCode='BNE' />";
            //stringXML += "		<DestinationLocation LocationCode='SYD' />";
            //stringXML += "		<TPA_Extensions>";
            //stringXML += "			<SegmentType Code='O' />";
            //stringXML += "		</TPA_Extensions>";
            //stringXML += "	</OriginDestinationInformation>";
            //stringXML += "	<TravelPreferences>";
            //stringXML += "		<TPA_Extensions>";
            //stringXML += "			<NumTrips Number='10' />";
            //stringXML += "		</TPA_Extensions>";
            //stringXML += "		<InterlineBrands>";
            //stringXML += "			<Brand Code='GW' />";
            //stringXML += "			<Brand Code='EV' />";
            //stringXML += "			<Brand Code='FD' />";
            //stringXML += "		</InterlineBrands>";
            //stringXML += "	</TravelPreferences>";
            //stringXML += "	<TravelerInfoSummary>";
            //stringXML += "		<SeatsRequested>1</SeatsRequested>";
            //stringXML += "		<AirTravelerAvail>";
            //stringXML += "			<PassengerTypeQuantity Code='ADT' Quantity='1' />";
            //stringXML += "		</AirTravelerAvail>";
            //stringXML += "		<PriceRequestInformation>";
            //stringXML += "			<TPA_Extensions></TPA_Extensions>";
            //stringXML += "		</PriceRequestInformation>";
            //stringXML += "	</TravelerInfoSummary>";
            //stringXML += "	<TPA_Extensions>";
            //stringXML += "		<IntelliSellTransaction Debug='0'>";
            //stringXML += "			<RequestType Name='ADVBRD' />";
            //stringXML += "			<ServiceTag Name='VA' />";
            //stringXML += "		</IntelliSellTransaction>";
            //stringXML += "		<SplitTaxes ByLeg='true' ByFareComponent='true' />";
            //stringXML += "	</TPA_Extensions>";
            //stringXML += "</OTA_AirLowFareSearchRQ>";

            soapEnvelopeXml.LoadXml(stringXML);
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
                    return soapEnvelopeXml.InnerText;
                }
            }

        }



    }
}
