using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using WebCore.Entities;
namespace WebCore.Services
{
    public interface IRoleControllerSettingService : IEntityService<RoleControllerSetting> { }
    public class RoleControllerSettingService : EntityService<RoleControllerSetting>, IRoleControllerSettingService
    {
        public RoleControllerSettingService() : base() { }
        public RoleControllerSettingService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
    }
}
