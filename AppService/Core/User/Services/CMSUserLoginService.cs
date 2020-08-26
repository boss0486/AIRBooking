using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using Helper;
using System.Web.Mvc;
using CMSCore.Entities;
using System.Web;
using WebCore.Model.Enum;
using System.Web.Security;

namespace CMSCore.Services
{
    public interface ICMSUserLoginService : IEntityService<CMSUserLogin> { }
    public class CMSUserLoginService : EntityService<CMSUserLogin>, ICMSUserLoginService
    {
        public CMSUserLoginService() : base() { }
        public CMSUserLoginService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
      
    }
}