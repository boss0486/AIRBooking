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
    public interface IRoleSettingService : IEntityService<RoleSetting> { }
    public class RoleSettingService : EntityService<RoleSetting>, IRoleSettingService
    {
        public RoleSettingService() : base() { }
        public RoleSettingService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################

        // permission setting
        //******************************************************************************************************************************************************************************
        public ActionResult RoleSettingPermission(RoleSettingModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.RoleID))
                    return Notifization.NotFound(NotifizationText.NotFound);

                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        RoleSettingService roleSettingService = new RoleSettingService(_connection);
                        // danh sach quyen cua nhom hien tai
                        List<string> funcOpion = new List<string>();
                        if (model.ArrFuncID != null)
                            funcOpion = model.ArrFuncID;

                        List<string> DbFuncId = new List<string>();
                        var roleSettings = roleSettingService.GetAlls(transaction: transaction).Where(m => m.RoleID.ToLower().Equals(model.RoleID.ToLower()) && m.ActionID == null).ToList();
                        if (roleSettings.Count > 0)
                            foreach (var item in roleSettings)
                                DbFuncId.Add(item.FuncID);

                        //  xoa tat ca cac phan tu co trong chuoi 1 ma ko co trong chuoi 2
                        var lstDelFuncId = DbFuncId.Except(funcOpion).ToList();
                        if (lstDelFuncId.Count > 0)
                        {
                            foreach (var itemFuncId in lstDelFuncId)
                            {
                                // remove FuncID and action
                                var roleSetting = roleSettingService.GetAlls(m => m.RoleID.ToLower().Equals(model.RoleID.ToLower()) && m.FuncID.ToLower().Equals(itemFuncId.ToLower()), transaction: transaction).ToList();
                                if (roleSetting.Count > 0)
                                    foreach (var item in roleSetting)
                                        roleSettingService.Remove(item.ID, transaction: transaction);
                            }
                        }

                        var lstAddFunc = funcOpion.Except(DbFuncId).ToList();
                        if (lstAddFunc.Count > 0)
                        {
                            foreach (var funcId in lstAddFunc)
                            {
                                var func = roleSettingService.GetAlls(m => m.FuncID.ToLower().Equals(funcId.ToLower()) && m.RoleID.ToLower().Equals(model.RoleID.ToLower()), transaction: transaction).FirstOrDefault();
                                if (func == null)
                                {
                                    roleSettingService.Create<string>(new RoleSetting()
                                    {
                                        RoleID = model.RoleID,
                                        FuncID = funcId,
                                        IsAction = false,
                                    }, transaction: transaction);
                                }
                            }
                        }
                        // Action
                        //  delete role action
                        if (!string.IsNullOrEmpty(model.FuncID))
                        {
                            List<string> actionOpion = new List<string>();
                            if (model.ArrActionID != null)
                                actionOpion = model.ArrActionID;

                            List<string> DbActionId = new List<string>();
                            var roleAction = roleSettingService.GetAlls(m => !string.IsNullOrEmpty(m.RoleID) && m.RoleID.ToLower().Equals(model.RoleID.ToLower()) && m.FuncID.ToLower().Equals(model.FuncID.ToLower()) && m.ActionID != null, transaction: transaction).ToList();
                            if (roleAction.Count > 0)
                                foreach (var item in roleAction)
                                    DbActionId.Add(item.ActionID);

                            //  xoa tat ca cac phan tu co trong chuoi 1 ma ko co trong chuoi 2
                            var lstActionDeleted = DbActionId.Except(actionOpion).ToList();
                            if (lstActionDeleted.Count > 0)
                            {
                                foreach (var itemActionId in lstActionDeleted)
                                {
                                    // remove FuncID and action
                                    var roleSetting = roleSettingService.GetAlls(m => m.ActionID != null && m.RoleID.ToLower().Equals(model.RoleID.ToLower()) && m.FuncID.ToLower().Equals(model.FuncID.ToLower()) && m.ActionID.ToLower().Equals(itemActionId.ToLower()), transaction: transaction).ToList();
                                    if (roleSetting.Count > 0)
                                        foreach (var item in roleSetting)
                                            roleSettingService.Remove(item.ID, transaction: transaction);
                                }
                            }
                            // add new role action
                            var lstAddActionID = actionOpion.Except(DbActionId).ToList();
                            if (lstAddActionID.Count > 0)
                            {
                                var checkFunc = roleSettingService.GetAlls(m => !string.IsNullOrEmpty(m.RoleID) && m.RoleID.ToLower().Equals(model.RoleID.ToLower()) && m.FuncID.ToLower().Equals(model.FuncID.ToLower()), transaction: transaction).FirstOrDefault();
                                if (checkFunc != null)
                                {
                                    foreach (var item in lstAddActionID)
                                    {
                                        var action = roleSettingService.GetAlls(m => !string.IsNullOrEmpty(m.ActionID) && m.ActionID.ToLower().Equals(item.ToLower()) && m.FuncID.ToLower().Equals(model.FuncID.ToLower()) && m.ActionID != null, transaction: transaction).FirstOrDefault();
                                        if (action == null)
                                        {
                                            roleSettingService.Create<string>(new RoleSetting()
                                            {
                                                RoleID = model.RoleID,
                                                FuncID = model.FuncID,
                                                ActionID = item,
                                                IsAction = true,
                                            }, transaction: transaction);
                                        }
                                    }
                                }
                            }
                        }
                        //////////////////////// -----------------------------------------------------------------------------------------------------------------------------------------------
                        //string.Join(",", removeItemRole)

                        transaction.Commit();
                        return Notifization.Success(NotifizationText.UPDATE_SUCCESS);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return Notifization.TEST("::" + ex);
                    }
                }
            }
            catch (Exception ex)
            {
                return Notifization.TEST("::" + ex);
            }
        }
        public string GetRoleGroupID(string _userId, string Id, IDbTransaction transaction = null)
        {
            try
            {
                RoleSettingService roleSettingService = new RoleSettingService(_connection);
                var roleSetting = roleSettingService.GetAlls(m => m.RoleID.ToLower().Equals(_userId.ToLower()) && m.ID.Equals(Id.ToLower()), transaction).FirstOrDefault();
                if (roleSetting != null)
                    return roleSetting.ID;
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
