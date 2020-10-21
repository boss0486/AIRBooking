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
    public interface IBalanceService : IEntityService<DbConnection> { }
    public class WalletService : EntityService<DbConnection>, IBalanceService
    {
        public WalletService() : base() { }
        public WalletService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public static WalletClientMessageModel ChangeSpendingLimitBalance(WalletClientChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletClientService service = new WalletClientService(dbConnection);
            return service.ExecuteChangeSpendingLimitBalance(model, dbConnection, dbTransaction);
        }
        public static WalletClientMessageModel ChangeSpendingBalance(WalletClientChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletClientService service = new WalletClientService(dbConnection);
            return service.ExecuteChangeSpendingBalance(model, dbConnection, dbTransaction);
        }
        public static WalletClientMessageModel ChangeDepositBalance(WalletClientChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletClientService service = new WalletClientService(dbConnection);
            return service.ExecuteChangeDepositBalance(model, dbConnection, dbTransaction);
        }
        public static WalletClientMessageModel ChangeInvestmentBalance(WalletClientChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletClientService service = new WalletClientService(dbConnection);
            return service.ExecuteChangeInvestmentBalance(model, dbConnection, dbTransaction);
        }
        public static WalletClientMessageModel GetBalanceByClientID(string clientId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            return WalletClientService.GetBalanceByCustomerID(clientId, dbConnection, dbTransaction);
        }
   
        //spending user ##############################################################################################################################################################################################################################################################
        public static WalletUserMessageModel ChangeBalanceForUser(WalletUserChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletUserService service = new WalletUserService(dbConnection);
            return service.ChangeBalanceForUser(model, dbConnection, dbTransaction);
        }
        public static WalletUserMessageModel GetBalanceOfUser(string userId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            WalletUserService service = new WalletUserService(dbConnection);
            return service.GetBalanceByUserID(userId, dbConnection, dbTransaction);
        }
        //User ##############################################################################################################################################################################################################################################################
    }
}
