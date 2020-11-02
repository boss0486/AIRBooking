﻿using AL.NetFrame.Attributes;
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
using WebCore.Model.Entities;
using Helper.Page;

namespace WebCore.Services
{
    public interface IRoleService : IEntityService<Role> { }
    public class RoleService : EntityService<Role>, IRoleService
    {
        public RoleService() : base() { }
        public RoleService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
            #region
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            int page = model.Page;
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            //
            string whereCondition = string.Empty;
            //
            SearchResult searchResult = WebCore.Model.Services.ModelService.SearchDefault(new SearchModel
            {
                Query = model.Query,
                TimeExpress = model.TimeExpress,
                Status = model.Status,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Page = model.Page,
                AreaID = model.AreaID,
                TimeZoneLocal = model.TimeZoneLocal
            });
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            #endregion

            int status = model.Status;
            if (status > 0)
                whereCondition += " AND Enabled = @Enabled";
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM Role WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY [Level] ASC";
            var dtList = _connection.Query<RoleResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), Enabled = status }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //

            List<RoleResult> roleResults = dtList.Where(m => string.IsNullOrWhiteSpace(m.ParentID)).ToList();
            List<RoleResult> result = roleResults.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();

            foreach (var item in result)
            {
                item.SubRoles = dtList.Where(m => m.ParentID == item.ID).ToList();
            }
            if (result.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(RoleCreateModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string parentId = model.ParentID;
            string title = model.Title;
            string summary = model.Summary;
            int level = model.Level;
            bool isAllowSpend = model.IsAllowSpend;
            //
            if (string.IsNullOrWhiteSpace(parentId) || parentId.Length != 36)
                parentId = null;
            else
                parentId = parentId.ToLower();
            // 
            if (string.IsNullOrWhiteSpace(title))
                return Notifization.Invalid("Không được để trống tên nhóm quyền");
            //
            title = title.Trim();
            if (!Validate.TestText(title))
                return Notifization.Invalid("Tên nhóm quyền không hợp lệ");
            //
            if (title.Length < 2 || title.Length > 80)
                return Notifization.Invalid("Tên nhóm quyền giới hạn 2-80 ký tự");
            // summary valid               
            if (!string.IsNullOrWhiteSpace(summary))
            {
                summary = summary.Trim();
                if (!Validate.TestText(summary))
                    return Notifization.Invalid("Mô tả không hợp lệ");
                if (summary.Length < 1 || summary.Length > 120)
                    return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
            }
            RoleService roleService = new RoleService(_connection);
            var role = roleService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower() && m.ParentID == parentId).ToList();
            if (role.Count > 0)
                return Notifization.Invalid("Tên nhóm quyền đã được sử dụng");
            //
            var id = roleService.Create<string>(new Role()
            {
                ParentID = parentId,
                Title = title,
                Alias = Helper.Page.Library.FormatToUni2NONE(title),
                Summary = summary,
                IsAllowSpend = isAllowSpend,
                //Level = model.Level,
                LanguageID = Helper.Current.UserLogin.LanguageID,
                Enabled = model.Enabled,
            });
            //
            return Notifization.Success(MessageText.CreateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(RoleUpdateModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            RoleService roleService = new RoleService(_connection);
            string id = model.ID.ToLower();
            var role = roleService.GetAlls(m => m.ID == id).FirstOrDefault();
            //
            if (role == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            string parentId = model.ParentID;
            string title = model.Title;
            string summary = model.Summary;
            int level = model.Level;
            bool isAllowSpend = model.IsAllowSpend;
            //
            if (string.IsNullOrWhiteSpace(parentId) || parentId.Length != 36)
                parentId = null;
            else
                parentId = parentId.ToLower();
            //
            if (string.IsNullOrWhiteSpace(title))
                return Notifization.Invalid("Không được để trống tên nhóm quyền");
            title = title.Trim();
            if (!Validate.TestText(title))
                return Notifization.Invalid("Tên nhóm quyền không hợp lệ");
            if (title.Length < 2 || title.Length > 80)
                return Notifization.Invalid("Tên nhóm quyền giới hạn 2-80 ký tự");
            // summary valid
            if (!string.IsNullOrWhiteSpace(summary))
            {
                if (!Validate.TestAlphabet(summary))
                    return Notifization.Invalid("Mô tả không hợp lệ");
                if (summary.Length < 1 || summary.Length > 120)
                    return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                summary = summary.Trim();
            }
            //
            var roleName = roleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && m.ParentID == parentId && m.ID != id).FirstOrDefault();
            if (roleName != null)
                return Notifization.Invalid("Tên nhóm quyền đã được sử dụng");
            // update user information
            role.ParentID = parentId;
            role.Title = title;
            role.Alias = Helper.Page.Library.FormatToUni2NONE(title);
            role.Summary = model.Summary;
            role.IsAllowSpend = isAllowSpend;
            //role.Level = model.Level;
            role.Enabled = model.Enabled;
            roleService.Update(role);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public Role GetRoleModel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            string query = string.Empty;
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT TOP (1) * FROM Role WHERE ID = @ID";
            return _connection.Query<Role>(sqlQuery, new { ID = id }).FirstOrDefault();
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound();
            //
            id = id.ToLower();
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    RoleService service = new RoleService(_connection);
                    var role = service.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (role == null)
                        return Notifization.NotFound();
                    //
                    var parentList = service.GetAlls(m => m.ParentID == id, transaction: _transaction).ToList();
                    if (parentList.Count > 0)
                    {
                        string sqlQuery = "DELETE Role WHERE ParentID = @ParentID";
                        service.Execute(sqlQuery, new { ParentID = id }, transaction: _transaction);
                    }
                    service.Remove(role.ID, transaction: _transaction);
                    // remover seo
                    _transaction.Commit();
                    return Notifization.Success(MessageText.DeleteSuccess);
                }
                catch
                {
                    _transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        //public static string DropdownList(string id)
        //{
        //    try
        //    {
        //        string result = string.Empty;
        //        using (var service = new RoleService())
        //        {
        //            var dtList = service.DataOption();
        //            if (dtList.Count > 0)
        //            {
        //                foreach (var item in dtList)
        //                {
        //                    string select = string.Empty;
        //                    if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
        //                        select = "selected";
        //                    result += "<option value='" + item.ID + "'" + select + ">" + item.Title + "</option>";
        //                }
        //            }
        //            return result;
        //        }
        //    }
        //    catch
        //    {
        //        return string.Empty;
        //    }
        //}
        //##############################################################################################################################################################################################################################################################
        public static string DDLRoleLevel(string id)
        {
            try
            {
                string result = string.Empty;
                using (var roleService = new RoleService())
                {
                    var dtList = roleService.DataOption();
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                                select = "selected";
                            result += "<option value='" + item.ID + "'" + select + ">" + item.Title + "</option>";
                        }
                    }
                    return result;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        public List<RoleOption> DataOption()
        {
            try
            {
                string sqlQuery = @"SELECT * FROM Role WHERE Enabled = 1 ORDER BY Level, Title ASC";
                List<RoleOption> roleOptions = _connection.Query<RoleOption>(sqlQuery).ToList();
                List<RoleOption> roleResults = roleOptions.Where(m => string.IsNullOrWhiteSpace(m.ParentID)).ToList();
                 
                foreach (var item in roleResults)
                {
                    item.SubOption = roleOptions.Where(m => m.ParentID == item.ID).ToList();
                }

                return _connection.Query<RoleOption>(sqlQuery).ToList();
            }
            catch
            {
                return new List<RoleOption>();
            }
        }
        public static List<string> GetRoleForUser(string userId)
        {
            using (var service = new RoleService())
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return null;
                //
                string sqlQuery = @"SELECT RoleID FROM UserRole WHERE UserID = @UserID";
                return service.Query<string>(sqlQuery, new { UserID = userId }).ToList();
            }
        }
        public static string DDLListRoleMultiSelect(List<string> arrData = null, bool isNotcheck = false)
        {
            try
            {
                string result = string.Empty;

                using (var service = new RoleService())
                {
                    var dtList = service.DataOption();
                    if (dtList.Count > 0)
                    {
                        int cnt = 1;
                        foreach (var item in dtList)
                        {
                            string strIndex = cnt + "";
                            if (cnt < 10)
                                strIndex = "0" + cnt;
                            //
                            string active = string.Empty;
                            if (arrData != null && arrData.Count() > 0)
                            {
                                if (arrData.Contains(item.ID))
                                    active = "checked";
                            }
                            //
                            if (!isNotcheck)
                            {
                                result += "<a class='list-group-item'>";
                                result += "   <input id='" + item.ID + "' type='checkbox' class='filled-in action-item-input  ' value='" + item.ID + "' " + active + " />";
                                result += "   <label style='margin:0px;' for='" + item.ID + "'>" + strIndex + ". " + item.Title + "</label>";
                                result += "</a>";
                            }
                            else
                            {

                                result += "<a class='list-group-item " + active + "'>";
                                result += "   <input id='" + item.ID + "' type='checkbox' class='filled-in action-item-input  ' value='" + item.ID + "' " + active + "  disabled />";
                                result += "   <label style='margin:0px;' for='" + item.ID + "'>" + strIndex + ". " + item.Title + "</label>";
                                result += "</a>";
                            }
                            cnt++;
                        }
                    }
                }
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }
        //##############################################################################################################################################################################################################################################################
    }
}
