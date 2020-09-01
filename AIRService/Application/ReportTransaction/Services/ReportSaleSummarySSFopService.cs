using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using WebCore.Entities;
using Helper;
using System.Web;
using WebCore.Core;
using WebCore.Model.Entities;
using WebCore.Model.Enum;
using WebCore.ENM;
using Helper.File;

namespace WebCore.Services
{
    public interface IReportSaleSummarySSFopService : IEntityService<ReportSaleSummarySSFop> { }
    public class App_ReportSaleSummarySSFopService : EntityService<ReportSaleSummarySSFop>, IReportSaleSummarySSFopService
    {
        public App_ReportSaleSummarySSFopService() : base() { }
        public App_ReportSaleSummarySSFopService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################       
    }
}