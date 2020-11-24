using AL.NetFrame.Interfaces;
using AL.NetFrame.Services;
using Dapper;
using Helper;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebCore.ENM;
using WebCore.Entities;
using WebCore.Model.Entities;

namespace WebCore.Services
{
    public interface IAirAgentFeeService : IEntityService<Entities.AirAgentFee> { }
    public class AirAgentFeeService : EntityService<Entities.AirAgentFee>, IAirAgentFeeService
    {
        public AirAgentFeeService() : base() { }
        public AirAgentFeeService(System.Data.IDbConnection db) : base(db) { }
        //##############################################################################################################################################################################################################################################################
        public ActionResult DataList(SearchModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            int page = model.Page;
            string query = model.Query;
            if (string.IsNullOrWhiteSpace(query))
                query = "";
            //
            string whereCondition = string.Empty;
            //
            SearchResult searchResult = WebCore.Model.Services.ModelService.SearchDefault(new SearchModel
            {
                Query = model.Query,
                TimeExpress = model.TimeExpress,
                Status = model.Status,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Page = model.Page,
                AreaID = model.AreaID,
                TimeZoneLocal = model.TimeZoneLocal
            });
            if (searchResult != null)
            {
                if (searchResult.Status == 1)
                    whereCondition = searchResult.Message;
                else
                    return Notifization.Invalid(searchResult.Message);
            }
            //
            string areaId = model.AreaID;
            if (!string.IsNullOrWhiteSpace(areaId) && areaId != "-")
                whereCondition += " AND AreaID = @AreaID ";
            // query
            string sqlQuery = @"SELECT * FROM App_AirAgentFee WHERE Title LIKE N'%'+ @Query +'%'" + whereCondition + " ORDER BY [Title] ASC";
            var dtList = _connection.Query<AirAgentFeeResult>(sqlQuery, new { Query = Helper.Page.Library.FormatNameToUni2NONE(query), AreaID = areaId }).ToList();
            if (dtList.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            var result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            if (result.Count == 0 && page > 1)
            {
                page -= 1;
                result = dtList.ToPagedList(page, Helper.Pagination.Paging.PAGESIZE).ToList();
            }
            if (result.Count == 0)
                return Notifization.NotFound(MessageText.NotFound);
            // Pagination
            Helper.Pagination.PagingModel pagingModel = new Helper.Pagination.PagingModel
            {
                PageSize = Helper.Pagination.Paging.PAGESIZE,
                Total = dtList.Count,
                Page = page
            };
            // reusult
            return Notifization.Data(MessageText.Success, data: result, role: RoleActionSettingService.RoleListForUser(), paging: pagingModel);
        }


        public ActionResult Update(AirAgentFeeConfigModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid + "1");
            //
            string clientId = model.AgentID;
            float amount = model.Amount;
            if (amount < 0 && amount > 100000000)
                return Notifization.Invalid("Số tiền giới hạn từ 0 - 100 000 000 đ");
            //
            if (Helper.Current.UserLogin.IsSupplierLogged() || Helper.Current.UserLogin.IsCustomerLogged())
            {
                string userId = Helper.Current.UserLogin.IdentifierID;
                clientId = ClientLoginService.GetClientIDByUserID(userId);
            }
            ClientLoginService clientLoginService = new ClientLoginService();
            if (string.IsNullOrWhiteSpace(clientId))
                return Notifization.Invalid(MessageText.Invalid + "2");
            // 
            AirAgentFeeService airFeeAgentService = new AirAgentFeeService(_connection);
            var airFeeAgent = airFeeAgentService.GetAlls(m => m.AgentID == clientId).FirstOrDefault();
            if (airFeeAgent == null)
            {
                // create
                airFeeAgentService.Create<string>(new AirAgentFee
                {
                    Title = clientId,
                    AgentID = clientId,
                    Amount = amount,
                    Enabled = 1
                });
                return Notifization.Success(MessageText.CreateSuccess);
            }
            // update
            airFeeAgent.Amount = amount;
            airFeeAgentService.Update(airFeeAgent);
            return Notifization.Success(MessageText.UpdateSuccess);
        }

        public ActionResult GetAgentFee(AirAgentFeeModel model)
        {
            if (model == null)
                return Notifization.Invalid(MessageText.Invalid);
            //
            string agentId = model.AgentID;
            if (string.IsNullOrWhiteSpace(agentId))
                return Notifization.Invalid(MessageText.Invalid);
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirAgentFee WHERE AgentID = @AgentID";
            AirAgentFeeResult airAirFeeAgent = _connection.Query<AirAgentFeeResult>(sqlQuery, new { AgentID = agentId.ToLower() }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return Notifization.Data("OK", new AirAgentFeeResult
                {
                    AgentID = agentId,
                    Amount = 0
                });
            //
            return Notifization.Data("OK", airAirFeeAgent);
        }
        //##############################################################################################################################################################################################################################################################
        public AirAgentFee GetAgentFee(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirAgentFee WHERE AgentID = @AgentID";
            AirAgentFee airAirFeeAgent = _connection.Query<AirAgentFee>(sqlQuery, new { AgentID = id }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return null;
            //
            return airAirFeeAgent;
        }
        public AirAgentFeeResult GetAgentFeeByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;
            //
            string sqlQuery = @"SELECT TOP (1) * FROM App_AirAgentFee WHERE ID = @ID";
            AirAgentFeeResult airAirFeeAgent = _connection.Query<AirAgentFeeResult>(sqlQuery, new { ID = id }).FirstOrDefault();
            //
            if (airAirFeeAgent == null)
                return null;
            //
            return airAirFeeAgent;
        }
    }
}
