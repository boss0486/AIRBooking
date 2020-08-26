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
        public ActionResult Datalist(SearchModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            string query = model.Query;
            //
            if (!string.IsNullOrEmpty(query))
                query = query.Trim();
            else
                query = "";
            int page = model.Page;
            //
            string whereCondition = string.Empty;
            int status = model.Status;
            if (status > 0)
                whereCondition += " AND Enabled = @Enabled";
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM Role WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' " + whereCondition + " ORDER BY [Level] ASC";
            var dtList = _connection.Query<RoleResult>(sqlQuery, new { Query = query, Enabled = status }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(MessageText.NotFound);

            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            Helper.Model.RoleAccountModel roleAccountModel = new Helper.Model.RoleAccountModel
            {
                Create = true,
                Update = true,
                Details = true,
                Delete = true,
                Block = true,
                Active = true,
            };
            return Notifization.Data(MessageText.Success, data: result, role: roleAccountModel, paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(RoleCreateModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string title = model.Title;
            string summary = model.Summary;
            int level = model.Level;
            bool isAllowSpend = model.IsAllowSpend;
            // 
            if (string.IsNullOrWhiteSpace(title))
                return Notifization.Invalid("Không được để trống tên nhóm quyền");
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
            var role = roleService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower()).ToList();
            if (role.Count > 0)
                return Notifization.Invalid("Tên nhóm quyền đã được sử dụng");
            //
            var id = roleService.Create<string>(new Role()
            {
                Title = title,
                Alias = Helper.Page.Library.FormatToUni2NONE(title),
                Summary = summary,
                IsAllowSpend = isAllowSpend,
                Level = model.Level,
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
            var role = roleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(id)).FirstOrDefault();
            if (role == null)
                return Notifization.NotFound(MessageText.NotFound);

            string title = model.Title;
            string summary = model.Summary;
            int level = model.Level;
            bool isAllowSpend = model.IsAllowSpend;

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

            var roleName = roleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower().Equals(title.ToLower()) && !m.ID.ToLower().Equals(id)).FirstOrDefault();
            if (roleName != null)
                return Notifization.Invalid("Tên nhóm quyền đã được sử dụng");
            // update user information
            role.Title = title;
            role.Alias = Helper.Page.Library.FormatToUni2NONE(title);
            role.Summary = model.Summary;
            role.IsAllowSpend = isAllowSpend;
            role.Level = model.Level;
            role.Enabled = model.Enabled;
            roleService.Update(role);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public Role GetRoleModel(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM Role WHERE ID = @Query";
                return _connection.Query<Role>(sqlQuery, new { Query = Id }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string id)
        {
            if (id == null)
                return Notifization.NotFound();

            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    RoleService roleService = new RoleService(_connection);
                    var department = roleService.GetAlls(m => m.ID.Equals(id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (department == null)
                        return Notifization.NotFound();
                    roleService.Remove(department.ID, transaction: transaction);
                    // remover seo
                    transaction.Commit();
                    return Notifization.Success(MessageText.DeleteSuccess);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DropdownList(string id)
        {
            try
            {
                string result = string.Empty;
                using (var service = new RoleService())
                {
                    var dtList = service.DataOption();
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(item.ID) && !string.IsNullOrWhiteSpace(id) && item.ID.ToLower().Equals(id.ToLower()))
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
                            if (item.ID.Equals(id.ToLower()))
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
                return _connection.Query<RoleOption>(sqlQuery).ToList();
            }
            catch
            {
                return new List<RoleOption>();
            }
        }

        public List<RoleOptionByUser> DataOptionByUser(string userId)
        {

            if (string.IsNullOrWhiteSpace(userId))
                return null;
            //
            string sqlQuery = @"SELECT r.ID, r.Title, r.Level, (SELECT CASE WHEN EXISTS (SELECT ID FROM UserRole as s WHERE s.RoleID = r.ID AND  s.UserID = @UserID ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END ) as IsAllow 
                                    FROM Role as r WHERE r.Enabled = 1 ORDER BY r.Level, r.Title";
            return _connection.Query<RoleOptionByUser>(sqlQuery, new { UserID = userId }).ToList();
        }

        public static string DDLListRoleMultiSelect(string arrData)
        {
            try
            {
                string result = string.Empty;

                using (var departmentService = new RoleService())
                {
                    var dtList = departmentService.DataOption();
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (arrData.Contains(","))
                            {
                                string[] idList = arrData.Split(',');
                                foreach (var _item in idList)
                                {
                                    if (item.ID.Equals(_item.ToLower()))
                                        select = "selected";
                                }
                            }
                            result += "<a class='list-group-item list-group-item-action'" + select + " data-val='" + item.ID + "'><i class='far fa-square'></i> " + item.Title + "</a>";
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
