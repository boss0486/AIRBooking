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
    public class LoggerHistoryService : EntityService<DbConnection>, ITransactionHistoryService
    {
        public LoggerHistoryService() : base() { }
        public LoggerHistoryService(System.Data.IDbConnection db) : base(db) { }
        //Payment history ##############################################################################################################################################################################################################################################################
        public static TransactionHistoryMessageModel LoggerPaymentHistory(WalletDepositHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletDepositHistoryService service = new WalletDepositHistoryService(dbConnection);
            return service.WalletDepositHistoryCreate(model, dbConnection, dbTransaction);
        }

        //Deposit history ##############################################################################################################################################################################################################################################################
        public static TransactionHistoryMessageModel LoggerWalletDepositHistory(WalletDepositHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletDepositHistoryService service = new WalletDepositHistoryService(dbConnection);
            return service.WalletDepositHistoryCreate(model, dbConnection, dbTransaction);
        }
        //wallet customer history ##############################################################################################################################################################################################################################################################

        public static TransactionHistoryMessageModel LoggerWalletSpendingHistory(WalletSpendingHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletSpendingHistoryService service = new WalletSpendingHistoryService(dbConnection);
            return service.WalletSpendingHistoryCreate(model, dbConnection, dbTransaction);
        }
        public static TransactionHistoryMessageModel LoggerWalletSpendingLimitHistory(WalletSpendingLimitHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletSpendingLimitHistoryService service = new WalletSpendingLimitHistoryService(dbConnection);
            return service.WalletCustomerSpendingLimitHistoryCreate(model, dbConnection, dbTransaction);
        }

        //wallet user history ##############################################################################################################################################################################################################################################################

        public static TransactionHistoryMessageModel LoggerWalletUserSpendingHistory(WalletUserSpendingHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletUserHistoryService service = new WalletUserHistoryService(dbConnection);
            return service.WalletUserSpendingHistoryCreate(model, dbConnection, dbTransaction);
        }
        //wallet investment history ##############################################################################################################################################################################################################################################################
        public static TransactionHistoryMessageModel LoggerWalletInvestmentHistory(WalletInvestmentHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletInvestmentHistoryService service = new WalletInvestmentHistoryService(dbConnection);
            return service.WalletInvestmentHistoryCreate(model, dbConnection, dbTransaction);
        }
        //
    }
}
