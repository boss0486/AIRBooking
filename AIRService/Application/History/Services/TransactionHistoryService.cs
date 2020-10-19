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
    public interface ITransactionHistoryService : IEntityService<DbConnection> { }
    public class TransactionHistoryService : EntityService<DbConnection>, ITransactionHistoryService
    {
        public TransactionHistoryService() : base() { }
        public TransactionHistoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public static TransactionHistoryMessageModel LoggerTransactionDepositHistory(TransactionDepositHistoryCreateModel model, IDbConnection dbConnection = null,IDbTransaction dbTransaction = null)
        {
            TransactionDepositHistoryService service = new TransactionDepositHistoryService();
            return service.TransactionDepositHistoryCreate(model, dbConnection, dbTransaction);
        }
        public static TransactionHistoryMessageModel LoggerWalletCustomerSpendingHistory(WalletCustomerSpendingHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                WalletCustomerSpendingHistoryService service = new WalletCustomerSpendingHistoryService();
                return service.WalletCustomerSpendingHistoryCreate(model, dbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                return new TransactionHistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" + ex };
            }
        }
        public static TransactionHistoryMessageModel LoggerWalletUserSpendingHistory(WalletUserHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                WalletUserHistoryService service = new WalletUserHistoryService();
                return service.WalletUserSpendingHistoryCreate(model, dbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                return new TransactionHistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" + ex };
            }
        }
        //##############################################################################################################################################################################################################################################################
    }
}
