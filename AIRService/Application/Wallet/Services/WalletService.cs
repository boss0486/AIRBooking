﻿using AL.NetFrame.Attributes;
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
using AIRService.WebService.WSGetReservationRQ;

namespace WebCore.Services
{
    public interface IBalanceService : IEntityService<DbConnection> { }
    public class WalletService : EntityService<DbConnection>, IBalanceService
    {
        public WalletService() : base() { }
        public WalletService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public static WalletCustomerMessageModel ChangeBalanceSpendingForCustomer(WalletCustomerChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                WalletCustomerService service = new WalletCustomerService();
                return service.ExecuteChangeBalanceSpendingForCustomer(model, dbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                throw ex;
                return new WalletCustomerMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" + ex };
            }
        }
        public static WalletCustomerMessageModel ChangeBalanceDepositForCustomer(WalletCustomerChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                WalletCustomerService service = new WalletCustomerService();
                return service.ExecuteChangeBalanceDepositForCustomer(model, dbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                throw ex;
                return new WalletCustomerMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" + ex };
            }
        }
        public static WalletCustomerMessageModel GetBalanceByCustomerID(string clientId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                WalletCustomerService service = new WalletCustomerService();
                return service.GetBalanceByCustomerID(clientId, dbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                throw ex;
                return new WalletCustomerMessageModel { Status = false, SpendingBalance = 0, DepositBalance = 0, Message = "Không tìm thây dữ liệu" };
            }
        }
        public static WalletCustomerMessageModel GetBalanceOfCustomerByUserID(string userId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return new WalletCustomerMessageModel { Status = false, SpendingBalance = 0, DepositBalance = 0, Message = "Không tìm thây dữ liệu" };
                //
                ClientLoginService  clientLoginService = new ClientLoginService();
                var client = clientLoginService.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.UserID.ToLower().Equals(userId.ToLower()), dbTransaction).FirstOrDefault();
                if (client == null)
                    return new WalletCustomerMessageModel { Status = false, SpendingBalance = 0, DepositBalance = 0, Message = "Không tìm thây dữ liệu" };
                //
                string clientId = client.ClientID;
                WalletCustomerService service = new WalletCustomerService();
                return service.GetBalanceByCustomerID(clientId, dbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                throw ex;
                return new WalletCustomerMessageModel { Status = false, SpendingBalance = 0, DepositBalance = 0, Message = "Không tìm thây dữ liệu" };
            }
        }
        //
        public static WalletUserMessageModel ChangeBalanceForUser(WalletUserChangeModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                WalletUserService service = new WalletUserService();
                return service.ChangeBalanceForUser(model, dbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                throw ex;
                return new WalletUserMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" + ex };
            }
        }
        public static WalletUserMessageModel GetBalanceOfUserByUserID(string userId, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                WalletUserService service = new WalletUserService();
                return service.GetBalanceByUserID(userId, dbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                throw ex;
                return new WalletUserMessageModel { Status = false, Balance = 0, Message = "Không tìm thây dữ liệu" };
            }
        }
        //##############################################################################################################################################################################################################################################################
    }
}
