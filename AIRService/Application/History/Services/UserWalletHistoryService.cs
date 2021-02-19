﻿using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using Helper;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using WebCore.Model.Enum;
using WebCore.Core;
using WebCore.Entities;
using WebCore.Services;
using Helper.Page;
using WebCore.Model.Entities;
using WebCore.ENM;
using System.Data;

namespace WebCore.Services
{
    public interface IUserWalletHistoryService : IEntityService<UserWalletHistory> { }
    public class UserWalletHistoryService : EntityService<UserWalletHistory>, IUserWalletHistoryService
    {
        public UserWalletHistoryService() : base() { }
        public UserWalletHistoryService(System.Data.IDbConnection db) : base(db) { }
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
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT *, u.FullName  FROM App_UserWalletHistory 
            WHERE dbo.Uni2NONE(FullName) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY [CreatedDate] DESC";
            var dtList = _connection.Query<UserWalletHistoryResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count <= 0 && page > 1)
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


        public static HistoryMessageModel LoggedUserWalletHistory(UserWalletHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            if (model == null)
                return new HistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" };
            //
            string clientId = model.AgentID;
            string userId = model.UserID;
            double amount = model.Amount;
            int transType = model.TransactionType;
            //
            string transState = "x";
            if (transType == (int)TransactionEnum.TransactionType.IN)
                transState = "+";
            if (transType == (int)TransactionEnum.TransactionType.OUT)
                transState = "-";
            //
            string title = "Số dư thay đổi. GD " + transState + " " + Helper.Page.Library.FormatCurrency(amount) + " đ";
            string summary = "";
            if (dbConnection == null)
                dbConnection = DbConnect.Connection.CMS;
            //
            UserWalletHistoryService UserWalletHistoryService = new UserWalletHistoryService(dbConnection);

            UserWalletHistoryService.Create<string>(new UserWalletHistory()
            {
                UserID = userId,
                AgentID = clientId,
                Title = title,
                Summary = summary,
                Amount = amount,
                TransactionType = transType,
                Status = 1,
                Enabled = (int)WebCore.Model.Enum.ModelEnum.Enabled.ENABLED
            }, transaction: dbTransaction);
            //commit
            return new HistoryMessageModel { Status = true, Message = "Ok" };
        }
        //##############################################################################################################################################################################################################################################################
    }
}