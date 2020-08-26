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
                dbConnection = DbConnect.Connection.CMS;
            //
            var service = new WalletUserService(dbConnection);
            string customerId = model.CustomerID;
            double amount = model.Amount;
            string userId = model.UserID;
            int transType = model.TransactionType;
            WalletUser balanceUser = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.CustomerID.ToLower().Equals(customerId.ToLower()) && m.UserID.ToLower().Equals(userId.ToLower()), transaction: dbTransaction).FirstOrDefault();
            if (balanceUser == null)
            {
                service.Create<string>(new WalletUser()
                {
                    CustomerID = customerId,
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
                dbConnection = DbConnect.Connection.CMS;
            //
            if (string.IsNullOrWhiteSpace(userId))
                return new WalletUserMessageModel { Status = false, Balance = 0, Message = "Người dùng không xác định" };
            //
            double amount = 0;
            var service = new WalletUserService(dbConnection);
            var balance = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.UserID) && m.UserID.ToLower().Equals(userId.ToLower()), transaction: dbTransaction).FirstOrDefault();
            if (balance != null)
                amount = balance.Amount;
            //
            return new WalletUserMessageModel { Status = true, Balance = amount, Message = "Ok" };
        }
        //##############################################################################################################################################################################################################################################################
    }
}
