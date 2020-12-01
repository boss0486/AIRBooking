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
    public interface IAreaGeographicalService : IEntityService<AreaGeographical> { }
    public class AreaGeographicalService : EntityService<AreaGeographical>, IAreaGeographicalService
    {
        public AreaGeographicalService() : base() { }
        public AreaGeographicalService(System.Data.IDbConnection db) : base(db) { }
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
            string sqlQuery = @"SELECT * FROM App_Geographical WHERE Title LIKE N'%'+ @Query +'%'" + whereCondition + " ORDER BY [Title] ASC";
            var dtList = _connection.Query<AreaGeographicalResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query), AreaID = areaId }).ToList();
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
        public ActionResult Create(AreaGeographicalCreateModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string nationalId = model.NationalID;
            string title = model.Title;
            string summary = model.Summary;
            if (string.IsNullOrWhiteSpace(nationalId))
                return Notifization.Invalid("Vui lòng lựa chọn quốc gia");
            //
            if (string.IsNullOrWhiteSpace(title))
                return Notifization.Invalid("Không được để trống tiêu đề");
            title = title.Trim();
            if (!Validate.TestText(title))
                return Notifization.Invalid("Tiêu đề không hợp lệ");
            if (title.Length < 2 || title.Length > 80)
                return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
            // 
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
            AreaGeographicalService appAreaService = new AreaGeographicalService(_connection);
            AreaGeographical areaGeographical = appAreaService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.NationalID == nationalId && m.Title.ToLower() == title.ToLower()).FirstOrDefault();
            if (areaGeographical != null)
                return Notifization.Invalid("Tiêu đề đã được sử dụng");
            //
            var id = appAreaService.Create<string>(new AreaGeographical()
            {
                NationalID = nationalId,
                Title = title,
                Alias = Helper.Page.Library.FormatToUni2NONE(title),
                Summary = summary,
                LanguageID = Helper.Page.Default.LanguageID,
                Enabled = model.Enabled,
            });
            string temp = string.Empty;
            //sort
            return Notifization.Success(MessageText.CreateSuccess);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(AreaGeographicalUpdateModel model)
        {
            if (model == null)
                return Notifization.Invalid();
            //
            string nationalId = model.NationalID;
            string title = model.Title;
            string summary = model.Summary;
            if (string.IsNullOrWhiteSpace(nationalId))
                return Notifization.Invalid("Vui lòng lựa chọn quốc gia");
            //
            if (string.IsNullOrWhiteSpace(title))
                return Notifization.Invalid("Không được để trống tiêu đề");
            title = title.Trim();
            if (!Validate.TestText(title))
                return Notifization.Invalid("Tiêu đề không hợp lệ");
            if (title.Length < 2 || title.Length > 80)
                return Notifization.Invalid("Tiêu đề giới hạn 2-80 ký tự");
            // 
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
            var areaGeographicalService = new AreaGeographicalService(_connection);
            string id = model.ID.ToLower();
            AreaGeographical areaGeographical = areaGeographicalService.GetAlls(m => m.ID == id).FirstOrDefault();
            if (areaGeographical == null)
                return Notifization.NotFound(MessageText.NotFound);
            //
            AreaGeographical areaGeographicalTitle = areaGeographicalService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && m.ID != id && m.NationalID == nationalId).FirstOrDefault();
            if (areaGeographicalTitle != null)
                return Notifization.Invalid("Tiêu đề đã được sử dụng");
            // update user information
            areaGeographical.NationalID = nationalId;
            areaGeographical.Title = title;
            areaGeographical.Alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
            areaGeographical.Summary = model.Summary;
            areaGeographical.Enabled = model.Enabled;
            areaGeographicalService.Update(areaGeographical);
            return Notifization.Success(MessageText.UpdateSuccess);
        }
        public AreaGeographical GetAreaGeographicalByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_Geographical WHERE ID = @Query";
            return _connection.Query<AreaGeographical>(sqlQuery, new { Query = id }).FirstOrDefault();
        }
        public AreaGeographicalResult ViewAreaGeographicalByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_Geographical WHERE ID = @Query";
            return _connection.Query<AreaGeographicalResult>(sqlQuery, new { Query = id }).FirstOrDefault();
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(AreaGeographicalIDModel model)
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
                    var areaGeographicalService = new AreaGeographicalService(_connection);
                    var areaGeographical = areaGeographicalService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id, transaction: transaction).FirstOrDefault();
                    if (areaGeographical == null)
                        return Notifization.NotFound();
                    //
                    areaGeographicalService.Remove(areaGeographical.ID, transaction: transaction);
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
        public static string AreaDropdownList(string id, int itineraryType = 0)
        {
            try
            {
                string result = string.Empty;
                using (var AppAreaService = new AreaGeographicalService())
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
        public List<AreaGeographicalOptionModel> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM App_Geographical ORDER BY Title ASC";
                return _connection.Query<AreaGeographicalOptionModel>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<AreaGeographicalOptionModel>();
            }
        }
        //Static function
        // ##############################################################################################################################################################################################################################################################
        public static string GetAreaName(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return string.Empty;
                //
                id = id.ToLower();
                AreaGeographicalService appAreaService = new AreaGeographicalService();
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
