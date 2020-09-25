using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper.File;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebCore.Model.Entities;
namespace WebCore.Entities
{
    // Api - winservice
    public class APIAuthenModel
    {
        public readonly string UserName = "api-booking";
        public readonly string Password = "***********";

    }
    public class APIDailyReportModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReportDate { get; set; }
    }

}