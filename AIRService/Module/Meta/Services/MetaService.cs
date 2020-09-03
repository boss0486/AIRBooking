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
    public interface IMetaSEOService : IEntityService<Meta> { }
    public class MetaService : EntityService<Meta>, IMetaSEOService
    {
        public MetaService() : base() { }
        public MetaService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Datalist(string strQuery, int page)
        {
            try
            {
                string query = string.Empty;
                if (string.IsNullOrEmpty(strQuery))
                    query = "";
                else
                    query = strQuery;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM View_App_Meta WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                    ORDER BY [CreatedDate]";
                var dtList = _connection.Query<Meta>(sqlQuery, new { Query = query }).ToList();
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
            catch
            {
                return Notifization.NotService;
            }
        }

        //##############################################################################################################################################################################################################################################################
        public ActionResult Create(MetaCreateModel model)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        MetaService MetaService = new MetaService(_connection);
                        var Metas = MetaService.GetAlls(m => m.MetaTitle.ToLower() == model.MetaTitle.ToLower(), transaction: transaction);
                        if (Metas.Count > 0)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");

                        var Id = MetaService.Create<string>(new Meta()
                        {
                            MetaTitle = model.MetaTitle,
                            Alias = Helper.Page.Library.FormatToUni2NONE(model.MetaTitle),
                            MetaDescription = model.MetaDescription,
                            MetaKeyword = model.MetaKeyword,
                            LanguageID = Helper.Current.UserLogin.LanguageID,
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
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Update(MetaUpdateModel model)
        {
            try
            {
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        var MetaService = new MetaService(_connection);
                        string Id = model.ID.ToLower();
                        var Meta = MetaService.GetAlls(m => m.ID.Equals(Id), transaction: transaction).FirstOrDefault();
                        if (Meta == null)
                            return Notifization.NotFound(MessageText.NotFound);

                        string title = model.MetaTitle;
                        var dpm = MetaService.GetAlls(m => m.MetaTitle.ToLower().Equals(title.ToLower()) && !Meta.ID.ToLower().Equals(Id), transaction: transaction).ToList();
                        if (dpm.Count > 0)
                            return Notifization.Invalid("Tiêu đề đã được sử dụng");
                        // update user information
                        Meta.MetaTitle = title;
                        Meta.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                        Meta.MetaDescription = model.MetaDescription;
                        Meta.MetaKeyword = model.MetaKeyword;
                        Meta.Enabled = model.Enabled;
                        MetaService.Update(Meta, transaction: transaction);
                        transaction.Commit();
                        return Notifization.Success(MessageText.UpdateSuccess);
                    }
                    catch
                    {
                        transaction.Rollback();
                        return Notifization.NotService;
                    }
                }
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        public Meta UpdateForm(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_Meta WHERE ID = @Query";
                return _connection.Query<Meta>(sqlQuery, new { Query = Id }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string Id)
        {
            try
            {
                if (Id == null)
                    return Notifization.NotFound();
                _connection.Open();
                using (var transaction = _connection.BeginTransaction())
                {
                    try
                    {
                        var MetaService = new MetaService(_connection);
                        var Meta = MetaService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                        if (Meta == null)
                            return Notifization.NotFound();
                        MetaService.Remove(Meta.ID, transaction: transaction);
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
            catch
            {
                return Notifization.NotService;
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
                string sqlQuery = @"SELECT * FROM View_App_Meta WHERE ID = @ID";
                var item = _connection.Query<MetaResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
                if (item == null)
                    return Notifization.NotFound(MessageText.NotFound);
                //
                return Notifization.Data(MessageText.Success, data: item, role: null, paging: null);
            }
            catch
            {
                return Notifization.NotService;
            }
        }
        //##############################################################################################################################################################################################################################################################
        public static string DDLMeta(string id)
        {
            try
            {
                string result = string.Empty;
                using (var MetaService = new MetaService())
                {
                    var dtList = MetaService.DataOption(id);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (item.ID.Equals(id.ToLower()))
                                select = "selected";
                            result += "<option value='" + item.ID + "'" + select + ">" + item.MetaTitle + "</option>";
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
        public List<MetaOption> DataOption(string langID)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_App_Meta ORDER BY Title ASC";
                return _connection.Query<MetaOption>(sqlQuery, new { LangID = langID }).ToList();
            }
            catch
            {
                return new List<MetaOption>();
            }
        }
    }
}