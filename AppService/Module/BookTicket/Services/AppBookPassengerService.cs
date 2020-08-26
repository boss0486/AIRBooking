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
    public interface IAppBookPassengerService : IEntityService<AppBookPassenger> { }
    public class AppBookPassengerService : EntityService<AppBookPassenger>, IAppBookPassengerService
    {
        public AppBookPassengerService() : base() { }
        public AppBookPassengerService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
    }
}
