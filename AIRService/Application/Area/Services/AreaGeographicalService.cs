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

namespace WebCore.Services
{
    public interface IAreaGeographicalService : IEntityService<AreaGeographical> { }
    public class AreaGeographicalService : EntityService<AreaGeographical>, IAreaGeographicalService
    {
        public AreaGeographicalService() : base() { }
        public AreaGeographicalService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;
            string sqlQuery = @"SELECT * FROM App_Geographical WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
            var dtList = _connection.Query<ResultAreaGeographical>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count <= 0)
                return Notifization.NotFound(MessageText.NotFound);
            //    
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
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
        public ActionResult Create(AreaGeographicalCreateModel model)
        {
            _connection.Open();
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    AreaGeographicalService appAreaService = new AreaGeographicalService(_connection);
                    var appAreas = appAreaService.GetAlls(m => m.Title.ToLower() == model.Title.ToLower(), transaction: transaction);
                    if (appAreas.Count > 0)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    //
                    var id = appAreaService.Create<string>(new AreaGeographical()
                    {
                        Title = model.Title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(model.Title),
                        Summary = model.Summary,
                        LanguageID = Helper.Page.Default.LanguageID,
                        Enabled = model.Enabled,
                    }, transaction: transaction);
                    string temp = string.Empty;
                    //sort
                    transaction.Commit();
                    return Notifization.Success(MessageText.CreateSuccess);
                }
                catch
                {
                    transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(AreaGeographicalUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    var areaGeographicalService = new AreaGeographicalService(_connection);
                    string id = model.ID.ToLower();
                    var areaGeographical = areaGeographicalService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == id, transaction: _transaction).FirstOrDefault();
                    if (areaGeographical == null)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    string title = model.Title;
                    areaGeographical = areaGeographicalService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower() == title.ToLower() && areaGeographical.ID != id, transaction: _transaction).FirstOrDefault();
                    if (areaGeographical != null)
                        return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information
                    areaGeographical.Title = title;
                    areaGeographical.Alias = Helper.Page.Library.FormatToUni2NONE(model.Title);
                    areaGeographical.Summary = model.Summary;
                    areaGeographical.Enabled = model.Enabled;
                    areaGeographicalService.Update(areaGeographical, transaction: _transaction);
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch
                {
                    _transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }
        public AreaGeographical UpdateForm(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return null;
            string query = string.Empty;
            string sqlQuery = @"SELECT TOP (1) * FROM App_Geographical WHERE ID = @Query";
            return _connection.Query<AreaGeographical>(sqlQuery, new { Query = Id }).FirstOrDefault();
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
                    var  areaGeographicalService = new AreaGeographicalService(_connection);
                    var  areaGeographical = areaGeographicalService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID ==id, transaction: transaction).FirstOrDefault();
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
        public static string AreaDropdownList(string id)
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