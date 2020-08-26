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

        // permission setting
        //******************************************************************************************************************************************************************************

        public string GetRoleGroupID(string _userId, string Id, IDbTransaction transaction = null)
        {
            try
            {
                RoleControllerSettingService RoleControllerSettingService = new RoleControllerSettingService(_connection);
                var RoleControllerSetting = RoleControllerSettingService.GetAlls(m => m.RoleID.ToLower().Equals(_userId.ToLower()) && m.ID.Equals(Id.ToLower()), transaction).FirstOrDefault();
                if (RoleControllerSetting != null)
                    return RoleControllerSetting.ID;
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        //##############################################################################################################################################################################################################################################################
        //check permission
    }
}
