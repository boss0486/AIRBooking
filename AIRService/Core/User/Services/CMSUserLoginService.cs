using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using Helper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using WebCore.Model.Enum;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Services;
using Helper.Page;
using Helper.File;

namespace WebCore.Services
{
    public interface ICMSUserLoginService : IEntityService<CMSUserLogin> { }
    public class CMSUserLoginService : EntityService<CMSUserLogin>, ICMSUserLoginService
    {
        public CMSUserLoginService() : base() { }
        public CMSUserLoginService(System.Data.IDbConnection db) : base(db) { }

        //##############################################################################################################################################################################################################################################################
    }
}