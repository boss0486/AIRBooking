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
        [Route("Action/EPR-Rpdata")]
        public ActionResult APIReportData(APIDailyReportModel model)
        {
            try
            {
                ApiReportService apiReportService = new ApiReportService();
                return apiReportService.APIReportData(model);
            }
            catch (Exception ex)
            {
                return Notifization.NotService;
            }

        }
        [HttpPost]
        [Route("Action/EPR-test")]
        public ActionResult APIReportTest()
        {
            
                ApiReportService apiReportService = new ApiReportService();
                var a = apiReportService.APITest();

                return Notifization.Data("ok", a);
             

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