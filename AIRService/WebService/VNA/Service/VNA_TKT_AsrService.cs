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
        public List<EmployeeNumber> GetEmployeeNumber(VNA_EmpReportModel model)
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
        public VNA_ReportSaleSummaryResult ReportSaleSummaryReport(VNA_ReportModel model)
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
                                if (itemSale.LocalName == "Transaction")
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
                                            if (localName == "TransactionTime")
                                                transactionTime = item.InnerText;
                                            //      
                                            if (localName == "ExceptionItem")
                                                exceptionItem = Convert.ToBoolean(item.InnerText);
                                            //
                                            if (localName == "DecoupleItem")
                                                decoupleItem = Convert.ToBoolean(item.InnerText);
                                            //     
                                            if (localName == "TicketStatusCode")
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
                                                        if (ssfopLocalName == "FareAmount")
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
                        return new VNA_ReportSaleSummaryResult
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
                return new VNA_ReportSaleSummaryResult
                {
                    EmpNumber = model.EmpNumber + ex,
                    Status = false,
                    SaleSummaryTransaction = null
                };
            }
        }
        //
        public VNA_ReportSaleSummaryDetailResult EprReportSaleReportDetails(VNA_ReportDetailsModel model)
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
                foreach (var item in model.DocumentNumbers)
                {
                    stringXML += "<DocumentNumber>" + item + "</DocumentNumber>";
                }
                stringXML += "    </SearchParameters>";
                stringXML += "</GetTicketingDocumentRQ>";
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
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Indent = true;
                        // Save the document to a file and auto-indent the output.
                        string fileName = model.EmpNumber + "_ticketingdocumentrq_details.xml";
                        var urlFile = HttpContext.Current.Server.MapPath(@"~/WS/" + fileName);
                        XmlWriter writer = XmlWriter.Create(urlFile, settings);
                        soapEnvelopeXml.Save(writer);

                        // ********************************************************************************************
                        VNA_ReportSaleSummaryDetailResult reportSaleSummaryDetailResult = new VNA_ReportSaleSummaryDetailResult();
                        return new VNA_ReportSaleSummaryDetailResult
                        {
                            DocumentNumber = "ok",
                            Status = true,
                            SaleSummaryDetail = null
                        };


                        //

                    }
                }



                //HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
                //XmlDocument soapEnvelopeXml = new XmlDocument();
                //var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
                //soapEnvelopeXml.Load(path);
                //soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
                //soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "TKT_AsrServicesRQ";
                //soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "TKT_AsrServicesRQ";
                //soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = model.Token;
                //soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = model.ConversationID;
                //XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
                //var stringXML = "";
                //stringXML += "<GetTicketingDocumentRQ Version='3.12.0' xmlns='http://www.sabre.com/ns/Ticketing/DC'>";
                //stringXML += "    <ns1:STL_Header.RQ xmlns:ns1='http://services.sabre.com/STL/v01' />";
                //stringXML += "    <ns2:POS xmlns:ns2='http://services.sabre.com/STL/v01' />";
                //stringXML += "    <SearchParameters>";
                //stringXML += "        <TicketingProvider>VN</TicketingProvider>";
                //foreach (var item in model.DocumentNumbers)
                //{
                //    stringXML += "        <DocumentNumber>" + item + "</DocumentNumber>";
                //}
                //stringXML += "    </SearchParameters>";
                //stringXML += "</GetTicketingDocumentRQ>";

                //var stringXML = "";
                //stringXML += "        <DetailRQ version='1.0.0' xmlns='http://www.sabre.com/ns/Ticketing/AsrServices/1.0'>";
                //stringXML += "            <Header />";
                //stringXML += "            <SelectionCriteria>";
                //stringXML += "                <TicketingProvider>VN</TicketingProvider>";
                //stringXML += "                <DocumentNumber>" + model.DocumentNumber + "</DocumentNumber>";
                //stringXML += "            </SelectionCriteria>";
                //stringXML += "        </DetailRQ>";

                //////child.InnerXml = stringXML;
                //////soapEnvelopeXml.GetElementsByTagName("soapenv:Body")[0].AppendChild(child);
                //////using (Stream stream = request.GetRequestStream())
                //////{
                //////    soapEnvelopeXml.Save(stream);
                //////}
                ////////
                //////using (WebResponse response = request.GetResponse())
                //////{
                //////    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                //////    {
                //////        string soapResult = rd.ReadToEnd();
                //////        soapEnvelopeXml = new XmlDocument();
                //////        soapEnvelopeXml.LoadXml(soapResult);

                //////        XmlWriterSettings settings = new XmlWriterSettings();
                //////        settings.Indent = true;
                //////        // Save the document to a file and auto-indent the output.
                //////        string fileName = "" + model.DocumentNumbers + "_details.xml";
                //////        var urlFile = HttpContext.Current.Server.MapPath(@"~/WS/" + fileName);
                //////        XmlWriter writer = XmlWriter.Create(urlFile, settings);
                //////        soapEnvelopeXml.Save(writer);

                //////        //

                //////        // data test ################################################################################################################################################
                //////        //var urlFileTest = HttpContext.Current.Server.MapPath(@"~/Team/DetailRS.xml");
                //////        //soapEnvelopeXml = new XmlDocument();
                //////        //soapEnvelopeXml.Load(urlFileTest);
                //////        // data test ################################################################################################################################################
                //////        XmlNode salesNode = soapEnvelopeXml.GetElementsByTagName("TT:GetTicketingDocumentRQ")[0];
                //////        ReportSummaryDetail reportSummaryDetails = new ReportSummaryDetail();
                //////        //if (salesNode != null && salesNode.ChildNodes.Count > 0)
                //////        //{
                //////        //    foreach (XmlNode itemDetails in salesNode.ChildNodes)
                //////        //    {
                //////        //        if (itemDetails.LocalName == "AssociatedDocInfo")
                //////        //        {
                //////        //            XmlNodeList xmlNodeList = itemDetails.ChildNodes;
                //////        //            if (xmlNodeList != null)
                //////        //            {
                //////        //                string name = string.Empty;
                //////        //                string documentNumber = model.DocumentNumbers;
                //////        //                string associatedDocument = string.Empty;
                //////        //                string reasonForIssuanceCode = string.Empty;
                //////        //                string reasonForIssuanceDesc = string.Empty;
                //////        //                int couponNumber = 0;
                //////        //                string ticketingProvider = string.Empty;
                //////        //                int flightNumber = 0;
                //////        //                string classOfService = string.Empty;
                //////        //                DateTime departureDtm = DateTime.Now;
                //////        //                DateTime arrivalDtm = DateTime.Now;
                //////        //                string departureCity = string.Empty;
                //////        //                string arrivalCity = string.Empty;
                //////        //                string couponStatus = string.Empty;
                //////        //                string fareBasis = string.Empty;
                //////        //                string baggageAllowance = string.Empty;
                //////        //                //
                //////        //                foreach (XmlNode item in xmlNodeList)
                //////        //                {
                //////        //                    string localName = item.LocalName;
                //////        //                    //
                //////        //                    if (localName == "AssociatedDocument")
                //////        //                        associatedDocument = item.InnerText;
                //////        //                    //
                //////        //                    if (localName == "ReasonForIssuanceCode")
                //////        //                        reasonForIssuanceCode = item.InnerText;
                //////        //                    //
                //////        //                    if (localName == "ReasonForIssuanceDesc")
                //////        //                        reasonForIssuanceDesc = item.InnerText;
                //////        //                    //
                //////        //                    if (localName == "CouponDetail")
                //////        //                    {
                //////        //                        XmlNodeList couponDetailNodeList = item.ChildNodes;
                //////        //                        if (couponDetailNodeList.Count > 0)
                //////        //                        {
                //////        //                            foreach (XmlNode couponDetailNode in couponDetailNodeList)
                //////        //                            {
                //////        //                                localName = couponDetailNode.LocalName;
                //////        //                                string valText = couponDetailNode.InnerText;

                //////        //                                if (localName == "CouponNumber" && !string.IsNullOrWhiteSpace(valText))
                //////        //                                    couponNumber = Convert.ToInt32(valText);
                //////        //                                //
                //////        //                                if (localName == "TicketingProvider")
                //////        //                                    ticketingProvider = valText;
                //////        //                                //
                //////        //                                if (localName == "FlightNumber" && !string.IsNullOrWhiteSpace(valText))
                //////        //                                    flightNumber = Convert.ToInt32(valText);
                //////        //                                //      
                //////        //                                if (localName == "ClassOfService")
                //////        //                                    classOfService = valText;
                //////        //                                // 
                //////        //                                if (localName == "DepartureDtm" && !string.IsNullOrWhiteSpace(valText))
                //////        //                                    departureDtm = Helper.VNALibrary.ConvertToDateTime(valText);
                //////        //                                // 
                //////        //                                if (localName == "ArrivalDtm" && !string.IsNullOrWhiteSpace(valText))
                //////        //                                    arrivalDtm = Helper.VNALibrary.ConvertToDateTime(valText);
                //////        //                                // 
                //////        //                                if (localName == "DepartureCity")
                //////        //                                    departureCity = valText;
                //////        //                                // 
                //////        //                                if (localName == "ArrivalCity")
                //////        //                                    arrivalCity = valText;
                //////        //                                // 
                //////        //                                if (localName == "CouponStatus")
                //////        //                                    couponStatus = valText;
                //////        //                                //
                //////        //                                if (localName == "FareBasis")
                //////        //                                    fareBasis = valText;
                //////        //                                // 
                //////        //                                if (localName == "BaggageAllowance")
                //////        //                                    baggageAllowance = valText;
                //////        //                                //
                //////        //                            }
                //////        //                        }
                //////        //                    }
                //////        //                }
                //////        //                reportSummaryDetails = new ReportSummaryDetail
                //////        //                {
                //////        //                    DocumentNumber = documentNumber,
                //////        //                    AssociatedDocument = associatedDocument,
                //////        //                    ReasonForIssuanceCode = reasonForIssuanceCode,
                //////        //                    ReasonForIssuanceDesc = reasonForIssuanceDesc,
                //////        //                    CouponNumber = couponNumber,
                //////        //                    TicketingProvider = ticketingProvider,
                //////        //                    FlightNumber = flightNumber,
                //////        //                    ClassOfService = classOfService,
                //////        //                    DepartureDtm = Convert.ToString(departureDtm),
                //////        //                    ArrivalDtm = Convert.ToString(arrivalDtm),
                //////        //                    DepartureCity = departureCity,
                //////        //                    ArrivalCity = arrivalCity,
                //////        //                    CouponStatus = couponStatus,
                //////        //                    FareBasis = fareBasis,
                //////        //                    BaggageAllowance = baggageAllowance
                //////        //                };
                //////        //            }
                //////        //        }
                //////        //    }
                //////        //}
                //////        return new ReportSaleSummaryDetailResult
                //////        {
                //////            DocumentNumber = null,
                //////            Status = true,
                //////            SaleSummaryDetail = reportSummaryDetails
                //////        };
                //////    }
                //////}
            }
            catch (Exception ex)
            {
                return new VNA_ReportSaleSummaryDetailResult
                {
                    DocumentNumber = null,
                    Status = false,
                    SaleSummaryDetail = null
                };
            }
        }
        //
        public VNA_ReportSaleSummaryResult ExcuteEprReportClose(VNA_ReportModel model)
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

                        return new VNA_ReportSaleSummaryResult
                        {
                            EmpNumber = model.EmpNumber,
                            Status = true,
                        };
                    }
                }
            }
            catch (Exception)
            {
                return new VNA_ReportSaleSummaryResult
                {
                    EmpNumber = model.EmpNumber,
                    Status = false,
                };
            }
        }
        //
        public VNA_ReportSaleSummaryResult ExcuteEprReportExtend(VNA_ReportModel model)
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
                        return new VNA_ReportSaleSummaryResult
                        {
                            EmpNumber = model.EmpNumber,
                            Status = true,
                        };
                    }
                }
            }
            catch (Exception)
            {
                return new VNA_ReportSaleSummaryResult
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
                string date = "2020-01-01";
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
                ReportSaleSummaryClosedService reportSaleSummaryCloseService = new ReportSaleSummaryClosedService();
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


















        public string Test01(VNA_ReportModel model)
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
                stringXML += "        <DocumentNumber>7382435519474</DocumentNumber>";
                stringXML += "        <DocumentNumber>7382435507975</DocumentNumber>";
                stringXML += "        <DocumentNumber>7382435507976</DocumentNumber>";
                stringXML += "        <DocumentNumber>7382435507977</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435507983</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435507984</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435512115</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435513897</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435519902</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435519903</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435521980</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435521983</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435519602</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435519603</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435509846</DocumentNumber>";
                //stringXML += "        <DocumentNumber>7382435516413</DocumentNumber>";
                stringXML += "    </SearchParameters>";
                stringXML += "</GetTicketingDocumentRQ>";
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

        // ##########################################################################################################################################################################
        public VNA_ReportSaleSummaryTicketing GetSaleReportTicketByDocNumber(string docNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(docNumber))
                    return new VNA_ReportSaleSummaryTicketing();
                //
                using (var sessionService = new VNA_SessionService())
                {
                    TokenModel tokenModel = sessionService.GetSession();
                    HttpWebRequest request = XMLHelper.CreateWebRequest(XMLHelper.URL_WS);
                    XmlDocument soapEnvelopeXml = new XmlDocument();
                    var path = HttpContext.Current.Server.MapPath(@"~/WS/Xml/Common.xml");
                    soapEnvelopeXml.Load(path);
                    soapEnvelopeXml.GetElementsByTagName("eb:Timestamp")[0].InnerText = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss");
                    soapEnvelopeXml.GetElementsByTagName("eb:Service")[0].InnerText = "TicketingDocumentServicesRQ";
                    soapEnvelopeXml.GetElementsByTagName("eb:Action")[0].InnerText = "TicketingDocumentServicesRQ";
                    soapEnvelopeXml.GetElementsByTagName("eb:BinarySecurityToken")[0].InnerText = tokenModel.Token;
                    soapEnvelopeXml.GetElementsByTagName("eb:ConversationId")[0].InnerText = tokenModel.ConversationID;
                    XmlDocumentFragment child = soapEnvelopeXml.CreateDocumentFragment();
                    var stringXML = "";
                    stringXML += "<GetTicketingDocumentRQ Version='3.12.0' xmlns='http://www.sabre.com/ns/Ticketing/DC'>";
                    stringXML += "    <ns1:STL_Header.RQ xmlns:ns1='http://services.sabre.com/STL/v01' />";
                    stringXML += "    <ns2:POS xmlns:ns2='http://services.sabre.com/STL/v01' />";
                    stringXML += "    <SearchParameters>";
                    stringXML += "        <TicketingProvider>VN</TicketingProvider>";
                    stringXML += "        <DocumentNumber>" + docNumber + "</DocumentNumber>";
                    stringXML += "    </SearchParameters>";
                    stringXML += "</GetTicketingDocumentRQ>";
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
                            ////// Save the document to a file and auto-indent the output.
                            ////XmlWriterSettings settings = new XmlWriterSettings();
                            ////settings.Indent = true;
                            ////string fileName = docNumber + "_ticketingdocumentrq_details.xml";
                            ////var urlFile = HttpContext.Current.Server.MapPath(@"~/WS/" + fileName);
                            ////XmlWriter writer = XmlWriter.Create(urlFile, settings);
                            ////soapEnvelopeXml.Save(writer);
                            //
                            // ********************************************************************************************
                            //XmlNode ticketingDocumentNode = soapEnvelopeXml.GetElementsByTagName("TT:GetTicketingDocumentRS")[0];
                            //XmlNode detailsNode = soapEnvelopeXml.GetElementsByTagName("TT:Details")[0];
                            List<VNA_ReportSaleSummaryTicketingDocument> vna_ReportSaleSummaryTicketingDocument = new List<VNA_ReportSaleSummaryTicketingDocument>();
                            XmlNodeList xmlNodeList = soapEnvelopeXml.GetElementsByTagName("TT:ServiceCoupon");
                            if (xmlNodeList.Count > 0)
                            {

                                string marketingFlightNumber = string.Empty;
                                string classOfService = string.Empty;
                                string fareBasis = string.Empty;
                                string startLocation = string.Empty;
                                string endLocation = string.Empty;
                                string bookingStatus = string.Empty;
                                string currentStatus = string.Empty;
                                string systemDateTime = string.Empty;
                                string flownCoupon_DepartureDateTime = string.Empty;
                                foreach (XmlNode itemCoupon in xmlNodeList)
                                {
                                    XmlNodeList xmlNode = itemCoupon.ChildNodes;
                                    if (xmlNode.Count > 0)
                                    {
                                        foreach (XmlNode item in xmlNode)
                                        {
                                            string localName = item.LocalName;
                                            if (localName == "MarketingFlightNumber")
                                                marketingFlightNumber = item.InnerText;
                                            //
                                            if (localName == "ClassOfService")
                                                classOfService = item.InnerText;
                                            // 
                                            if (localName == "FareBasis")
                                                fareBasis = item.InnerText;
                                            //
                                            if (localName == "StartLocation")
                                                startLocation = item.InnerText;
                                            //
                                            if (localName == "EndLocation")
                                                endLocation = item.InnerText;
                                            //
                                            if (localName == "BookingStatus")
                                                bookingStatus = item.InnerText;
                                            //
                                            if (localName == "CurrentStatus")
                                                currentStatus = item.InnerText;
                                            //     
                                            if (localName == "FlownCoupon")
                                            {
                                                XmlNodeList childNodes = item.ChildNodes;
                                                if (childNodes.Count > 0)
                                                {
                                                    foreach (XmlNode itemChild in childNodes)
                                                    {
                                                        localName = itemChild.LocalName;
                                                        if (localName == "DepartureDateTime")
                                                            flownCoupon_DepartureDateTime = itemChild.InnerText;
                                                        //
                                                    }
                                                }
                                            }
                                        }
                                        vna_ReportSaleSummaryTicketingDocument.Add(new VNA_ReportSaleSummaryTicketingDocument
                                        {
                                            MarketingFlightNumber = marketingFlightNumber,
                                            ClassOfService = classOfService,
                                            FareBasis = fareBasis,
                                            StartLocation = startLocation,
                                            EndLocation = endLocation,
                                            BookingStatus = bookingStatus,
                                            CurrentStatus = currentStatus,
                                            FlownCoupon_DepartureDateTime = flownCoupon_DepartureDateTime
                                        });
                                    }
                                }

                            }
                            // get amount
                            VNA_ReportSaleSummaryTicketingDocumentAmount vna_ReportSaleSummaryTicketingDocumentAmount = new VNA_ReportSaleSummaryTicketingDocumentAmount();
                            XmlNode amountNode = soapEnvelopeXml.GetElementsByTagName("TT:Amounts")[0];
                            if (amountNode != null)
                            {
                                XmlNodeList amountChildNodes = amountNode.ChildNodes;
                                if (amountChildNodes.Count > 0)
                                {
                                    double baseAmount = 0;
                                    double totalTax = 0;
                                    double total = 0;
                                    double nonRefundable = 0;
                                    string unit = "";

                                    foreach (XmlNode itemAmountNode in amountChildNodes)
                                    {
                                        string localName = itemAmountNode.LocalName;
                                        if (localName == "New")
                                        {
                                            XmlNodeList newNodes = itemAmountNode.ChildNodes;
                                            if (newNodes.Count > 0)
                                            {
                                                foreach (XmlNode newNode in newNodes)
                                                {
                                                    //
                                                    localName = newNode.LocalName;
                                                    if (localName == "Base")
                                                    {
                                                        XmlNode baseNodeChild = newNode.ChildNodes[0];
                                                        if (baseNodeChild != null && baseNodeChild.LocalName == "Amount")
                                                        {
                                                            baseAmount = Convert.ToDouble(baseNodeChild.InnerText);
                                                            //
                                                            XmlAttribute xmlAttributeUnit = baseNodeChild.Attributes["currencyCode"];
                                                            if (xmlAttributeUnit != null)
                                                            {
                                                                unit = xmlAttributeUnit.Value;
                                                            }
                                                        }
                                                    }
                                                    //
                                                    if (localName == "TotalTax")
                                                    {
                                                        XmlNode totalTaxNode = newNode.ChildNodes[0];
                                                        if (totalTaxNode != null && totalTaxNode.LocalName == "Amount")
                                                        {
                                                            totalTax = Convert.ToDouble(totalTaxNode.InnerText);
                                                        }
                                                    }
                                                    //
                                                    if (localName == "Total")
                                                    {
                                                        XmlNode totalNode = newNode.ChildNodes[0];
                                                        if (totalNode != null && totalNode.LocalName == "Amount")
                                                        {
                                                            total = Convert.ToDouble(totalNode.InnerText);
                                                        }
                                                    }

                                                }
                                            }

                                        }
                                        if (localName == "Other")
                                        {
                                            XmlNode newOther = itemAmountNode.ChildNodes[0];
                                            if (newOther != null && newOther.LocalName == "NonRefundable")
                                            {
                                                XmlNode nonRefundableAmountNode = newOther.ChildNodes[0];
                                                if (nonRefundableAmountNode != null && nonRefundableAmountNode.LocalName == "Amount")
                                                {
                                                    nonRefundable = Convert.ToDouble(nonRefundableAmountNode.InnerText);
                                                }
                                            }

                                        }

                                        //
                                    }
                                    // add to model
                                    vna_ReportSaleSummaryTicketingDocumentAmount.BaseAmount = baseAmount;
                                    vna_ReportSaleSummaryTicketingDocumentAmount.TotalTax = totalTax;
                                    vna_ReportSaleSummaryTicketingDocumentAmount.Total = total;
                                    vna_ReportSaleSummaryTicketingDocumentAmount.NonRefundable = nonRefundable;
                                    vna_ReportSaleSummaryTicketingDocumentAmount.Unit = unit;
                                }
                            }
                            // get taxes 
                            List<VNA_ReportSaleSummaryTicketingDocumentTaxes> vna_ReportSaleSummaryTicketingDocumentTaxes = new List<VNA_ReportSaleSummaryTicketingDocumentTaxes>();
                            XmlNode taxNode = soapEnvelopeXml.GetElementsByTagName("TT:Taxes")[0];
                            if (taxNode != null)
                            {

                                XmlNode newNode = taxNode.ChildNodes[0];
                                if (newNode != null)
                                {
                                    XmlNodeList newChildNodes = newNode.ChildNodes;
                                    if (newChildNodes.Count > 0)
                                    {

                                        //
                                        foreach (XmlNode itemTaxChild in newChildNodes)
                                        {
                                            string taxCode = "";
                                            double taxAmount = 0;
                                            string unit = "";
                                            string localName = itemTaxChild.LocalName;
                                            if (localName == "Tax")
                                            {
                                                XmlAttribute xmlAttributeTaxCode = itemTaxChild.Attributes["code"];
                                                if (xmlAttributeTaxCode != null)
                                                {
                                                    taxCode = Convert.ToString(xmlAttributeTaxCode.Value);
                                                }
                                                //
                                                XmlNode taxAmountNode = itemTaxChild.ChildNodes[0];
                                                if (taxAmountNode != null && taxAmountNode.LocalName == "Amount")
                                                {
                                                    taxAmount = Convert.ToDouble(taxAmountNode.InnerText);
                                                    //
                                                    XmlAttribute xmlAttributeUnit = taxAmountNode.Attributes["currencyCode"];
                                                    if (xmlAttributeUnit != null)
                                                    {
                                                        unit = xmlAttributeUnit.Value;
                                                    }
                                                }
                                                vna_ReportSaleSummaryTicketingDocumentTaxes.Add(new VNA_ReportSaleSummaryTicketingDocumentTaxes
                                                {
                                                    TaxCode = taxCode,
                                                    Amount = taxAmount,
                                                    Unit = unit
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                            // result
                            VNA_ReportSaleSummaryTicketing vna_ReportSaleSummaryTicketing = new VNA_ReportSaleSummaryTicketing
                            {
                                SaleSummaryTicketingDocument = vna_ReportSaleSummaryTicketingDocument,
                                SaleSummaryTicketingDocumentAmount = vna_ReportSaleSummaryTicketingDocumentAmount,
                                SaleSummaryTicketingDocumentTaxes = vna_ReportSaleSummaryTicketingDocumentTaxes
                            };
                            //
                            var data = vna_ReportSaleSummaryTicketing;
                            return vna_ReportSaleSummaryTicketing;
                            //
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
                return new VNA_ReportSaleSummaryTicketing();
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
