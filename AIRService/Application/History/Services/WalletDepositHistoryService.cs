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
            else if (Helper.Current.UserLogin.IsCustomerLogged() || Helper.Current.UserLogin.IsSupplierLogged())
            {
                string agentId = ClientLoginService.GetAgentIDByUserID(userId);
                whereCondition += " AND AgentID = '" + agentId + "'";
            }
            else
            {
                return Notifization.AccessDenied(MessageText.AccessDenied);
            }
            // 
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_WalletDepositHistory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ @Query +'%' " + whereCondition + " ORDER BY [CreatedDate] DESC";

            var dtList = _connection.Query<WalletDepositHistoryResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query), SenderID = userId }).ToList();
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
            string agentId = model.AgentID;
            double amount = model.Amount;
            double balance = model.NewBalance;
            string transId = model.TransactionID;
            int transType = model.TransactionType;
            string languageId = Helper.Current.UserLogin.LanguageID;
            //
            string title = string.Empty;
            string transState = "";
            if (transType == (int)TransactionEnum.TransactionType.IN)
            {
                transState = "+";
                title = "Nạp tiền vào tài khoản.";
            }
            if (transType == (int)TransactionEnum.TransactionType.OUT)
            {
                transState = "-";
                title = "Nạp tiền cho đại lý.";
            }
            //
            title += "GD: " + transState + " " + Helper.Page.Library.FormatCurrency(amount) + " đ.";
            string summary = "";
            WalletDepositHistoryService walletDepositHistoryService = new WalletDepositHistoryService(dbConnection);
            var id = walletDepositHistoryService.Create<string>(new WalletDepositHistory()
            {
                AgentID = agentId,
                Title = title,
                Summary = summary,
                Amount = amount,
                TransactionID = transId,
                TransactionType = transType,
                Status = 1,
                Enabled = (int)WebCore.Model.Enum.ModelEnum.Enabled.ENABLED
            }, transaction: dbTransaction);
            //commit
            return new TransactionHistoryMessageModel { Status = true, Message = "Ok" };
        }
        //##############################################################################################################################################################################################################################################################
    }
}
