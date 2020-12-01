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
    public interface INationalService : IEntityService<National> { }
    public class NationalService : EntityService<National>, INationalService
    {
        public NationalService() : base() { }
        public NationalService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
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
            //
            string areaId = model.AreaID;
            if (!string.IsNullOrWhiteSpace(areaId) && areaId != "-")
                whereCondition += " AND AreaID = @AreaID ";
            // query
            string sqlQuery = @"SELECT * FROM App_National WHERE Title LIKE N'%'+ @Query +'%'" + whereCondition + " ORDER BY [Title] ASC";
            var dtList = _connection.Query<NationalResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), AreaID = areaId }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            // Pagination
            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            // reusult
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(NationalCreateModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string codeId = model.CodeID;
            string title = model.Title;
            string summary = model.Summary;
            if (string.IsNullOrWhiteSpace(title))
                return Notifization.Invalid("Không được để trống tiêu đề");
            title = title.Trim();
            if (!Validate.TestText(title))
                return Notifization.Invalid("Tiêu đề không hợp lệ");
            if (title.Length < 2 || title.Length > 80)
                return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
            //
            if (string.IsNullOrWhiteSpace(codeId))
                return Notifization.Invalid("Không được để trống mã quốc gia");
            codeId = codeId.Trim();
            if (!Validate.TestRoll(codeId))
                return Notifization.Invalid("Mã quốc gia không hợp lệ");
            if (codeId.Length < 2 || codeId.Length > 5)
                return Notifization.Invalid("Mã quốc gia giới hạn 2-5 ký tự");
            // summary valid               
            if (!string.IsNullOrWhiteSpace(summary))
            {
                summary = summary.Trim();
                if (!Validate.TestText(summary))
                    return Notifization.Invalid("Mô tả không hợp lệ");
                if (summary.Length < 1 || summary.Length > 120)
                    return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
            }
            //

            NationalService nationalService = new NationalService(_connection);
            National appTitle = nationalService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower()).FirstOrDefault();
            if (appTitle != null)
                return Notifization.Invalid("Tiêu đề đã được sử dụng");
            //
            National appCode = nationalService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower() == codeId.ToLower()).FirstOrDefault();
            if (appCode != null)
                return Notifization.Invalid("Mã quốc gia đã được sử dụng");
            //
            var id = nationalService.Create<string>(new National()
            {
                CodeID = codeId,
                Title = title,
                Alias = Helper.Page.Library.FormatToUni2NONE(title),
                Summary = summary,
                LanguageID = Helper.Page.Default.LanguageID,
                Enabled = model.Enabled,
            });
            return Notifization.Success(MessageText.CreateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(NationalUpdateModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string codeId = model.CodeID;
            string title = model.Title;
            string summary = model.Summary;
            if (string.IsNullOrWhiteSpace(title))
                return Notifization.Invalid("Không được để trống tiêu đề");
            title = title.Trim();
            if (!Validate.TestText(title))
                return Notifization.Invalid("Tiêu đề không hợp lệ");
            if (title.Length < 2 || title.Length > 80)
                return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
            //
            if (string.IsNullOrWhiteSpace(codeId))
                return Notifization.Invalid("Không được để trống mã quốc gia");
            codeId = codeId.Trim();
            if (!Validate.TestRoll(codeId))
                return Notifization.Invalid("Mã quốc gia không hợp lệ");
            if (codeId.Length < 2 || codeId.Length > 5)
                return Notifization.Invalid("Mã quốc gia giới hạn 2-5 ký tự");
            // summary valid               
            if (!string.IsNullOrWhiteSpace(summary))
            {
                summary = summary.Trim();
                if (!Validate.TestText(summary))
                    return Notifization.Invalid("Mô tả không hợp lệ");
                if (summary.Length < 1 || summary.Length > 120)
                    return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
            }
            //
            var nationalService = new NationalService(_connection);
            string id = model.ID.ToLower();
            National national = nationalService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id).FirstOrDefault();
            if (national == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            National nationalTitle = nationalService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && m.ID != id).FirstOrDefault();
            if (nationalTitle != null)
                return Notifization.Invalid("Tiêu đề đã được sử dụng");

            National nationalCode = nationalService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower() == codeId.ToLower() && m.ID != id).FirstOrDefault();
            if (nationalCode != null)
                return Notifization.Invalid("Mã quốc gia đã được sử dụng");
            // update user information
            national.Title = title;
            national.Alias = Helper.Page.Library.FormatToUni2NONE(title);
            national.Summary = summary;
            national.Enabled = model.Enabled;
            nationalService.Update(national);
            //
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public National GetNationalByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_National WHERE ID = @Query";
            return _connection.Query<National>(sqlQuery, new { Query = id }).FirstOrDefault();
        }
        public NationalResult ViewNationalByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_National WHERE ID = @Query";
            return _connection.Query<NationalResult>(sqlQuery, new { Query = id }).FirstOrDefault();
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(NationalIDModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string id = model.ID;
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.NotFound();
            //
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    id = id.ToLower();
                    var NationalService = new NationalService(_connection);
                    var National = NationalService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id, transaction: transaction).FirstOrDefault();
                    if (National == null)
                        return Notifization.NotFound();
                    //
                    NationalService.Remove(National.ID, transaction: transaction);
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
        public static string DropdownList(string id, int itineraryType = 0)
        {
            try
            {
                string result = string.Empty;
                using (var AppAreaService = new NationalService())
                {
                    var dtList = AppAreaService.DataOption(id);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrEmpty(id) && item.ID == id.ToLower())
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
        public List<NationalOptionModel> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_National ORDER BY Title ASC";
                return _connection.Query<NationalOptionModel>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<NationalOptionModel>();
            }
        }
        //Static function
        // ##############################################################################################################################################################################################################################################################
        public static string GetNationalName(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return string.Empty;
                //
                id = id.ToLower();
                NationalService appAreaService = new NationalService();
                var appArea = appAreaService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id).FirstOrDefault();
                if (appArea == null)
                    return string.Empty;
                // 
                return appArea.Title;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
