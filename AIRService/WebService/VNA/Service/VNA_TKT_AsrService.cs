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
//
using AIR.Helper.Session;
using AIRService.Models;
using AIRService.Entities;
using AIRService.WS.Service;
using AIRService.WS.Entities;
using AIRService.WebService.VNA.Enum;
using AIRService.WebService.VNA_OTA_AirTaxRQ;
//
using ApiPortalBooking.Models;
using System.Web.Mvc;
using Helper;
using System.Xml.Serialization;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using WebCore.Services;
using WebCore.Entities;

namespace AIRService.WS.Service
{
    public class VNA_TKT_AsrService
    {
        public List<EmployeeNumber> GetEmployeeNumber(EmpReportModel model)
        {
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
            stringXML += "<AgentListRQ version='1.2.1'";
            stringXML += "    xmlns='http://www.sabre.com/ns/Ticketing/AsrServices/1.0'>";
            stringXML += "    <Header />";
            stringXML += "    <SelectionCriteria>";
            stringXML += "        <TicketingProvider>VN</TicketingProvider>";
            stringXML += "        <StationNumber>37959681</StationNumber>";
            stringXML += "        <ReportDate>" + model.ReportDate.ToString("yyyy-MM-dd") + "</ReportDate>";
            stringXML += "    </SelectionCriteria>";
            stringXML += "</AgentListRQ>";
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
                    XmlNode agentListRSNode = soapEnvelopeXml.GetElementsByTagName("asr:AgentListRS")[0];
                    var nodeList = agentListRSNode.ChildNodes;
                    List<EmployeeNumber> employeeNumbers = new List<EmployeeNumber>();
                    if (nodeList.Count > 0)
                    {
                        foreach (XmlNode node in nodeList)
                        {
                            string name = node.Name;
                            if (node.Name.Contains("Agent"))
                            {
                                if (node.ChildNodes.Count > 0)
                                {
                                    string employeeNumber = string.Empty;
                                    string eprCity = string.Empty;
                                    string dieSine = string.Empty;
                                    foreach (XmlNode item in node.ChildNodes)
                                    {
                                        if (item.Name.Contains("IssuingAgentEmployeeNumber"))
                                            employeeNumber = item.InnerText;
                                        //
                                        if (item.Name.Contains("IssuingAgentEprCity"))
                                            eprCity = item.InnerText;
                                        //
                                        if (item.Name.Contains("IssuingAgentDieSine"))
                                            dieSine = item.InnerText;
                                        //                                        
                                    }
                                    if (!string.IsNullOrWhiteSpace(employeeNumber) && !string.IsNullOrWhiteSpace(eprCity) && !string.IsNullOrWhiteSpace(dieSine))
                                    {
                                        employeeNumbers.Add(new EmployeeNumber
                                        {
                                            IssuingAgentEmployeeNumber = employeeNumber,
                                            IssuingAgentEprCity = eprCity,
                                            IssuingAgentDieSine = dieSine
                                        });
                                    }

                                }
                            }
                        }
                        return employeeNumbers;
                    }
                    return null;
                }
            }
        }
        //
        public ReportSaleSummaryResult ReportSaleSummaryReport(ReportModel model)
        {
            try
            {
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
                stringXML += "<SalesSummaryRQ version ='1.2.1' xmlns='http://www.sabre.com/ns/Ticketing/AsrServices/1.0'>";
                stringXML += "   <Header/>";
                stringXML += "   <SelectionCriteria>";
                stringXML += "    <TicketingProvider>VN</TicketingProvider>";
                stringXML += "    <StationNumber>37959681</StationNumber>";
                stringXML += "    <EmployeeNumber>" + model.EmpNumber + "</EmployeeNumber>";
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
                        //
                        List<ReportSaleSummaryTransaction> reportTransactionResultModels = new List<ReportSaleSummaryTransaction>();
                        //
                        XmlNode salesNode = soapEnvelopeXml.GetElementsByTagName("asr:SalesSummaryRS")[0];
                        if (salesNode.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode itemSale in salesNode.ChildNodes)
                            {
                                if (itemSale.LocalName =="Transaction")
                                {
                                    XmlNodeList xmlNodeList = itemSale.ChildNodes;
                                    if (xmlNodeList != null)
                                    {
                                        string name = string.Empty;
                                        string documentType = string.Empty;
                                        string documentNumber = string.Empty;
                                        string passengerName = string.Empty;
                                        string pnrLocator = string.Empty;
                                        string ticketPrinterLniata = string.Empty;
                                        string transactionTime = string.Empty;
                                        bool exceptionItem = false;
                                        bool decoupleItem = false;
                                        string ticketStatusCode = string.Empty;
                                        bool isElectronicTicket = false;
                                        ReportSaleSummaryTransactionSSFop ePRReportTransactionSSFop = new ReportSaleSummaryTransactionSSFop();
                                        foreach (XmlNode item in xmlNodeList)
                                        {
                                            string localName = item.LocalName;
                                            //
                                            if (localName == "DocumentType")
                                                documentType = item.InnerText;
                                            //
                                            if (localName == "DocumentNumber")
                                                documentNumber = item.InnerText;
                                            //
                                            if (localName == "PassengerName")
                                                passengerName = item.InnerText;
                                            //
                                            if (localName == "PnrLocator")
                                                pnrLocator = item.InnerText;
                                            //
                                            if (localName == "TicketPrinterLniata")
                                                ticketPrinterLniata = item.InnerText;
                                            //
                                            if (localName=="TransactionTime")
                                                transactionTime = item.InnerText;
                                            //      
                                            if (localName == "ExceptionItem")
                                                exceptionItem = Convert.ToBoolean(item.InnerText);
                                            //
                                            if (localName == "DecoupleItem")
                                                decoupleItem = Convert.ToBoolean(item.InnerText);
                                            //     
                                            if (localName== "TicketStatusCode")
                                                ticketStatusCode = item.InnerText;
                                            //  
                                            if (localName == "IsElectronicTicket")
                                                isElectronicTicket = Convert.ToBoolean(item.InnerText);
                                            //
                                            // SSFop 
                                            if (localName == "SSFop")
                                            {
                                                XmlNodeList ssfopNode = item.ChildNodes;
                                                if (ssfopNode.Count > 0)
                                                {
                                                    foreach (XmlNode itemSSFop in ssfopNode)
                                                    {
                                                        string ssfopLocalName = itemSSFop.LocalName;
                                                        if (ssfopLocalName == "FopCode")
                                                            ePRReportTransactionSSFop.FopCode = itemSSFop.InnerText;
                                                        //
                                                        if (ssfopLocalName == "CurrencyCode")
                                                            ePRReportTransactionSSFop.CurrencyCode = itemSSFop.InnerText;
                                                        //
                                                        if (ssfopLocalName  == "FareAmount")
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(itemSSFop.InnerText))
                                                            {
                                                                ePRReportTransactionSSFop.FareAmount = Convert.ToDouble(itemSSFop.InnerText);
                                                            }
                                                        }
                                                        //
                                                        if (ssfopLocalName == "TaxAmount")
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(itemSSFop.InnerText))
                                                            {
                                                                ePRReportTransactionSSFop.TaxAmount = Convert.ToDouble(itemSSFop.InnerText);
                                                            }
                                                        }
                                                        //
                                                        if (ssfopLocalName == "TotalAmount")
                                                        {
                                                            if (!string.IsNullOrWhiteSpace(itemSSFop.InnerText))
                                                            {
                                                                ePRReportTransactionSSFop.TotalAmount = Convert.ToDouble(itemSSFop.InnerText);
                                                            }
                                                        }
                                                        //
                                                    }
                                                }
                                            }
                                        }
                                        reportTransactionResultModels.Add(new ReportSaleSummaryTransaction
                                        {
                                            DocumentType = documentType,
                                            DocumentNumber = documentNumber,
                                            PassengerName = passengerName,
                                            PnrLocator = pnrLocator,
                                            TicketPrinterLniata = ticketPrinterLniata,
                                            TransactionTime = transactionTime,
                                            ExceptionItem = exceptionItem,
                                            DecoupleItem = decoupleItem,
                                            TicketStatusCode = ticketStatusCode,
                                            IsElectronicTicket = isElectronicTicket,
                                            SaleSummaryTransactionSSFop = ePRReportTransactionSSFop
                                        });
                                    }
                                }
                            }
                        }
                        return new ReportSaleSummaryResult
                        {
                            EmpNumber = model.EmpNumber,
                            Status = true,
                            SaleSummaryTransaction = reportTransactionResultModels
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ReportSaleSummaryResult
                {
                    EmpNumber = model.EmpNumber + ex,
                    Status = false,
                    SaleSummaryTransaction = null
                };
            }
        }
        //
        public ReportSaleSummaryDetailsResult EprReportSaleReportDetails(ReportDetailsModel model)
        {
            try
            {
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
                stringXML += "        <DetailRQ version='1.0.0' xmlns='http://www.sabre.com/ns/Ticketing/AsrServices/1.0'>";
                stringXML += "            <Header />";
                stringXML += "            <SelectionCriteria>";
                stringXML += "                <TicketingProvider>VN</TicketingProvider>";
                stringXML += "                <DocumentNumber>" + model.DocumentNumber + "</DocumentNumber>";
                stringXML += "            </SelectionCriteria>";
                stringXML += "        </DetailRQ>";
                child.InnerXml = stringXML;
                soapEnvelopeXml.GetElementsByTagName("soapenv:Body")[0].AppendChild(child);
                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }
                //
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string soapResult = rd.ReadToEnd();
                        soapEnvelopeXml = new XmlDocument();
                        soapEnvelopeXml.LoadXml(soapResult);

                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        // Save the document to a file and auto-indent the output.
                        string fileName = "" + model.DocumentNumber + "_details.xml";
                        var urlFile = HttpContext.Current.Server.MapPath(@"~/WS/" + fileName);
                        XmlWriter writer = XmlWriter.Create(urlFile, settings);
                        soapEnvelopeXml.Save(writer);
                        //

                        // data test ################################################################################################################################################
                        //var urlFileTest = HttpContext.Current.Server.MapPath(@"~/Team/DetailRS.xml");
                        //soapEnvelopeXml = new XmlDocument();
                        //soapEnvelopeXml.Load(urlFileTest);
                        // data test ################################################################################################################################################
                        XmlNode salesNode = soapEnvelopeXml.GetElementsByTagName("ns:DetailRS")[0];
                        ReportSummaryDetails reportSummaryDetails = new ReportSummaryDetails();
                        if (salesNode != null && salesNode.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode itemDetails in salesNode.ChildNodes)
                            {
                                if (itemDetails.LocalName == "AssociatedDocInfo")
                                {
                                    XmlNodeList xmlNodeList = itemDetails.ChildNodes;
                                    if (xmlNodeList != null)
                                    {
                                        string name = string.Empty;
                                        string documentNumber = model.DocumentNumber;
                                        string associatedDocument = string.Empty;
                                        string reasonForIssuanceCode = string.Empty;
                                        string reasonForIssuanceDesc = string.Empty;
                                        int couponNumber = 0;
                                        string ticketingProvider = string.Empty;
                                        int flightNumber = 0;
                                        string classOfService = string.Empty;
                                        DateTime departureDtm = DateTime.Now;
                                        DateTime arrivalDtm = DateTime.Now;
                                        string departureCity = string.Empty;
                                        string arrivalCity = string.Empty;
                                        string couponStatus = string.Empty;
                                        string fareBasis = string.Empty;
                                        string baggageAllowance = string.Empty;
                                        //
                                        foreach (XmlNode item in xmlNodeList)
                                        {
                                            string localName = item.LocalName;
                                            //
                                            if (localName == "AssociatedDocument")
                                                associatedDocument = item.InnerText;
                                            //
                                            if (localName == "ReasonForIssuanceCode")
                                                reasonForIssuanceCode = item.InnerText;
                                            //
                                            if (localName == "ReasonForIssuanceDesc")
                                                reasonForIssuanceDesc = item.InnerText;
                                            //
                                            if (localName == "CouponDetail")
                                            {
                                                XmlNodeList couponDetailNodeList = item.ChildNodes;
                                                if (couponDetailNodeList.Count > 0)
                                                {
                                                    foreach (XmlNode couponDetailNode in couponDetailNodeList)
                                                    {
                                                        localName = couponDetailNode.LocalName;
                                                        string valText = couponDetailNode.InnerText;

                                                        if (localName == "CouponNumber" && !string.IsNullOrWhiteSpace(valText))
                                                            couponNumber = Convert.ToInt32(valText);
                                                        //
                                                        if (localName == "TicketingProvider")
                                                            ticketingProvider = valText;
                                                        //
                                                        if (localName == "FlightNumber" && !string.IsNullOrWhiteSpace(valText))
                                                            flightNumber = Convert.ToInt32(valText);
                                                        //      
                                                        if (localName == "ClassOfService")
                                                            classOfService = valText;
                                                        // 
                                                        if (localName == "DepartureDtm" && !string.IsNullOrWhiteSpace(valText))
                                                            departureDtm = Helper.VNALibrary.ConvertToDateTime(valText);
                                                        // 
                                                        if (localName == "ArrivalDtm" && !string.IsNullOrWhiteSpace(valText))
                                                            arrivalDtm = Helper.VNALibrary.ConvertToDateTime(valText);
                                                        // 
                                                        if (localName == "DepartureCity")
                                                            departureCity = valText;
                                                        // 
                                                        if (localName == "ArrivalCity")
                                                            arrivalCity = valText;
                                                        // 
                                                        if (localName == "CouponStatus")
                                                            couponStatus = valText;
                                                        //
                                                        if (localName == "FareBasis")
                                                            fareBasis = valText;
                                                        // 
                                                        if (localName == "BaggageAllowance")
                                                            baggageAllowance = valText;
                                                        //
                                                    }
                                                }
                                            }
                                        }
                                        reportSummaryDetails = new ReportSummaryDetails
                                        {
                                            DocumentNumber = documentNumber,
                                            AssociatedDocument = associatedDocument,
                                            ReasonForIssuanceCode = reasonForIssuanceCode,
                                            ReasonForIssuanceDesc = reasonForIssuanceDesc,
                                            CouponNumber = couponNumber,
                                            TicketingProvider = ticketingProvider,
                                            FlightNumber = flightNumber,
                                            ClassOfService = classOfService,
                                            DepartureDtm = Convert.ToString(departureDtm),
                                            ArrivalDtm = Convert.ToString(arrivalDtm),
                                            DepartureCity = departureCity,
                                            ArrivalCity = arrivalCity,
                                            CouponStatus = couponStatus,
                                            FareBasis = fareBasis,
                                            BaggageAllowance = baggageAllowance
                                        };
                                    }
                                }
                            }
                        }
                        return new ReportSaleSummaryDetailsResult
                        {
                            DocumentNumber = model.DocumentNumber,
                            Status = true,
                            SaleSummaryDetails = reportSummaryDetails
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ReportSaleSummaryDetailsResult
                {
                    DocumentNumber = model.DocumentNumber,
                    Status = false,
                    SaleSummaryDetails = null
                };
            }
        }
        //
        public ReportSaleSummaryResult ExcuteEprReportClose(ReportModel model)
        {
            try
            {
                return null;
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
                stringXML += "<SalesSummaryRQ version ='1.2.1' xmlns='http://www.sabre.com/ns/Ticketing/AsrServices/1.0'>";
                stringXML += "      <Header/>";
                stringXML += "      <SelectionCriteria>";
                stringXML += "          <AccountingReportOperation>close</AccountingReportOperation>";
                stringXML += "          <TicketingProvider>VN</TicketingProvider>";
                stringXML += "          <StationNumber>37959681</StationNumber>";
                stringXML += "          <EmployeeNumber>" + model.EmpNumber + "</EmployeeNumber>";
                stringXML += "          <ReportDate>" + model.ReportDate.ToString("yyyy-MM-dd") + "</ReportDate>";
                stringXML += "      </SelectionCriteria>";
                stringXML += "</SalesSummaryRQ>";
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

                        return new ReportSaleSummaryResult
                        {
                            EmpNumber = model.EmpNumber,
                            Status = true,
                        };
                    }
                }
            }
            catch (Exception)
            {
                return new ReportSaleSummaryResult
                {
                    EmpNumber = model.EmpNumber,
                    Status = false,
                };
            }
        }
        //
        public ReportSaleSummaryResult ExcuteEprReportExtend(ReportModel model)
        {
            try
            {
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
                stringXML += "<StationSummaryRQ version='1.0.0' xmlns='http://www.sabre.com/ns/Ticketing/AsrServices/1.0'>";
                stringXML += "	 <Header/>";
                stringXML += "	 <SelectionCriteria>";
                stringXML += "	 <ReportOperation>close</ReportOperation>";
                stringXML += "	 <TicketingProvider>VN</TicketingProvider>";
                stringXML += "	 <StationNumber>37959681</StationNumber>";
                stringXML += "	 <ReportDate>" + model.ReportDate.ToString("yyyy-MM-dd") + "</ReportDate>";
                stringXML += "	 </SelectionCriteria>";
                stringXML += "</StationSummaryRQ>";
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
                        return new ReportSaleSummaryResult
                        {
                            EmpNumber = model.EmpNumber,
                            Status = true,
                        };
                    }
                }
            }
            catch (Exception)
            {
                return new ReportSaleSummaryResult
                {
                    EmpNumber = model.EmpNumber,
                    Status = false,
                };
            }
        }
        //
        public string ReportSaleSummarySave(ApiPortalBooking.Models.VNA_WS_Model.ReportSaleSummaryTransaction model, DateTime reportDate, string empNumber)
        {
            try
            {
                if (model == null)
                    return MessageText.Invalid;
                //
                ReportSaleSummaryService reportTransactionService = new ReportSaleSummaryService();
                App_ReportSaleSummarySSFopService reportTransactionSSFopService = new App_ReportSaleSummarySSFopService();
                string reportTransactionId = reportTransactionService.Create<string>(new ReportSaleSummary
                {
                    Title = empNumber,
                    EmployeeNumber = empNumber,
                    DocumentType = model.DocumentType,
                    DocumentNumber = model.DocumentNumber,
                    PassengerName = model.PassengerName,
                    PnrLocator = model.PassengerName,
                    TicketPrinterLniata = model.TicketPrinterLniata,
                    TransactionTime = model.TransactionTime,
                    ExceptionItem = model.ExceptionItem,
                    DecoupleItem = model.DecoupleItem,
                    TicketStatusCode = model.TicketStatusCode,
                    IsElectronicTicket = model.IsElectronicTicket,
                    ReportDate = reportDate
                });
                ApiPortalBooking.Models.VNA_WS_Model.ReportSaleSummaryTransactionSSFop ePRReportTransactionSSFop = model.SaleSummaryTransactionSSFop;
                if (ePRReportTransactionSSFop != null)
                {
                    reportTransactionSSFopService.Create<string>(new ReportSaleSummarySSFop
                    {
                        Title = ePRReportTransactionSSFop.FopCode,
                        ReportTransactionID = reportTransactionId,
                        CurrencyCode = ePRReportTransactionSSFop.CurrencyCode,
                        FareAmount = ePRReportTransactionSSFop.FareAmount,
                        TaxAmount = ePRReportTransactionSSFop.TaxAmount,
                        TotalAmount = ePRReportTransactionSSFop.TaxAmount,
                    });
                }
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string ReportSaleSummaryDetailsSave(ReportSaleSummaryDetailsCreate model)
        {
            try
            {
                if (model == null)
                    return MessageText.Invalid;
                //
                ReportTransactionDetailsService reportTransactionDetailsService = new ReportTransactionDetailsService();
                string reportTransactionId = reportTransactionDetailsService.Create<string>(new ReportSaleSummaryDetails
                {
                    Title = model.DocumentNumber,
                    DocumentNumber = model.DocumentNumber,
                    AssociatedDocument = model.AssociatedDocument,
                    ReasonForIssuanceCode = model.ReasonForIssuanceCode,
                    ReasonForIssuanceDesc = model.ReasonForIssuanceDesc,
                    CouponNumber = model.CouponNumber,
                    TicketingProvider = model.TicketingProvider,
                    FlightNumber = model.FlightNumber,
                    ClassOfService = model.ClassOfService,
                    DepartureDtm = model.DepartureDtm,
                    DecoupleItem = model.DecoupleItem,
                    ArrivalDtm = model.ArrivalDtm,
                    DepartureCity = model.DepartureCity,
                    ArrivalCity = model.ArrivalCity,
                    CouponStatus = model.CouponStatus,
                    FareBasis = model.FareBasis,
                    BaggageAllowance = model.BaggageAllowance
                });
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        //
        public string GetReportDateInToday()
        {
            try
            {
                //var timezone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
                var timezone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); //  vn time
                var dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timezone);
                //string date = dateTime.ToString("yyyy-MM-dd");
                string date = "2020-08-29";
                return date;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        public bool CheckReportSaleSummary(DateTime reportDate)
        {
            try
            {
                ReportSaleSummaryService reportSaleSummaryService = new ReportSaleSummaryService();
                var reportSaleSummary = reportSaleSummaryService.GetAlls(m => m.ReportDate == Convert.ToDateTime(reportDate)).FirstOrDefault();
                if (reportSaleSummary != null)
                    return false;
                //
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //
        public bool CheckReportSaleSummaryClose(string empNumber, DateTime reportDate)
        {
            try
            {
                ReportSaleSummaryCloseService reportSaleSummaryCloseService = new ReportSaleSummaryCloseService();
                var reportSaleSummary = reportSaleSummaryCloseService.GetAlls(m => m.ReportDate == reportDate).FirstOrDefault();
                if (reportSaleSummary != null)
                    return false;
                //
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        // ##########################################################################################################################################################################
        public string Test01(ReportModel model)
        {
            try
            {
                HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
                soapEnvelopeXml.Load(path);
                soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
                soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "TicketingDocumentServicesRQ";
                soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "TicketingDocumentServicesRQ";
                soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
                soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
                XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
                var stringXML = "";

                stringXML += "<GetTicketingDocumentRQ Version='3.12.0' xmlns='http://www.sabre.com/ns/Ticketing/DC'>";
                stringXML += "    <ns1:STL_Header.RQ xmlns:ns1='http://services.sabre.com/STL/v01' />";
                stringXML += "    <ns2:POS xmlns:ns2='http://services.sabre.com/STL/v01' />";
                stringXML += "    <SearchParameters>";
                stringXML += "        <TicketingProvider>VN</TicketingProvider>";
                stringXML += "        <DocumentNumber>7382444368772</DocumentNumber>";
                stringXML += "    </SearchParameters>";
                stringXML += "</GetTicketingDocumentRQ>";



                //stringXML += "        <DetailRQ version='1.0.0' xmlns='http://www.sabre.com/ns/Ticketing/AsrServices/1.0'>";
                //stringXML += "            <Header />";
                //stringXML += "            <SelectionCriteria>";
                //stringXML += "                <TicketingProvider>VN</TicketingProvider>";
                //stringXML += "                <DocumentNumber>7382444368772</DocumentNumber>";
                //stringXML += "            </SelectionCriteria>";
                //stringXML += "        </DetailRQ>";



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

                        return "Error: " + soapResult;
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex;
            }
        }
    }
    public class EmployeeNumber
    {
        public string IssuingAgentEmployeeNumber { get; set; }
        public string IssuingAgentEprCity { get; set; }
        public string IssuingAgentDieSine { get; set; }
    }
}
