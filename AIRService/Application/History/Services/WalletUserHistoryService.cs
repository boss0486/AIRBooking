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
    public interface IWalletUserHistoryService : IEntityService<WalletUserHistory> { }
    public class WalletUserHistoryService : EntityService<WalletUserHistory>, IWalletUserHistoryService
    {
        public WalletUserHistoryService() : base() { }
        public WalletUserHistoryService(System.Data.IDbConnection db) : base(db) { }
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
            string clientTime = model.ClientTime;
            //
            if (timeExpress != 0 && !string.IsNullOrWhiteSpace(clientTime))
            {
                DateTime dateClient = Convert.ToDateTime(clientTime);
                if (timeExpress == 1)
                {
                    string strDate = Helper.Page.Library.FormatToDateSQL(dateClient, "-");
                    DateTime dtime = Convert.ToDateTime(strDate);
                    whereCondition = " AND CONVERT(DateTime,Convert(nvarchar(10),CreatedDate, 120), 120) = '" + Helper.Page.Library.FormatToDateSQL(dtime, "-") + "'";
                }
                //
                if (timeExpress == 2)
                {
                    DateTime dtime = dateClient.AddDays(-3);
                    whereCondition = " AND CONVERT(DateTime,Convert(nvarchar(10),CreatedDate, 120), 120) >= '" + Helper.Page.Library.FormatToDateSQL(dtime, "-") + "'";
                }
                //
                if (timeExpress == 3)
                {
                    DateTime dtime = dateClient.AddDays(-7);
                    whereCondition = " AND CONVERT(DateTime,Convert(nvarchar(10),CreatedDate, 120), 120) >= '" + Helper.Page.Library.FormatToDateSQL(dtime, " - ") + "'";
                }
                //
                if (timeExpress == 4)
                {
                    DateTime dtime = dateClient.AddDays(-15);
                    whereCondition = " AND CONVERT(DateTime,Convert(nvarchar(10),CreatedDate, 120), 120) >= '" + Helper.Page.Library.FormatToDateSQL(dtime, "-") + "'";
                }
                if (timeExpress == 5)
                {
                    DateTime dtime = dateClient.AddDays(-30);
                    whereCondition = " AND CONVERT(DateTime,Convert(nvarchar(10),CreatedDate, 120), 120) >= '" + Helper.Page.Library.FormatToDateSQL(dtime, "-") + "'";
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(startDate))
                {
                    DateTime dtime = Convert.ToDateTime(startDate);
                    whereCondition += " AND CONVERT(DateTime,Convert(nvarchar(10),CreatedDate, 120), 120) >= '" + Helper.Page.Library.FormatToDateSQL(dtime, "-") + "'";
                }
                //
                if (!string.IsNullOrWhiteSpace(endDate))
                {
                    if (Convert.ToDateTime(endDate) < Convert.ToDateTime(startDate))
                        return Notifization.NotFound("Thời gian kết thúc không hợp lệ");
                    //
                    DateTime dtime = Convert.ToDateTime(endDate);
                    whereCondition += " AND CONVERT(DateTime,Convert(nvarchar(10),CreatedDate, 120), 120) <= '" + Helper.Page.Library.FormatToDateSQL(dtime, "-") + "'";
                }
            }
            //
            string langID = Helper.Current.UserLogin.LanguageID;
            string sqlQuery = @"SELECT * FROM App_WalletUserHistory WHERE dbo.Uni2NONE(Title) LIKE N'%'+ dbo.Uni2NONE(@Query) +'%' " + whereCondition + " ORDER BY[CreatedDate] DESC";
            var dtList = _connection.Query<WalletCustomerDepositHistoryResult>(sqlQuery, new { Query = query }).ToList();
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


        public WalletHistoryMessageModel WalletUserHistoryCreate(WalletUserHistoryCreateModel model,IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            if (model == null)
                return new WalletHistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" };
            //
            string customerId = model.CustomerID;
            string userId = model.UserID;
            double amount = model.Amount;
            double balance = model.NewBalance;
            int transType = model.TransactionType;
            //
            string transState = "x";
            if (transType == (int)WalletHistoryEnum.WalletHistoryTransactionType.INPUT)
                transState = "+";
            if (transType == (int)WalletHistoryEnum.WalletHistoryTransactionType.OUTPUT)
                transState = "-";
            //
            string title = "TK hạn mức thay đổi. GD " + transState + " " + Helper.Page.Library.FormatThousands(amount) + " đ. Số dư: " + Helper.Page.Library.FormatThousands(balance) + " đ.";
            string summary = "Số dư hạn mức" + Helper.Page.Library.FormatThousands(balance) + " đ.";
            WalletUserHistoryService balanceUserHistoryService = new WalletUserHistoryService(dbConnection);
            var id = balanceUserHistoryService.Create<string>(new WalletUserHistory()
            {
                CustomerID = customerId,
                UserID = userId,
                Title = title,
                Summary = summary,
                Amount = amount,
                TransactionType = transType,
                Status = 1,
                Enabled = (int)WebCore.Model.Enum.ModelEnum.Enabled.ENABLED
            }, transaction: dbTransaction);
            //commit
            return new WalletHistoryMessageModel { Status = true, Message = "Ok" };
        }
        //##############################################################################################################################################################################################################################################################
    }
}
