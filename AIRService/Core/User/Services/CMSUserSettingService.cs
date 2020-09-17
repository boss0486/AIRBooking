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

namespace WebCore.Services
{
    public interface ICMSUserSettingService : IEntityService<CMSUserSetting> { }
    public class CMSUserSettingService : EntityService<CMSUserSetting>, ICMSUserSettingService
    {
        public CMSUserSettingService() : base() { }
        public CMSUserSettingService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
    }
}