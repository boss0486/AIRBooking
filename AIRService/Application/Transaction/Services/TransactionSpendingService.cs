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

namespace WebCore.Services
{
    public interface ITransactionSpendingService : IEntityService<TransactionSpending> { }
    public class TransactionSpendingService : EntityService<TransactionSpending>, ITransactionSpendingService
    {
        public TransactionSpendingService() : base() { }
        public TransactionSpendingService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            //
            int page = model.Page;

            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_TransactionSpending WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                ORDER BY [CreatedDate]";
            //
            var dtList = _connection.Query<TransactionSpendingResult>(sqlQuery, new { Query = query }).ToList();
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
            Helper.Model.RoleDefaultModel roleDefault = new Helper.Model.RoleDefaultModel
            {
                Create = true,
                Update = true,
                Details = true,
                Delete = true
            };
            return Notifization.Data(MessageText.Success, data: result, role: roleDefault, paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################

        public ActionResult Create(TransactionSpendingCreateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string title = "Thay đổi hạn mức";
                    string summary = model.Summary;
                    string customerId = model.CustomerID;
                    double amount = model.Amount;
                    string languageId = Helper.Current.UserLogin.LanguageID;
                    //
                    string currUserId = Helper.Current.UserLogin.IdentifierID;

                    CustomerService customerService = new CustomerService(_connection);
                    ClientLoginService clientLoginService = new ClientLoginService(_connection);
                    UserLoginService userLoginService = new UserLoginService(_connection);
                    //
                    if (string.IsNullOrWhiteSpace(customerId))
                        return Notifization.Invalid("Vui lòng chọn khách hàng");
                    //
                    var customer = customerService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.ID.Equals(customerId), transaction: _transaction).FirstOrDefault();
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
                    TransactionSpendingService transactionSpendingService = new TransactionSpendingService(_connection);
                    var transactionSpendingId = transactionSpendingService.Create<string>(new TransactionSpending()
                    {
                        CustomerID = customerId,
                        Title = title,
                        Summary = summary,
                        UserIDSend = currUserId,
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
                    var loggerWalletCustomerHistoryStatus = WalletHistoryService.LoggerWalletCustomerHistory(new WalletCustomerDepositHistoryCreateModel
                    {
                        CustomerID = customerId,
                        Amount = amount,
                        NewBalance = balanceCustomer.SpendingBalance + amount,
                        TransactionType = (int)TransactionEnum.TransactionType.IN,
                        TransactionOriginalType = (int)WalletHistoryEnum.WalletHistoryTransactionOriginal.DIRECTLY,
                        TransactionOriginalID = transactionSpendingId
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

        public TransactionSpending GetTransactionSpendingModel(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_TransactionSpending WHERE ID = @Query";
                return _connection.Query<TransactionSpending>(sqlQuery, new { Query = Id }).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
        //########################################################################tttt######################################################################################################################################################################################
        public ActionResult Delete(string Id)
        {
            if (Id == null)
                return Notifization.NotFound();
            using (var _connectDb = DbConnect.Connection.CMS)
            {
                _connectDb.Open();
                using (var transaction = _connectDb.BeginTransaction())
                {
                    try
                    {
                        TransactionSpendingService TransactionSpendingService = new TransactionSpendingService(_connectDb);
                        var TransactionSpending = TransactionSpendingService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                        if (TransactionSpending == null)
                            return Notifization.NotFound();
                        TransactionSpendingService.Remove(TransactionSpending.ID, transaction: transaction);
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
                string sqlQuery = @"SELECT * FROM App_TransactionSpending WHERE ID = @ID";
                var item = _connection.Query<TransactionSpendingResult>(sqlQuery, new { ID = Id }).FirstOrDefault();
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


        //##############################################################################################################################################################################################################################################################
    }
}
