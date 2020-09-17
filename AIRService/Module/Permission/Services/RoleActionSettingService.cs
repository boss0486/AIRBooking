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
using WebCore.ENM;

namespace WebCore.Services
{
    public interface IRoleActionSettingService : IEntityService<RoleActionSetting> { }
    public class RoleActionSettingService : EntityService<RoleActionSetting>, IRoleActionSettingService
    {
        public RoleActionSettingService() : base() { }
        public RoleActionSettingService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public static List<RoleForUser> RoleListForUser()
        {
            try
            {
                // controller
                string areaId = AreaApplicationService.GetRouteAreaID((int)AreaApplicationEnum.AreaType.MANAGEMENT);
                if (Helper.Current.UserLogin.IsCMSUser)
                    areaId = AreaApplicationService.GetRouteAreaID((int)AreaApplicationEnum.AreaType.DEVELOPMENT);
                //
                string controllerId = Helper.Security.Library.FakeGuidID(areaId + Helper.Page.MetaSEO.ControllerText);
                //
                RoleActionSettingService roleActionSettingService = new RoleActionSettingService();
                string sqlRole = @"SELECT KeyID, Title FROM MenuAction as a WHERE CategoryID = @ControllerID ORDER BY OrderID ";
                List<RoleForUser> dtList = roleActionSettingService._connection.Query<RoleForUser>(sqlRole, new { ControllerID = controllerId }).ToList();
                if (dtList.Count == 0)
                    return null;
                //
                if (Helper.Current.UserLogin.IsCMSUser || Helper.Current.UserLogin.IsAdminInApplication)
                    return dtList;
                // 
                // check permissons

                // neu co nhieu nhom quyen >>  group by

                // get role 
                string userId = Helper.Current.UserLogin.IdentifierID;
                sqlRole = @"SELECT a.KeyID, a.Title FROM RoleActionSetting as t LEFT JOIN MenuAction as a ON t.ActionID = a.ID 
                            WHERE t.ControllerID = @ControllerID AND t.RoleID = (select TOP 1 RoleID from UserRole WHERE UserID = @UserID) ORDER BY a.OrderID";
                dtList = roleActionSettingService._connection.Query<RoleForUser>(sqlRole, new { ControllerID = controllerId, UserID = userId }).ToList();
                if (dtList.Count == 0)
                    return null;
                return dtList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //##############################################################################################################################################################################################################################################################
    }
}
