//using System;
//using ApiPortalBooking.Models;
//using AIRService.Entities;
//using AIRService.Models;
//using AIR.Helper.Session;
//using System.Web.Mvc;
//using AIRService.WS.Service;
//using System.Collections.Generic;
//using AIRService.WebService.VNA.Enum;
//using AIRService.WS.Entities;
//using System.Linq;
//using System.Web.Script.Serialization;
//using Newtonsoft.Json;
//using ApiPortalBooking.Models.VNA_WS_Model;
//using WebCore.Services;
//using WebCore.Entities;
//using System.IO;
//using System.Web;
//using WebCore.ENM;
//using Helper;
//using AIRService.WebService.VNA_OTA_AirTaxRQ;

//namespace APIBooking.VNA.Controllers
//{

//    [RouteArea("APIBooking")]
//    [RoutePrefix("Report")]
//    public class ApiReportController : Controller
//    {
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


//        [HttpPost]
//        [Route("Action/EPR-Report")]
//        public ActionResult EPRReport(ReportEprSearchModel model)
//        {
//            try
//            {
//                if (model == null)
//                    return Notifization.Invalid(MessageText.Invalid);
//                //
//                string inputDate = model.ReportDate;
//                string query = model.Query;
//                //
//                if (!Helper.Page.Validate.TestDateSQL(inputDate))
//                    return Notifization.Invalid(MessageText.Invalid);
//                // chuan date
//                inputDate = Convert.ToDateTime(inputDate).ToString("yyyy-MM-dd");
//                //
//                DateTime reportDate = Convert.ToDateTime(inputDate);
//                VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
//                using (var sessionService = new VNA_SessionService())
//                {

//                    TokenModel tokenModel = sessionService.GetSession();
//                    DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel
//                    {
//                        ConversationID = tokenModel.ConversationID,
//                        Token = tokenModel.Token
//                    };
//                    VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
//                    var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
//                    // model
//                    VNA_EmpReportModel empReportModel = new VNA_EmpReportModel
//                    {
//                        Token = tokenModel.Token,
//                        ConversationID = tokenModel.ConversationID,
//                        ReportDate = reportDate
//                    };
//                    // 
//                    var data = vna_TKT_AsrService.GetEmployeeNumber(empReportModel);
//                    if (!string.IsNullOrWhiteSpace(query))
//                        data = data.Where(m => m.IssuingAgentEmployeeNumber == query).ToList();
//                    //
//                    if (data.Count() > 0)
//                    {
//                        List<VNA_ReportSaleSummaryResult> reportEprResult = new List<VNA_ReportSaleSummaryResult>();





//                        foreach (var employee in data)
//                        {
//                            string empNumber = employee.IssuingAgentEmployeeNumber;
//                            VNA_ReportSaleSummaryResult reportSaleSummaryResult = vna_TKT_AsrService.ReportSaleSummaryReport(new VNA_ReportModel
//                            {
//                                Token = tokenModel.Token,
//                                ConversationID = tokenModel.ConversationID,
//                                ReportDate = reportDate,
//                                EmpNumber = empNumber
//                            });
//                            reportEprResult.Add(reportSaleSummaryResult);
//                            // call service details
//                            //List<string> statusSaveData = new List<string>();
//                            //List<ReportSaleSummaryDetailResult> reportSaleSummaryDetailResult = new List<ReportSaleSummaryDetailResult>();

//                            //if (reportSaleSummaryResult != null)
//                            //{
//                            //    List<ReportSaleSummaryTransaction> reportSaleSummaryTransactions = reportSaleSummaryResult.SaleSummaryTransaction;
//                            //    if (reportSaleSummaryTransactions.Count() > 0)
//                            //    {

//                            //        foreach (var itemTrans in reportSaleSummaryTransactions)
//                            //        {
//                            //            if (itemTrans.DocumentType != ("TKT"))
//                            //            {
//                            //                reportSaleSummaryDetailResult.Add(new ReportSaleSummaryDetailResult
//                            //                {
//                            //                    DocumentNumber = itemTrans.DocumentNumber,
//                            //                    StatusGetDetail = false,
//                            //                    Status = false
//                            //                });
//                            //            }
//                            //            else
//                            //            {
//                            //                // 
//                            //                string docNumber = itemTrans.DocumentNumber;
//                            //                reportSaleSummaryDetailResult.Add(new ReportSaleSummaryDetailResult
//                            //                {
//                            //                    DocumentNumber = docNumber,
//                            //                    StatusGetDetail = true,
//                            //                    Status = true
//                            //                });
//                            //            }
//                            //        }
//                            //    }
//                            //}
//                            // result
//                            //
//                        }
//                        return Notifization.Data("Ok" + data.Count, reportEprResult);
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
//        [Route("Action/EPR-checkdate")]
//        public ActionResult Test(VNA_ReportModel model)
//        {
//            VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
//            string rpDate = vna_TKT_AsrService.GetReportDateInToday();
//            if (string.IsNullOrWhiteSpace(rpDate))
//                return Notifization.Invalid("Report is invalid");
//            //
//            DateTime reportDate = Convert.ToDateTime(rpDate);
//            return Notifization.TEST("" + vna_TKT_AsrService.CheckReportSaleSummary(reportDate));
//        }

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
//    }
//}