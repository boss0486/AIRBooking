using AIRService.WS.Helper;
using ApiPortalBooking.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace AIRService.WS.Service
{
    class VNAWSPassengerDetailsRQService
    {
        public AIRService.WebService.WSPassengerDetailsRQ.PassengerDetailsRS PassengerDetail(PassengerDetailsModel model)
        {
            string _token = model.Token;
            string _conversationId = model.ConversationID;

            WebService.WSPassengerDetailsRQ.MessageHeader messageHeader = new WebService.WSPassengerDetailsRQ.MessageHeader
            {
                MessageData = new WebService.WSPassengerDetailsRQ.MessageData()
            };
            messageHeader.MessageData.Timestamp = DateTime.Now.ToString("s").Replace("-", "").Replace(":", "") + "Z";
            messageHeader.ConversationId = model.ConversationID;
            messageHeader.Service = new WebService.WSPassengerDetailsRQ.Service();
            messageHeader.Action = "PassengerDetailsRQ";
            messageHeader.From = new WebService.WSPassengerDetailsRQ.From
            {
                PartyId = new WebService.WSPassengerDetailsRQ.PartyId[1]
            };
            var partyID = new WebService.WSPassengerDetailsRQ.PartyId
            {
                Value = "WebServiceClient"
            };
            messageHeader.From.PartyId[0] = partyID;
            //
            messageHeader.To = new WebService.WSPassengerDetailsRQ.To
            {
                PartyId = new WebService.WSPassengerDetailsRQ.PartyId[1]
            };
            partyID = new WebService.WSPassengerDetailsRQ.PartyId
            {
                Value = "WebServiceSupplier"
            };
            messageHeader.To.PartyId[0] = partyID;
            //
            WebService.WSPassengerDetailsRQ.Security security = new WebService.WSPassengerDetailsRQ.Security
            {
                BinarySecurityToken = model.Token
            };
            // #End header ***********************************************************************************************************************************************************
            AIRService.WebService.WSPassengerDetailsRQ.PassengerDetailsRQ passengerDetailsRQ = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQ();
            //PostProcessing
            passengerDetailsRQ.PostProcessing = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQPostProcessing();
            passengerDetailsRQ.PostProcessing.RedisplayReservation = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQPostProcessingRedisplayReservation
            {
                waitInterval = "100"
            };
            passengerDetailsRQ.PostProcessing.EndTransactionRQ = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQPostProcessingEndTransactionRQ();
            passengerDetailsRQ.PostProcessing.EndTransactionRQ.EndTransaction = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQPostProcessingEndTransactionRQEndTransaction();
            passengerDetailsRQ.PostProcessing.EndTransactionRQ.EndTransaction.Ind = true;
            passengerDetailsRQ.PostProcessing.EndTransactionRQ.Source = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQPostProcessingEndTransactionRQSource();
            passengerDetailsRQ.PostProcessing.EndTransactionRQ.Source.ReceivedFrom = "GOINGO";
            // PriceQuoteInfo
            var lstPassenger = model.lPassenger.OrderBy(m => m.PassengerType).ToList();
            int idxLink = 0;
            int record = 0;
            List<PassengerDetail_PersonNameModel> passengerDetail_PersonNameModels = new List<PassengerDetail_PersonNameModel>();
            List<PassengerDetail_PersonNameModel> specialList = new List<PassengerDetail_PersonNameModel>();
            int cnt = 0;
            passengerDetailsRQ.PriceQuoteInfo = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQLink[lstPassenger.Count];
            foreach (var item in lstPassenger)
            {
                cnt++;
                string giverName = item.GivenName.ToUpper() + " " + AIRService.WS.Helper.VNALibrary.TitleGenerator(item.PassengerType, item.Gender);
                string surName = item.Surname.ToUpper();

                if (item.PassengerType.ToUpper() == "ADT")
                {
                    record = 1;
                    var _nameNumber = cnt + ".1";
                    passengerDetailsRQ.PriceQuoteInfo[idxLink] = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQLink
                    {
                        hostedCarrier = true,
                        hostedCarrierSpecified = true,
                        nameNumber = _nameNumber,
                        record = Convert.ToString(record),
                    };
                    passengerDetail_PersonNameModels.Add(new PassengerDetail_PersonNameModel
                    {
                        PassengerType = item.PassengerType.ToUpper(),
                        NameNumber = _nameNumber,
                        IsInfant = false,
                        GivenName = giverName,
                        Surname = surName,
                        DateOfBirth = item.DateOfBirth
                    });
                }
                if (item.PassengerType.ToUpper() == "CNN")
                {
                    record = 2;
                    var _nameNumber = cnt + ".1";
                    passengerDetailsRQ.PriceQuoteInfo[idxLink] = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQLink
                    {
                        hostedCarrier = true,
                        hostedCarrierSpecified = true,
                        nameNumber = _nameNumber,
                        record = Convert.ToString(record)
                    };
                    passengerDetail_PersonNameModels.Add(new PassengerDetail_PersonNameModel
                    {
                        PassengerType = item.PassengerType.ToUpper(),
                        NameNumber = _nameNumber,
                        IsInfant = false,
                        GivenName = giverName,
                        Surname = surName,
                        DateOfBirth = item.DateOfBirth
                    });
                    specialList.Add(new PassengerDetail_PersonNameModel
                    {
                        PassengerType = item.PassengerType.ToUpper(),
                        NameNumber = _nameNumber,
                        IsInfant = false,
                        GivenName = giverName,
                        Surname = surName,
                        DateOfBirth = item.DateOfBirth
                    });
                }
                if (item.PassengerType.ToUpper() == "INF")
                {
                    record = 3;
                    var _nameNumber = cnt + ".1";
                    passengerDetailsRQ.PriceQuoteInfo[idxLink] = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQLink
                    {
                        hostedCarrier = true,
                        hostedCarrierSpecified = true,
                        nameNumber = _nameNumber,
                        record = Convert.ToString(record)
                    };

                    //passengerDetail_PersonNameModels.Add(new PassengerDetail_PersonNameModel
                    //{
                    //    PassengerType = item.PassengerType.ToUpper(),
                    //    NameNumber = _nameNumber,
                    //    IsInfant = false,
                    //    GivenName = item.GivenName,
                    //    Surname = item.Surname
                    //});
                    specialList.Add(new PassengerDetail_PersonNameModel
                    {
                        PassengerType = item.PassengerType.ToUpper(),
                        NameNumber = _nameNumber,
                        IsInfant = true,
                        GivenName = giverName,
                        Surname = surName,
                        DateOfBirth = item.DateOfBirth
                    });
                }
                idxLink++;
            }

            // SpecialReqDetails
            passengerDetailsRQ.SpecialReqDetails = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetails();
            passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQ();
            passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfo();

            if (specialList.Count > 0)
            {
                passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoService[specialList.Count];
                int idxService = 0;
                foreach (var item in specialList)
                {
                    if (item.PassengerType == "CNN")
                    {
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService] = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoService();
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].SegmentNumber = (idxService + 1) + "";// // null
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].SSR_Code = "CHLD"; // null
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].PersonName = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServicePersonName();
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].PersonName.NameNumber = item.NameNumber;
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].Text = item.DateOfBirth.ToString("ddMMMyy").ToUpper(); // null
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].VendorPrefs = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefs();
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].VendorPrefs.Airline = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefsAirline();
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].VendorPrefs.Airline.Hosted = true;
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].VendorPrefs.Airline.HostedSpecified = true;
                    }
                    if (item.PassengerType == "INF")
                    {
                        var name = item.Surname.ToUpper() + @"/" + item.GivenName.ToUpper() +
                        " " + @"/" + item.DateOfBirth.ToString("ddMMMyy").ToUpper();
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService] = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoService();
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].SegmentNumber = (idxService + 1) + "";// // null
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].SSR_Code = "INFT"; // null
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].PersonName = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServicePersonName();
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].PersonName.NameNumber = item.NameNumber;
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].Text = name; // null
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].VendorPrefs = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefs();
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].VendorPrefs.Airline = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQSpecialReqDetailsSpecialServiceRQSpecialServiceInfoServiceVendorPrefsAirline();
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].VendorPrefs.Airline.Hosted = true;
                        passengerDetailsRQ.SpecialReqDetails.SpecialServiceRQ.SpecialServiceInfo.Service[idxService].VendorPrefs.Airline.HostedSpecified = true;
                    }
                    idxService++;
                }
            }
            //TravelItineraryAddInfoRQ
            passengerDetailsRQ.TravelItineraryAddInfoRQ = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQ();
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfo();
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Address = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoAddress();
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Address.AddressLine = "144 Doi Can";
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Address.CityName = "Ha Noi";
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Address.CountryCode = "VN";
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Address.PostalCode = "10000";
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Address.StreetNmbr = "144 Doi Can";
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Address.VendorPrefs = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoAddressVendorPrefs();
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Address.VendorPrefs.Airline = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoAddressVendorPrefsAirline();
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Address.VendorPrefs.Airline.Hosted = true;

            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Ticketing = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQAgencyInfoTicketing();
            passengerDetailsRQ.TravelItineraryAddInfoRQ.AgencyInfo.Ticketing.TicketType = "8TL30";

            passengerDetailsRQ.TravelItineraryAddInfoRQ.CustomerInfo = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfo();
            // contact number
            passengerDetailsRQ.TravelItineraryAddInfoRQ.CustomerInfo.ContactNumbers = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoContactNumber[1];
            passengerDetailsRQ.TravelItineraryAddInfoRQ.CustomerInfo.ContactNumbers[0] = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoContactNumber();
            passengerDetailsRQ.TravelItineraryAddInfoRQ.CustomerInfo.ContactNumbers[0].NameNumber = "1.1";
            passengerDetailsRQ.TravelItineraryAddInfoRQ.CustomerInfo.ContactNumbers[0].Phone = "0983498218";
            passengerDetailsRQ.TravelItineraryAddInfoRQ.CustomerInfo.ContactNumbers[0].PhoneUseType = "H";
            // email
            passengerDetailsRQ.TravelItineraryAddInfoRQ.CustomerInfo.Email = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoEmail[1];
            passengerDetailsRQ.TravelItineraryAddInfoRQ.CustomerInfo.Email[0] = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoEmail
            {
                Address = model.ContactEmail,
                NameNumber = "1.1"
            };
            ///
            int _cntInfo = 0;
            passengerDetailsRQ.TravelItineraryAddInfoRQ.CustomerInfo.PersonName = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoPersonName[passengerDetail_PersonNameModels.Count];

            foreach (var item in passengerDetail_PersonNameModels)
            {
                passengerDetailsRQ.TravelItineraryAddInfoRQ.CustomerInfo.PersonName[_cntInfo] = new WebService.WSPassengerDetailsRQ.PassengerDetailsRQTravelItineraryAddInfoRQCustomerInfoPersonName
                {
                    NameNumber = item.NameNumber,
                    Infant = item.IsInfant,
                    InfantSpecified = true,
                    PassengerType = item.PassengerType.ToUpper(),
                    GivenName = item.GivenName,
                    Surname = item.Surname
                };
                _cntInfo++;
            }
            //
            //string xml = GetXMLFromObject(passengerDetailsRQ);
            //return xml;

            AIRService.WebService.WSPassengerDetailsRQ.PassengerDetailsPortTypeClient passengerDetailsPortTypeClient = new WebService.WSPassengerDetailsRQ.PassengerDetailsPortTypeClient();
            var data = passengerDetailsPortTypeClient.PassengerDetailsRQ(ref messageHeader, ref security, passengerDetailsRQ);
            return data;
        }

        public Response_PassengerDetailsModel PassengerDetail2(PassengerDetailsModel model)
        {
            try
            {
                HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                var path = System.Web.HttpContext.Current.Server.MapPath(@"~/WS/Xml/PassengerDetails.xml");
                soapEnvelopeXml.Load(path);
                //soapEnvelopeXml.LoadXml(path);
                soapEnvelopeXml.GetElementsByTagName("mes:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
                soapEnvelopeXml.GetElementsByTagName("sec:BinarySecurityToken")[0].InnerText = model.Token;
                soapEnvelopeXml.GetElementsByTagName("mes:ConversationId")[0].InnerText = model.ConversationID;
                XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
                var pricequote = "";
                var index = 0;
                var record = 0;
                var isHaveCNNOrINF = false;
                var contactNumber = "";
                var isHaveContact = false;
                var Email = "";
                var PersonName = "";
                model.lPassenger = model.lPassenger.OrderBy(m => m.PassengerType).ToList();
                foreach (var item in model.lPassenger)
                {
                    var isInf = "false";
                    if (item.PassengerType.ToUpper() == "ADT")
                    {
                        record = 1;
                    }
                    if (item.PassengerType.ToUpper() == "CNN")
                    {
                        isHaveCNNOrINF = true;
                        record = 2;
                    }
                    if (item.PassengerType.ToUpper() == "INF")
                    {
                        isInf = "true";
                        isHaveCNNOrINF = true;
                        record = 3;
                    }
                    index++;
                    var nameNumber = index + ".1";

                    pricequote += " <Link hostedCarrier=\"true\" nameNumber=\"" + nameNumber + "\" record=\"" + record + "\" />";

                    if (!isHaveContact && !string.IsNullOrEmpty(item.PhoneNumber))
                    {
                        contactNumber = "<ContactNumber NameNumber=\"" + nameNumber + "\" Phone=\"" + item.PhoneNumber + "\" PhoneUseType=\"H\"/>";
                        Email = "<Email Address=\"" + model.ContactEmail + "\" NameNumber=\"" + nameNumber + "\"/>";
                        isHaveContact = true;
                    }

                    if (item.PassengerType.ToUpper() != "INF")
                    {
                        PersonName += "          <PersonName NameNumber=\"" + nameNumber + "\" Infant=\"" + isInf + "\" PassengerType=\"" + item.PassengerType.ToUpper() + "\">";
                        PersonName += "            <GivenName>" + item.GivenName.ToUpper() + " " + AIRService.WS.Helper.VNALibrary.TitleGenerator(item.PassengerType, item.Gender) + "</GivenName>";
                        PersonName += "            <Surname>" + item.Surname.ToUpper() + "</Surname>";
                        PersonName += "          </PersonName>";
                    }
                }
                var SpecialReqDetails = "";
                if (isHaveCNNOrINF)
                {
                    SpecialReqDetails += "      <SpecialReqDetails>";
                    SpecialReqDetails += "        <SpecialServiceRQ>";
                    SpecialReqDetails += "          <SpecialServiceInfo>";
                    var segmentNumber = 0;
                    foreach (var item in model.AirBook.OriginDestinationOption)
                    {
                        segmentNumber++;
                        index = 0;
                        var infIndex = 0;
                        foreach (var item2 in model.lPassenger)
                        {
                            index++;
                            var nameNumber = index + ".1";
                            if (item2.PassengerType.ToUpper() == "CNN")
                            {
                                SpecialReqDetails += "            <Service SegmentNumber=\"" + segmentNumber + "\" SSR_Code=\"CHLD\">";
                                SpecialReqDetails += "              <PersonName NameNumber=\"" + nameNumber + "\" />";
                                SpecialReqDetails += "              <Text>" + item2.DateOfBirth.ToString("ddMMMyy").ToUpper() + "</Text>";
                                SpecialReqDetails += "              <VendorPrefs>";
                                SpecialReqDetails += "                <Airline Hosted=\"true\"/>";
                                SpecialReqDetails += "              </VendorPrefs>";
                                SpecialReqDetails += "            </Service>";
                            }
                            if (item2.PassengerType.ToUpper() == "INF")
                            {
                                infIndex++;
                                nameNumber = infIndex + ".1";
                                var name = item2.Surname.ToUpper() + @"/" + item2.GivenName.ToUpper() +
                                " " + AIRService.WS.Helper.VNALibrary.TitleGenerator(item2.PassengerType, item2.Gender) + @"/" + item2.DateOfBirth.ToString("ddMMMyy").ToUpper();
                                SpecialReqDetails += "            <Service SegmentNumber=\"" + segmentNumber + "\" SSR_Code=\"INFT\">";
                                SpecialReqDetails += "              <PersonName NameNumber=\"" + nameNumber + "\" />";
                                SpecialReqDetails += "              <Text>" + name + "</Text>";
                                SpecialReqDetails += "              <VendorPrefs>";
                                SpecialReqDetails += "                <Airline Hosted=\"true\"/>";
                                SpecialReqDetails += "              </VendorPrefs>";
                                SpecialReqDetails += "            </Service>";
                            }
                        }
                    }
                    SpecialReqDetails += "          </SpecialServiceInfo>";
                    SpecialReqDetails += "        </SpecialServiceRQ>";
                    SpecialReqDetails += "      </SpecialReqDetails>";
                }
                var stringXML = "";
                stringXML += " <PassengerDetailsRQ xmlns=\"http://services.sabre.com/sp/pd/v3_4\" version=\"3.4.0\">";
                stringXML += "      <PostProcessing>";
                stringXML += "        <RedisplayReservation waitInterval=\"100\"/>";
                stringXML += "        <EndTransactionRQ>";
                stringXML += "          <EndTransaction Ind=\"true\" />";
                stringXML += "          <Source ReceivedFrom=\"GOINGO\"/>";
                stringXML += "        </EndTransactionRQ>";
                stringXML += "      </PostProcessing>";
                stringXML += "      <PriceQuoteInfo>";
                stringXML += pricequote;
                stringXML += "      </PriceQuoteInfo>";
                stringXML += SpecialReqDetails;
                stringXML += "      <TravelItineraryAddInfoRQ>";
                stringXML += "        <AgencyInfo>";
                stringXML += "          <Address>";
                stringXML += "            <AddressLine>144 Doi Can</AddressLine>";
                stringXML += "            <CityName>Ha Noi</CityName>";
                stringXML += "            <CountryCode>VN</CountryCode>";
                stringXML += "            <PostalCode>10000</PostalCode>";
                stringXML += "            <StreetNmbr>144 Doi Can</StreetNmbr>";
                stringXML += "            <!--<CompanyName Code=\"SSW\"/>-->";
                stringXML += "            <VendorPrefs>";
                stringXML += "              <Airline Hosted=\"true\" />";
                stringXML += "            </VendorPrefs>";
                stringXML += "          </Address>";
                stringXML += "          <Ticketing TicketType=\"8TL30\"/>";
                stringXML += "        </AgencyInfo>";
                stringXML += "        <CustomerInfo>";
                stringXML += "          <ContactNumbers>";
                stringXML += contactNumber;
                stringXML += "          </ContactNumbers>";
                stringXML += Email;
                //personName
                stringXML += PersonName;
                stringXML += "        </CustomerInfo>";
                stringXML += "      </TravelItineraryAddInfoRQ>";
                stringXML += "    </PassengerDetailsRQ>";
                child.InnerXml = stringXML;
                soapEnvelopeXml.GetElementsByTagName("soapenv:Body")[0].AppendChild(child);
                using (System.IO.Stream stream = request.GetRequestStream())
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
                        if (soapEnvelopeXml.GetElementsByTagName("ApplicationResults")[0].Attributes["status"].Value.Equals("Complete"))
                        {
                            string pna = soapEnvelopeXml.GetElementsByTagName("ItineraryRef")[0].Attributes["ID"].Value;
                            return new Response_PassengerDetailsModel
                            {
                                PNR = pna
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string GetXMLFromObject(object o)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }
    }
}
