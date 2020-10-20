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

namespace WebCore.Services
{
    public interface ITransactionCustomerSpendingService : IEntityService<TransactionCustomerSpending> { }
    public class TransactionCustomerSpendingService : EntityService<TransactionCustomerSpending>, ITransactionCustomerSpendingService
    {
        public TransactionCustomerSpendingService() : base() { }
        public TransactionCustomerSpendingService(System.Data.IDbConnection db) : base(db) { }
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
            if (Helper.Current.UserLogin.IsSupplierLogged())
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                string supplierId = ClientLoginService.GetClientIDByUserID(userId);
                // admin of supplier
                if (Helper.Current.UserLogin.IsAdminSupplierLogged())
                {
                    whereCondition += " AND SupplierID = '" + supplierId + "'";
                }
                else  // emp of supplier
                {
                    whereCondition += " AND SendUserID = '" + userId + "'";
                }
            }
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_TransactionCustomerSpending WHERE (dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' OR ID LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY [CreatedDate]";
            //
            var dtList = _connection.Query<TransactionCustomerSpendingResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query) }).ToList();
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
            //
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################

        public ActionResult Create(TransactionCustomerSpendingCreateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string title = "Giao dịch cấp hạn mức";
                    string summary = model.Summary;
                    string customerId = model.CustomerID;
                    double amount = model.Amount;
                    string languageId = Helper.Current.UserLogin.LanguageID;
                    //
                    string userId = Helper.Current.UserLogin.IdentifierID;
                    string supplierId = model.SupplierID;
                    if (Helper.Current.UserLogin.IsAdminSupplierLogged() || Helper.Current.UserLogin.IsSupplierLogged())
                        supplierId = ClientLoginService.GetClientIDByUserID(userId);
                    // 
                    if (string.IsNullOrWhiteSpace(supplierId))
                        return Notifization.Invalid(MessageText.Invalid);
                    //
                    CustomerService customerService = new CustomerService(_connection);
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    UserLoginService userLoginService = new UserLoginService(_connection);
                    //
                    if (string.IsNullOrWhiteSpace(customerId))
                        return Notifization.Invalid("Vui lòng chọn khách hàng");
                    //
                    customerId = customerId.ToLower();
                    var customer = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID == customerId, transaction: _transaction).FirstOrDefault();
                    if (customer == null)
                        return Notifization.Invalid("Khách hàng không hợp lệ");
                    //
                    if (amount <= 0 || amount > 100000000)
                        return Notifization.Invalid("Số tiền hạn mức giới hạn [1-100 000 000]");
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
                    TransactionCustomerSpendingService transactionSpendingService = new TransactionCustomerSpendingService(_connection);
                    var transactionSpendingId = transactionSpendingService.Create<string>(new TransactionCustomerSpending()
                    {
                        Title = title,
                        Summary = summary,
                        SupplierID = supplierId,
                        SupplierUserID = userId,
                        CustomerID = customerId,
                        Amount = amount,
                        Status = (int)TransactionEnum.TransactionType.IN,
                        LanguageID = languageId,
                        Enabled = (int)WebCore.Model.Enum.ModelEnum.Enabled.ENABLED
                    }, transaction: _transaction);
                    //
                    // update balance ************************************************************************************************************************************
                    WalletCustomerMessageModel balanceCustomer = WalletService.GetBalanceByCustomerID(customerId, dbConnection: _connection, dbTransaction: _transaction);
                    if (!balanceCustomer.Status)
                        return Notifization.Error("Không thể cập nhật giao dịch");
                    // 
                    var changeBalanceSpendingForCustomerStatus = WalletService.ChangeBalanceSpendingForCustomer(new WalletCustomerChangeModel { CustomerID = customerId, Amount = amount, TransactionType = (int)TransactionEnum.TransactionType.IN }, dbConnection: _connection, dbTransaction: _transaction);
                    if (!changeBalanceSpendingForCustomerStatus.Status)
                        return Notifization.Error("Không thể cập nhật giao dịch");
                    // create histories for balance changed
                    var loggerWalletCustomerHistoryStatus = TransactionHistoryService.LoggerTransactionDepositHistory(new TransactionDepositHistoryCreateModel
                    {
                        SenderID =  userId,
                        ReceivedID = customerId,
                        Amount = amount,
                        NewBalance = balanceCustomer.SpendingLimitBalance + amount,
                        TransactionType = (int)TransactionEnum.TransactionType.IN
                    }, dbConnection: _connection, dbTransaction: _transaction);
                    //
                    if (!loggerWalletCustomerHistoryStatus.Status)
                        return Notifization.Error("Không thể cập nhật giao dịch");
                    // end update balance ************************************************************************************************************************************
                    //commit
                    _transaction.Commit();
                    return Notifization.Success(MessageText.CreateSuccess);
                }
                catch
                {
                    _transaction.Rollback();
                    return Notifization.NotService;
                }
            }
        }

        public TransactionCustomerSpendingResult GetTransactionSpendingModel(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_TransactionCustomerSpending WHERE ID = @Query";
                return _connection.Query<TransactionCustomerSpendingResult>(sqlQuery, new { Query = Id }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Notifization.Invalid(MessageText.Invalid);
            id = id.ToLower();
            using (var _connectDb = DbConnect.Connection.CMS)
            {
                _connectDb.Open();
                using (var transaction = _connectDb.BeginTransaction())
                {
                    try
                    {
                        TransactionCustomerSpendingService transactionSpendingService = new TransactionCustomerSpendingService(_connectDb);
                        var TransactionSpending = transactionSpendingService.GetAlls(m => m.ID == id, transaction: transaction).FirstOrDefault();
                        if (TransactionSpending == null)
                            return Notifization.NotFound();
                        //
                        transactionSpendingService.Remove(TransactionSpending.ID, transaction: transaction);
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
        }
        //##############################################################################################################################################################################################################################################################
        public ActionResult Details(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT * FROM App_TransactionCustomerSpending WHERE ID = @ID";
                var item = _connection.Query<TransactionCustomerSpendingResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
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
    }
}
