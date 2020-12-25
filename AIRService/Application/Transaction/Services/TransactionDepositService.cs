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
using Helper.TimeData;

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
            string userId = Helper.Current.UserLogin.IdentifierID;
            if (Helper.Current.UserLogin.IsCMSUser || Helper.Current.UserLogin.IsAdminInApplication)
            {
                // show all
            }
            else if (Helper.Current.UserLogin.IsClientInApplication())
            {
                string clientId = ClientLoginService.GetAgentIDByUserID(userId);
                whereCondition += " AND (AgentSentID = '" + clientId + "' OR AgentReceivedID = '" + clientId + "')";
            }
            else
            {
                return Notifization.AccessDenied(MessageText.AccessDenied);
            }
            // 
            string areaId = model.AreaID;
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_TransactionDeposit WHERE (dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' OR TransactionCode LIKE N'%'+ @Query +'%') " + whereCondition + " ORDER BY [CreatedDate]";
            //
            var dtList = _connection.Query<TransactionDepositResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query) }).ToList();
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

        //##############################################################################################################################################################################################################################################################
        public ActionResult Deposit(TransactionDepositCreateModel model)
        {
            _connection.Open();
            string userId = Helper.Current.UserLogin.IdentifierID;
            string languageId = Helper.Current.UserLogin.LanguageID;
            using (var _transaction = _connection.BeginTransaction())
            {
                try
                {
                    if (model == null)
                        return Notifization.Invalid();
                    //
                    string agentSentId = string.Empty;
                    string agentReceivedId = model.AgentReceivedID;
                    //
                    string title = "Giao dịch nạp tiền";
                    string summary = model.Summary;
                    string transactionCode = model.TransactionCode;
                    string bankSentId = model.BankSentID;
                    string bankSentNumber = model.BankSentNumber;
                    string bankReceivedId = model.BankReceivedID;
                    string bankReceivedNumber = model.BankReceivedNumber;
                    string receivedDate = model.ReceivedDate;
                    double amount = model.Amount;
                    int enabled = model.Enabled;
                    //
                    if (string.IsNullOrWhiteSpace(agentReceivedId))
                        return Notifization.Invalid("Vui lòng chọn đại lý");
                    //
                    agentReceivedId = agentReceivedId.Trim();
                    // transaction 
                    if (string.IsNullOrWhiteSpace(transactionCode))
                        return Notifization.Invalid("Không được để trống mã giao dịch");
                    transactionCode = transactionCode.Trim();

                    if (!Validate.TestText(transactionCode))
                        return Notifization.Invalid("Mã giao dịch không hợp lệ");
                    // bank name send
                    if (string.IsNullOrWhiteSpace(bankSentId))
                        return Notifization.Invalid("Vui lòng chọn ngân hàng chuyển");
                    bankSentId = bankSentId.Trim();

                    if (string.IsNullOrWhiteSpace(bankSentNumber))
                        return Notifization.Invalid("Không được để trống số TK Ng.Hàng chuyển");
                    bankSentNumber = bankSentNumber.Trim();

                    if (!Validate.TestText(bankSentNumber))
                        return Notifization.Invalid("Số TK Ng.Hàng chuyển không hợp lệ");
                    // bank name receive
                    if (string.IsNullOrWhiteSpace(bankReceivedId))
                        return Notifization.Invalid("Vui lòng chọn ngân hàng nhận");
                    bankReceivedId = bankReceivedId.Trim();

                    if (string.IsNullOrWhiteSpace(bankReceivedNumber))
                        return Notifization.Invalid("Không được để trống số TK Ng.Hàng nhận");
                    bankReceivedNumber = bankReceivedNumber.Trim();

                    if (!Validate.TestText(bankReceivedNumber))
                        return Notifization.Invalid("Số TK Ng.Hàng nhận không hợp lệ");
                    // amount
                    if (amount <= 0)
                        return Notifization.Invalid("Số tiền nạp phải > 0");
                    // 

                    if (string.IsNullOrWhiteSpace(receivedDate))
                        return Notifization.Invalid("Không được để trống ngày nhận tiền");
                    receivedDate = receivedDate.Trim();

                    if (!Validate.TestDate(receivedDate))
                        return Notifization.Invalid("Ngày nhận tiền không hợp lệ");
                    //
                    title = "Nạp tiền";
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
                    AirAgentService airAgentService = new AirAgentService(_connection);
                    AirAgent agentReceived = airAgentService.GetAlls(m => m.ID == agentReceivedId, transaction: _transaction).FirstOrDefault();
                    if (agentReceived == null)
                        return Notifization.Invalid("Đại lý không xác định");
                    // 
                    UserService userService = new UserService(_connection);
                    Logged looged = userService.LoggedModel(transaction: _transaction);
                    if (looged.IsCMSUser || looged.IsAdministrator)
                    {
                        agentSentId = agentReceived.ParentID;
                    }
                    else if (userService.IsClientInApplication(userId, transaction: _transaction))
                    {
                        ClientLoginService clientLoginService = new ClientLoginService(_connection);
                        ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == userId, transaction: _transaction).FirstOrDefault();
                        agentSentId = clientLogin.ClientID;
                        //
                        string providerId = agentReceived.ParentID;
                        if (providerId != clientLogin.ClientID)
                            return Notifization.Invalid(MessageText.Forbidden);
                        //
                    }
                    else
                    {
                        return Notifization.Invalid(MessageText.Forbidden);
                    }

                    //
                    if (string.IsNullOrWhiteSpace(agentSentId))
                        return Notifization.NotFound(MessageText.NotFound);
                    //
                    AirAgent agentSent = airAgentService.GetAlls(m => m.ID == agentSentId, transaction: _transaction).FirstOrDefault();
                    if (agentSent == null)
                        return Notifization.Invalid("Đại lý cung cấp không hợp lệ");
                    //

                    // check bank
                    BankService bankService = new BankService(_connection);

                    Bank bankSend = bankService.GetAlls(m => m.ID == bankSentId, transaction: _transaction).FirstOrDefault();
                    if (bankSend == null)
                        return Notifization.Invalid("Ngân hàng gửi không hợp lệ");
                    //
                    Bank bankReceive = bankService.GetAlls(m => m.ID == bankReceivedId, transaction: _transaction).FirstOrDefault();
                    if (bankReceive == null)
                        return Notifization.Invalid("Ngân hàng nhận không hợp lệ");
                    // 
                    TransactionDepositService transactionDepositService = new TransactionDepositService(_connection);
                    transactionCode = transactionCode.ToLower().Trim();
                    TransactionDeposit transactionDeposit = transactionDepositService.GetAlls(m => m.TransactionCode == transactionCode, transaction: _transaction).FirstOrDefault();
                    if (transactionDeposit != null)
                        return Notifization.Invalid("Mã giao dịch đã được sử dụng");
                    //
                    var transactionDepositId = transactionDepositService.Create<string>(new TransactionDeposit()
                    {
                        Title = title,
                        Alias = Helper.Page.Library.FormatToUni2NONE(title),
                        Summary = summary,
                        UserSentID = userId,
                        AgentSentID = agentSentId,
                        AgentSentCode = agentSent.CodeID,
                        AgentReceivedID = agentReceivedId,
                        AgentReceivedCode = agentReceived.CodeID,
                        BankSentID = bankSentId,
                        BankSentCode = bankSend.CodeID,
                        BankSentNumber = bankSentNumber,
                        BankReceivedID = bankReceivedId,
                        BankReceivedCode = bankReceive.CodeID,
                        BankReceivedNumber = bankReceivedNumber,
                        ReceivedDate = TimeFormat.FormatToServerDate(receivedDate),
                        Amount = amount,
                        TransactionCode = transactionCode.ToLower(),
                        TransactionStatus = (int)TransactionEnum.TransactionType.IN,
                        Enabled = enabled,
                    }, transaction: _transaction);

                    TransactionHistoryMessageModel loggerWalletDepositHistory1 = LoggerHistoryService.LoggerWalletDepositHistory(new WalletDepositHistoryCreateModel
                    {
                        AgentID = agentSentId,
                        Amount = amount,
                        NewBalance = +amount,
                        TransactionType = (int)TransactionEnum.TransactionType.OUT
                    }, dbConnection: _connection, dbTransaction: _transaction);
                    //
                    if (!loggerWalletDepositHistory1.Status)
                        return Notifization.Invalid(MessageText.Invalid);
                    //
                    TransactionHistoryMessageModel loggerWalletDepositHistory2 = LoggerHistoryService.LoggerWalletDepositHistory(new WalletDepositHistoryCreateModel
                    {
                        AgentID = agentReceivedId,
                        Amount = amount,
                        NewBalance = +amount,
                        TransactionType = (int)TransactionEnum.TransactionType.IN
                    }, dbConnection: _connection, dbTransaction: _transaction);
                    //
                    if (!loggerWalletDepositHistory1.Status)
                        return Notifization.Invalid(MessageText.Invalid);

                    //commit ************************************************************************************************************************************
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
            return Notifization.NotService;
        }
        public TransactionDepositResult ViewTransactionDeposit(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return null;
                string query = string.Empty;
                //
                string langID = Helper.Current.UserLogin.LanguageID;
                string sqlQuery = @"SELECT TOP (1) * FROM App_TransactionDeposit WHERE ID = @Query";
                return _connection.Query<TransactionDepositResult>(sqlQuery, new { Query = id }).FirstOrDefault();
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
                return Notifization.Invalid();
            //
            id = id.ToLower();
            using (var _connectDb = DbConnect.Connection.CMS)
            {
                _connectDb.Open();
                using (var _transaction = _connectDb.BeginTransaction())
                {
                    try
                    {
                        TransactionDepositService transactionDepositService = new TransactionDepositService(_connectDb);
                        var transactionDeposit = transactionDepositService.GetAlls(m => m.ID == id, transaction: _transaction).FirstOrDefault();
                        if (transactionDeposit == null)
                            return Notifization.NotFound();
                        //
                        transactionDepositService.Remove(transactionDeposit.ID, transaction: _transaction);
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
        public ActionResult Details(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Notifization.NotFound(MessageText.Invalid);
                string langID = Helper.Current.UserLogin.LanguageID;
                var item = ViewTransactionDeposit(id);
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
                            if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
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
                string sqlQuery = @"SELECT * FROM App_TransactionDeposit WHERE Enabled = 1 ORDER BY Title ASC";
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
