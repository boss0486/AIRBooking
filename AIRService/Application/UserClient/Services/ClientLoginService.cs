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

        public List<ClientOption> GetAllAgent()
        {
            string sqlQuery = @"SELECT s.ID,s.CodeID, s.Title FROM App_Supplier as s WHERE s.Enabled = 1 
                         Union
                         SELECT c.ID,c.CodeID, c.Title FROM App_Customer as c WHERE c.Enabled = 1 AND TypeID = 'agent'";

            List<ClientOption> clientOptions = _connection.Query<ClientOption>(sqlQuery, new { }).ToList();
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
                List<ClientOption> dtList = clientLoginService.GetAllAgent();
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

    }
}