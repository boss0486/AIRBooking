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
    public interface ITransactionDepositService : IEntityService<TransactionDeposit> { }
    public class TransactionDepositService : EntityService<TransactionDeposit>, ITransactionDepositService
    {
        public TransactionDepositService() : base() { }
        public TransactionDepositService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            //
            int page = model.Page;
            //
            string whereCondition = string.Empty;
            int timeExpress = model.TimeExpress;
            string startDate = model.StartDate;
            string endDate = model.EndDate;
            string timeZoneLocal = model.TimeZoneLocal;
            //
            string clientTime = Helper.Time.TimeHelper.GetDateByTimeZone(timeZoneLocal);
            //
            if (timeExpress != 0 && !string.IsNullOrWhiteSpace(clientTime))
            {
                // client time
                DateTime today = Convert.ToDateTime(clientTime);
                if (timeExpress == 1)
                {
                    string strDate = Helper.Time.TimeHelper.FormatToDateSQL(today);
                    DateTime dtime = Convert.ToDateTime(strDate);
                    whereCondition = " AND cast(CreatedDate as Date) = cast('" + dtime + "' as Date)";
                }
                // Yesterday
                if (timeExpress == 2)
                {
                    DateTime dtime = today.AddDays(-1);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date) AND cast(CreatedDate as Date) <= cast('" + today + "' as Date)";
                }
                // ThreeDayAgo
                if (timeExpress == 3)
                {
                    DateTime dtime = today.AddDays(-3);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
                // SevenDayAgo
                if (timeExpress == 4)
                {
                    DateTime dtime = today.AddDays(-7);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
                // OneMonthAgo
                if (timeExpress == 5)
                {
                    DateTime dtime = today.AddMonths(-1);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }

                // ThreeMonthAgo
                if (timeExpress == 6)
                {
                    DateTime dtime = today.AddMonths(-3);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
                // SixMonthAgo
                if (timeExpress == 7)
                {
                    DateTime dtime = today.AddMonths(-6);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
                // OneYearAgo
                if (timeExpress == 8)
                {
                    DateTime dtime = today.AddYears(-1);
                    whereCondition = " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    DateTime dtime = Convert.ToDateTime(startDate);
                    whereCondition += " AND cast(CreatedDate as Date) >= cast('" + dtime + "' as Date)";
                }
                //
                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (Convert.ToDateTime(endDate) < Convert.ToDateTime(startDate))
                        return Notifization.NotFound("Thời gian kết thúc không hợp lệ");
                    //
                    DateTime dtime = Convert.ToDateTime(endDate);
                    whereCondition += " AND cast(CreatedDate as Date) <= cast('" + dtime + "' as Date)";
                }
            }
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_TransactionDeposit WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' " + whereCondition + " ORDER BY [CreatedDate]";
            //
            var dtList = _connection.Query<TransactionDepositResult>(sqlQuery, new { Query = query }).ToList();
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
        public ActionResult Create(TransactionDepositCreateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string customerId = model.CustomerID;
                    string title = "Giao dịch nạp tiền";
                    string summary = model.Summary;
                    string transactionId = model.TransactionID;
                    string bankSent = model.BankSent;
                    string bankIDSent = model.BankIDSent;
                    string bankReceived = model.BankReceived;
                    string bankIDReceived = model.BankIDReceived;
                    string receivedDate = model.ReceivedDate;
                    double amount = model.Amount;
                    int enabled = model.Enabled;
                    string languageId = Helper.Current.UserLogin.LanguageID;
                    //
                    if (string.IsNullOrWhiteSpace(customerId))
                        return Notifization.Invalid("Vui lòng chọn khách hàng");
                    customerId = customerId.Trim();
                    // transaction 
                    if (string.IsNullOrWhiteSpace(transactionId))
                        return Notifization.Invalid("Không được để trống mã giao dịch");
                    transactionId = transactionId.Trim();

                    if (!Validate.TestText(transactionId))
                        return Notifization.Invalid("Mã giao dịch không hợp lệ");
                    // bank name send
                    if (string.IsNullOrWhiteSpace(bankSent))
                        return Notifization.Invalid("Vui lòng chọn ngân hàng chuyển");
                    bankSent = bankSent.Trim();

                    if (string.IsNullOrWhiteSpace(bankIDSent))
                        return Notifization.Invalid("Không được để trống số TK Ng.Hàng chuyển");
                    bankIDSent = bankIDSent.Trim();

                    if (!Validate.TestText(bankIDSent))
                        return Notifization.Invalid("Số TK Ng.Hàng chuyển không hợp lệ");
                    // bank name receive
                    if (string.IsNullOrWhiteSpace(bankReceived))
                        return Notifization.Invalid("Vui lòng chọn ngân hàng nhận");
                    bankReceived = bankReceived.Trim();

                    if (string.IsNullOrWhiteSpace(bankIDReceived))
                        return Notifization.Invalid("Không được để trống số TK Ng.Hàng nhận");
                    bankIDReceived = bankIDReceived.Trim();

                    if (!Validate.TestText(bankIDReceived))
                        return Notifization.Invalid("Số TK Ng.Hàng nhận không hợp lệ");
                    // amount
                    if (amount <= 0)
                        return Notifization.Invalid("Số tiền nạp phải > 0");
                    // 

                    if (string.IsNullOrWhiteSpace(receivedDate))
                        return Notifization.Invalid("Không được để trống ngày nhận tiền");
                    receivedDate = receivedDate.Trim();

                    if (!Validate.TestDateVN(receivedDate))
                        return Notifization.Invalid("Ngày nhận tiền không hợp lệ");
                    //
                    title = "Nạp tiền vào tài khoản";
                    // summary valid               
                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }

                    // check bank
                    string bankNameSent = BankService.GetBankNameByID(bankSent);
                    if (string.IsNullOrWhiteSpace(bankNameSent))
                        return Notifization.NotFound("Lỗi không xác định được ngân hàng gửi");

                    string bankNameReceived = BankService.GetBankNameByID(bankReceived);
                    if (string.IsNullOrWhiteSpace(bankNameSent))
                        return Notifization.NotFound("Lỗi không xác định được ngân hàng nhận");
                    //
                    //
                    TransactionDepositService transactionDeposit = new TransactionDepositService(_connection);
                    //var history = historyService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower().Equals(title.ToLower()), transaction: transaction).FirstOrDefault();
                    //if (history != null)
                    //    return Notifization.Invalid("Tên giao dịch đã được sử dụng");
                    //
                    var transactionDepositId = transactionDeposit.Create<string>(new TransactionDeposit()
                    {
                        CustomerID = customerId,
                        Title = title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(title),
                        Summary = summary,
                        TransactionID = transactionId,
                        BankSent = bankNameSent,
                        BankIDSent = bankIDSent,
                        BankReceived = bankNameReceived,
                        BankIDReceived = bankIDReceived,
                        ReceivedDate = Helper.Time.TimeHelper.FormatToDateSQL(receivedDate),
                        Amount = amount,
                        Status = (int)TransactionEnum.TransactionType.IN,
                        LanguageID = languageId,
                        Enabled = enabled,
                    }, transaction: _transaction);
                    // update 

                    // update balance ************************************************************************************************************************************
                    WalletCustomerMessageModel balanceCustomer = WalletService.GetBalanceByCustomerID(customerId, dbConnection: _connection, dbTransaction: _transaction);
                    if (!balanceCustomer.Status)
                        return Notifization.Error("Không thể cập nhật giao dịch");
                    // update deposit wallet 
                    var changeBalanceDepositForCustomerStatus = WalletService.ChangeBalanceDepositForCustomer(new WalletCustomerChangeModel { CustomerID = customerId, Amount = amount, TransactionType = (int)TransactionEnum.TransactionType.IN }, dbConnection: _connection, dbTransaction: _transaction);
                    if (!changeBalanceDepositForCustomerStatus.Status)
                        return Notifization.Error("Không thể cập nhật giao dịch");
                    // create histories for balance changed
                    var loggerTransactionDepositHistoryStatus = TransactionHistoryService.LoggerTransactionDepositHistory(new TransactionDepositHistoryCreateModel
                    {
                        CustomerID = customerId,
                        Amount = amount,
                        NewBalance = balanceCustomer.DepositBalance + amount,
                        TransactionType = (int)TransactionEnum.TransactionType.IN,
                        TransactionOriginal = (int)WalletHistoryEnum.WalletHistoryTransactionOriginal.DEPOSIT
                    }, dbConnection: _connection, dbTransaction: _transaction);
                    //
                    if (!loggerTransactionDepositHistoryStatus.Status)
                        return Notifization.Error("Không thể cập nhật giao dịch");

                    // update sepending wallet 
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
                        TransactionOriginalType = (int)WalletHistoryEnum.WalletHistoryTransactionOriginal.DEPOSIT
                    }, dbConnection: _connection, dbTransaction: _transaction);
                    //
                    if (!loggerWalletCustomerHistoryStatus.Status)
                        return Notifization.Error("Không thể cập nhật giao dịch");
                    // end update balance ************************************************************************************************************************************
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
        public ActionResult Update(TransactionDepositUpdateModel model)
        {
            _connection.Open();
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string customerId = model.CustomerID;
                    string title = model.Title;
                    string summary = model.Summary;
                    string transactionId = model.TransactionID;
                    string bankSent = model.BankSent;
                    string bankIDSent = model.BankIDSent;
                    string bankReceived = model.BankReceived;
                    string bankIDReceived = model.BankIDReceived;
                    string receivedDate = model.ReceivedDate;
                    double amount = model.Amount;
                    int enabled = model.Enabled;
                    string languageId = Helper.Current.UserLogin.LanguageID;
                    //
                    if (string.IsNullOrWhiteSpace(customerId))
                        return Notifization.Invalid("Vui lòng chọn khách hàng");
                    customerId = customerId.Trim();
                    // transaction 
                    if (string.IsNullOrWhiteSpace(transactionId))
                        return Notifization.Invalid("Không được để trống mã giao dịch");
                    transactionId = transactionId.Trim();

                    if (!Validate.TestText(transactionId))
                        return Notifization.Invalid("Mã giao dịch không hợp lệ");
                    // bank name send
                    if (string.IsNullOrWhiteSpace(bankSent))
                        return Notifization.Invalid("Vui lòng chọn ngân hàng chuyển");
                    bankSent = bankSent.Trim();

                    if (string.IsNullOrWhiteSpace(bankIDSent))
                        return Notifization.Invalid("Không được để trống số TK Ng.Hàng chuyển");
                    bankIDSent = bankIDSent.Trim();

                    if (!Validate.TestText(bankIDSent))
                        return Notifization.Invalid("Số T.Khoản Ng.Hàng chuyển không hợp lệ");
                    // bank name receive
                    if (string.IsNullOrWhiteSpace(bankReceived))
                        return Notifization.Invalid("Vui lòng chọn ngân hàng nhận");
                    bankReceived = bankReceived.Trim();

                    if (string.IsNullOrWhiteSpace(bankIDReceived))
                        return Notifization.Invalid("Không được để trống số TK Ng.Hàng nhận");
                    bankIDReceived = bankIDReceived.Trim();

                    if (!Validate.TestText(bankIDReceived))
                        return Notifization.Invalid("Số TK Ng.Hàng nhận không hợp lệ");
                    // amount
                    if (amount <= 0)
                        return Notifization.Invalid("Số tiền nạp phải > 0");
                    // 

                    if (string.IsNullOrWhiteSpace(receivedDate))
                        return Notifization.Invalid("Không được để trống ngày nhận tiền");
                    receivedDate = receivedDate.Trim();

                    if (!Validate.TestDateVN(receivedDate))
                        return Notifization.Invalid("Ngày nhận tiền không hợp lệ");
                    //
                    title = "Nạp tiền vào tài khoản";
                    // summary valid               
                    if (!string.IsNullOrWhiteSpace(summary))
                    {
                        summary = summary.Trim();
                        if (!Validate.TestText(summary))
                            return Notifization.Invalid("Mô tả không hợp lệ");
                        if (summary.Length < 1 || summary.Length > 120)
                            return Notifization.Invalid("Mô tả giới hạn từ 1-> 120 ký tự");
                    }
                    TransactionDepositService transactionDepositService = new TransactionDepositService(_connection);
                    string Id = model.ID.ToLower();
                    var transactionDeposit = transactionDepositService.GetAlls(m => m.ID.Equals(Id), transaction: _transaction).FirstOrDefault();
                    if (transactionDeposit == null)
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    //var dpm = historyService.GetAlls(m => !string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower().Equals(title.ToLower()) && !m.ID.ToLower().Equals(Id), transaction: transaction).FirstOrDefault();
                    //if (dpm != null)
                    //    return Notifization.Invalid("Tiêu đề đã được sử dụng");
                    // update user information

                    string bankNameSent = BankService.GetBankNameByID(bankSent);
                    if (string.IsNullOrWhiteSpace(bankNameSent))
                        return Notifization.NotFound("Lỗi không xác định được ngân hàng gửi");

                    string bankNameReceived = BankService.GetBankNameByID(bankReceived);
                    if (string.IsNullOrWhiteSpace(bankNameSent))
                        return Notifization.NotFound("Lỗi không xác định được ngân hàng nhận");
                    //


                    transactionDeposit.CustomerID = customerId;
                    transactionDeposit.Alias = Helper.Page.Library.FormatToUni2NONE(title);
                    transactionDeposit.Summary = summary;
                    transactionDeposit.TransactionID = transactionId;
                    transactionDeposit.BankSent = bankNameSent;
                    transactionDeposit.BankIDSent = bankIDSent;
                    transactionDeposit.BankReceived = bankNameReceived;
                    transactionDeposit.BankIDReceived = bankIDReceived;
                    transactionDeposit.ReceivedDate = Helper.Time.TimeHelper.FormatToDateSQL(receivedDate);
                    transactionDeposit.Status = (int)TransactionEnum.TransactionType.IN;
                    transactionDeposit.Amount = amount;
                    transactionDeposit.Enabled = enabled;
                    transactionDepositService.Update(transactionDeposit, transaction: _transaction);
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
        public TransactionDeposit TransactionDepositModel(string Id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Id))
                    return null;
                string query = string.Empty;
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM View_App_TransactionDeposit WHERE ID = @Query";
                return _connection.Query<TransactionDeposit>(sqlQuery, new { Query = Id }).FirstOrDefault();
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
                        TransactionDepositService TransactionDepositService = new TransactionDepositService(_connectDb);
                        var TransactionDeposit = TransactionDepositService.GetAlls(m => m.ID.Equals(Id.ToLower()), transaction: transaction).FirstOrDefault();
                        if (TransactionDeposit == null)
                            return Notifization.NotFound();
                        TransactionDepositService.Remove(TransactionDeposit.ID, transaction: transaction);
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
        public ActionResult Details(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                var item = TransactionDepositModel(id);
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
        public static string DropdownList(string id)
        {
            try
            {
                string result = string.Empty;
                using (var TransactionDepositService = new TransactionDepositService())
                {
                    var dtList = TransactionDepositService.DataOption(null);
                    if (dtList.Count > 0)
                    {
                        foreach (var item in dtList)
                        {
                            string select = string.Empty;
                            if (item.ID.Equals(id.ToLower()))
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
        public List<TransactionDepositOption> DataOption(string languageId)
        {
            try
            {
                string sqlQuery = @"SELECT * FROM View_App_TransactionDeposit WHERE Enabled = 1 ORDER BY Title ASC";
                return _connection.Query<TransactionDepositOption>(sqlQuery, new { LangID = languageId }).ToList();
            }
            catch
            {
                return new List<TransactionDepositOption>();
            }
        }
        //##############################################################################################################################################################################################################################################################  
    }
}
