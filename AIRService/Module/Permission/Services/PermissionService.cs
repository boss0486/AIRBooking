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
using WebCore.Model.Entities;
using System.Data;

namespace WebCore.Services
{
    public interface IPermissionService : IEntityService<DbConnection> { }
    public class PermissionService : EntityService<DbConnection>, IPermissionService
    {
        public PermissionService() : base() { }
        public PermissionService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################

        public ActionResult SettingPermission(RoleSettingRequest model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string roleId = model.RoleID;
            string routeArea = model.RouteArea;
            //
            if (string.IsNullOrWhiteSpace(roleId))
                return Notifization.Invalid("Vui lòng chọn nhóm người dùng");
            roleId = roleId.ToLower();
            //
            List<RoleSettingController> controllerList = model.Controllers;
            if (controllerList == null)
            {
                return Notifization.Invalid(MessageText.Invalid);
            }
            //
            DateTime _date = DateTime.Now;
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    // xoa tat ca controller ko co trong model
                    List<string> lstController = controllerList.Select(m => m.ID).ToList();
                    //#1. Delete action in controller, in Role
                    string sqlQuery = @" DELETE RoleActionSetting WHERE RouteArea = @RouteArea AND RoleID = @RoleID AND ControllerID NOT IN ('" + String.Join("','", lstController) + "') ";
                    _connection.Execute(sqlQuery, new { RouteArea = routeArea, RoleID = roleId }, transaction: _transaction);
                    //#2. Delete controller in Role
                    //
                    sqlQuery = @" DELETE RoleControllerSetting WHERE RouteArea = @RouteArea AND RoleID = @RoleID AND ControllerID NOT IN ('" + String.Join("','", lstController) + "') ";
                    _connection.Execute(sqlQuery, new { RouteArea = routeArea, RoleID = roleId }, transaction: _transaction);
                    //
                    RoleControllerSettingService roleControllerSettingService = new RoleControllerSettingService(_connection);
                    RoleActionSettingService roleActionSettingService = new RoleActionSettingService(_connection);
                    //
                    foreach (var controller in controllerList)
                    {
                        var controllerId = controller.ID;
                        // #1. Check controller
                        var controllerInDb = roleControllerSettingService.GetAlls(m => m.RouteArea.Equals(routeArea) && m.RoleID.Equals(roleId) && m.ControllerID.Equals(controllerId), transaction: _transaction).FirstOrDefault();
                        // insert |  update
                        if (controllerInDb == null)
                        {
                            var controllerSettingId = roleControllerSettingService.Create<string>(new RoleControllerSetting
                            {
                                RouteArea = routeArea,
                                RoleID = roleId,
                                ControllerID = controllerId,
                                CreatedDate = _date
                            }, transaction: _transaction);

                            var actionList = controller.Action;
                            if (actionList != null)
                            {
                                foreach (var action in actionList)
                                {
                                    // neu chua co thi them moi
                                    var actionSetting = roleActionSettingService.GetAlls(m => m.RouteArea.Equals(routeArea) && m.RoleID.Equals(roleId) &&
                                    m.ControllerID.Equals(controllerId) && m.ActionID.Equals(action), transaction: _transaction).FirstOrDefault();
                                    //
                                    if (actionSetting == null)
                                    {
                                        var actionSettingId = roleActionSettingService.Create<string>(new RoleActionSetting
                                        {
                                            RouteArea = routeArea,
                                            RoleID = roleId,
                                            ControllerID = controllerId,
                                            ActionID = action.ToLower()
                                        }, transaction: _transaction);
                                    }
                                    // da ton tai thi ko lam gi
                                }
                            }
                        }
                        else
                        {
                            controllerInDb.CreatedDate = _date;
                            roleControllerSettingService.Update(controllerInDb, transaction: _transaction);
                            //
                            //var actionList = controller.Action;
                            //sqlQuery = @" DELETE RoleActionSetting WHERE RoleID = @RoleID AND ControllerID = @ControllerID AND ActionID NOT IN ('" + String.Join("','", actionList) + "') ";
                            //_connection.Execute(sqlQuery, new { RoleID = roleId, ControllerID = controllerId }, transaction: _transaction);

                            var actionList = controller.Action;
                            if (actionList == null)
                            {
                                // delete all action
                                sqlQuery = @" DELETE RoleActionSetting WHERE RouteArea = @RouteArea AND RoleID = @RoleID AND ControllerID = @ControllerID ";
                                _connection.Execute(sqlQuery, new { RouteArea = routeArea, RoleID = roleId, ControllerID = controllerId }, transaction: _transaction);
                            }
                            else
                            {
                                // delete action not in model
                                sqlQuery = @" DELETE RoleActionSetting WHERE RouteArea = @RouteArea AND RoleID = @RoleID AND ControllerID = @ControllerID AND ActionID NOT IN ('" + String.Join("','", actionList) + "') ";
                                _connection.Execute(sqlQuery, new { RouteArea = routeArea, RoleID = roleId, ControllerID = controllerId }, transaction: _transaction);
                                //
                                foreach (var action in actionList)
                                {
                                    // neu chua co thi them moi
                                    var actionSetting = roleActionSettingService.GetAlls(m => m.RouteArea.Equals(routeArea) && m.RoleID.Equals(roleId) &&
                                    m.ControllerID.Equals(controllerId) && m.ActionID.Equals(action), transaction: _transaction).FirstOrDefault();
                                    //
                                    if (actionSetting == null)
                                    {
                                        var actionSettingId = roleActionSettingService.Create<string>(new RoleActionSetting
                                        {
                                            RouteArea = routeArea,
                                            RoleID = roleId,
                                            ControllerID = controllerId,
                                            ActionID = action.ToLower()
                                        }, transaction: _transaction);
                                    }
                                    // da ton tai thi ko lam gi
                                }
                            }

                        }
                    }
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST("::" + ex);
                }
            }
        }

        public static bool CheckPermission(string routeArea, string controllerText, string actionText)
        {
            try
            {
                if (Helper.Current.UserLogin.IsAdministratorInApplication)
                    return true;
                //
                if (string.IsNullOrWhiteSpace(controllerText) || string.IsNullOrWhiteSpace(actionText))
                    return false;

                controllerText = controllerText.Replace("Controller", "");
                //
                string userId = Helper.Current.UserLogin.IdentifierID;
                //#1. Get role of user
                UserRoleService userRoleService = new UserRoleService();
                var userRole = userRoleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID.ToLower().Equals(userId.ToLower())).FirstOrDefault();
                if (userRole == null)
                    return false;
                //
                string roleId = userRole.RoleID;
                string controllerId = Helper.Security.Library.FakeGuidID(routeArea + controllerText);
                string actionId = Helper.Security.Library.FakeGuidID(controllerId + actionText);
                string apiActionId = Helper.Security.Library.FakeGuidID(controllerId + "-Api-" + actionText);
                //#2. check  
                using (PermissionService service = new PermissionService())
                {
                    string sqlQuery = @" SELECT c.ID FROM RoleControllerSetting as c INNER JOIN RoleActionSetting as a ON a.ControllerID = c.ControllerID AND a.RoleID = c.RoleID
                                         WHERE c.RoleID = @RoleID AND c.ControllerID = @ControllerID AND (a.ActionID = @ActionID OR a.ActionID = @ApiActionID) ";
                    var role = service.Query<PermissionIDModel>(sqlQuery, new { RoleID = roleId, ControllerID = controllerId, ActionID = actionId, ApiActionID = apiActionId }).FirstOrDefault();
                    if (role != null)
                        return true;
                    //
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        //##############################################################################################################################################################################################################################################################
   
        
        
        //##############################################################################################################################################################################################################################################################


    }
}
