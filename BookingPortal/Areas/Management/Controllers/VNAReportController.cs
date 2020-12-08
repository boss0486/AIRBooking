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
using WebCore.Services;
using WebCore.Entities;
using System.IO;
using System.Web;
using WebCore.ENM;
using Helper;
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
        public ActionResult SyncData()
        {
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
        [Route("Action/Search")]
        public ActionResult EPRSearch(ReportEprSearchModel model)
        {
            try
            {
                ReportSaleSummaryService service = new ReportSaleSummaryService();
                return service.DataListByDeparture(model);
            }
            catch (Exception ex)
            {
                return Notifization.Error("Error: " + ex);
            }
        }

        [HttpPost]
        [Route("Action/EprExport")]
        public ActionResult EPRExport(ReportEprSearchModel model)
        {
            try
            {
                ReportSaleSummaryService service = new ReportSaleSummaryService();
                return service.EPRExportDeparture(model);
            }
            catch (Exception ex)
            {
                return Notifization.Error("Error: " + ex);
            }
        }
        // API save for daily report *******************************************************************************************************
    }
}