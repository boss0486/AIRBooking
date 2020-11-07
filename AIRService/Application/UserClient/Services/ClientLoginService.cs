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
        //
        public List<ClientOption> GetAllProvider()
        {
            string userId = Helper.Current.UserLogin.IdentifierID;
            string sqlQuery = "";
            List<ClientOption> clientOptions = new List<ClientOption>();
            if (Helper.Current.UserLogin.IsCMSUser || Helper.Current.UserLogin.IsAdminInApplication)
            {
                sqlQuery = @"
                         SELECT s.ID,s.CodeID, s.Title FROM App_Supplier as s WHERE s.Enabled = 1 
                         Union
                         SELECT c.ID,c.CodeID, c.Title FROM App_Customer as c WHERE c.Enabled = 1 AND TypeID = 'agent'";
                //
                clientOptions = _connection.Query<ClientOption>(sqlQuery, new { }).ToList();
            }
            else if (Helper.Current.UserLogin.IsSupplierLogged())
            {
                string clientId = ClientLoginService.GetClientIDByUserID(userId);
                sqlQuery = @"SELECT s.ID, s.CodeID, s.Title FROM App_Supplier as s WHERE s.Enabled = 1 AND ID = @ClientID";
                clientOptions = _connection.Query<ClientOption>(sqlQuery, new { ClientID = clientId }).ToList();
            }
            else if (Helper.Current.UserLogin.IsCustomerLogged())
            {
                string clientId = ClientLoginService.GetClientIDByUserID(userId);
                sqlQuery = @"SELECT c.ID, c.CodeID, c.Title FROM App_Customer as c WHERE c.Enabled = 1 AND TypeID = 'agent' AND ID = @ClientID";
                clientOptions = _connection.Query<ClientOption>(sqlQuery, new { ClientID = clientId }).ToList();
            }
            //
            if (clientOptions.Count == 0)
                return new List<ClientOption>();
            //
            return clientOptions;
        }


        public static string DropdownListAgent(string id)
        {
            string result = string.Empty;
            using (var service = new CustomerService())
            {
                ClientLoginService clientLoginService = new ClientLoginService();
                List<ClientOption> dtList = clientLoginService.GetAllProvider();
                if (dtList.Count > 0)
                {
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                            select = "selected";
                        result += "<option value='" + item.ID + "' data-codeid ='" + item.CodeID + "' " + select + ">" + item.Title + "</option>";
                    }
                }
                return result;
            }

        }
        public static string DropdownProvider(string id)
        {
            string result = string.Empty;
            using (var service = new CustomerService())
            {
                ClientLoginService clientLoginService = new ClientLoginService();
                List<ClientOption> dtList = clientLoginService.GetAllProvider();
                if (dtList.Count > 0)
                {
                    foreach (var item in dtList)
                    {
                        string select = string.Empty;
                        if (!string.IsNullOrWhiteSpace(id) && item.ID == id.ToLower())
                            select = "selected";
                        result += "<option value='" + item.ID + "' data-codeid ='" + item.CodeID + "' " + select + ">" + item.Title + "</option>";
                    }
                }
                return result;
            }
        }


        public List<ClientOption> GetCompany()
        {
            string userId = Helper.Current.UserLogin.IdentifierID;
            string sqlQuery = "";
            List<ClientOption> clientOptions = new List<ClientOption>();
            if (Helper.Current.UserLogin.IsCMSUser || Helper.Current.UserLogin.IsAdminInApplication)
            {
                sqlQuery = @"SELECT c.ID,c.CodeID, c.Title FROM App_Customer as c WHERE c.Enabled = 1 AND TypeID = 'comp' ORDER BY c.CodeID";
                //
                clientOptions = _connection.Query<ClientOption>(sqlQuery, new { }).ToList();
            }
            else if (Helper.Current.UserLogin.IsSupplierLogged())
            {
                string clientId = ClientLoginService.GetClientIDByUserID(userId);
                sqlQuery = @"SELECT c.ID,c.CodeID, c.Title FROM App_Customer as c WHERE c.Enabled = 1 AND TypeID = 'comp' AND SupplierID = @ClientID ORDER BY c.CodeID";
                clientOptions = _connection.Query<ClientOption>(sqlQuery, new { ClientID = clientId }).ToList();
            }
            else if (Helper.Current.UserLogin.IsCustomerLogged())
            {
                string clientId = ClientLoginService.GetClientIDByUserID(userId);
                sqlQuery = @"SELECT c.ID,c.CodeID, c.Title FROM App_Customer as c WHERE c.Enabled = 1 AND TypeID = 'comp' AND ParentID = @ClientID ORDER BY c.CodeID";
                clientOptions = _connection.Query<ClientOption>(sqlQuery, new { ClientID = clientId }).ToList();
            }
            //
            if (clientOptions.Count == 0)
                return new List<ClientOption>();
            //
            return clientOptions;
        }
    }
}