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
    public interface IBalanceCustomerService : IEntityService<WalletCustomer> { }
    public class WalletCustomerService : EntityService<WalletCustomer>, IBalanceCustomerService
    {
        public WalletCustomerService() : base() { }
        public WalletCustomerService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public WalletCustomerMessageModel ExecuteChangeBalanceSpendingForCustomer(WalletCustomerChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            if (dbConnection == null)
                dbConnection = DbConnect.Connection.CMS;
            //
            var service = new WalletCustomerService(dbConnection);
            string customerId = model.CustomerID.ToLower();
            double amount = model.Amount;
            double transType = model.TransactionType;
            WalletCustomer balanceCustomer = service.GetAlls(m => m.CustomerID == customerId, transaction: dbTransaction).FirstOrDefault();
            if (balanceCustomer == null)
                return new WalletCustomerMessageModel { Status = false, Message = "Không thể cập nhật giao dịch" };
            // + 
            if (transType == (int)TransactionEnum.TransactionType.IN)
                balanceCustomer.SpendingAmount += amount;
            // -
            if (transType == (int)TransactionEnum.TransactionType.OUT)
                balanceCustomer.SpendingAmount -= amount;
            // update
            service.Update(balanceCustomer, transaction: dbTransaction);
            return new WalletCustomerMessageModel { Status = true, Message = "Ok" };
        }

        public WalletCustomerMessageModel ExecuteChangeBalanceDepositForCustomer(WalletCustomerChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            if (dbConnection == null)
                dbConnection = DbConnect.Connection.CMS;
            //
            var service = new WalletCustomerService(dbConnection);
            string customerId = model.CustomerID.ToLower();
            double amount = model.Amount;
            double transType = model.TransactionType;
            WalletCustomer balanceCustomer = service.GetAlls(m => m.CustomerID == customerId, transaction: dbTransaction).FirstOrDefault();
            if (balanceCustomer == null)
                return new WalletCustomerMessageModel { Status = false, Message = "Không thể cập nhật giao dịch" };
            // + 
            if (transType == (int)TransactionEnum.TransactionType.IN)
                balanceCustomer.DepositAmount += amount;
            // -
            if (transType == (int)TransactionEnum.TransactionType.OUT)
                balanceCustomer.DepositAmount -= amount;
            // update
            service.Update(balanceCustomer, transaction: dbTransaction);
            return new WalletCustomerMessageModel { Status = true, Message = "Ok" };
        }

        public WalletCustomerMessageModel GetBalanceByCustomerID(string customerId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            if (dbConnection == null)
                dbConnection = DbConnect.Connection.CMS;
            double spendingAmount = 0;
            double depositAmount = 0;
            var service = new WalletCustomerService(dbConnection);
            var balance = service.GetAlls(m => m.CustomerID == customerId.ToLower(), transaction: dbTransaction).FirstOrDefault();
            if (balance != null)
            {
                spendingAmount = balance.SpendingAmount;
                depositAmount = balance.DepositAmount;
            }
            //
            return new WalletCustomerMessageModel { Status = true, SpendingBalance = spendingAmount, DepositBalance = depositAmount, Message = "Ok" };
        }
        //##############################################################################################################################################################################################################################################################
    }
}
