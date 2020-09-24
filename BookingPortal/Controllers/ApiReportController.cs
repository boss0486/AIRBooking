using System;
using ApiPortalBooking.Models;
using AIRService.Entities;
using AIRService.Models;
using AIR.Helper.Session;
using System.Web.Mvc;
using AIRService.WS.Service;
using System.Collections.Generic;
using AIRService.WebService.VNA.Enum;
using AIRService.WS.Entities;
using System.Linq;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using ApiPortalBooking.Models.VNA_WS_Model;
using WebCore.Services;
using WebCore.Entities;
using System.IO;
using System.Web;
using WebCore.ENM;
using Helper;
using AIRService.WebService.VNA_OTA_AirTaxRQ;
using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
 
namespace APIBooking.VNA.Controllers
{

    [RouteArea("APIBooking")]
    [RoutePrefix("Report")]
    public class ApiReportController : Controller
    {
        //        // GET: Booking
        //        [HttpPost]
        //        [Route("Action/EPR-Date")]
        //        public ActionResult ReportDate()
        //        {
        //            VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
        //            string rpDate = vna_TKT_AsrService.GetReportDateInToday();
        //            if (string.IsNullOrWhiteSpace(rpDate))
        //                return Notifization.Invalid("Report is invalid");
        //            //
        //            return Notifization.Success("::" + rpDate);
        //        }

        /// #1. run get data
        /// #2. save data

        [HttpPost]
        [Route("Action/EPR-Daily-GetReport")]
        public ActionResult EPRReport()
        {
             
            var _conn = DbConnect.Connection.CMS;
            _conn.Open();
            using (var _transaction = _conn.BeginTransaction())
            {
                try
                {
                    // check report
                    VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
                    string rpDate = vna_TKT_AsrService.GetReportDateInToday();
                    if (string.IsNullOrWhiteSpace(rpDate))
                        return Notifization.Invalid("Report date is invalid");
                    //
                    DateTime reportDate = Convert.ToDateTime(rpDate);
                    // delete old report
                    ReportSaleSummaryService reportSaleSummaryService = new ReportSaleSummaryService(_conn);
                    string sqlQuery = @"SELECT DocumentNumber FROM App_ReportSaleSummary WHERE cast(ReportDate as Date) = cast('" + reportDate + "' as Date)";
                    List<string> lstDocmummentNumber = reportSaleSummaryService.Query<string>(sqlQuery, new { ReportDate = reportDate }, transaction: _transaction).ToList();
                    if (lstDocmummentNumber.Count > 0)
                    {
                        reportSaleSummaryService.Execute("DELETE App_ReportSaleSummary WHERE ReportDate = @ReportDate AND DocumentNumber IN ('" + String.Join("','", lstDocmummentNumber) + "')", new { ReportDate = reportDate }, transaction: _transaction);
                        reportSaleSummaryService.Execute("DELETE App_ReportTicketingDocument_Coupon WHERE ReportDate = @ReportDate AND DocumentNumber IN ('" + String.Join("','", lstDocmummentNumber) + "')", new { ReportDate = reportDate }, transaction: _transaction);
                        reportSaleSummaryService.Execute("DELETE App_ReportTicketingDocument_Amount WHERE ReportDate = @ReportDate AND DocumentNumber IN ('" + String.Join("','", lstDocmummentNumber) + "')", new { ReportDate = reportDate }, transaction: _transaction);
                        reportSaleSummaryService.Execute("DELETE App_ReportTicketingDocument_Taxes WHERE ReportDate = @ReportDate AND DocumentNumber IN ('" + String.Join("','", lstDocmummentNumber) + "')", new { ReportDate = reportDate }, transaction: _transaction);
                    }
                    //
                    using (var sessionService = new VNA_SessionService())
                    {
                        TokenModel tokenModel = sessionService.GetSession();
                        DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel
                        {
                            ConversationID = tokenModel.ConversationID,
                            Token = tokenModel.Token
                        };
                        VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                        var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                        // model
                        VNA_EmpReportModel empReportModel = new VNA_EmpReportModel
                        {
                            Token = tokenModel.Token,
                            ConversationID = tokenModel.ConversationID,
                            ReportDate = reportDate
                        };
                        // 
                        var empData = vna_TKT_AsrService.GetEmployeeNumber(empReportModel);
                        if (empData == null || empData.Count() == 0)
                            return Notifization.NotFound("Employee is not found");
                        //
                        foreach (var employee in empData)
                        {
                            string empNumber = employee.IssuingAgentEmployeeNumber;
                            VNA_ReportSaleSummaryResult reportSaleSummaryResult = vna_TKT_AsrService.ReportSaleSummaryReport(new VNA_ReportModel
                            {
                                Token = tokenModel.Token,
                                ConversationID = tokenModel.ConversationID,
                                ReportDate = reportDate,
                                EmpNumber = empNumber
                            });
                            //get tickketing
                            List<string> docummentNunberForDetails = new List<string>();
                            if (reportSaleSummaryResult != null)
                            {
                                List<ReportSaleSummaryTransaction> reportSaleSummaryTransactions = reportSaleSummaryResult.SaleSummaryTransaction;
                                if (reportSaleSummaryTransactions.Count() > 0)
                                {
                                    foreach (var itemTrans in reportSaleSummaryTransactions)
                                    {
                                        // save sale summary
                   
                                        App_ReportSaleSummarySSFopService reportTransactionSSFopService = new App_ReportSaleSummarySSFopService(_conn);
                                        string reportTransactionId = reportSaleSummaryService.Create<string>(new ReportSaleSummary
                                        {
                                            Title = empNumber,
                                            EmployeeNumber = empNumber,
                                            DocumentType = itemTrans.DocumentType,
                                            DocumentNumber = itemTrans.DocumentNumber,
                                            PassengerName = itemTrans.PassengerName,
                                            PnrLocator = itemTrans.PnrLocator,
                                            TicketPrinterLniata = itemTrans.TicketPrinterLniata,
                                            TransactionTime = itemTrans.TransactionTime,
                                            ExceptionItem = itemTrans.ExceptionItem,
                                            DecoupleItem = itemTrans.DecoupleItem,
                                            TicketStatusCode = itemTrans.TicketStatusCode,
                                            IsElectronicTicket = itemTrans.IsElectronicTicket,
                                            ReportDate = reportDate
                                        }, transaction: _transaction);
                                        ApiPortalBooking.Models.VNA_WS_Model.ReportSaleSummaryTransactionSSFop ePRReportTransactionSSFop = itemTrans.SaleSummaryTransactionSSFop;
                                        if (ePRReportTransactionSSFop != null)
                                        {
                                            reportTransactionSSFopService.Create<string>(new ReportSaleSummarySSFop
                                            {
                                                Title = ePRReportTransactionSSFop.FopCode,
                                                ReportTransactionID = reportTransactionId,
                                                CurrencyCode = ePRReportTransactionSSFop.CurrencyCode,
                                                FareAmount = ePRReportTransactionSSFop.FareAmount,
                                                TaxAmount = ePRReportTransactionSSFop.TaxAmount,
                                                TotalAmount = ePRReportTransactionSSFop.TotalAmount,
                                            }, transaction: _transaction);
                                        }
                                        // save tiketting with  documment type value = tkt
                                        string docummentNumber = itemTrans.DocumentNumber;
                                        if (itemTrans.DocumentType == ("TKT") && !string.IsNullOrWhiteSpace(docummentNumber))
                                        {
                                            VNA_ReportSaleSummaryTicketing vna_ReportSaleSummaryTicketing = vna_TKT_AsrService.GetSaleReportTicketByDocNumber(docummentNumber);
                                            if (vna_ReportSaleSummaryTicketing != null)
                                            {
                                                List<VNA_ReportSaleSummaryTicketingDocument> vna_ReportSaleSummaryTicketingDocuments = vna_ReportSaleSummaryTicketing.SaleSummaryTicketingDocument;
                                                VNA_ReportSaleSummaryTicketingDocumentAmount vna_ReportSaleSummaryTicketingDocumentAmount = vna_ReportSaleSummaryTicketing.SaleSummaryTicketingDocumentAmount;
                                                List<VNA_ReportSaleSummaryTicketingDocumentTaxes> vna_ReportSaleSummaryTicketingDocumentTaxes = vna_ReportSaleSummaryTicketing.SaleSummaryTicketingDocumentTaxes;
                                                //
                                                if (vna_ReportSaleSummaryTicketingDocuments != null && vna_ReportSaleSummaryTicketingDocuments.Count > 0)
                                                {
                                                    ReportTicketingDocumentCouponService reportTicketingDocumentCouponService = new ReportTicketingDocumentCouponService(_conn);
                                                    foreach (var item in vna_ReportSaleSummaryTicketingDocuments)
                                                    {
                                                        reportTicketingDocumentCouponService.Create<string>(new ReportTicketingDocumentCoupon
                                                        {
                                                            Title = null,
                                                            Summary = null,
                                                            ReportDate = reportDate,
                                                            DocumentNumber = docummentNumber,
                                                            MarketingFlightNumber = item.MarketingFlightNumber,
                                                            ClassOfService = item.ClassOfService,
                                                            FareBasis = item.FareBasis,
                                                            StartLocation = item.StartLocation,
                                                            EndLocation = item.EndLocation,
                                                            BookingStatus = item.BookingStatus,
                                                            CurrentStatus = item.CurrentStatus,
                                                            SystemDateTime = item.SystemDateTime,
                                                            FlownCoupon_DepartureDateTime = item.FlownCoupon_DepartureDateTime
                                                        }, transaction: _transaction);
                                                    }
                                                }
                                                //
                                                if (vna_ReportSaleSummaryTicketingDocumentAmount != null)
                                                {
                                                    ReportTicketingDocumentAmountService reportTicketingDocumentAmountService = new ReportTicketingDocumentAmountService(_conn);
                                                    string unit = vna_ReportSaleSummaryTicketingDocumentAmount.Unit;
                                                    double baseAmount = vna_ReportSaleSummaryTicketingDocumentAmount.BaseAmount;
                                                    double totalTax = vna_ReportSaleSummaryTicketingDocumentAmount.TotalTax;
                                                    double total = vna_ReportSaleSummaryTicketingDocumentAmount.Total;
                                                    double nonRefundable = vna_ReportSaleSummaryTicketingDocumentAmount.NonRefundable;
                                                    //
                                                    reportTicketingDocumentAmountService.Create<string>(new ReportTicketingDocumentAmount
                                                    {
                                                        ReportDate = reportDate,
                                                        DocumentNumber = docummentNumber,
                                                        BaseAmount = baseAmount,
                                                        TotalTax = totalTax,
                                                        Total = total,
                                                        NonRefundable = nonRefundable,
                                                        Unit = unit
                                                    }, transaction: _transaction);
                                                }
                                                //
                                                if (vna_ReportSaleSummaryTicketingDocumentTaxes != null && vna_ReportSaleSummaryTicketingDocumentTaxes.Count > 0)
                                                {
                                                    ReportTicketingDocumentTaxesService reportTicketingDocumentTaxes = new ReportTicketingDocumentTaxesService(_conn);
                                                    foreach (var item in vna_ReportSaleSummaryTicketingDocumentTaxes)
                                                    { 
                                                        reportTicketingDocumentTaxes.Create<string>(new ReportTicketingDocumentTaxes
                                                        {
                                                            ReportDate = reportDate,
                                                            DocumentNumber = docummentNumber,
                                                            TaxCode = item.TaxCode,
                                                            Amount = item.Amount,
                                                            Unit = item.Unit
                                                        }, transaction: _transaction);
                                                    }
                                                }
                                            }
                                        }
                                        //
                                    }
                                }
                            }
                        }

                    }
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.Error(MessageText.NotService + ex);
                }
            }
        }
        //        [HttpPost]
        //        [Route("Action/EPR-ReportClose")]
        //        public ActionResult EPRReportClose(VNA_EmpReportModel model)
        //        {
        //            try
        //            {
        //                VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
        //                string rpDate = vna_TKT_AsrService.GetReportDateInToday();
        //                if (string.IsNullOrWhiteSpace(rpDate))
        //                    return Notifization.Invalid("Report is invalid");
        //                //
        //                DateTime reportDate = Convert.ToDateTime(rpDate);
        //                if (!vna_TKT_AsrService.CheckReportSaleSummaryClose("", reportDate))
        //                    return Notifization.Invalid("Report is executed");
        //                //
        //                using (var sessionService = new VNA_SessionService())
        //                {
        //                    DateTime date = model.ReportDate;
        //                    TokenModel tokenModel = sessionService.GetSession();
        //                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
        //                    designatePrinter.ConversationID = tokenModel.ConversationID;
        //                    designatePrinter.Token = tokenModel.Token;
        //                    VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
        //                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
        //                    // model
        //                    VNA_EmpReportModel empReportModel = new VNA_EmpReportModel();
        //                    empReportModel.Token = tokenModel.Token;
        //                    empReportModel.ConversationID = tokenModel.ConversationID;
        //                    empReportModel.ReportDate = date;
        //                    //
        //                    VNA_TKT_AsrService vNA_TKT_AsrService = new VNA_TKT_AsrService();
        //                    var data = vNA_TKT_AsrService.GetEmployeeNumber(empReportModel);
        //                    if (data.Count() > 0)
        //                    {
        //                        List<VNA_ReportSaleSummaryResult> ePRReportResultModels = new List<VNA_ReportSaleSummaryResult>();
        //                        foreach (var item in data)
        //                        {
        //                            var salesSummaryRS = vNA_TKT_AsrService.ExcuteEprReportClose(new VNA_ReportModel
        //                            {
        //                                Token = tokenModel.Token,
        //                                ConversationID = tokenModel.ConversationID,
        //                                ReportDate = date,
        //                                EmpNumber = item.IssuingAgentEmployeeNumber
        //                            });
        //                            //
        //                            ePRReportResultModels.Add(salesSummaryRS);
        //                            // insert data to database
        //                        }
        //                        return Notifization.Data("Ok" + data.Count, ePRReportResultModels);
        //                    }
        //                    return Notifization.NotFound("Data is not found");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return Notifization.Error("Error: " + ex);
        //            }
        //        }

        //        [HttpPost]
        //        [Route("Action/EPR-ReportExtend")]
        //        public ActionResult EPRReportExtend(VNA_EmpReportModel model)
        //        {
        //            try
        //            {
        //                using (var sessionService = new VNA_SessionService())
        //                {
        //                    DateTime date = model.ReportDate;
        //                    TokenModel tokenModel = sessionService.GetSession();
        //                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
        //                    designatePrinter.ConversationID = tokenModel.ConversationID;
        //                    designatePrinter.Token = tokenModel.Token;
        //                    VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
        //                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
        //                    // model
        //                    VNA_ReportModel reportModel = new VNA_ReportModel
        //                    {
        //                        Token = tokenModel.Token,
        //                        ConversationID = tokenModel.ConversationID,
        //                        ReportDate = date
        //                    };
        //                    //
        //                    VNA_TKT_AsrService vNA_TKT_AsrService = new VNA_TKT_AsrService();
        //                    VNA_ReportSaleSummaryResult eprSaleReportResult = vNA_TKT_AsrService.ExcuteEprReportExtend(reportModel);
        //                    //
        //                    return Notifization.Data("Data is not found", eprSaleReportResult);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return Notifization.Error("Error: " + ex);
        //            }
        //        }

        //        // #########################################################################################################################
        //        //[HttpPost]
        //        //[Route("Action/EPR-DetailsByDocumentNumber")]
        //        //public ActionResult GetDetailsByDocumentNumber()
        //        //{
        //        //    VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
        //        //    using (var sessionService = new VNA_SessionService())
        //        //    {

        //        //        TokenModel tokenModel = sessionService.GetSession();
        //        //        DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
        //        //        designatePrinter.ConversationID = tokenModel.ConversationID;
        //        //        designatePrinter.Token = tokenModel.Token;
        //        //        VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
        //        //        var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
        //        //        // model
        //        //        List<string> lstDocNumber = new List<string>();
        //        //        lstDocNumber.Add("7382435519474");// 
        //        //        ReportSaleSummaryDetailResult reportSaleSummaryDetailsResult = vna_TKT_AsrService.EprReportSaleReportDetails(new ReportDetailsModel
        //        //        {
        //        //            Token = tokenModel.Token,
        //        //            ConversationID = tokenModel.ConversationID,
        //        //            ReportDate = Convert.ToDateTime("2020-01-01"),
        //        //            DocumentNumbers = lstDocNumber
        //        //        });
        //        //        // save details
        //        //        return Notifization.NotFound("Data is not found");
        //        //    }
        //        //}

        //        // #########################################################################################################################
        //        [HttpPost]
        //        [Route("Action/EPR-Test01")]
        //        public ActionResult Test01()
        //        {
        //            VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
        //            using (var sessionService = new VNA_SessionService())
        //            {

        //                TokenModel tokenModel = sessionService.GetSession();
        //                DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
        //                designatePrinter.ConversationID = tokenModel.ConversationID;
        //                designatePrinter.Token = tokenModel.Token;
        //                VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
        //                var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
        //                // model
        //                string reportSaleSummaryDetailsResult = vna_TKT_AsrService.Test01(new VNA_ReportModel
        //                {
        //                    Token = tokenModel.Token,
        //                    ConversationID = tokenModel.ConversationID,
        //                });
        //                // save details
        //                return Notifization.TEST("::" + reportSaleSummaryDetailsResult);
        //            }
        //        }
    }
}