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
    public interface IWalletDepositHistoryService : IEntityService<WalletDepositHistory> { }
    public class WalletDepositHistoryService : EntityService<WalletDepositHistory>, IWalletDepositHistoryService
    {
        public WalletDepositHistoryService() : base() { }
        public WalletDepositHistoryService(System.Data.IDbConnection db) : base(db) { }
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
                    return Notifization.Invalid(MessageText.Invalid);
            }
            #endregion

            string userId = Helper.Current.UserLogin.IdentifierID;
            if (Helper.Current.UserLogin.IsCMSUser || Helper.Current.UserLogin.IsAdminInApplication)
            {
                // show all
            }
            else if (Helper.Current.UserLogin.IsAdminCustomerLogged() || Helper.Current.UserLogin.IsCustomerLogged())
            {
                string customerId = CustomerService.GetCustomerIDByUserID(userId);
                whereCondition += " AND ReceivedID = '" + customerId + "' AND TransactionType = " + (int)TransactionEnum.TransactionType.IN;
            }
            else if (Helper.Current.UserLogin.IsAdminSupplierLogged() || Helper.Current.UserLogin.IsSupplierLogged())
            {
                if (Helper.Current.UserLogin.IsAdminSupplierLogged())
                {
                    string supplierCode = ClientLoginService.GetClientIDByUserID(userId);
                    whereCondition += " AND SenderID = '" + supplierCode + "' AND TransactionType = " + (int)TransactionEnum.TransactionType.OUT;
                }
                else
                    whereCondition += " AND SenderUserID = '" + userId + "' AND TransactionType = " + (int)TransactionEnum.TransactionType.OUT;
            }
            else
            {
                return Notifization.AccessDenied(MessageText.AccessDenied);
            }
            // 
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_TransactionDepositHistory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' " + whereCondition + " ORDER BY [CreatedDate] DESC";
            var dtList = _connection.Query<WalletDepositHistoryResult>(sqlQuery, new { Query = query, SenderID = userId }).ToList();
            //
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound + sqlQuery);
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
            //;
            return Notifization.Data(MessageText.Success + whereCondition, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }
        //##############################################################################################################################################################################################################################################################

        public TransactionHistoryMessageModel WalletDepositHistoryCreate(WalletDepositHistoryCreateModel model, IDbConnection dbConnection, IDbTransaction dbTransaction = null)
        {
            if (model == null)
                return new TransactionHistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" };
            //
            string senderUserId = model.SenderUserID;
            string senderId = model.SenderID;
            string receivedId = model.ReceivedID;
            double amount = model.Amount;
            double balance = model.NewBalance;
            int transType = model.TransactionType;
            int transOriginal = model.TransactionOriginal;
            string languageId = Helper.Current.UserLogin.LanguageID;
            //
            string transState = "x";
            if (transType == (int)WalletHistoryEnum.WalletHistoryTransactionType.INPUT)
                transState = "+";
            if (transType == (int)WalletHistoryEnum.WalletHistoryTransactionType.OUTPUT)
                transState = "-";
            //
            string title = "Nạp tiền. GD " + transState + " " + Helper.Page.Library.FormatCurrency(amount) + " đ. Số dư: " + Helper.Page.Library.FormatCurrency(balance) + " đ.";
            string summary = "";
            WalletDepositHistoryService BalanceCustomerHistoryService = new WalletDepositHistoryService(dbConnection);
            var id = BalanceCustomerHistoryService.Create<string>(new WalletDepositHistory()
            {
                SenderUserID = senderUserId,
                SenderID = senderId,
                ReceivedID = receivedId,
                Title = title,
                Summary = summary,
                Amount = amount,
                TransactionType = transType,
                TransactionOriginal = transOriginal,
                Status = 1,
                Enabled = (int)WebCore.Model.Enum.ModelEnum.Enabled.ENABLED
            }, transaction: dbTransaction);
            //commit
            return new TransactionHistoryMessageModel { Status = true, Message = "Ok" };
        }
        //##############################################################################################################################################################################################################################################################
    }
}
