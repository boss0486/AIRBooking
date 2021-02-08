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
    public interface IAgentWalletService : IEntityService<AgentWallet> { }
    public class AgentWalletService : EntityService<AgentWallet>, IAgentWalletService
    {
        public AgentWalletService() : base() { }
        public AgentWalletService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################

        // spending agent
        public static string ViewSpendingLimit(string userId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (connection == null)
                connection = DbConnect.Connection.CMS;
            //
            UserClientService userClientService = new UserClientService(connection);
            ClientLoginService clientLoginService = new ClientLoginService(connection);
            AirAgentService airAgentService = new AirAgentService(connection);
            ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == userId, transaction: transaction).FirstOrDefault();
            if (clientLogin == null)
                return string.Empty;
            //
            string agentId = clientLogin.AgentID;
            AirAgent airAgent = airAgentService.GetAlls(m => m.ID == agentId).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(airAgent.ParentID))
                return "Unlimited";
            //
            AgentSpendingLimitService agentSpendingLimitService = new AgentSpendingLimitService(connection);
            AgentSpendingLimit agentSpendingLimit = agentSpendingLimitService.GetAlls(m => m.ID == userId, transaction: transaction).FirstOrDefault();
            if (agentSpendingLimit == null)
                return string.Empty;
            //
            return $"{Helper.Page.Library.FormatCurrency(agentSpendingLimit.Amount)} đ";
        }
        public static double GetSpendingLimit(string userId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (connection == null)
                connection = DbConnect.Connection.CMS;
            //
            UserClientService userClientService = new UserClientService(connection);
            ClientLoginService clientLoginService = new ClientLoginService(connection);
            AirAgentService airAgentService = new AirAgentService(connection);
            ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == userId, transaction: transaction).FirstOrDefault();
            if (clientLogin == null)
                return 0;
            //
            AgentSpendingLimitService agentSpendingLimitService = new AgentSpendingLimitService(connection);
            AgentSpendingLimit agentSpendingLimit = agentSpendingLimitService.GetAlls(m => m.ID == userId, transaction: transaction).FirstOrDefault();
            if (agentSpendingLimit == null)
                return 0;
            //
            return agentSpendingLimit.Amount;
        }
        // balance
        public static string ViewBalance(string userId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (connection == null)
                connection = DbConnect.Connection.CMS;
            //
            UserClientService userClientService = new UserClientService(connection);
            ClientLoginService clientLoginService = new ClientLoginService(connection);
            AirAgentService airAgentService = new AirAgentService(connection);
            ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == userId, transaction: transaction).FirstOrDefault();
            if (clientLogin == null)
                return string.Empty;
            //
            string agentId = clientLogin.AgentID;
            AgentWalletService agentWalletService = new AgentWalletService(connection);
            AgentWallet agentWallet = agentWalletService.GetAlls(m => m.AgentID == agentId).FirstOrDefault();
            if (agentWallet == null)
                return string.Empty;
            //
            if (agentWallet.Unlimited)
                return "Unlimited";
            //
            return $"{Helper.Page.Library.FormatCurrency(agentWallet.Balance)} đ";
        }
        public static double GetBalance(string userId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (connection == null)
                connection = DbConnect.Connection.CMS;
            //
            UserClientService userClientService = new UserClientService(connection);
            ClientLoginService clientLoginService = new ClientLoginService(connection);
            AirAgentService airAgentService = new AirAgentService(connection);
            ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == userId, transaction: transaction).FirstOrDefault();
            if (clientLogin == null)
                return 0;
            //
            string agentId = clientLogin.AgentID;
            AgentWalletService agentWalletService = new AgentWalletService(connection);
            AgentWallet agentWallet = agentWalletService.GetAlls(m => m.AgentID == agentId).FirstOrDefault();
            if (agentWallet == null)
                return 0;
            //
            return agentWallet.Balance;
        }
        public WalletClientMessageModel ExecuteChangBalance(AgentWalletChangeModel model, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (connection == null)
                connection = _connection;
            //
            AgentWalletService agentWalletService = new AgentWalletService(connection);
            string agentId = model.AgentID.ToLower();
            double amount = model.Amount;
            AgentWallet agentWallet = agentWalletService.GetAlls(m => m.AgentID == agentId, transaction: transaction).FirstOrDefault();
            if (agentWallet == null)
                return new WalletClientMessageModel { Status = false, Message = "Không thể cập nhật giao dịch" };
            // + 
            if (agentWallet.Balance < amount)
                return new WalletClientMessageModel { Status = false, Message = "Số dư không đủ" };
            //
            agentWallet.Balance -= amount;
            agentWalletService.Update(agentWallet, transaction: transaction);
            //
            return new WalletClientMessageModel { Status = true, Message = "Ok" };
        }
        //##############################################################################################################################################################################################################################################################
        public static double GetBalance2(string userId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (connection == null)
                connection = DbConnect.Connection.CMS;
            //
            UserClientService userClientService = new UserClientService(connection);
            ClientLoginService clientLoginService = new ClientLoginService(connection);
            AirAgentService airAgentService = new AirAgentService(connection);
            ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == userId, transaction: transaction).FirstOrDefault();
            if (clientLogin == null)
                return 0;
            // get spending limit
            string agentId = clientLogin.AgentID;
            AgentSpendingLimitService agentSpendingLimitService = new AgentSpendingLimitService(connection);
            AgentSpendingLimit agentSpendingLimit = agentSpendingLimitService.GetAlls(m => m.AgentID == agentId, transaction).FirstOrDefault();
            if (agentSpendingLimit == null)
                return 0;
            //
            double spendingAmount = agentSpendingLimit.Amount;
            //
            DateTime today = Helper.TimeData.TimeHelper.UtcDateTime;
            DateTime firstDayOfMonth = today.AddMonths(-1);
            firstDayOfMonth = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.Month, 01);
            int paymentDayTotal = today.Subtract(firstDayOfMonth).Days;
            string sqlQuery = $@" SELECT (
                ISNULL((select sum (Amount) from App_BookPrice where BookOrderID = o.ID),0) +
                ISNULL((select sum (Amount) from App_BookTax where BookOrderID = o.ID),0) +
                ISNULL((select sum (AgentPrice + ProviderFee + AgentFee) from App_BookAgent where BookOrderID = o.ID),0) 
                ) as 'Spent'              
                FROM App_BookOrder AS o
                INNER JOIN App_BookAgent AS a ON a.BookOrderID = o.ID 
                WHERE a.AgentID = @AgentID AND a.TicketingID = @TicketingID 
                AND cast(IssueDate as Date) >= cast('{firstDayOfMonth}' as Date) AND cast(IssueDate as Date) <= cast('{today}' as Date)";
            double totalSpent = agentSpendingLimitService.Query<double>(sqlQuery, new { AgentID = agentId, TicketingID = userId }).FirstOrDefault();
            // 
            return spendingAmount - totalSpent;
        }

        public static string ViewBalance2(string userId, IDbConnection connection = null, IDbTransaction transaction = null)
        {
            if (connection == null)
                connection = DbConnect.Connection.CMS;
            //
            UserClientService userClientService = new UserClientService(connection);
            ClientLoginService clientLoginService = new ClientLoginService(connection);
            AirAgentService airAgentService = new AirAgentService(connection);
            ClientLogin clientLogin = clientLoginService.GetAlls(m => m.UserID == userId, transaction: transaction).FirstOrDefault();
            if (clientLogin == null)
                return "0 đ";
            // get spending limit
            string agentId = clientLogin.AgentID;
            AgentWalletService agentWalletService = new AgentWalletService(connection);
            AgentWallet agentWallet = agentWalletService.GetAlls(m => m.AgentID == agentId).FirstOrDefault();
            if (agentWallet == null)
                return "0 đ";
            //
            if (agentWallet.Unlimited)
                return "Unlimited";

            AgentSpendingLimitService agentSpendingLimitService = new AgentSpendingLimitService(connection);
            AgentSpendingLimit agentSpendingLimit = agentSpendingLimitService.GetAlls(m => m.AgentID == agentId, transaction).FirstOrDefault();
            if (agentSpendingLimit == null)
                return "0 đ";
            //
            double spendingAmount = agentSpendingLimit.Amount;
            //
            DateTime today = Helper.TimeData.TimeHelper.UtcDateTime;
            DateTime firstDayOfMonth = today.AddMonths(-1);
            firstDayOfMonth = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.Month, 01);
            int paymentDayTotal = today.Subtract(firstDayOfMonth).Days;
            string sqlQuery = $@" SELECT (
                ISNULL((select sum (Amount) from App_BookPrice where BookOrderID = o.ID),0) +
                ISNULL((select sum (Amount) from App_BookTax where BookOrderID = o.ID),0) +
                ISNULL((select sum (AgentPrice + ProviderFee + AgentFee) from App_BookAgent where BookOrderID = o.ID),0) 
                ) as 'Spent'              
                FROM App_BookOrder AS o
                INNER JOIN App_BookAgent AS a ON a.BookOrderID = o.ID 
                WHERE a.AgentID = @AgentID  
                AND cast(IssueDate as Date) >= cast('{firstDayOfMonth}' as Date) AND cast(IssueDate as Date) <= cast('{today}' as Date)";
            double totalSpent = agentSpendingLimitService.Query<double>(sqlQuery, new { AgentID = agentId}).FirstOrDefault();
            // 
            return $"{Helper.Page.Library.FormatCurrency(spendingAmount - totalSpent)} đ";
        }
        //##############################################################################################################################################################################################################################################################
    }
}
