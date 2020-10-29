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
    public interface IWalletUserHistoryService : IEntityService<WalletUserSpendingHistory> { }
    public class WalletUserHistoryService : EntityService<WalletUserSpendingHistory>, IWalletUserHistoryService
    {
        public WalletUserHistoryService() : base() { }
        public WalletUserHistoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
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
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_WalletUserHistory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' " + whereCondition + " ORDER BY[CreatedDate] DESC";
            var dtList = _connection.Query<WalletUserSpendingHistory>(sqlQuery, new { Query = query }).ToList();
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


        public TransactionHistoryMessageModel WalletUserSpendingHistoryCreate(WalletUserSpendingHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            if (model == null)
                return new TransactionHistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" };
            //
            string clientId = model.ClientID;
            string userId = model.UserID;
            double amount = model.Amount;
            double balance = model.NewBalance;
            int transType = model.TransactionType;
            //
            string transState = "x";
            if (transType == (int)TransactionEnum.TransactionType.IN)
                transState = "+";
            if (transType == (int)TransactionEnum.TransactionType.OUT)
                transState = "-";
            //
            string title = "TK thay đổi. GD " + transState + " " + Helper.Page.Library.FormatCurrency(amount) + " đ. Số dư: " + Helper.Page.Library.FormatCurrency(balance) + " đ.";
            string summary = "";
            WalletUserHistoryService balanceUserHistoryService = new WalletUserHistoryService(dbConnection);
            balanceUserHistoryService.Create<string>(new WalletUserSpendingHistory()
            {
                UserID =  userId,
                ClientID = clientId,
                Title = title,
                Summary = summary,
                Amount = amount,
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
