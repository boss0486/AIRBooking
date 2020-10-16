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
    public interface IWalletCustomerDepositHistoryService : IEntityService<TrasactionCustomerDepositHistory> { }
    public class TransactionCustomerDepositHistoryService : EntityService<TrasactionCustomerDepositHistory>, IWalletCustomerDepositHistoryService
    {
        public TransactionCustomerDepositHistoryService() : base() { }
        public TransactionCustomerDepositHistoryService(System.Data.IDbConnection db) : base(db) { }
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
            string sqlQuery = @"SELECT * FROM App_WalletCustomerDepositHistory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY[CreatedDate] DESC";
            var dtList = _connection.Query<TransactionCustomerDepositHistoryResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query) }).ToList();
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
        public TransactionHistoryMessageModel TransactionCustomerDepositHistoryCreate(TransactionCustomerDepositHistoryCreateModel model, IDbConnection dbConnection, IDbTransaction dbTransaction = null)
        {
            if (model == null)
                return new TransactionHistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" };
            //
            string customerId = model.CustomerID;
            double amount = model.Amount;
            double balance = model.NewBalance;
            int transType = model.TransactionType;
            int transOriginalType = model.TransactionOriginalType;
            string transOriginalId = model.TransactionOriginalID;
            string languageId = Helper.Current.UserLogin.LanguageID;
            //
            string transState = "x";
            if (transType == (int)WalletHistoryEnum.WalletHistoryTransactionType.INPUT)
                transState = "+";
            if (transType == (int)WalletHistoryEnum.WalletHistoryTransactionType.OUTPUT)
                transState = "-";
            //
            string title = "Nạp tiền. GD " + transState + " " + Helper.Page.Library.FormatCurrency(amount) + " đ";
            string summary = "";
            TransactionCustomerDepositHistoryService BalanceCustomerHistoryService = new TransactionCustomerDepositHistoryService(dbConnection);
            var id = BalanceCustomerHistoryService.Create<string>(new TrasactionCustomerDepositHistory()
            {
                CustomerID = customerId,
                Title = title,
                Summary = summary,
                Amount = amount,
                TransactionType = transType,
                TransactionOriginalType = transOriginalType,
                TransactionOriginalID = transOriginalId,
                Status = 1,
                Enabled = (int)WebCore.Model.Enum.ModelEnum.Enabled.ENABLED
            }, transaction: dbTransaction);
            //commit
            return new TransactionHistoryMessageModel { Status = true, Message = "Ok" };
        }


        public static string TransactionTypeText(int transactionType)
        {
            if (transactionType == (int)WalletHistoryEnum.WalletHistoryTransactionType.INPUT)
                return "Nhận";
            else
            if (transactionType == (int)WalletHistoryEnum.WalletHistoryTransactionType.OUTPUT)
                return "Chuyển";
            else
                return "Không xác định";
        }
        public static string TransactionOriginalText(int transactionType)
        {
            if (transactionType == (int)WalletHistoryEnum.WalletHistoryTransactionOriginal.DEPOSIT)
                return "G.Dịch nạp tiền";
            else
            if (transactionType == (int)WalletHistoryEnum.WalletHistoryTransactionOriginal.DEPOSIT)
                return "Cấp trực tiếp";
            else
                return "Không xác định";
        }
        //##############################################################################################################################################################################################################################################################
    }
}
