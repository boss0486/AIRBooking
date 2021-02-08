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
    public interface IUserSpendingService : IEntityService<UserSpending> { }
    public class UserSpendingService : EntityService<UserSpending>, IUserSpendingService
    {
        public UserSpendingService() : base() { }
        public UserSpendingService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
            #region
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
            #endregion
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT s.*,u.FullName FROM App_UserSpending as s LEFT JOIN UserInfo  as u ON u.UserID = s.TicketingID
            WHERE dbo.Uni2NONE(u.FullName) LIKE N'%'+ @Query +'%' ORDER BY [CreatedDate]";
            //
            var dtList = _connection.Query<UserSpendingResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);


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
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Setting(UserSpendingCreateModel model)
        {
            string logUserId = Helper.Current.UserLogin.IdentifierID;
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string summary = model.Summary;
                    string agentId = model.AgentID;
                    string ticketingId = model.TicketingID;
                    double amount = model.Amount;
                    int enabled = model.Enabled;
                    //
                    AirAgentService airAgentService = new AirAgentService(_connection);
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    UserLoginService userLoginService = new UserLoginService(_connection);
                    UserService userService = new UserService(_connection);
                    UserSpendingService userSpendingService = new UserSpendingService(_connection);
                    UserInfoService userInfoService = new UserInfoService(_connection);
                    //
                    agentId = agentId.ToLower();
                    if (Helper.Current.UserLogin.IsAgentLogged())
                    {
                        ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == logUserId, transaction: _transaction).FirstOrDefault();
                        if (clientLogin == null)
                            return Notifization.Invalid("Đại lý không hợp lệ");
                        //
                        agentId = clientLogin.AgentID;
                    }
                    if (string.IsNullOrWhiteSpace(agentId))
                        return Notifization.Invalid("Đại lý không hợp lệ");
                    //
                    AirAgent airAgent = airAgentService.GetAlls(m => m.ID == agentId, transaction: _transaction).FirstOrDefault();
                    if (airAgent == null)
                        return Notifization.Invalid("Đại lý không hợp lệ");
                    // 
                    if (airAgent.Enabled != (int)ModelEnum.Enabled.ENABLED)
                        return Notifization.Invalid("Đại lý đã tạm dừng hoạt động");
                    //
                    string parendId = airAgent.ParentID;


                    if (string.IsNullOrWhiteSpace(ticketingId))
                        return Notifization.Invalid("Vui lòng chọn nhân viên");
                    //  
                    ClientLogin clientLogin1 = clientLoginService.GetAlls(m => m.UserID == ticketingId && m.AgentID == agentId, transaction: _transaction).FirstOrDefault();
                    if (clientLogin1 == null)
                        return Notifization.Invalid("Nhân viên không hợp lệ");
                    //
                    UserInfo userInfo = userInfoService.GetAlls(m => m.UserID == ticketingId, transaction: _transaction).FirstOrDefault();
                    // get spending of agent
                    AgentSpendingLimitService agentSpendingLimitService = new AgentSpendingLimitService(_connection);

                    if (!string.IsNullOrWhiteSpace(parendId))
                    {
                        AgentSpendingLimit agentSpendingLimit = agentSpendingLimitService.GetAlls(m => m.AgentID == agentId, transaction: _transaction).FirstOrDefault();
                        if (agentSpendingLimit == null)
                            return Notifization.Invalid("Lỗi hạn mức đại lý");
                        // 
                        double agentSpendingTotal = agentSpendingLimit.Amount; //  tinh toan luong da cap cho nhan vien #
                        double agentSpending = userSpendingService.GetAlls(m => m.AgentID == agentId && m.TicketingID != ticketingId, transaction: _transaction).Sum(m => m.Amount);
                        double agentRemaining = agentSpendingTotal - agentSpending;
                        if (amount < 0 || amount > agentRemaining)
                            return Notifization.Invalid($"Hạn mức giới hạn [0-{Helper.Page.Library.FormatCurrency(agentRemaining)}]");
                    }
                    //            
                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }
                    //

                    UserSpending userSpending = userSpendingService.GetAlls(m => m.TicketingID == ticketingId, transaction: _transaction).FirstOrDefault();
                    // create
                    if (userSpending == null)
                    {
                        userSpendingService.Create<string>(new UserSpending()
                        {
                            Summary = summary,
                            AgentID = agentId,
                            TicketingID = ticketingId,
                            Amount = amount,
                            Enabled = (int)WebCore.Model.Enum.ModelEnum.Enabled.ENABLED
                        }, transaction: _transaction);
                        // History   
                        UserSpendingHistoryService.LoggedUserSpendingHistory(new UserSpendingHistoryCreateModel
                        {
                            AgentID = agentId,
                            Amount = amount,
                            UserID = ticketingId,
                            FullName = userInfo.FullName,
                            Summary = summary,
                            TransactionType = (int)TransactionEnum.TransactionType.NONE
                        }, connection: _connection, transaction: _transaction);
                    }
                    else
                    {
                        userSpending.Amount = amount;
                        userSpendingService.Update(userSpending, transaction: _transaction);
                        // History
                        UserSpendingHistoryService.LoggedUserSpendingHistory(new UserSpendingHistoryCreateModel
                        {
                            AgentID = agentId,
                            Amount = amount,
                            UserID = ticketingId,
                            FullName = userInfo.FullName,
                            Summary = summary,
                            TransactionType = (int)TransactionEnum.TransactionType.NONE
                        }, connection: _connection, transaction: _transaction);
                    }
                    //commit
                    _transaction.Commit();
                    return Notifization.Success(MessageText.UpdateSuccess);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST(">>:" + ex);
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public UserSpendingResult GetUserSpendingModel(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_UserSpending WHERE ID = @Query";
                return _connection.Query<UserSpendingResult>(sqlQuery, new { Query = Id }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        //

        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.Invalid(MessageText.Invalid);
            //
            id = id.ToLower();
            using (var _connectDb = DbConnect.Connection.CMS)
            {
                _connectDb.Open();
                using (var _transaction = _connectDb.BeginTransaction())
                {
                    try
                    {
                        UserSpendingService transactionUserSpendingService = new UserSpendingService(_connectDb);
                        var transactionUserSpending = transactionUserSpendingService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                        if (transactionUserSpending == null)
                            return Notifization.NotFound();
                        transactionUserSpendingService.Remove(transactionUserSpending.ID, transaction: _transaction);
                        // remover seo
                        _transaction.Commit();
                        return Notifization.Success(MessageText.DeleteSuccess);
                    }
                    catch
                    {
                        _transaction.Rollback();
                        return Notifization.NotService;
                    }
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM App_UserSpending WHERE ID = @ID";
                var item = _connection.Query<UserSpendingResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
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
        //##############################################################################################################################################################################################################################################################
        public static Double GetSpendingLimit(string userId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (connection == null)
                connection = DbConnect.Connection.CMS;
            //
            UserSpendingService userSettingService = new UserSpendingService(connection);
            UserSpending userSpending = userSettingService.GetAlls(m => m.TicketingID == userId, transaction: transaction).FirstOrDefault();
            if (userSpending == null)
                return 0;
            //
            return userSpending.Amount;

        }

        public static double GetBalance(string userId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (connection == null)
                connection = DbConnect.Connection.CMS;
            //
            UserClientService userClientService = new UserClientService(connection);
            ClientLoginService clientLoginService = new ClientLoginService(connection);
            AirAgentService airAgentService = new AirAgentService(connection);
            ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == userId, transaction: transaction).FirstOrDefault();
            if (clientLogin == null)
                return 0;
            // get spending limit
            string agentId = clientLogin.AgentID;
            AgentSpendingLimitService agentSpendingLimitService = new AgentSpendingLimitService(connection);
            AgentSpendingLimit agentSpendingLimit = agentSpendingLimitService.GetAlls(m => m.AgentID == agentId, transaction).FirstOrDefault();
            if (agentSpendingLimit == null)
                return 0;
            //
            double spendingAmount = agentSpendingLimit.Amount;
            //
            DateTime today = Helper.TimeData.TimeHelper.UtcDateTime;
            DateTime firstDayOfMonth = today.AddMonths(-1);
            firstDayOfMonth = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.Month, 01);
            int paymentDayTotal = today.Subtract(firstDayOfMonth).Days;
            string sqlQuery = $@" SELECT (
                ISNULL((select sum (Amount) from App_BookPrice where BookOrderID = o.ID),0) +
                ISNULL((select sum (Amount) from App_BookTax where BookOrderID = o.ID),0) +
                ISNULL((select sum (AgentPrice + ProviderFee + AgentFee) from App_BookAgent where BookOrderID = o.ID),0) 
                ) as 'Spent'              
                FROM App_BookOrder AS o
                INNER JOIN App_BookAgent AS a ON a.BookOrderID = o.ID 
                WHERE a.AgentID = @AgentID AND a.TicketingID = @TicketingID 
                AND cast(IssueDate as Date) >= cast('{firstDayOfMonth}' as Date) AND cast(IssueDate as Date) <= cast('{today}' as Date)";
            double totalSpent = agentSpendingLimitService.Query<double>(sqlQuery, new { AgentID = agentId, TicketingID = userId }).FirstOrDefault();
            // 
            return spendingAmount - totalSpent;
        }
        //##############################################################################################################################################################################################################################################################
    }
}
