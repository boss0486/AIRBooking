using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using Helper;
using System.Web;
using WebCore.Entities;

namespace WebCore.Services
{
    public interface IAppBookFareService : IEntityService<AppBookFare> { }
    public class AppBookFareService : EntityService<AppBookFare>, IAppBookFareService
    {
        public AppBookFareService() : base() { }
        public AppBookFareService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
    }
}
