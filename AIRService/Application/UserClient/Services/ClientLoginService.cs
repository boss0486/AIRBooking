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
using WebCore.ENM;

namespace WebCore.Services
{
    public interface IClientLoginService : IEntityService<ClientLogin> { }
    public class ClientLoginService : EntityService<ClientLogin>, IClientLoginService
    {
        public ClientLoginService() : base() { }
        public ClientLoginService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################

        public static string GetTypeLogin(string clientId, int typeId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                return string.Empty;
            //
            if (typeId == (int)ClientLoginEnum.ClientType1.AGENT)
            {
                AirAgentService airAgentService = new AirAgentService();
                AirAgent airAgent = airAgentService.GetAlls(m => m.ID == clientId).FirstOrDefault();
                if (airAgent == null)
                    return airAgent.CodeID;
            }
            if (typeId == (int)ClientLoginEnum.ClientType1.COMP)
            {
                CompanyService companyService = new CompanyService();
                Company company = companyService.GetAlls(m => m.ID == clientId).FirstOrDefault();
                if (company == null)
                    return company.CodeID;
            }
            return string.Empty;
        }
        //##############################################################################################################################################################################################################################################################

    }
}