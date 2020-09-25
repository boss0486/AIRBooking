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
using WebCore.Core;
using WebCore.Model.Entities;

namespace WebApplication.Management.Controllers
{

    [IsManage]
    [RouteArea("Management")]
    [RoutePrefix("VNAReport")]
    public class VNAReportController : CMSController
    {
        // GET: Management/VNAReport
        public ActionResult DataList()
        {
            return View();
        }
        public ActionResult Search()
        {
            return View();
        }

        public ActionResult RpDetails(string id)
        {
            ViewData["DocummentNumber"] = id;
            VNA_TKT_AsrService service = new VNA_TKT_AsrService();
            var model = service.GetSaleReportTicketByDocNumber(id);
            if (model != null)
                return View(model);
            //////
            return View();
        }
        public ActionResult Details(string id)
        {
            
            ReportSaleSummaryService service = new ReportSaleSummaryService();
            ReportSaleSummaryModel model = service.GetReportSaleSummaryByDocumentNumber(id);
            if (model != null)
                return View(model);
            //
            return View();
        }

        // API ********************************************************************************************************
        [HttpPost]
        [Route("Action/DataList")]
        public ActionResult DataList(SearchModel model)
        {
            try
            {
                ReportSaleSummaryService service = new ReportSaleSummaryService();
                return service.DataList(model);
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }


        // API search ********************************************************************************************************

        [HttpPost]
        [Route("Action/EPR-Search")]
        public ActionResult EPRSearch(ReportEprSearchModel model)
        {
            try
            {
                if (model == null)
                    return Notifization.Invalid(MessageText.Invalid);
                //
                string inputDate = model.ReportDate;
                string query = model.Query;
                //
                if (!Helper.Page.Validate.TestDateSQL(inputDate))
                    return Notifization.Invalid(MessageText.Invalid);
                // chuan date
                inputDate = Convert.ToDateTime(inputDate).ToString("yyyy-MM-dd");
                //
                DateTime reportDate = Convert.ToDateTime(inputDate);
                VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
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
                    var data = vna_TKT_AsrService.GetEmployeeNumber(empReportModel);
                    //
                    if (data.Count() > 0)
                    {
                        List<VNA_ReportSaleSummaryResult> reportEprResult = new List<VNA_ReportSaleSummaryResult>();
                        foreach (var employee in data)
                        {
                            string empNumber = employee.IssuingAgentEmployeeNumber;
                            VNA_ReportSaleSummaryResult reportSaleSummaryResult = vna_TKT_AsrService.ReportSaleSummaryReport(new VNA_ReportModel
                            {
                                Token = tokenModel.Token,
                                ConversationID = tokenModel.ConversationID,
                                ReportDate = reportDate,
                                EmpNumber = empNumber
                            });
                            reportEprResult.Add(reportSaleSummaryResult);
                        }
                        return Notifization.Data("Ok" + data.Count, reportEprResult);
                    }
                    return Notifization.NotFound("Data is not found");
                }
            }
            catch (Exception ex)
            {
                return Notifization.Error("Error: " + ex);
            }
        }
        // API save for daily report ********************************************************************************************************

        [HttpPost]
        [Route("Action/EPRDailyReport")]
        public ActionResult EPRDailyReport()
        {
            try
            {
                VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();

                string inputDate = vna_TKT_AsrService.GetReportDateInToday();
                if (!Helper.Page.Validate.TestDateSQL(inputDate))
                    return Notifization.Invalid(MessageText.Invalid);
                // chuan date
                inputDate = Convert.ToDateTime(inputDate).ToString("yyyy-MM-dd");
                //
                DateTime reportDate = Convert.ToDateTime(inputDate);
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
                    VNA_EmpReportModel vna_EmpReportModel = new VNA_EmpReportModel
                    {
                        Token = tokenModel.Token,
                        ConversationID = tokenModel.ConversationID,
                        ReportDate = reportDate
                    };
                    // 
                    var data = vna_TKT_AsrService.GetEmployeeNumber(vna_EmpReportModel);
                    if (data.Count() > 0)
                    {
                        List<VNA_ReportSaleSummaryResult> reportSaleSummaryResults = new List<VNA_ReportSaleSummaryResult>();
                        foreach (var employee in data)
                        {
                            string empNumber = employee.IssuingAgentEmployeeNumber;
                            VNA_ReportSaleSummaryResult reportSaleSummaryResult = vna_TKT_AsrService.ReportSaleSummaryReport(new VNA_ReportModel
                            {
                                Token = tokenModel.Token,
                                ConversationID = tokenModel.ConversationID,
                                ReportDate = reportDate,
                                EmpNumber = empNumber
                            });
                            //
                            reportSaleSummaryResults.Add(reportSaleSummaryResult);
                        }
                        return Notifization.Data("Ok" + data.Count, reportSaleSummaryResults);
                    }
                    return Notifization.NotFound("Data is not found");
                }
            }
            catch (Exception ex)
            {
                return Notifization.Error("Error: " + ex);
            }
        }

    }
}