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

        // API ********************************************************************************************************

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

        //[HttpPost]
        //[Route("Action/DataList")]
        //public ActionResult EPRReport(ReportEprModel model)
        //{
        //    try
        //    {
        //        string inputDate = model.ReportDate;
        //        if (!Helper.Page.Validate.TestDateSQL(inputDate))
        //            return Notifization.Invalid(MessageText.Invalid);
        //        // chuan date
        //        inputDate = Convert.ToDateTime(inputDate).ToString("yyyy-MM-dd");
        //        //
        //        DateTime reportDate = Convert.ToDateTime(inputDate);
        //        VNA_TKT_AsrService vna_TKT_AsrService = new VNA_TKT_AsrService();
        //        using (var sessionService = new VNA_SessionService())
        //        {

        //            TokenModel tokenModel = sessionService.GetSession();
        //            DesignatePrinterLLSModel designatePrinter = new DesignatePrinterLLSModel();
        //            designatePrinter.ConversationID = tokenModel.ConversationID;
        //            designatePrinter.Token = tokenModel.Token;
        //            VNA_DesignatePrinterLLSRQService wSDesignatePrinterLLSRQService = new VNA_DesignatePrinterLLSRQService();
        //            var printer = wSDesignatePrinterLLSRQService.DesignatePrinterLLS(designatePrinter);
        //            // model
        //            EmpReportModel empReportModel = new EmpReportModel();
        //            empReportModel.Token = tokenModel.Token;
        //            empReportModel.ConversationID = tokenModel.ConversationID;
        //            empReportModel.ReportDate = reportDate;
        //            // 
        //            var data = vna_TKT_AsrService.GetEmployeeNumber(empReportModel);
        //            if (data.Count() > 0)
        //            {
        //                List<ReportSaleSummaryResult> reportEprResult = new List<ReportSaleSummaryResult>();
        //                foreach (var employee in data)
        //                {
        //                    string empNumber = employee.IssuingAgentEmployeeNumber;
        //                    ReportSaleSummaryResult reportSaleSummaryResult = vna_TKT_AsrService.ReportSaleSummaryReport(new ReportModel
        //                    {
        //                        Token = tokenModel.Token,
        //                        ConversationID = tokenModel.ConversationID,
        //                        ReportDate = reportDate,
        //                        EmpNumber = empNumber
        //                    });
        //                    //
        //                    reportEprResult.Add(reportSaleSummaryResult);
        //                }
        //                return Notifization.Data("Ok" + data.Count, reportEprResult);
        //            }
        //            return Notifization.NotFound("Data is not found");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Notifization.Error("Error: " + ex);
        //    }
        //}
    }
}