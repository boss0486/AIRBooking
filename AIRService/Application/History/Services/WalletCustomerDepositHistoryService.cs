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
    public interface IWalletCustomerDepositHistoryService : IEntityService<WalletCustomerDepositHistory> { }
    public class WalletCustomerDepositHistoryService : EntityService<WalletCustomerDepositHistory>, IWalletCustomerDepositHistoryService
    {
        public WalletCustomerDepositHistoryService() : base() { }
        public WalletCustomerDepositHistoryService(System.Data.IDbConnection db) : base(db) { }
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
            var dtList = _connection.Query<WalletCustomerDepositHistoryResult>(sqlQuery, new { Query = Helper.Page.Library.FormatToUni2NONE(query) }).ToList();
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
        public WalletHistoryMessageModel WalletCustomerDepositHistoryCreate(WalletCustomerDepositHistoryCreateModel model, IDbConnection dbConnection, IDbTransaction dbTransaction = null)
        {
            if (model == null)
                return new WalletHistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" };
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
            string title = "TK hạn mức thay đổi. GD " + transState + " " + Helper.Page.Library.FormatCurrency(amount) + " đ. Số dư: " + Helper.Page.Library.FormatCurrency(balance) + " đ.";
            string summary = "";
            WalletCustomerDepositHistoryService BalanceCustomerHistoryService = new WalletCustomerDepositHistoryService(dbConnection);
            var id = BalanceCustomerHistoryService.Create<string>(new WalletCustomerDepositHistory()
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
            return new WalletHistoryMessageModel { Status = true, Message = "Ok" };
        }
        //##############################################################################################################################################################################################################################################################
    }
}
