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
    public interface IRoleActionService : IEntityService<RoleActionAction> { }
    public class RoleActionService : EntityService<RoleActionAction>, IRoleActionService
    {
        public RoleActionService() : base() { }
        public RoleActionService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public static string DDLRoleAction(string Id)
        {
            try
            {
                string result = string.Empty;
                using (var roleService = new RoleActionService())
                {
                    var dtList = roleService.DataOptionByGroupID(Id);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (item.ID.Equals(Id.ToLower()))
                                select = "selected";
                            result += "<li class='list-group-item'" + select + "><a class='role-item far fa-square'  data-id='" + item.ID + "'>&nbsp;<span>" + item.Title + "</span></a></li>";
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                return string.Empty + ex;
            }
        }
        public string RoleActionListByGroupID(string groupId)
        {
            try
            {
                string result = string.Empty;
                using (var roleActionService = new RoleActionService())
                {
                    var dtList = roleActionService.DataOptionByGroupID(groupId);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string selected = GetSelectedAction(item.ID);
                            result += "<li class='list-group-item'><a class='role-item far " + selected + "' data-func='" + groupId + "' data-id='" + item.ID + "'>&nbsp;<span>" + item.Title + "</span></a></li>";
                        }

                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                return string.Empty + ex;
            }
        }

        public string GetSelectedAction(string actionId)
        {
            try
            {
                RoleSettingService roleSettingService = new RoleSettingService(_connection);
                var roleSetting = roleSettingService.GetAlls(m => m.ActionID != null && m.ActionID.ToLower().Equals(actionId.ToLower())).FirstOrDefault();
                if (roleSetting != null)
                    return "fa-check-square actived ";
                else
                    return "fa-square";

            }
            catch (Exception ex)
            {
                return string.Empty + ex;
            }

        }
        //##############################################################################################################################################################################################################################################################
        public List<RoleActionOption> DataOptionByGroupID(string groupId)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_RoleAction_Option WHERE GroupID = @GroupID ORDER BY Summary,Val ASC";
                return _connection.Query<RoleActionOption>(sqlQuery, new { GroupID = groupId }).ToList();
            }
            catch
            {
                return new List<RoleActionOption>();
            }
        }

        public List<RoleActionOption> DataOption()
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_RoleAction_Option WHERE Enabled = 1 ORDER BY Title ASC";
                return _connection.Query<RoleActionOption>(sqlQuery, new { }).ToList();
            }
            catch
            {
                return new List<RoleActionOption>();
            }
        }
    }
}
