using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helper;
using Helper.Page;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Model.Entities;
using WebCore.Services;
namespace WebApplication.Management.Controllers
{
    public class VNAReportController : CMSController
    {
        // GET: Management/VNAReport
        public ActionResult DataList()
        {
            return View();
        }
    }
}