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
    public interface IAirlineService : IEntityService<Airline> { }
    public class AirlineService : EntityService<Airline>, IAirlineService
    {
        public AirlineService() : base() { }
        public AirlineService(System.Data.IDbConnection db) : base(db) { }
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
            string sqlQuery = @"SELECT * FROM App_Airline WHERE Title LIKE N'%'+ @Query +'%'" + whereCondition + " ORDER BY [Title] ASC";
            var dtList = _connection.Query<AirlineResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), AreaID = areaId }).ToList();
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
        public ActionResult Create(AirlineCreateModel model)
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
                return Notifization.Invalid("Không được để trống mã hàng không");
            codeId = codeId.Trim();
            if (!Validate.TestRoll(codeId))
                return Notifization.Invalid("Mã hàng không không hợp lệ");
            if (codeId.Length < 2 || codeId.Length > 5)
                return Notifization.Invalid("Mã hàng không giới hạn 2-5 ký tự");
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

            AirlineService airlineService = new AirlineService(_connection);
            Airline appTitle = airlineService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower()).FirstOrDefault();
            if (appTitle != null)
                return Notifization.Invalid("Tiêu đề đã được sử dụng");
            //
            Airline appCode = airlineService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower() == codeId.ToLower()).FirstOrDefault();
            if (appCode != null)
                return Notifization.Invalid("Mã hàng không đã được sử dụng");
            //
            var id = airlineService.Create<string>(new Airline()
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
        public ActionResult Update(AirlineUpdateModel model)
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
                return Notifization.Invalid("Không được để trống mã hàng không");
            codeId = codeId.Trim();
            if (!Validate.TestRoll(codeId))
                return Notifization.Invalid("Mã hàng không không hợp lệ");
            if (codeId.Length < 2 || codeId.Length > 5)
                return Notifization.Invalid("Mã hàng không giới hạn 2-5 ký tự");
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
            AirlineService airlineService = new AirlineService(_connection);
            string id = model.ID.ToLower();
            Airline airline = airlineService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id).FirstOrDefault();
            if (airline == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            Airline airlineTitle = airlineService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && m.ID != id).FirstOrDefault();
            if (airlineTitle != null)
                return Notifization.Invalid("Tiêu đề đã được sử dụng");

            Airline airlineCode = airlineService.GetAlls(m => !string.IsNullOrWhiteSpace(m.CodeID) && m.CodeID.ToLower() == codeId.ToLower() && m.ID != id).FirstOrDefault();
            if (airlineCode != null)
                return Notifization.Invalid("Mã hàng không đã được sử dụng");
            // update user information
            airline.CodeID = codeId;
            airline.Title = title;
            airline.Alias = Helper.Page.Library.FormatToUni2NONE(title);
            airline.Summary = summary;
            airline.Enabled = model.Enabled;
            airlineService.Update(airline);
            //
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public Airline GetAirlineByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_Airline WHERE ID = @Query";
            return _connection.Query<Airline>(sqlQuery, new { Query = id }).FirstOrDefault();
        }
        public AirlineResult ViewAirlineByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_Airline WHERE ID = @Query";
            return _connection.Query<AirlineResult>(sqlQuery, new { Query = id }).FirstOrDefault();
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(AirlineIDModel model)
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
                    AirlineService airlineService = new AirlineService(_connection);
                    Airline airline = airlineService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id, transaction: transaction).FirstOrDefault();
                    if (airline == null)
                        return Notifization.NotFound();
                    //
                    airlineService.Remove(airline.ID, transaction: transaction);
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
                using (var AppAreaService = new AirlineService())
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
        public List<AirlineOptionModel> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_Airline ORDER BY Title ASC";
                return _connection.Query<AirlineOptionModel>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<AirlineOptionModel>();
            }
        }
        //Static function
        // ##############################################################################################################################################################################################################################################################
        public static string GetAirlineName(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return string.Empty;
                //
                id = id.ToLower();
                AirlineService appAreaService = new AirlineService();
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
