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
    public interface IWalletHistoryService : IEntityService<DbConnection> { }
    public class WalletHistoryService : EntityService<DbConnection>, IWalletHistoryService
    {
        public WalletHistoryService() : base() { }
        public WalletHistoryService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public static WalletHistoryMessageModel LoggerWalletCustomerHistory(WalletCustomerDepositHistoryCreateModel model, IDbConnection dbConnection = null,IDbTransaction dbTransaction = null)
        {
            try
            {
                WalletCustomerDepositHistoryService service = new WalletCustomerDepositHistoryService();
                return service.WalletCustomerDepositHistoryCreate(model, dbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                return new WalletHistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" + ex };
            }
        }
        public static WalletHistoryMessageModel LoggerWalletUserHistory(WalletUserHistoryCreateModel model, IDbConnection dbConnection = null, IDbTransaction dbTransaction = null)
        {
            try
            {
                WalletUserHistoryService service = new WalletUserHistoryService();
                return service.WalletUserHistoryCreate(model, dbConnection, dbTransaction);
            }
            catch (Exception ex)
            {
                return new WalletHistoryMessageModel { Status = false, Message = "Dữ liệu không hợp lệ" + ex };
            }
        }
        //##############################################################################################################################################################################################################################################################
    }
}
