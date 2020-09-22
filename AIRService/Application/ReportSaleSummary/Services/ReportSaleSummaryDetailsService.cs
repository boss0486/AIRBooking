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
    public interface IReportTransactionDetailsService : IEntityService<ReportSaleSummaryDetails> { }
    public class ReportTransactionDetailsService : EntityService<ReportSaleSummaryDetails>, IReportTransactionDetailsService
    {
        public ReportTransactionDetailsService() : base() { }
        public ReportTransactionDetailsService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
    }
}