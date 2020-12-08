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
    public interface IWalletUserService : IEntityService<WalletUser> { }
    public class WalletUserService : EntityService<WalletUser>, IWalletUserService
    {
        public WalletUserService() : base() { }
        public WalletUserService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public WalletUserMessageModel ChangeBalanceForUser(WalletUserChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            if (dbConnection == null)
                dbConnection = _connection;
            //
            var service = new WalletUserService(dbConnection);
            string customerId = model.ClientID.ToLower();
            double amount = model.Amount;
            string userId = model.UserID.ToLower();
            int transType = model.TransactionType;
            WalletUser balanceUser = service.GetAlls(m => m.ClientID == customerId && m.UserID == userId, transaction: dbTransaction).FirstOrDefault();
            if (balanceUser == null)
            {
                service.Create<string>(new WalletUser()
                {
                    ClientID = customerId,
                    UserID = userId,
                    Amount = amount
                }, transaction: dbTransaction);
                return new WalletUserMessageModel { Status = true, Message = "Ok" };
            }
            // for update
            // + 
            if (transType == (int)TransactionEnum.TransactionType.IN)
                balanceUser.Amount += amount;
            // -
            if (transType == (int)TransactionEnum.TransactionType.OUT)
                balanceUser.Amount -= amount;
            // update
            service.Update(balanceUser, transaction: dbTransaction);
            return new WalletUserMessageModel { Status = true, Message = "Ok" };

        }
        //##############################################################################################################################################################################################################################################################
        public WalletUserMessageModel GetBalanceByUserID(string userId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            if (dbConnection == null)
                dbConnection = _connection;
            //
            if (string.IsNullOrWhiteSpace(userId))
                return new WalletUserMessageModel { Status = false, Balance = 0, Message = "Người dùng không xác định" };
            //
            userId = userId.ToLower();
            double amount = 0;
            WalletUserService walletUserService = new WalletUserService(dbConnection);
            var balance = walletUserService.GetAlls(m => m.UserID == userId, transaction: dbTransaction).FirstOrDefault();
            if (balance != null)
                amount = balance.Amount;
            //
            return new WalletUserMessageModel { Status = true, Balance = amount, Message = "Ok" };
        }
        //##############################################################################################################################################################################################################################################################
    }
}
