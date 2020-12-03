using AL.NetFrame.Attributes;
using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using System;
using Dapper;
using System.Linq;
using PagedList;
using System.Web.Mvc;
using System.Collections.Generic;
using WebCore.Entities;
using Helper;
using System.Web;
using System.Web.Configuration;
using System.Data;

namespace WebCore.Services
{
    public interface IClientLoginService : IEntityService<ClientLogin> { }
    public class ClientLoginService : EntityService<ClientLogin>, IClientLoginService
    {
        public ClientLoginService() : base() { }
        public ClientLoginService(System.Data.IDbConnection db) : base(db) { }


        //
        public string GetClientCodeByID(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                return string.Empty;
            //
            string sqlQuery = @"SELECT s.ID,s.CodeID, s.Title, IsSupplier = 1 FROM App_Supplier as s WHERE s.Enabled = 1 AND s.ID = @ClientID
                         Union
                         SELECT c.ID,c.CodeID, c.Title, IsSupplier = 0 FROM App_AirAgent as c WHERE c.Enabled = 1 AND c.ID = @ClientID";
            //
            ClientProviderOption clientOption = _connection.Query<ClientProviderOption>(sqlQuery, new { ClientID = clientId }).FirstOrDefault();
            if (clientOption == null)
                return string.Empty;
            //
            return clientOption.CodeID;
        }

        public List<ClientProviderOption> GetAllProvider()
        {
            string userId = Helper.Current.UserLogin.IdentifierID;
            string sqlQuery = "";
            List<ClientProviderOption> clientOptions = new List<ClientProviderOption>();
            if (Helper.Current.UserLogin.IsCMSUser || Helper.Current.UserLogin.IsAdminInApplication)
            {
                sqlQuery = @"
                         SELECT s.ID,s.CodeID, s.Title, IsSupplier = 1 FROM App_Supplier as s WHERE s.Enabled = 1 
                         Union
                         SELECT c.ID,c.CodeID, c.Title, IsSupplier = 0 FROM App_AirAgent as c WHERE c.Enabled = 1 AND TypeID = 'agent'";
                //
                clientOptions = _connection.Query<ClientProviderOption>(sqlQuery, new { }).ToList();
            }
            else if (Helper.Current.UserLogin.IsSupplierLogged())
            {
                string clientId = ClientLoginService.GetClientIDByUserID(userId);
                sqlQuery = @"SELECT s.ID, s.CodeID, s.Title, IsSupplier = 1 FROM App_Supplier as s WHERE s.Enabled = 1 AND ID = @ClientID";
                clientOptions = _connection.Query<ClientProviderOption>(sqlQuery, new { ClientID = clientId }).ToList();
            }
            else if (Helper.Current.UserLogin.IsCustomerLogged())
            {
                string clientId = ClientLoginService.GetClientIDByUserID(userId);
                sqlQuery = @"SELECT c.ID, c.CodeID, c.Title, IsSupplier = 0 FROM App_AirAgent as c WHERE c.Enabled = 1 AND TypeID = 'agent' AND ID = @ClientID";
                clientOptions = _connection.Query<ClientProviderOption>(sqlQuery, new { ClientID = clientId }).ToList();
            }
            //
            if (clientOptions.Count == 0)
                return new List<ClientProviderOption>();
            //
            return clientOptions;
        }
        //##############################################################################################################################################################################################################################################################

        public static string GetClientIDByUserID(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return string.Empty;
            //
            userId = userId.ToLower();
            using (var service = new ClientLoginService())
            {
                var customer = service.GetAlls(m => !string.IsNullOrWhiteSpace(m.ID) && m.UserID == userId).FirstOrDefault();
                if (customer == null)
                    return string.Empty;
                //
                return customer.ClientID;

            }
        }

         
        //##############################################################################################################################################################################################################################################################

    }
}