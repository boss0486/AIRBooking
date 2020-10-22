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
    public interface ITransactionPaymentHistoryService : IEntityService<TransactionPaymentHistory> { }
    public class TransactionPaymentHistoryService : EntityService<TransactionPaymentHistory>, ITransactionPaymentHistoryService
    {
        public TransactionPaymentHistoryService() : base() { }
        public TransactionPaymentHistoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            //
            int page = model.Page;

            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_TransactionDepositHistory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%'                                          
                                ORDER BY [CreatedDate] DESC";
            var dtList = _connection.Query<TransactionDepositResult>(sqlQuery, new { Query = query }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            //
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
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
                //Create = true,
                //Update = true,
                //Details = true,
                //Delete = true
            };
            return Notifization.Data(MessageText.Success, data: result, role: roleDefault, paging: pagingModel);
        }

        //##############################################################################################################################################################################################################################################################

        public TransactionHistoryMessageModel TransactionPaymentHistoryCreate(TransactionPaymentHistoryCreateModel model,IDbConnection dbConnection, IDbTransaction dbTransaction = null)
        {
            if (model == null)
                return new TransactionHistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" };
            //
            string customerId = model.CustomerID;
            double amount = model.Amount;
            double balance = model.NewBalance;
            int transType = model.TransactionType;
            int transOriginal = model.TransactionOriginal;
            string languageId = Helper.Current.UserLogin.LanguageID;
            //
            string transState = "x";
            if (transType == (int)TransactionEnum.TransactionType.IN)
                transState = "+";
            if (transType == (int)TransactionEnum.TransactionType.OUT)
                transState = "-";
            //
            string title = "Giao dịch thanh toán. GD " + transState + " " + Helper.Page.Library.FormatCurrency(amount) + " đ. Số dư: " + Helper.Page.Library.FormatCurrency(balance) + " đ.";
            string summary = "Số dư hạn mức" + Helper.Page.Library.FormatCurrency(balance) + " đ.";
            TransactionPaymentHistoryService BalanceCustomerHistoryService = new TransactionPaymentHistoryService(dbConnection);
            var id = BalanceCustomerHistoryService.Create<string>(new TransactionPaymentHistory()
            {
                CustomerID = customerId,
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
