using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebCore.Entities;
using WebCore.Model.Enum;

namespace WebCore.Services
{
    public interface ILanguageService : IEntityService<Language> { }
    public class LanguageService : EntityService<Language>, ILanguageService
    {
        public LanguageService() : base() { }
        public LanguageService(System.Data.IDbConnection db) : base(db) { }

        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(string strQuery, int page, string langID)
        {
            string query = string.Empty;
            if (string.IsNullOrEmpty(strQuery))
                query = "";
            else
                query = strQuery;

            if (string.IsNullOrEmpty(langID))
                langID = Helper.Page.Default.LanguageID;

            string sqlQuery = @"SELECT 
                                     [ID]
                                    ,[Title]
                                    ,[LanguageID]
                                    ,[LangID]
                                    ,[Enabled]
                                    ,[CreatedDate]
                                     FROM Language 
                                     WHERE (dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%')  
                                     AND LangID = @LangID ORDER BY Title ASC";
            var dtList = _connection.Query<LanguageModel>(sqlQuery, new { Query = query, @LangID = langID }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);

            List<RsLanguage> resultData = new List<RsLanguage>();
            foreach (var item in dtList)
            {
                string isBlock = string.Empty;
                string isEnable = string.Empty;
                if (item.Enabled == (int)ModelEnum.Enabled.ENABLED)
                    isEnable = "Actived";
                else
                    isEnable = "Not active";

                resultData.Add(new RsLanguage(item.ID, item.Title, item.LanguageID, item.LangID, isEnable, Convert.ToString(item.CreatedDate)));
            }
            var result = resultData.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
            {
                page -= 1;
                result = resultData.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count <= 0)
                return Notifization.NotFound(MessageText.NotFound);
            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            return Notifization.Data(MessageText.Success, data: result, paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################
        public static string DdlLanguage(string langId, string selected)
        {
            try
            {
                string result = string.Empty;
                using (LanguageService languageService = new LanguageService())
                {
                    List<LanguageOption> dtList = languageService.DataOption(langId);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (!string.IsNullOrWhiteSpace(selected) && item.LanguageID == selected.ToLower())
                                select = "selected";
                            result += "<option value='" + item.LanguageID + "'" + select + ">" + item.Title + "</option>";
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
        public List<LanguageOption> DataOption(string langID)
        {
            try
            {
                List<LanguageOption> optionList = new List<LanguageOption>();
                string sqlQuery = @"SELECT [LanguageID],[Title] FROM Language ORDER BY Title ASC";
                optionList = _connection.Query<LanguageOption>(sqlQuery, new { LangID = langID }).ToList();
                return optionList;
            }
            catch
            {
                return new List<LanguageOption>();
            }
        }

    }
    //##############################################################################################################################################################################################################################################################



    //##############################################################################################################################################################################################################################################################
}