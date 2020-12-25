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
    public interface IWalletAgentService : IEntityService<WalletAgent> { }
    public class WalletAgentService : EntityService<WalletAgent>, IWalletAgentService
    {
        public WalletAgentService() : base() { }
        public WalletAgentService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        //public WalletClientMessageModel ExecuteChangeInvestmentBalance(WalletAgentChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        //{
        //    if (dbConnection == null)
        //        dbConnection = _connection;
        //    //
        //    var service = new WalletClientService(dbConnection);
        //    string clientId = model.AgentID.ToLower();
        //    double amount = model.Amount;
        //    double transType = model.TransactionType;
        //    WalletAgent walletClient = service.GetAlls(m => m.AgentID == clientId, transaction: dbTransaction).FirstOrDefault();
        //    if (walletClient == null)
        //        return new WalletClientMessageModel { Status = false, Message = "Không thể cập nhật giao dịch" + clientId };
        //    // + 
        //    if (transType == (int)TransactionEnum.TransactionType.IN)
        //        walletClient.InvestedAmount += amount;
        //    // -
        //    if (transType == (int)TransactionEnum.TransactionType.OUT)
        //        walletClient.InvestedAmount -= amount;
        //    // update
        //    service.Update(walletClient, transaction: dbTransaction);
        //    return new WalletClientMessageModel { Status = true, Message = "Ok" };
        //}
        //public WalletClientMessageModel ExecuteChangeSpendingLimitBalance(WalletAgentChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        //{
        //    if (dbConnection == null)
        //        dbConnection = _connection;
        //    //
        //    var service = new WalletClientService(dbConnection);
        //    string clientId = model.AgentID.ToLower();
        //    double amount = model.Amount;
        //    double transType = model.TransactionType;
        //    WalletAgent walletCustomer = service.GetAlls(m => m.AgentID == clientId, transaction: dbTransaction).FirstOrDefault();
        //    if (walletCustomer == null)
        //        return new WalletClientMessageModel { Status = false, Message = "Không thể cập nhật giao dịch" };
        //    // + 
        //    if (transType == (int)TransactionEnum.TransactionType.IN)
        //        walletCustomer.SpendingLimitAmount += amount;
        //    // -
        //    if (transType == (int)TransactionEnum.TransactionType.OUT)
        //        walletCustomer.SpendingLimitAmount -= amount;
        //    // update
        //    service.Update(walletCustomer, transaction: dbTransaction);
        //    return new WalletClientMessageModel { Status = true, Message = "Ok" };
        //}
        //public WalletClientMessageModel ExecuteChangeSpendingBalance(WalletAgentChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        //{
        //    if (dbConnection == null)
        //        dbConnection = _connection;
        //    //
        //    var service = new WalletClientService(dbConnection);
        //    string customerId = model.AgentID.ToLower();
        //    double amount = model.Amount;
        //    double transType = model.TransactionType;
        //    WalletAgent balanceCustomer = service.GetAlls(m => m.AgentID == customerId, transaction: dbTransaction).FirstOrDefault();
        //    if (balanceCustomer == null)
        //        return new WalletClientMessageModel { Status = false, Message = "Không thể cập nhật giao dịch" };
        //    // + 
        //    if (transType == (int)TransactionEnum.TransactionType.IN)
        //        balanceCustomer.SpendingAmount += amount;
        //    // -
        //    if (transType == (int)TransactionEnum.TransactionType.OUT)
        //        balanceCustomer.SpendingAmount -= amount;
        //    // update
        //    service.Update(balanceCustomer, transaction: dbTransaction);
        //    return new WalletClientMessageModel { Status = true, Message = "Ok" };
        //}

        //public WalletClientMessageModel ExecuteChangeDepositBalance(WalletAgentChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        //{
        //    if (dbConnection == null)
        //        dbConnection = _connection;
        //    //
        //    var service = new WalletClientService(dbConnection);
        //    string customerId = model.AgentID.ToLower();
        //    double amount = model.Amount;
        //    double transType = model.TransactionType;
        //    WalletAgent balanceCustomer = service.GetAlls(m => m.AgentID == customerId, transaction: dbTransaction).FirstOrDefault();
        //    if (balanceCustomer == null)
        //        return new WalletClientMessageModel { Status = false, Message = "Không thể cập nhật giao dịch" };
        //    // + 
        //    if (transType == (int)TransactionEnum.TransactionType.IN)
        //        balanceCustomer.DepositAmount += amount;
        //    // -
        //    if (transType == (int)TransactionEnum.TransactionType.OUT)
        //        balanceCustomer.DepositAmount -= amount;
        //    // update
        //    service.Update(balanceCustomer, transaction: dbTransaction);
        //    return new WalletClientMessageModel { Status = true, Message = "Ok" };
        //}

        //public static WalletClientMessageModel GetBalanceByCustomerID(string clientId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        //{
        //    if (dbConnection == null)
        //        dbConnection = DbConnect.Connection.CMS;
        //    //
        //    double investedAmount = 0;
        //    double spendingLimitAmount = 0;
        //    double spendingAmount = 0;
        //    double depositAmount = 0;
        //    var service = new WalletClientService(dbConnection);
        //    var balance = service.GetAlls(m => m.AgentID == clientId.ToLower(), transaction: dbTransaction).FirstOrDefault();
        //    if (balance != null)
        //    {
        //        investedAmount = balance.InvestedAmount;
        //        spendingLimitAmount = balance.SpendingLimitAmount;
        //        depositAmount = balance.DepositAmount;
        //        spendingAmount = balance.SpendingAmount;
        //    }
        //    //
        //    return new WalletClientMessageModel { Status = true, InvestedAmount = investedAmount, SpendingLimitBalance = spendingLimitAmount, DepositBalance = depositAmount, SpendingBalance = spendingAmount, Message = "Ok" };
        //}
        //##############################################################################################################################################################################################################################################################
    }
}
