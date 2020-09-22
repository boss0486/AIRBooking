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
    public interface IReportSaleSummaryClosedService : IEntityService<ReportSaleSummaryClosed> { }
    public class ReportSaleSummaryClosedService : EntityService<ReportSaleSummaryClosed>, IReportSaleSummaryClosedService
    {
        public ReportSaleSummaryClosedService() : base() { }
        public ReportSaleSummaryClosedService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
    }
}