using AL.NetFrame.Attributes;
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
    public interface ITransactionUserSpendingService : IEntityService<TransactionUserSpending> { }
    public class TransactionUserSpendingService : EntityService<TransactionUserSpending>, ITransactionUserSpendingService
    {
        public TransactionUserSpendingService() : base() { }
        public TransactionUserSpendingService(System.Data.IDbConnection db) : base(db) { }
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
            string sqlQuery = @"SELECT * FROM App_TransactionUserSpending WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' ORDER BY [CreatedDate]";
            //
            var dtList = _connection.Query<TransactionUserSpendingResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query) }).ToList();
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
        public ActionResult Create(TransactionUserSpendingCreateModel model)
        {
            _connection.Open();
            string sendUserId = Helper.Current.UserLogin.IdentifierID;
            string currentUserId = Helper.Current.UserLogin.IdentifierID;
            string languageId = Helper.Current.UserLogin.LanguageID;
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string title = "Thay đổi hạn mức";
                    string summary = model.Summary;
                    string senderId = model.SenderID;

                    string receivedUserId = model.ReceivedUserID;
                    double amount = model.Amount;
                    int enabled = model.Enabled;
                    //
                    CustomerService customerService = new CustomerService(_connection);
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    UserLoginService userLoginService = new UserLoginService(_connection);
                    UserService userService = new UserService(_connection);
                    //
                    senderId = senderId.ToLower();
                    var customer = customerService.GetAlls(m => m.ID == senderId, transaction: _transaction).FirstOrDefault();
                    if (customer == null)
                        return Notifization.Invalid("Khách hàng không hợp lệ");
                    //
                    if (string.IsNullOrWhiteSpace(receivedUserId))
                        return Notifization.Invalid("Vui lòng chọn nhân viên");
                    // 
                    receivedUserId = receivedUserId.Trim().ToLower();
                    var clientReceived = clientLoginService.GetAlls(m => m.UserID == receivedUserId, transaction: _transaction).FirstOrDefault();
                    if (clientReceived == null)
                        return Notifization.Invalid("Nhân viên không hợp lệ");
                    //
                    string received = clientReceived.ClientID;

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
                    if (userService.IsCustomerLogged(currentUserId, _connection, _transaction))
                    {
                        // check han muc cua customer
                        WalletClientMessageModel balanceSender = WalletService.GetBalanceByClientID(senderId, dbConnection: _connection, dbTransaction: _transaction);
                        if (!balanceSender.Status)
                            return Notifization.Error("Không thể cập nhật giao dịch");
                        //
                        if (amount > balanceSender.SpendingBalance)
                            return Notifization.Invalid("Số dư không đủ để giao dịch");
                        //
                    }
                    else if (userService.IsSupplierLogged(currentUserId, _connection, _transaction))
                    {


                    }

                    TransactionUserSpendingService transactionUserSpendingService = new TransactionUserSpendingService(_connection);
                    var Id = transactionUserSpendingService.Create<string>(new TransactionUserSpending()
                    {
                        Title = title,
                        Summary = summary,
                        SenderID = senderId,
                        SenderUserID = sendUserId,
                        ReceivedID = received,
                        ReceivedUserID = receivedUserId,
                        Amount = amount,
                        Status = (int)TransactionEnum.TransactionType.IN,
                        LanguageID = languageId,
                        Enabled = (int)WebCore.Model.Enum.ModelEnum.Enabled.ENABLED
                    }, transaction: _transaction);
                    // #1. sender ************************************************************************************************************************************

                    // Cấp hạn mức người dùng
                    // + Thay đổi tài khoản người dùng
                    // + Lịch sử ví chi tiêu của người dùng
                    // * Nhà cung cấp:
                    //     -Thay đổi ví đầu tư
                    //     -Lịch sử ví đầu tư
                    //   *Khách hàng:
                    //   -Thay đổi Tài khoản chính
                    //   -Lịch sử tài khoản chính

                    //if (userService.IsCustomerLogged(currentUserId, _connection, _transaction))
                    //{
                    //    var changeSpendingBalance = WalletService.ChangeSpendingBalance(new WalletClientChangeModel { ClientID = senderId, Amount = amount, TransactionType = (int)TransactionEnum.TransactionType.OUT }, dbConnection: _connection, dbTransaction: _transaction);
                    //    if (!changeSpendingBalance.Status)
                    //        return Notifization.Error("Không thể cập nhật giao dịch");
                    //    // create histories for balance changed
                    //    var balanceCustomerHistoryStatus = LoggerHistoryService.LoggerWalletSpendingHistory(new WalletSpendingHistoryCreateModel
                    //    {
                    //        SenderID = senderId,
                    //        SenderUserID = sendUserId,
                    //        ReceivedID = receivedUserId,
                    //        Amount = amount,
                    //        NewBalance = balanceSender.SpendingBalance - amount,
                    //        TransactionType = (int)TransactionEnum.TransactionType.OUT,
                    //        TransactionOriginal = (int)TransactionEnum.TransactionOriginal.SPENDING
                    //    }, dbConnection: _connection, dbTransaction: _transaction);
                    //}
                    //else if (userService.IsSupplierLogged(currentUserId, _connection, _transaction))
                    //{
                    //    var changeSpendingBalance = WalletService.ChangeInvestmentBalance(new WalletClientChangeModel { ClientID = senderId, Amount = amount, TransactionType = (int)TransactionEnum.TransactionType.IN }, dbConnection: _connection, dbTransaction: _transaction);
                    //    if (!changeSpendingBalance.Status)
                    //        return Notifization.Error("Không thể cập nhật giao dịch");
                    //    // create histories for balance changed
                    //    var balanceCustomerHistoryStatus = LoggerHistoryService.LoggerWalletInvestmentHistory(new WalletInvestmentHistoryCreateModel
                    //    {
                    //        SenderID = senderId,
                    //        SenderUserID = sendUserId,
                    //        ReceivedID = receivedUserId,
                    //        Amount = amount,
                    //        NewBalance = balanceSender.SpendingBalance + amount,
                    //        TransactionType = (int)TransactionEnum.TransactionType.IN,
                    //        TransactionOriginal = (int)TransactionEnum.TransactionOriginal.USER_SPENDING
                    //    }, dbConnection: _connection, dbTransaction: _transaction);
                    //}

                    // #2. receive ************************************************************************************************************************************
                    WalletUserMessageModel balanceReceived = WalletService.GetBalanceOfUser(receivedUserId, dbConnection: _connection, dbTransaction: _transaction);
                    if (!balanceReceived.Status)
                        return Notifization.Error("Không thể cập nhật giao dịch");
                    //
                    var changeBalanceForUser = WalletService.ChangeBalanceForUser(new WalletUserChangeModel { ClientID = senderId, UserID = receivedUserId, Amount = amount, TransactionType = (int)TransactionEnum.TransactionType.IN }, dbConnection: _connection, dbTransaction: _transaction);
                    if (!changeBalanceForUser.Status)
                        return Notifization.Error("Không thể cập nhật giao dịch");
                    // create histories for balance changed
                    var balanceUserHistoryStatus = LoggerHistoryService.LoggerWalletUserSpendingHistory(new WalletUserSpendingHistoryCreateModel
                    {
                        SenderID = senderId,
                        SenderUserID = sendUserId,
                        ReceivedID = received,
                        ReceivedUserID = receivedUserId,
                        Amount = amount,
                        NewBalance = balanceReceived.Balance + amount,
                        TransactionType = (int)TransactionEnum.TransactionType.IN,
                        TransactionOriginal = (int)TransactionEnum.TransactionOriginal.SPENDING
                    }, dbConnection: _connection, dbTransaction: _transaction);
                    //
                    if (!balanceUserHistoryStatus.Status)
                        return Notifization.Error("Không thể cập nhật giao dịch");
                    //end update balance ************************************************************************************************************************************
                    //commit
                    _transaction.Commit();
                    return Notifization.Success(MessageText.CreateSuccess);
                }
                catch (Exception ex)
                {
                    _transaction.Rollback();
                    return Notifization.TEST(">>:" + ex);
                }
            }
        }
        //##############################################################################################################################################################################################################################################################
        public TransactionUserSpendingResult GetTransactionUserSpendingModel(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_TransactionUserSpending WHERE ID = @Query";
                return _connection.Query<TransactionUserSpendingResult>(sqlQuery, new { Query = Id }).FirstOrDefault();
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
            //
            id = id.ToLower();
            using (var _connectDb = DbConnect.Connection.CMS)
            {
                _connectDb.Open();
                using (var _transaction = _connectDb.BeginTransaction())
                {
                    try
                    {
                        TransactionUserSpendingService transactionUserSpendingService = new TransactionUserSpendingService(_connectDb);
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
                string sqlQuery = @"SELECT * FROM App_TransactionUserSpending WHERE ID = @ID";
                var item = _connection.Query<TransactionUserSpendingResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
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
