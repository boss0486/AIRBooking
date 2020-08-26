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
using System.Web.Configuration;
using System.Data;
using Helper.File;
using Helper.Page;
using WebCore.Model.Entities;
using System.Globalization;
using WebCore.Model.Enum;
using WebCore.ENM;

namespace WebCore.Services
{

    public class ReportService
    {
        private IDbConnection _connecion;
        private bool _disposed = false;

        public ReportService()
        {
            _connecion = DbConnect.Connection.CMS;
            _connecion.Open();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    _connecion.Close();
                //
                _disposed = true;
            }
        }

        ~ReportService()
        {
            Dispose(false);
        }

        //###############################################################################################################################
    }
}