using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using WebCore.Entities;
using System;
using Dapper;
using System.Linq;
using PagedList;
using Helper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
namespace WebCore.Services
{
    public interface IUserRole : IEntityService<UserRole> { }
    public class UserRoleService : EntityService<UserRole>, IUserRole
    {
        public UserRoleService() : base() { }
        public UserRoleService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult RoleListByUserID(RoleIDModel model)
        {
            var roleSettingService = new RoleSettingService(_connection);
            string result = string.Empty;
            string _query = "SELECT * FROM  View_Role_Option WHERE Enabled = 1 ORDER BY Level ASC";
            var _data = roleSettingService._connection.Query<RoleOption>(_query).ToList();
            if (_data.Count > 0)
            {
                foreach (var item in _data)
                {
                    string _id = item.ID;
                    string _select = GetSelected(model.ID, _id);
                    string _actived = string.Empty;
                    if (!string.IsNullOrEmpty(_select) && _select.Equals("fa-check-square actived"))
                        _actived = "actived";
                    result += "<li class='list-group-item " + _actived + "'><i data-id='" + _id + "' class='far " + _select + "' aria-hidden='true'></i> <span data-option='" + _id + "'> " + item.Title + " </span></li>";
                }
            }
            return Notifization.OPTION("ok", result);
        }
        public ActionResult RoleUserSettingByUserID(UserRoleModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrEmpty(model.UserID))
                        return Notifization.Invalid(NotifizationText.Invalid);

                    var userRoleService = new UserRoleService(_connection);

                    List<string> DbUserRole = new List<string>();
                    var roleSettings = userRoleService.GetAlls(m => m.UserID.ToLower().Equals(model.UserID.ToLower()), transaction: transaction).ToList();
                    if (roleSettings.Count > 0)
                        foreach (var item in roleSettings)
                            DbUserRole.Add(item.RoleID);

                    List<string> roleOpion = new List<string>();
                    if (model.ArrRole != null)
                        roleOpion = model.ArrRole;


                    var lstDelRoleId = DbUserRole.Except(roleOpion).ToList();
                    if (lstDelRoleId.Count > 0)
                    {
                        foreach (var item in lstDelRoleId)
                        {

                            var role = userRoleService.GetAlls(m => m.RoleID.ToLower().Equals(item.ToLower()) && m.UserID.ToLower().Equals(model.UserID.ToLower()), transaction: transaction).FirstOrDefault();
                            if (role != null)
                                userRoleService.Remove(role.ID, transaction: transaction);
                        }
                    }

                    if (model.ArrRole.Count > 0)
                    {
                        foreach (var item in model.ArrRole)
                        {
                            var userRole = userRoleService.GetAlls(m => m.RoleID != null && m.UserID.ToLower().Equals(model.UserID.ToLower()) && m.RoleID.ToLower().Equals(item.ToLower()), transaction: transaction).FirstOrDefault();
                            if (userRole == null)
                            {
                                userRoleService.Create<string>(new UserRole()
                                {
                                    UserID = model.UserID,
                                    RoleID = item
                                }, transaction: transaction);
                            }
                        }
                    }

                    transaction.Commit();
                    return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        public string GetSelected(string userId, string Id)
        {
            var userRoleService = new UserRoleService(_connection);
            var userRole = userRoleService.GetAlls(m => m.UserID.ToLower().Equals(userId.ToLower()) && m.RoleID.ToLower().Equals(Id.ToLower())).FirstOrDefault();
            if (userRole != null)
                return "fa-check-square actived";
            else
                return "fa-square";
        }
        //##############################################################################################################################################################################################################################################################
        //check permission
        public static bool AuthenzationAccess(string  userId,string funcId, string actionId)
        {
            try
            {
                if (Current.LoginUser.SkipAuthorization)
                    return true;

                if (string.IsNullOrEmpty(funcId))
                    return false;
                var userRoleService = new UserRoleService();
                var roleSettingService = new RoleSettingService();
                var lstUserRole = userRoleService.GetAlls(m => m.UserID.ToLower().Equals(userId.ToLower())).ToList();
                if (lstUserRole.Count == 0)
                    return false;

                foreach (var item in lstUserRole)
                {
                    var roleSetting = roleSettingService.GetAlls(m => m.RoleID.ToLower().Equals(item.RoleID.ToLower()) && m.FuncID.ToLower().Equals(funcId.ToLower())).FirstOrDefault();
                    if (roleSetting != null)
                        return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool AuthenzationAction(string funcId, string actionId)
        {
            try
            {
                if (!string.IsNullOrEmpty(funcId))
                {
                    string userId = Current.LoginUser.ID;
                    var userRoleService = new UserRoleService();
                    var roleSettingService = new RoleSettingService();
                    var lstUserRole = userRoleService.GetAlls(m => m.UserID.ToLower().Equals(userId.ToLower())).ToList();
                    if (lstUserRole.Count == 0)
                        return false;

                    foreach (var item in lstUserRole)
                    {
                        var roleSetting = roleSettingService.GetAlls(
                                m => m.RoleID.ToLower().Equals(item.RoleID.ToLower())
                                && m.FuncID.ToLower().Equals(funcId.ToLower())
                                && m.ActionID.ToLower().Equals(actionId.ToLower())

                            ).FirstOrDefault();

                        if (roleSetting != null)
                            return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static string AuthenzationAccessTest(string funcId, string actionId)
        {
            if (!string.IsNullOrEmpty(funcId))
            {
                string userId = Current.LoginUser.ID;
                var userRoleService = new UserRoleService();
                var roleSettingService = new RoleSettingService();
                var lstUserRole = userRoleService.GetAlls(m => m.UserID.ToLower().Equals(userId.ToLower())).ToList();
                if (lstUserRole.Count == 0)
                    return "0";

                foreach (var item in lstUserRole)
                {
                    var roleSetting = roleSettingService.GetAlls(m => m.RoleID.ToLower().Equals(item.RoleID.ToLower()) && m.FuncID.ToLower().Equals(funcId.ToLower())).FirstOrDefault();
                    if (roleSetting != null)
                        return "1";
                }
            }
            return "2";
        }
    }
}