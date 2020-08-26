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
            string langID = Current.LanguageID;
            string sqlQuery = @"SELECT * FROM View_Role WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' " + whereCondition + " ORDER BY [Level] ASC";
            var dtList = _connection.Query<Role>(sqlQuery, new { Query = query, Enabled = status }).ToList();
            if (dtList.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);

            var resultData = new List<RsRole>();
            foreach (var item in dtList)
            {
                resultData.Add(new RsRole(item.ID, item.Title, item.Summary, item.Alias, item.Level, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate));
            }
            var result = resultData.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = resultData.ToPagedList(page, Library.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(NotifizationText.NotFound);

            Library.PagingModel pagingModel = new Library.PagingModel
            {
                PageSize = Library.Paging.PAGESIZE,
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
            return Notifization.DATALIST(NotifizationText.Success, data: result, role: roleAccountModel, paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(RoleCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    RoleService roleService = new RoleService(_connection);
                    var role = roleService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction).ToList();
                    if (role.Count > 0)
                        return Notifization.Invalid("Tên nhóm quyền đã được sử dụng");

                    role = roleService.GetAlls(m => m.Level == model.Level, transaction: transaction).ToList();
                    if (role.Count > 0)
                        return Notifization.Invalid("Cấp độ đã được sử dụng");

                    var Id = roleService.Create<string>(new Role()
                    {
                        Title = model.Title,
                        Alias = Helper.Library.Uni2NONE(model.Title),
                        Summary = model.Summary,
                        Level = model.Level,
                        LanguageID = Current.LanguageID,
                        Enabled = model.Enabled,
                    }, transaction: transaction);
                    string temp = string.Empty;

                    //sort
                    transaction.Commit();
                    return Notifization.Success(NotifizationText.CREATE_SUCCESS);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(RoleUpdateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    RoleService roleService = new RoleService(_connection);
                    string Id = model.ID.ToLower();
                    var role = roleService.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                    if (role == null)
                        return Notifization.NotFound(NotifizationText.NotFound);

                    string title = model.Title;
                    var roleName = roleService.GetAlls(m => m.Title.ToLower().Equals(title.ToLower()) && !m.ID.Equals(Id), transaction: transaction).ToList();
                    if (roleName.Count > 0)
                        return Notifization.Invalid("Tên nhóm quyền đã được sử dụng");

                    var roleLevel = roleService.GetAlls(m => m.Level == model.Level && !m.ID.Equals(Id), transaction: transaction).ToList();
                    if (roleLevel.Count > 0)
                        return Notifization.Invalid("Cấp độ đã được sử dụng");

                    // update user information
                    role.Title = title;
                    role.Alias = Helper.Library.Uni2NONE(title);
                    role.Summary = model.Summary;
                    role.Level = model.Level;
                    role.Enabled = model.Enabled;
                    roleService.Update(role, transaction: transaction);
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
        public Role UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_Role WHERE ID = @Query";
                return _connection.Query<Role>(sqlQuery, new { Query = Id }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string Id)
        {
            if (Id == null)
                return Notifization.NotFound();

            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    RoleService roleService = new RoleService(_connection);
                    var department = roleService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (department == null)
                        return Notifization.NotFound();
                    roleService.Remove(department.ID, transaction: transaction);
                    // remover seo
                    transaction.Commit();
                    return Notifization.Success(NotifizationText.DELETE_SUCCESS);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(NotifizationText.Invalid);
                string langID = Current.LanguageID;
                string sqlQuery = @"SELECT * FROM View_Role WHERE ID = @ID";
                var item = _connection.Query<Role>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(NotifizationText.NotFound);
                RsRole result = new RsRole(item.ID, item.Title, item.Summary, item.Alias, item.Level, item.LanguageID, item.Enabled, item.SiteID, item.CreatedBy, item.CreatedDate);
                return Notifization.DATALIST(NotifizationText.Success, data: result, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }

        public static string DDLRole(string Id)
        {
            try
            {
                string result = string.Empty;
                using (var departmentService = new RoleService())
                {
                    var dtList = departmentService.DataOption(Id);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (item.ID.Equals(Id.ToLower()))
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

        public static string DDLListRole(string Id)
        {
            try
            {
                string result = string.Empty;
                using (var roleService = new RoleService())
                {
                    var dtList = roleService.DataOption(Id);
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
                    var dtList = roleService.DataOption(id);
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
        public List<RoleOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_Role_Option ORDER BY Title ASC";
                return _connection.Query<RoleOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<RoleOption>();
            }
        }
        public static string DDLListRoleMultiSelect(string arrData)
        {
            try
            {
                string result = string.Empty;

                using (var departmentService = new RoleService())
                {
                    var dtList = departmentService.DataOption("");
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
