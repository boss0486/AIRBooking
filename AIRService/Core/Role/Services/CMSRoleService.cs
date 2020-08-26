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
using WebCore.Entities;
using WebCore.Model.Entities;
using Helper.Page;

namespace WebCore.Services
{
    public interface ICMSRoleService : IEntityService<CMSRole> { }
    public class CMSRoleService : EntityService<CMSRole>, ICMSRoleService
    {
        public CMSRoleService() : base() { }
        public CMSRoleService(System.Data.IDbConnection db) : base(db) { }
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
            //
            string langID = Helper.Page.Default.LanguageID;
            string sqlQuery = @"SELECT * FROM View_CMSRole WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' " + whereCondition + " ORDER BY [Level] ASC";
            var dtList = _connection.Query<CMSRoleResult>(sqlQuery, new { Query = query, Enabled = status }).ToList();
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
            //
            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            //
            Helper.Model.RoleDefaultModel role = new Helper.Model.RoleDefaultModel
            {
                Create = true,
                Update = true,
                Details = true,
                Delete = true
            };

            return Notifization.Data(MessageText.Success, data: result, role: role, paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(CMSRoleCreateModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);

            string title = model.Title;
            string summary = model.Summary;
            string areaId = model.AreaID;
            if (string.IsNullOrEmpty(title))
                return Notifization.Invalid("Không được để trống tên nhóm quyền ");
            title = title.Trim();
            if (!Validate.TestText(title))
                return Notifization.Invalid("Tên nhóm quyền không hợp lệ");
            if (title.Length < 2 || title.Length > 80)
                return Notifization.Invalid("Tên nhóm quyền giới hạn 2-80 ký tự");
            // summary valid
            if (!string.IsNullOrEmpty(summary))
            {
                if (!Validate.TestAlphabet(summary))
                    return Notifization.Invalid("Mô tả không hợp lệ");
                if (summary.Length < 1 || summary.Length > 120)
                    return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                summary = summary.Trim();
            };
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string langId = Helper.Page.Default.LanguageID;
                    CMSRoleService cmsRoleService = new CMSRoleService(_connection);
                    var CMSRoleApplication = cmsRoleService.GetAlls(m => m.Title.ToLower().Equals(model.Title.ToLower()), transaction: transaction).ToList();
                    if (CMSRoleApplication.Count > 0)
                        return Notifization.Invalid("Tên nhóm quyền đã được sử dụng");

                    CMSRoleApplication = cmsRoleService.GetAlls(m => m.Level == model.Level, transaction: transaction).ToList();
                    if (CMSRoleApplication.Count > 0)
                        return Notifization.Invalid("Cấp độ đã được sử dụng");

                    var Id = cmsRoleService.Create<string>(new CMSRole()
                    {
                        AreaID = areaId,
                        Title = title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(model.Title),
                        Summary = summary,
                        Level = model.Level,
                        LanguageID = langId,
                        Enabled = model.Enabled,
                    }, transaction: transaction);
                    string temp = string.Empty;

                    //sort
                    transaction.Commit();
                    return Notifization.Success(MessageText.CreateSuccess);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Notifization.TEST("::" + ex);
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(CMSRoleUpdateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(model.ID))
                        return Notifization.NotFound(MessageText.Invalid);
                    //
                    CMSRoleService cmsRoleService = new CMSRoleService(_connection);
                    string Id = model.ID.ToLower();
                    var cmsRole = cmsRoleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.ToLower().Equals(Id), transaction: transaction).FirstOrDefault();
                    if (cmsRole == null)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    string title = model.Title;
                    var titleModel = cmsRoleService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) &&  m.Title.ToLower().Equals(title.ToLower()) && !m.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (titleModel.Count > 0)
                        return Notifization.Invalid("Tên nhóm quyền đã được sử dụng");
                    //
                    var levelModel = cmsRoleService.GetAlls(m => m.Level == model.Level && !m.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                    if (levelModel.Count > 0)
                        return Notifization.Invalid("Cấp độ đã được sử dụng");
                    // update user information
                    cmsRole.Title = title;
                    cmsRole.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    cmsRole.Summary = model.Summary;
                    cmsRole.Level = model.Level;
                    cmsRole.Enabled = model.Enabled;
                    cmsRoleService.Update(cmsRole, transaction: transaction);
                    transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Notifization.TEST("::" + ex);
                }
            }
        }
        public CMSRole CMSRoleModel(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                //
                string langId = Helper.Page.Default.LanguageID;
                string query = string.Empty;
                string langID = langId;
                string sqlQuery = @"SELECT TOP (1) * FROM View_CMSRole WHERE ID = @Query";
                return _connection.Query<CMSRole>(sqlQuery, new { Query = id }).FirstOrDefault();
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
                    CMSRoleService CMSRoleApplicationService = new CMSRoleService(_connection);
                    var department = CMSRoleApplicationService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                    if (department == null)
                        return Notifization.NotFound();
                    CMSRoleApplicationService.Remove(department.ID, transaction: transaction);
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
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM View_CMSRole WHERE ID = @ID";
                var item = _connection.Query<CMSRole>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                return Notifization.Data(MessageText.Success, data: item);
            }
            catch
            {
                return Notifization.NotService;
            }
        }

        public static string DropdownList(string id)
        {
            try
            {
                string result = string.Empty;
                using (var departmentService = new CMSRoleService())
                {
                    var dtList = departmentService.DataOption(id);
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

        public static string DDLListCMSRoleApplication(string Id)
        {
            try
            {
                string result = string.Empty;
                using (var CMSRoleApplicationService = new CMSRoleService())
                {
                    var dtList = CMSRoleApplicationService.DataOption(Id);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (item.ID.Equals(Id.ToLower()))
                                select = "selected";
                            result += "<li class='list-group-item'" + select + "><a class='CMSRoleApplication-item far fa-square'  data-id='" + item.ID + "'>&nbsp;<span>" + item.Title + "</span></a></li>";
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
        public static string DDLCMSRoleApplicationLevel(string id)
        {
            try
            {
                string result = string.Empty;
                using (var CMSRoleApplicationService = new CMSRoleService())
                {
                    var dtList = CMSRoleApplicationService.DataOption(id);
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
        public List<CMSRoleOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_CMSRole ORDER BY Title ASC";
                return _connection.Query<CMSRoleOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<CMSRoleOption>();
            }
        }
        public static string DropdowListCMSUserRoleMultiSelect(string arrData)
        {
            try
            {
                string result = string.Empty;

                using (var departmentService = new CMSRoleService())
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
