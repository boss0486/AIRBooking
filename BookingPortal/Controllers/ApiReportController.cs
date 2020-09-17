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

namespace APIBooking.VNA.Controllers
{

    [RouteArea("APIBooking")]
    [RoutePrefix("Report")]
    public class ApiReportController : Controller
    {
        // GET: Booking
        [HttpPost]
        [Route("Action/EPR-Date")]
        public ActionResult ReportDate(ReportModel model)
        {
            VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
            string rpDate = vna_TKT_AsrService.GetReportDateInToday();
            if (string.IsNullOrWhiteSpace(rpDate))
                return Notifization.Invalid("Report is invalid");
            //
            return Notifization.TEST("::" + rpDate);

        }
        [HttpPost]
        [Route("Action/EPR-Report")]
        public ActionResult EPRReport()
        {
            try
            {
                VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
                string rpDate = vna_TKT_AsrService.GetReportDateInToday();
                if (string.IsNullOrWhiteSpace(rpDate))
                    return Notifization.Invalid("Report is invalid");
                //
                DateTime reportDate = Convert.ToDateTime(rpDate);
                if (!vna_TKT_AsrService.CheckReportSaleSummary(reportDate))
                    return Notifization.Invalid("Report is executed");
                //
                using (var sessionService = new VNA_SessionService())
                {

                    TokenModel tokenModel = sessionService.GetSession();
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                    designatePrinter.ConversationID = tokenModel.ConversationID;
                    designatePrinter.Token = tokenModel.Token;
                    VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    // model
                    EmpReportModel empReportModel = new EmpReportModel();
                    empReportModel.Token = tokenModel.Token;
                    empReportModel.ConversationID = tokenModel.ConversationID;
                    empReportModel.ReportDate = reportDate;
                    // 
                    var data = vna_TKT_AsrService.GetEmployeeNumber(empReportModel);
                    if (data.Count() > 0)
                    {
                        List<ReportEprResult> reportEprResults = new List<ReportEprResult>();
                        foreach (var employee in data)
                        {
                            ReportSaleSummaryResult reportSaleSummaryResult = vna_TKT_AsrService.ReportSaleSummaryReport(new ReportModel
                            {
                                Token = tokenModel.Token,
                                ConversationID = tokenModel.ConversationID,
                                ReportDate = reportDate,
                                EmpNumber = employee.IssuingAgentEmployeeNumber
                            });
                            // call service details
                            List<string> statusSaveData = new List<string>();
                            List<ReportSaleSummaryDetailsResult> reportSaleSummaryDetailsResults = new List<ReportSaleSummaryDetailsResult>();
                            if (reportSaleSummaryResult != null)
                            {
                                List<ReportSaleSummaryTransaction> reportSaleSummaryTransactions = reportSaleSummaryResult.SaleSummaryTransaction;
                                if (reportSaleSummaryTransactions.Count() > 0)
                                {
                                    foreach (var itemTrans in reportSaleSummaryTransactions)
                                    {
                                        if (itemTrans.DocumentType != ("TKT"))
                                        {
                                            reportSaleSummaryDetailsResults.Add(new ReportSaleSummaryDetailsResult
                                            {
                                                DocumentNumber = itemTrans.DocumentNumber,
                                                Status = false
                                            });
                                        }
                                        else
                                        {
                                            ReportSaleSummaryDetailsResult reportSaleSummaryDetailsResult = vna_TKT_AsrService.EprReportSaleReportDetails(new ReportDetailsModel
                                            {
                                                Token = tokenModel.Token,
                                                ConversationID = tokenModel.ConversationID,
                                                ReportDate = reportDate,
                                                DocumentNumber = itemTrans.DocumentNumber
                                            });
                                            // save details
                                            if (reportSaleSummaryDetailsResult != null)
                                            {
                                                ReportSummaryDetails reportSummaryDetails = reportSaleSummaryDetailsResult.SaleSummaryDetails;
                                                if (reportSummaryDetails != null)
                                                {
                                                    string stateReportSummaryDetails = vna_TKT_AsrService.ReportSaleSummaryDetailsSave(new ReportSaleSummaryDetailsCreate
                                                    {
                                                        DocumentNumber = reportSummaryDetails.DocumentNumber,
                                                        AssociatedDocument = reportSummaryDetails.AssociatedDocument,
                                                        ReasonForIssuanceCode = reportSummaryDetails.ReasonForIssuanceCode,
                                                        ReasonForIssuanceDesc = reportSummaryDetails.ReasonForIssuanceDesc,
                                                        CouponNumber = reportSummaryDetails.CouponNumber,
                                                        TicketingProvider = reportSummaryDetails.TicketingProvider,
                                                        FlightNumber = reportSummaryDetails.FlightNumber,
                                                        ClassOfService = reportSummaryDetails.ClassOfService,
                                                        DepartureDtm = Convert.ToDateTime(reportSummaryDetails.DepartureDtm),
                                                        ArrivalDtm = Convert.ToDateTime(reportSummaryDetails.ArrivalDtm),
                                                        DepartureCity = reportSummaryDetails.DepartureCity,
                                                        ArrivalCity = reportSummaryDetails.ArrivalCity,
                                                        CouponStatus = reportSummaryDetails.CouponStatus,
                                                        FareBasis = reportSummaryDetails.FareBasis,
                                                        BaggageAllowance = reportSummaryDetails.BaggageAllowance
                                                    });
                                                }
                                            }
                                            //
                                            reportSaleSummaryDetailsResults.Add(reportSaleSummaryDetailsResult);
                                        }
                                        // save report transaction
                                        string stateReportSaleSummarySave = vna_TKT_AsrService.ReportSaleSummarySave(itemTrans, reportDate, employee.IssuingAgentEmployeeNumber);
                                        statusSaveData.Add(stateReportSaleSummarySave);
                                    }
                                }
                            }
                            reportEprResults.Add(new ReportEprResult
                            {
                                SaleSummary = reportSaleSummaryResult,
                                SaleSummaryDetails = reportSaleSummaryDetailsResults
                            });
                        }
                        return Notifization.Data("Ok" + data.Count, reportEprResults);
                    }
                    return Notifization.NotFound("Data is not found");
                }
            }
            catch (Exception ex)
            {
                return Notifization.Error("Error: " + ex);
            }
        }

        [HttpPost]
        [Route("Action/EPR-checkdate")]
        public ActionResult Test(ReportModel model)
        {
            VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
            string rpDate = vna_TKT_AsrService.GetReportDateInToday();
            if (string.IsNullOrWhiteSpace(rpDate))
                return Notifization.Invalid("Report is invalid");
            //
            DateTime reportDate = Convert.ToDateTime(rpDate);
            return Notifization.TEST("" + vna_TKT_AsrService.CheckReportSaleSummary(reportDate));
        }

        [HttpPost]
        [Route("Action/EPR-ReportClose")]
        public ActionResult EPRReportClose(EmpReportModel model)
        {
            try
            {
                VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
                string rpDate = vna_TKT_AsrService.GetReportDateInToday();
                if (string.IsNullOrWhiteSpace(rpDate))
                    return Notifization.Invalid("Report is invalid");
                //
                DateTime reportDate = Convert.ToDateTime(rpDate);
                if (!vna_TKT_AsrService.CheckReportSaleSummaryClose("", reportDate))
                    return Notifization.Invalid("Report is executed");
                //
                using (var sessionService = new VNA_SessionService())
                {
                    DateTime date = model.ReportDate;
                    TokenModel tokenModel = sessionService.GetSession();
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                    designatePrinter.ConversationID = tokenModel.ConversationID;
                    designatePrinter.Token = tokenModel.Token;
                    VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    // model
                    EmpReportModel empReportModel = new EmpReportModel();
                    empReportModel.Token = tokenModel.Token;
                    empReportModel.ConversationID = tokenModel.ConversationID;
                    empReportModel.ReportDate = date;
                    //
                    VNA_TKT_AsrService vNA_TKT_AsrService = new VNA_TKT_AsrService();
                    var data = vNA_TKT_AsrService.GetEmployeeNumber(empReportModel);
                    if (data.Count() > 0)
                    {
                        List<ReportSaleSummaryResult> ePRReportResultModels = new List<ReportSaleSummaryResult>();
                        foreach (var item in data)
                        {
                            var salesSummaryRS = vNA_TKT_AsrService.ExcuteEprReportClose(new ReportModel
                            {
                                Token = tokenModel.Token,
                                ConversationID = tokenModel.ConversationID,
                                ReportDate = date,
                                EmpNumber = item.IssuingAgentEmployeeNumber
                            });
                            //
                            ePRReportResultModels.Add(salesSummaryRS);
                            // insert data to database
                        }
                        return Notifization.Data("Ok" + data.Count, ePRReportResultModels);
                    }
                    return Notifization.NotFound("Data is not found");
                }
            }
            catch (Exception ex)
            {
                return Notifization.Error("Error: " + ex);
            }
        }

        [HttpPost]
        [Route("Action/EPR-ReportExtend")]
        public ActionResult EPRReportExtend(EmpReportModel model)
        {
            try
            {
                using (var sessionService = new VNA_SessionService())
                {
                    DateTime date = model.ReportDate;
                    TokenModel tokenModel = sessionService.GetSession();
                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                    designatePrinter.ConversationID = tokenModel.ConversationID;
                    designatePrinter.Token = tokenModel.Token;
                    VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                    // model
                    ReportModel reportModel = new ReportModel
                    {
                        Token = tokenModel.Token,
                        ConversationID = tokenModel.ConversationID,
                        ReportDate = date
                    };
                    //
                    VNA_TKT_AsrService vNA_TKT_AsrService = new VNA_TKT_AsrService();
                    ReportSaleSummaryResult eprSaleReportResult = vNA_TKT_AsrService.ExcuteEprReportExtend(reportModel);
                    //
                    return Notifization.Data("Data is not found", eprSaleReportResult);
                }
            }
            catch (Exception ex)
            {
                return Notifization.Error("Error: " + ex);
            }
        }

        // #########################################################################################################################
        [HttpPost]
        [Route("Action/EPR-DetailsByDocumentNumber")]
        public ActionResult GetDetailsByDocumentNumber()
        {
            VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
            using (var sessionService = new VNA_SessionService())
            {

                TokenModel tokenModel = sessionService.GetSession();
                DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                designatePrinter.ConversationID = tokenModel.ConversationID;
                designatePrinter.Token = tokenModel.Token;
                VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                // model
                ReportSaleSummaryDetailsResult reportSaleSummaryDetailsResult = vna_TKT_AsrService.EprReportSaleReportDetails(new ReportDetailsModel
                {
                    Token = tokenModel.Token,
                    ConversationID = tokenModel.ConversationID,
                    ReportDate = Convert.ToDateTime("2020-08-29"),
                    DocumentNumber = "7382444368772"
                });
                // save details
                return Notifization.NotFound("Data is not found");
            }
        }

        // #########################################################################################################################
        [HttpPost]
        [Route("Action/EPR-Test01")]
        public ActionResult Test01()
        {
            VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
            using (var sessionService = new VNA_SessionService())
            {

                TokenModel tokenModel = sessionService.GetSession();
                DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
                designatePrinter.ConversationID = tokenModel.ConversationID;
                designatePrinter.Token = tokenModel.Token;
                VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
                var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
                // model
                string reportSaleSummaryDetailsResult = vna_TKT_AsrService.Test01(new ReportModel
                {
                    Token = tokenModel.Token,
                    ConversationID = tokenModel.ConversationID,
                });
                // save details
                return Notifization.TEST("::" + reportSaleSummaryDetailsResult);
            }
        }
    }
}